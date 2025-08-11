#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
🎯 ARCHIVO PRINCIPAL DEL CHATBOT DE KICK.COM
Este es el archivo que debes ejecutar: python main_chatbot.py
"""

import os
import asyncio
from datetime import datetime
from kick_chatbot_persistent import PersistentChatbot

def print_banner():
    """Banner de inicio"""
    print("🔥" * 70)
    print("🎯 KICK.COM CHATBOT - CONFIGURACIÓN PERSISTENTE")
    print("🔥" * 70)
    print("✅ 6 cuentas verificadas que envían mensajes reales")
    print("⚙️ Configuración se guarda automáticamente")
    print("💬 Mensajes organizados en archivos separados")
    print("🤖 Comportamiento humano realista")
    print("🔥" * 70)

def show_quick_start():
    """Guía de inicio rápido"""
    print("\n🚀 GUÍA DE INICIO RÁPIDO:")
    print("=" * 50)
    print("1️⃣ Configurar delays (recomendado para principiantes):")
    print("   - Entre mensajes: 3-5 minutos")
    print("   - Bots simultáneos: 2-3 máximo")
    print("   - Mensajes por sesión: 3-5")
    
    print("\n2️⃣ Configuración segura:")
    print("   ✅ Usar solo en canales sin restricciones")
    print("   ✅ Probar en canales pequeños primero")
    print("   ✅ No usar 24/7, solo sesiones cortas")
    
    print("\n3️⃣ Canales recomendados:")
    print("   🎯 apollo2121 (ya probado)")
    print("   🎯 Streamers con <500 viewers")
    print("   🎯 Canales de gaming casual")

def show_presets():
    """Muestra configuraciones predefinidas"""
    print("\n⚙️ CONFIGURACIONES PREDEFINIDAS:")
    print("=" * 50)
    
    presets = {
        "conservador": {
            "desc": "Muy seguro, delays largos",
            "between_messages": "5-8 minutos",
            "concurrent_bots": 2,
            "messages_per_session": 3
        },
        "normal": {
            "desc": "Equilibrado, recomendado",
            "between_messages": "3-5 minutos", 
            "concurrent_bots": 3,
            "messages_per_session": 5
        },
        "agresivo": {
            "desc": "Más rápido, mayor riesgo",
            "between_messages": "2-3 minutos",
            "concurrent_bots": 4,
            "messages_per_session": 8
        }
    }
    
    for name, config in presets.items():
        print(f"🔹 {name.upper()}:")
        print(f"   📝 {config['desc']}")
        print(f"   ⏰ Entre mensajes: {config['between_messages']}")
        print(f"   🤖 Bots simultáneos: {config['concurrent_bots']}")
        print(f"   💬 Mensajes por sesión: {config['messages_per_session']}")
        print()

async def main():
    """Función principal del chatbot"""
    os.system('cls' if os.name == 'nt' else 'clear')  # Limpiar pantalla
    
    print_banner()
    
    # Crear instancia del chatbot
    bot = PersistentChatbot()
    
    # Cargar cuentas
    print("\n📂 Cargando cuentas...")
    if not bot.load_accounts():
        print("❌ ERROR: No se pudieron cargar las cuentas")
        print("💡 Verifica que existe el archivo kicks.json")
        return
    
    if not bot.accounts:
        print("❌ ERROR: No hay cuentas en kicks.json")
        print("💡 Verifica que el archivo kicks.json tenga cuentas válidas")
        return
    
    print(f"✅ Listo! {len(bot.accounts)} cuentas cargadas")
    print(f"💡 Se intentarán usar TODAS las cuentas")
    
    # Menú principal
    while True:
        print("\n" + "🎯" * 50)
        print("🎯 MENÚ PRINCIPAL")
        print("🎯" * 50)
        print("1. 🚀 INICIO RÁPIDO (configuración automática)")
        print("2. 🔥 INICIAR CHATBOT (configuración actual)")
        print("3. 🔄 CHATBOT ROTATIVO (1 mensaje por cuenta, sin límite)")
        print("4. ⚙️ CONFIGURAR DELAYS PERSONALIZADOS")
        print("5. 📊 VER CONFIGURACIÓN ACTUAL")
        print("6. 📈 ESTADÍSTICAS DE CUENTAS")
        print("7. 💡 CONFIGURACIONES PREDEFINIDAS")
        print("8. 🆘 AYUDA Y CONSEJOS")
        print("9. 🛡️ GESTIÓN DE PROXIES")
        print("10. 📤 ENVIAR MENSAJE MANUAL")
        print("0. ❌ SALIR")
        
        choice = input("\n🔢 Selecciona opción: ").strip()
        
        if choice == "1":
            # Inicio rápido con configuración automática
            print("\n🚀 INICIO RÁPIDO")
            print("=" * 40)
            
            channel = input("📺 Canal (ej: nhyrkal): ").strip()
            if not channel:
                print("❌ Canal requerido")
                continue
            
            print("\n⚙️ Configuraciones disponibles:")
            print("1. 🛡️ Conservador (muy seguro)")
            print("2. ⚖️ Normal (recomendado)")
            print("3. ⚡ Agresivo (más rápido)")
            
            preset_choice = input("\n🔢 Selecciona (default: 2): ").strip() or "2"
            
            # Aplicar configuración predefinida
            if preset_choice == "1":
                # Conservador
                bot.config['delays']['between_messages'] = {"min": 300, "max": 480}  # 5-8 min
                bot.config['behavior']['max_concurrent_bots'] = 2
                bot.config['behavior']['messages_per_session'] = 3
                bot.save_config()  # Guardar cambios
                print("🛡️ Configuración CONSERVADORA aplicada y guardada")
                
            elif preset_choice == "3":
                # Agresivo
                bot.config['delays']['between_messages'] = {"min": 120, "max": 180}  # 2-3 min
                bot.config['behavior']['max_concurrent_bots'] = 4
                bot.config['behavior']['messages_per_session'] = 8
                bot.save_config()  # Guardar cambios
                print("⚡ Configuración AGRESIVA aplicada y guardada")
                
            else:
                # Normal (default)
                bot.config['delays']['between_messages'] = {"min": 180, "max": 300}  # 3-5 min
                bot.config['behavior']['max_concurrent_bots'] = 3
                bot.config['behavior']['messages_per_session'] = 5
                bot.save_config()  # Guardar cambios
                print("⚖️ Configuración NORMAL aplicada y guardada")
            
            print(f"\n🎬 ¡Iniciando chatbot en {channel}!")
            await bot.start_chatbot(channel)
        
        elif choice == "2":
            channel = input("📺 Canal: ").strip()
            if channel:
                await bot.start_chatbot(channel)
        
        elif choice == "3":
            print("\n🔄 CHATBOT ROTATIVO")
            print("=" * 40)
            print("♾️ Envía mensajes de forma rotativa:")
            print("   • 1 mensaje por cuenta")
            print("   • 5-10 segundos entre mensajes")
            print("   • Sin límite total de mensajes")
            print("   • Usa todas las 18 cuentas")
            print()
            
            channel = input("📺 Canal: ").strip()
            if channel:
                await bot.start_rotating_chatbot(channel)
        
        elif choice == "4":
            bot.modify_config()
        
        elif choice == "5":
            bot.show_config()
        
        elif choice == "6":
            print(f"\n📈 ESTADÍSTICAS DE TODAS LAS CUENTAS:")
            print("-" * 70)
            for i, acc in enumerate(bot.accounts, 1):
                msgs = acc.get('messages_sent', 0)
                personality = acc['personality']
                last_time = acc.get('last_message_time', 0)
                last_msg = datetime.fromtimestamp(last_time).strftime("%H:%M:%S") if last_time > 0 else "Nunca"
                status = "✅ Activa" if msgs > 0 else "⚠️ Sin envíos"
                line = (
                    f"   {i:2d}. {acc['username']:15} | {personality:12} | {status:12} | "
                    f"Mensajes: {msgs:3d} | Último: {last_msg}"
                )
                print(line)
        
        elif choice == "7":
            show_presets()
        
        elif choice == "8":
            show_quick_start()

        elif choice == "9":
            bot.manage_proxies()

        elif choice == "10":
            channel = input("📺 Canal: ").strip()
            if channel:
                message = input("✉️ Mensaje a enviar: ").strip()
                if message:
                    await bot.start_manual_message(channel, message)

        elif choice == "0":
            print("\n👋 ¡Gracias por usar el chatbot!")
            print("💡 Recuerda: úsalo responsablemente")
            break
        
        else:
            print("❌ Opción inválida")

if __name__ == "__main__":
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        print("\n\n🛑 Programa interrumpido por el usuario")
    except Exception as e:
        print(f"\n💥 Error inesperado: {e}")
        print("🆘 Reporta este error si persiste")