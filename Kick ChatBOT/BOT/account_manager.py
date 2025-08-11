#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Gestor de Cuentas para Kick.com Chatbot
Permite agregar, gestionar y verificar múltiples cuentas
"""

import json
import time
import asyncio
import aiocurl
from datetime import datetime

class KickAccountManager:
    def __init__(self):
        self.accounts = []
        
    def load_accounts(self):
        """Carga cuentas existentes"""
        try:
            with open("kicks.json", "r") as f:
                self.accounts = json.load(f)
            return True
        except:
            self.accounts = []
            return False
    
    def save_accounts(self):
        """Guarda todas las cuentas"""
        try:
            with open("kicks.json", "w") as f:
                json.dump(self.accounts, f, indent=4)
            return True
        except Exception as e:
            print(f"❌ Error guardando: {e}")
            return False
    
    def add_manual_account(self):
        """Permite agregar cuenta manualmente"""
        print("➕ AGREGAR NUEVA CUENTA MANUALMENTE")
        print("=" * 50)
        print("🔧 Proceso: Extraer token de navegador como antes")
        print("=" * 50)
        
        # Datos de la cuenta
        username = input("👤 Username de Kick: ").strip()
        if not username:
            print("❌ Username requerido")
            return None
            
        user_id = input("🆔 User ID (número): ").strip()
        if not user_id or not user_id.isdigit():
            print("❌ User ID debe ser numérico")
            return None
            
        token = input("🔑 Token completo (sin 'Bearer '): ").strip()
        if not token:
            print("❌ Token requerido")
            return None
        
        email = input("📧 Email (opcional): ").strip() or f"{username}@manual.com"
        
        # Personalidad
        personalities = ["enthusiastic", "casual", "supportive", "gamer", "lurker", "toxic"]
        print(f"\n🎭 Personalidades disponibles:")
        for i, p in enumerate(personalities, 1):
            print(f"   {i}. {p}")
        
        personality_choice = input("Selecciona (1-6, default: 2-casual): ").strip()
        personality_map = {str(i): p for i, p in enumerate(personalities, 1)}
        personality = personality_map.get(personality_choice, "casual")
        
        # Crear cuenta
        new_account = {
            "token": token,
            "email": email,
            "password": "manual_account",
            "username": username,
            "user_id": int(user_id),
            "verified": True,
            "personality": personality,
            "last_message_time": 0,
            "messages_sent": 0,
            "session_start": time.time(),
            "created_at": time.time(),
            "real": True,
            "manual": True,
            "added_at": datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        }
        
        return new_account
    
    async def verify_account(self, account):
        """Verifica que una cuenta sea válida"""
        try:
            session = aiocurl.Session()
            headers = {
                "Authorization": f"Bearer {account['token']}",
                "Accept": "application/json",
                "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
            }
            
            # Probar el token
            response = await session.request("GET", "https://kick.com/api/v1/user", headers=headers)
            
            if response.status == "200":
                user_data = json.loads(response.body)
                print(f"✅ {account['username']}: Token válido")
                return True
            else:
                print(f"❌ {account['username']}: Token inválido (Status: {response.status})")
                return False

        except Exception as e:
            print(f"❌ {account['username']}: Error verificando - {e}")
            return False
        finally:
            # Evitar rate limiting
            await asyncio.sleep(1)
    
    async def verify_all_accounts(self):
        """Verifica todas las cuentas"""
        print("🔍 VERIFICANDO TODAS LAS CUENTAS")
        print("=" * 40)
        
        tasks = [self.verify_account(acc) for acc in self.accounts]
        results = await asyncio.gather(*tasks, return_exceptions=True)

        valid_accounts = []
        invalid_accounts = []

        for account, result in zip(self.accounts, results):
            if isinstance(result, Exception):
                print(f"❌ {account['username']}: Error verificando - {result}")
                invalid_accounts.append(account)
            elif result:
                valid_accounts.append(account)
            else:
                invalid_accounts.append(account)

        print(f"\n📊 RESULTADOS:")
        print(f"   ✅ Válidas: {len(valid_accounts)}")
        print(f"   ❌ Inválidas: {len(invalid_accounts)}")

        if invalid_accounts:
            print(f"\n🗑️ Cuentas inválidas encontradas:")
            for acc in invalid_accounts:
                print(f"   ❌ {acc['username']} - Token expirado")

        return valid_accounts, invalid_accounts
    
    def show_accounts_status(self):
        """Muestra estado de todas las cuentas"""
        print(f"\n📊 ESTADO DE CUENTAS ({len(self.accounts)} total)")
        print("=" * 60)
        
        if not self.accounts:
            print("❌ No hay cuentas registradas")
            return
        
        for i, account in enumerate(self.accounts, 1):
            account_type = "🧪 Demo" if account.get('demo') else "🔑 Real"
            manual = "📝 Manual" if account.get('manual') else "🤖 Auto"
            
            print(f"{i:2d}. {account_type} {manual}")
            print(f"    👤 {account['username']} ({account['personality']})")
            print(f"    🆔 ID: {account['user_id']}")
            print(f"    📧 {account['email']}")
            print(f"    💬 Mensajes enviados: {account.get('messages_sent', 0)}")
            
            if account.get('added_at'):
                print(f"    📅 Agregada: {account['added_at']}")
            print()
    
    def remove_account(self):
        """Elimina una cuenta"""
        self.show_accounts_status()
        
        if not self.accounts:
            return
        
        try:
            choice = int(input("🗑️ Número de cuenta a eliminar (0 para cancelar): "))
            if choice == 0:
                return
            
            if 1 <= choice <= len(self.accounts):
                account = self.accounts[choice - 1]
                confirm = input(f"¿Eliminar {account['username']}? (s/N): ").strip().lower()
                
                if confirm in ['s', 'sí', 'si', 'y', 'yes']:
                    removed = self.accounts.pop(choice - 1)
                    print(f"🗑️ Cuenta {removed['username']} eliminada")
                    return True
            else:
                print("❌ Número inválido")
                
        except ValueError:
            print("❌ Ingresa un número válido")
        
        return False

def main():
    """Función principal"""
    print("🔧 GESTOR DE CUENTAS KICK.COM")
    print("=" * 50)
    
    manager = KickAccountManager()
    manager.load_accounts()
    
    while True:
        print("\n🎯 OPCIONES:")
        print("1. 📊 Ver estado de cuentas")
        print("2. ➕ Agregar nueva cuenta")
        print("3. 🔍 Verificar todas las cuentas")
        print("4. 🗑️ Eliminar cuenta")
        print("5. 💾 Guardar cambios")
        print("0. ❌ Salir")
        
        choice = input("\n🔢 Selecciona opción: ").strip()
        
        if choice == "1":
            manager.show_accounts_status()
            
        elif choice == "2":
            account = manager.add_manual_account()
            if account:
                manager.accounts.append(account)
                print(f"✅ Cuenta {account['username']} agregada")
                print("💡 No olvides guardar los cambios (opción 5)")
                
        elif choice == "3":
            asyncio.run(manager.verify_all_accounts())
            
        elif choice == "4":
            if manager.remove_account():
                print("💡 No olvides guardar los cambios (opción 5)")
                
        elif choice == "5":
            if manager.save_accounts():
                print("💾 Cambios guardados exitosamente")
            else:
                print("❌ Error guardando cambios")
                
        elif choice == "0":
            # Preguntar si guardar antes de salir
            if input("💾 ¿Guardar cambios antes de salir? (S/n): ").strip().lower() != 'n':
                manager.save_accounts()
            print("👋 ¡Hasta luego!")
            break
            
        else:
            print("❌ Opción inválida")

if __name__ == "__main__":
    main()