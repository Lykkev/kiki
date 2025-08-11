#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
🔥 KICK.COM CHATBOT - VERSIÓN CON CONFIGURACIÓN PERSISTENTE
Configuración se guarda automáticamente en config.json
Mensajes organizados en messages.json
"""

import json
import asyncio
import aiocurl
import random
import time
from datetime import datetime
import os

class PersistentChatbot:
    def __init__(self):
        self.session = aiocurl.Session()
        self.accounts = []
        self.config = {}
        self.messages = {}
        self.category_messages = {}
        self.greeting_templates = {}
        self.proxies = []
        self.proxy_assignments = {}  # account_id -> proxy
        self.current_category = None

        # Cargar configuración y mensajes
        self.load_config()
        self.load_messages()
        self.load_category_messages()
        self.load_greeting_templates()

        # Cargar proxies
        self.load_proxies()
        
    def load_config(self):
        """Carga configuración persistente desde config.json"""
        try:
            if os.path.exists("config.json"):
                with open("config.json", "r", encoding="utf-8") as f:
                    self.config = json.load(f)
                print("✅ Configuración cargada desde config.json")
            else:
                # Configuración por defecto si no existe el archivo
                self.config = self.get_default_config()
                self.save_config()
                print("📝 Configuración por defecto creada")
        except Exception as e:
            print(f"⚠️ Error cargando config: {e}, usando defaults")
            self.config = self.get_default_config()
    
    def get_default_config(self):
        """Configuración por defecto"""
        return {
            "delays": {
                "initial_wait": {"min": 10, "max": 30},
                "between_messages": {"min": 180, "max": 300},
                "typing_simulation": {"min": 1, "max": 4},
                "between_accounts": 5,
                "batch_pause": {"min": 300, "max": 600}
            },
            "behavior": {
                "max_concurrent_bots": 3,
                "messages_per_session": 5,
                "add_human_variations": True,
                "randomize_order": True,
                "only_emotes": False
            },
            "safety": {
                "respect_rate_limits": True,
                "auto_retry_on_429": True,
                "max_retries_per_account": 3,
                "stop_on_token_error": True
            }
        }
    
    def save_config(self):
        """Guarda configuración en config.json"""
        try:
            with open("config.json", "w", encoding="utf-8") as f:
                json.dump(self.config, f, indent=4, ensure_ascii=False)
            print("💾 Configuración guardada en config.json")
            return True
        except Exception as e:
            print(f"❌ Error guardando config: {e}")
            return False
    
    def load_messages(self):
        """Carga mensajes desde messages.json"""
        try:
            if os.path.exists("messages.json"):
                with open("messages.json", "r", encoding="utf-8") as f:
                    self.messages = json.load(f)
                print("✅ Mensajes cargados desde messages.json")
            else:
                print("⚠️ messages.json no existe, creando archivo por defecto...")
                self.messages = self.get_default_messages()
                self.save_messages()
        except Exception as e:
            print(f"⚠️ Error cargando mensajes: {e}, usando defaults")
            self.messages = self.get_default_messages()

    def load_category_messages(self):
        """Carga frases por categoría desde category_messages.json"""
        try:
            if os.path.exists("category_messages.json"):
                with open("category_messages.json", "r", encoding="utf-8") as f:
                    data = json.load(f)
                # Normalizar claves a minúsculas para comparaciones
                self.category_messages = {k.lower(): v for k, v in data.items()}
                print("✅ Frases por categoría cargadas desde category_messages.json")
            else:
                print("⚠️ category_messages.json no existe, creando archivo por defecto...")
                self.category_messages = self.get_default_category_messages()
                self.save_category_messages()
        except Exception as e:
            print(f"⚠️ Error cargando frases por categoría: {e}, usando defaults")
            self.category_messages = self.get_default_category_messages()

    def save_category_messages(self):
        """Guarda frases por categoría en category_messages.json"""
        try:
            with open("category_messages.json", "w", encoding="utf-8") as f:
                json.dump(self.category_messages, f, indent=4, ensure_ascii=False)
            print("💾 Frases por categoría guardadas en category_messages.json")
        except Exception as e:
            print(f"❌ Error guardando category_messages: {e}")

    def get_default_category_messages(self):
        """Frases por defecto para categorías conocidas"""
        return {
            "valorant": [
                "qué buena ronda 🔥",
                "tremendo aim bro",
                "esa ulti fue dios"
            ],
            "just chatting": [
                "jajaja qué historia",
                "¿y eso en qué acabó?",
                "esto parece una charla de café ☕"
            ],
            "default": [
                "nice jugada",
                "buen movimiento ahí",
                "buena partida"
            ]
        }

    def get_category_messages(self, category_name):
        """Obtiene frases para la categoría dada"""
        if not category_name:
            return None
        return self.category_messages.get(category_name.lower())

    def load_greeting_templates(self):
        """Carga plantillas de saludos y despedidas desde greetings.json"""
        try:
            if os.path.exists("greetings.json"):
                with open("greetings.json", "r", encoding="utf-8") as f:
                    self.greeting_templates = json.load(f)
                print("✅ Plantillas cargadas desde greetings.json")
            else:
                print("⚠️ greetings.json no existe, creando archivo por defecto...")
                self.greeting_templates = self.get_default_greeting_templates()
                self.save_greeting_templates()
        except Exception as e:
            print(f"⚠️ Error cargando greetings.json: {e}, usando defaults")
            self.greeting_templates = self.get_default_greeting_templates()

    def save_greeting_templates(self):
        """Guarda plantillas de saludos y despedidas"""
        try:
            with open("greetings.json", "w", encoding="utf-8") as f:
                json.dump(self.greeting_templates, f, indent=4, ensure_ascii=False)
            print("💾 Plantillas guardadas en greetings.json")
        except Exception as e:
            print(f"❌ Error guardando greetings.json: {e}")

    def get_default_greeting_templates(self):
        """Plantillas por defecto de saludos y despedidas"""
        return {
            "greetings": [
                "holaaa, recién llego",
                "hola a todos",
                "buenas, me sumo",
                "hey, cómo va eso",
                "holi, entrando al chat",
                "qué tal gente",
                "saludos, listo para ver"
            ],
            "farewells": [
                "chau",
                "nos vemos",
                "me voy yendo",
                "hasta luego",
                "adiós, cuídense",
                "me despido, bye",
                "nos leemos luego"
            ]
        }

    def extract_live_category(self, data):
        """Extrae la categoría del video que está en vivo.

        Busca dentro de ``data['videos']`` el objeto cuyo campo
        ``is_live`` sea ``True`` y devuelve el nombre de la primera
        categoría disponible. Si no se encuentra una categoría válida,
        retorna ``None`` para que el sistema utilice frases genéricas.
        """
        if not data:
            return None

        videos = data.get('videos') or []
        for video in videos:
            if video.get('is_live'):
                categories = video.get('categories') or []
                if categories:
                    name = categories[0].get('name')
                    if name:
                        return name
        return None
    
    def load_proxies(self):
        """Carga proxies desde proxies.json"""
        try:
            if os.path.exists("proxies.json"):
                with open("proxies.json", "r", encoding="utf-8") as f:
                    proxy_data = json.load(f)
                    self.proxies = proxy_data.get("proxies", [])
                    self.proxy_settings = proxy_data.get("settings", {})
                print(f"✅ {len(self.proxies)} proxies cargados desde proxies.json")
                
                # Verificar si los proxies están habilitados
                if self.proxy_settings.get("proxy_enabled", True):
                    print("🛡️ Sistema de proxies ACTIVADO")
                else:
                    print("⚠️ Sistema de proxies DESACTIVADO (revisa proxies.json)")
            else:
                print("⚠️ proxies.json no existe - funcionando SIN proxies")
                self.proxies = []
                self.proxy_settings = {"proxy_enabled": False}
        except Exception as e:
            print(f"❌ Error cargando proxies: {e}")
            self.proxies = []
            self.proxy_settings = {"proxy_enabled": False}
    
    def get_available_proxy(self, exclude_ids=None):
        """Obtiene un proxy disponible con menos errores"""
        if not self.proxies or not self.proxy_settings.get("proxy_enabled", False):
            return None
            
        exclude_ids = exclude_ids or []
        available_proxies = [
            p for p in self.proxies 
            if p["active"] and p["id"] not in exclude_ids and 
               p["error_count"] < self.proxy_settings.get("max_error_count", 3)
        ]
        
        if not available_proxies:
            return None
            
        # Usar random o el menos usado
        if self.proxy_settings.get("use_random_proxy", True):
            return random.choice(available_proxies)
        else:
            # Devolver el menos usado
            return min(available_proxies, key=lambda x: x.get("last_used") or 0)
    
    def assign_proxy_to_account(self, account_id):
        """Asigna un proxy a una cuenta usando rotación circular"""
        if not self.proxies or not self.proxy_settings.get("proxy_enabled", False):
            return None
            
        # Si ya tiene proxy asignado, devolverlo
        if account_id in self.proxy_assignments:
            proxy_id = self.proxy_assignments[account_id]
            proxy = next((p for p in self.proxies if p["id"] == proxy_id), None)
            if proxy and proxy["active"] and proxy["error_count"] < 3:
                return proxy
        
        # Obtener proxies activos disponibles
        active_proxies = [
            p for p in self.proxies 
            if p["active"] and p["error_count"] < self.proxy_settings.get("max_error_count", 3)
        ]
        
        if not active_proxies:
            print("⚠️ No hay proxies activos disponibles")
            return None
        
        # NUEVA LÓGICA: Rotación circular de proxies
        # Calcular qué proxy usar basado en el account_id
        account_hash = hash(str(account_id)) % len(active_proxies)
        selected_proxy = active_proxies[account_hash]
        
        # Asignar el proxy
        self.proxy_assignments[account_id] = selected_proxy["id"]
        
        # Contar cuántas cuentas usan este proxy
        accounts_using_proxy = sum(1 for pid in self.proxy_assignments.values() if pid == selected_proxy["id"])
        
        print(f"🔄 Proxy {selected_proxy['host']}:{selected_proxy['port']} asignado a cuenta {account_id}")
        print(f"   📊 Cuentas usando este proxy: {accounts_using_proxy}")
        
        return selected_proxy
    
    def assign_proxy_to_account_unique(self, account_id):
        """Asigna un proxy único a una cuenta (método original)"""
        if not self.proxies or not self.proxy_settings.get("proxy_enabled", False):
            return None
            
        # Si ya tiene proxy asignado, devolverlo
        if account_id in self.proxy_assignments:
            proxy_id = self.proxy_assignments[account_id]
            proxy = next((p for p in self.proxies if p["id"] == proxy_id), None)
            if proxy and proxy["active"] and proxy["error_count"] < 3:
                return proxy
        
        # Obtener proxies ya asignados
        assigned_proxy_ids = list(self.proxy_assignments.values())
        
        # Buscar proxy disponible no asignado
        proxy = self.get_available_proxy(exclude_ids=assigned_proxy_ids)
        if proxy:
            self.proxy_assignments[account_id] = proxy["id"]
            print(f"🛡️ Proxy {proxy['host']}:{proxy['port']} asignado ÚNICAMENTE a cuenta {account_id}")
        
        return proxy
    
    def mark_proxy_error(self, proxy_id, error_msg=""):
        """Marca error en un proxy y lo desactiva si es necesario"""
        for proxy in self.proxies:
            if proxy["id"] == proxy_id:
                proxy["error_count"] += 1
                print(f"⚠️ Error en proxy {proxy['host']}:{proxy['port']} (errores: {proxy['error_count']})")
                
                if proxy["error_count"] >= self.proxy_settings.get("max_error_count", 3):
                    proxy["active"] = False
                    print(f"❌ Proxy {proxy['host']}:{proxy['port']} DESACTIVADO por exceso de errores")
                    
                    # Reasignar cuentas que usaban este proxy
                    accounts_to_reassign = [
                        acc_id for acc_id, p_id in self.proxy_assignments.items() 
                        if p_id == proxy_id
                    ]
                    for acc_id in accounts_to_reassign:
                        del self.proxy_assignments[acc_id]
                        print(f"🔄 Cuenta {acc_id} necesita nuevo proxy")
                break
    
    def get_default_messages(self):
        """Mensajes por defecto si no existe el archivo"""
        return {
            "personalities": {
                "casual": ["nice stream! 😄", "gg wp 👏", "lol 😂", "good play 🎮"],
                "enthusiastic": ["AMAZING! 🤩", "SO GOOD! 🔥", "POGGERS! 😍"],
                "supportive": ["great job! 👏", "you got this! 💪", "keep it up! ⭐"],
                "gamer": ["ez clap 😎", "pogchamp 🎮", "5head play 🧠"],
                "lurker": ["👀", "nice 👍", "gg", "wp"],
                "toxic": ["noob 🙄", "ez game", "git gud"]
            }
        }
    
    def save_messages(self):
        """Guarda mensajes en messages.json"""
        try:
            with open("messages.json", "w", encoding="utf-8") as f:
                json.dump(self.messages, f, indent=4, ensure_ascii=False)
            print("💾 Mensajes guardados en messages.json")
            return True
        except Exception as e:
            print(f"❌ Error guardando mensajes: {e}")
            return False

    def save_account_stats(self):
        """Guarda estadísticas de cuentas en kicks.json"""
        try:
            data = [
                {k: v for k, v in acc.items() if k != 'recent_messages'}
                for acc in self.accounts
            ]
            with open("kicks.json", "w", encoding="utf-8") as f:
                json.dump(data, f, indent=4)
            return True
        except Exception as e:
            print(f"⚠️ No se pudieron guardar estadísticas: {e}")
            return False
    
    def load_accounts(self):
        """Carga TODAS las cuentas del kicks.json"""
        try:
            with open("kicks.json", "r", encoding="utf-8") as f:
                self.accounts = json.load(f)
            
            print(f"📊 CUENTAS CARGADAS: {len(self.accounts)} total")
            print(f"💡 Se intentarán usar TODAS las cuentas")
            print(f"🔧 Si alguna falla, se reportará en consola")
            
            print(f"\n📋 TODAS LAS CUENTAS:")
            for i, acc in enumerate(self.accounts, 1):
                account_type = "🧪 Demo" if acc.get('demo') else "🔑 Real"
                print(f"   {i:2d}. {account_type} {acc['username']} ({acc['personality']})")
            
            return True
            
        except Exception as e:
            print(f"❌ Error cargando cuentas: {e}")
            return False
    
    def show_config(self):
        """Muestra la configuración actual"""
        print(f"\n⚙️ CONFIGURACIÓN ACTUAL:")
        print("=" * 50)
        print(f"🕐 DELAYS:")
        delays = self.config['delays']
        print(f"   ⏰ Espera inicial: {delays['initial_wait']['min']}-{delays['initial_wait']['max']}s")
        print(f"   💬 Entre mensajes: {delays['between_messages']['min']//60}-{delays['between_messages']['max']//60} minutos")
        print(f"   ⌨️ Simulación escritura: {delays['typing_simulation']['min']}-{delays['typing_simulation']['max']}s")
        print(f"   ⏸️ Entre tandas: {delays['batch_pause']['min']//60}-{delays['batch_pause']['max']//60} minutos")
        
        print(f"\n🎭 COMPORTAMIENTO:")
        behavior = self.config['behavior']
        print(f"   🤖 Bots simultáneos: {behavior['max_concurrent_bots']}")
        print(f"   💬 Mensajes por sesión: {behavior['messages_per_session']}")
        print(f"   🎲 Variaciones humanas: {'Sí' if behavior['add_human_variations'] else 'No'}")
        print(f"   🔀 Orden aleatorio: {'Sí' if behavior['randomize_order'] else 'No'}")
        print(f"   🎭 Solo emotes de Kick: {'Sí' if behavior.get('only_emotes', False) else 'No'}")
        
        print(f"\n🛡️ SEGURIDAD:")
        safety = self.config['safety']
        print(f"   ⏰ Respetar rate limits: {'Sí' if safety['respect_rate_limits'] else 'No'}")
        print(f"   🔄 Auto-retry en 429: {'Sí' if safety['auto_retry_on_429'] else 'No'}")
        print(f"   🔢 Max reintentos: {safety['max_retries_per_account']}")
    
    def modify_config(self):
        """Permite modificar la configuración (se guarda automáticamente)"""
        print(f"\n🔧 MODIFICAR CONFIGURACIÓN")
        print("💾 Los cambios se guardan automáticamente")
        print("=" * 50)
        
        while True:
            print(f"\n¿Qué modificar?")
            print(f"1. ⏰ Espera inicial")
            print(f"2. 💬 Delay entre mensajes")
            print(f"3. 🤖 Bots simultáneos")
            print(f"4. 💬 Mensajes por sesión") 
            print(f"5. 🎲 Variaciones humanas")
            print(f"6. 🎭 Solo emotes de Kick")
            print(f"7. 🛡️ Configuración de seguridad")
            print(f"8. 📊 Ver configuración completa")
            print(f"9. 🔄 Resetear a defaults")
            print(f"0. ✅ Finalizar")
            
            choice = input("\n🔢 Opción: ").strip()
            
            if choice == "1":
                try:
                    min_wait = int(input("⏰ Espera inicial mínima (segundos): "))
                    max_wait = int(input("⏰ Espera inicial máxima (segundos): "))
                    self.config['delays']['initial_wait'] = {"min": min_wait, "max": max_wait}
                    self.save_config()
                    print("✅ Actualizado y guardado")
                except:
                    print("❌ Valores inválidos")
            
            elif choice == "2":
                try:
                    min_msg = int(input("💬 Delay mínimo entre mensajes (minutos): ")) * 60
                    max_msg = int(input("💬 Delay máximo entre mensajes (minutos): ")) * 60
                    self.config['delays']['between_messages'] = {"min": min_msg, "max": max_msg}
                    self.save_config()
                    print("✅ Actualizado y guardado")
                except:
                    print("❌ Valores inválidos")
            
            elif choice == "3":
                try:
                    concurrent = int(input(f"🤖 Bots simultáneos (max {len(self.accounts)}): "))
                    self.config['behavior']['max_concurrent_bots'] = min(concurrent, len(self.accounts))
                    self.save_config()
                    print("✅ Actualizado y guardado")
                except:
                    print("❌ Valor inválido")
            
            elif choice == "4":
                try:
                    messages = int(input("💬 Mensajes por sesión: "))
                    self.config['behavior']['messages_per_session'] = messages
                    self.save_config()
                    print("✅ Actualizado y guardado")
                except:
                    print("❌ Valor inválido")
            
            elif choice == "5":
                current = self.config['behavior']['add_human_variations']
                print(f"🎲 Variaciones humanas actualmente: {'Activadas' if current else 'Desactivadas'}")
                toggle = input("¿Cambiar? (s/n): ").strip().lower()
                if toggle in ['s', 'sí', 'si', 'y', 'yes']:
                    self.config['behavior']['add_human_variations'] = not current
                    self.save_config()
                    print("✅ Actualizado y guardado")
            
            elif choice == "6":
                current = self.config['behavior'].get('only_emotes', False)
                print(f"🎭 Solo emotes de Kick actualmente: {'Activado' if current else 'Desactivado'}")
                print("   Cuando está activado, el bot solo enviará emotes de Kick (ratJAM, HYPERCLAPH, etc.)")
                print("   Cuando está desactivado, usa mensajes con personalidades")
                toggle = input("¿Cambiar? (s/n): ").strip().lower()
                if toggle in ['s', 'sí', 'si', 'y', 'yes']:
                    self.config['behavior']['only_emotes'] = not current
                    self.save_config()
                    nuevo_estado = "Activado" if not current else "Desactivado"
                    print(f"✅ Solo emotes de Kick: {nuevo_estado}")
            
            elif choice == "7":
                print(f"\n🛡️ CONFIGURACIÓN DE SEGURIDAD:")
                safety = self.config['safety']
                
                print(f"1. Rate limits: {'Sí' if safety['respect_rate_limits'] else 'No'}")
                print(f"2. Auto-retry: {'Sí' if safety['auto_retry_on_429'] else 'No'}")
                print(f"3. Max reintentos: {safety['max_retries_per_account']}")
                
                sub_choice = input("¿Qué cambiar? (1-3, 0=volver): ").strip()
                if sub_choice == "1":
                    safety['respect_rate_limits'] = not safety['respect_rate_limits']
                    self.save_config()
                    print("✅ Rate limits actualizado")
                elif sub_choice == "2":
                    safety['auto_retry_on_429'] = not safety['auto_retry_on_429']
                    self.save_config()
                    print("✅ Auto-retry actualizado")
                elif sub_choice == "3":
                    try:
                        retries = int(input("Max reintentos: "))
                        safety['max_retries_per_account'] = retries
                        self.save_config()
                        print("✅ Max reintentos actualizado")
                    except:
                        print("❌ Valor inválido")
            
            elif choice == "8":
                self.show_config()
            
            elif choice == "9":
                confirm = input("🔄 ¿Resetear toda la configuración? (sí/no): ").strip().lower()
                if confirm in ['sí', 'si', 'yes']:
                    self.config = self.get_default_config()
                    self.save_config()
                    print("🔄 Configuración reseteada a defaults")
            
            elif choice == "0":
                break
            
            else:
                print("❌ Opción inválida")
    
    def get_personality_messages(self, personality):
        """Obtiene mensajes según personalidad desde messages.json"""
        personalities = self.messages.get('personalities', {})
        return personalities.get(personality, personalities.get('casual', ["nice! 👍"]))
    
    def get_kick_emotes_only(self):
        """Obtiene solo emotes de Kick.com desde messages.json"""
        kick_emotes = self.messages.get('kick_emotes', {})
        all_emotes = []
        
        # Combinar todos los emotes de Kick
        for category, emotes in kick_emotes.items():
            all_emotes.extend(emotes)
        
        # Si no hay emotes de Kick, usar algunos por defecto
        if not all_emotes:
            all_emotes = [
                "[emote:37248:ratJAM]", 
                "[emote:39268:HYPERCLAPH]", 
                "[emote:39284:vibePlz]", 
                "[emote:1730752:emojiAngel]", 
                "[emote:39273:MuteD]"
            ]
        
        return all_emotes
    
    def get_message_for_account(self, account, category=None):
        """Selecciona mensaje evitando repeticiones frecuentes"""

        personality = account['personality']

        def generate_message():
            # Si está en modo only_emotes, siempre usar emotes de Kick
            if self.config['behavior']['only_emotes']:
                kick_emotes = self.get_kick_emotes_only()
                return random.choice(kick_emotes) if kick_emotes else "nice! 👍"

            # ===== NUEVA LÓGICA: PROBABILIDAD DE SOLO EMOTES =====
            PROB_ONLY_EMOTE = 0.5  # 50% probabilidad de enviar solo emote

            # 30% chance de enviar SOLO un emote de Kick (sin texto)
            if random.random() < PROB_ONLY_EMOTE:
                kick_emotes = self.get_kick_emotes_only()
                if kick_emotes:
                    selected_emote = random.choice(kick_emotes)
                    print(f"   🎭 Seleccionado: SOLO emote ({selected_emote})")
                    return selected_emote

            # Obtener frases según categoría o personalidad
            cat_msgs = self.get_category_messages(category) if category else None
            if not cat_msgs:
                cat_msgs = self.category_messages.get('default')

            personality_msgs = self.get_personality_messages(personality)

            # ===== PROBABILIDAD DE USAR MENSAJES DE CATEGORIA =====
            CATEGORY_MESSAGE_PROBABILITY = 0.50  # 50% categoría, 50% personalidad

            use_category = cat_msgs and random.random() < CATEGORY_MESSAGE_PROBABILITY

            if use_category:
                base_message = random.choice(cat_msgs)
            else:
                base_message = random.choice(personality_msgs)

            return self.add_human_variations(base_message)

        # Evitar repetir mensajes recientes
        recent = account.setdefault('recent_messages', [])
        attempts = 0
        final_message = generate_message()
        while final_message in recent and attempts < 5:
            final_message = generate_message()
            attempts += 1

        recent.append(final_message)
        if len(recent) > 5:
            recent.pop(0)

        return final_message
    
    def add_human_variations(self, message):
        """Añade variaciones humanas usando datos de messages.json"""
        if not self.config['behavior']['add_human_variations']:
            return message
        
        # Si solo_emotes está activado, no modificar los emotes de Kick
        if self.config['behavior']['only_emotes']:
            return message
        
        # ===== CONFIGURACIÓN DE PROBABILIDADES =====
        # Puedes cambiar estos valores para ajustar las probabilidades:
        PROB_ADD_EMOJI_OR_EMOTE = 0.50    # 50% chance de agregar emoji o emote
        PROB_EMOJI_VS_EMOTE = 0.25        # 30% chance de emoji vs 70% emote
        PROB_CHANGE_CASE = 0.15           # 15% chance de cambiar MAYÚSCULAS/minúsculas
        PROB_EXTRA_PUNCTUATION = 0.10     # 10% chance de puntuación extra
        # ===========================================
        
        # Emojis desde messages.json
        emojis = self.messages.get('emojis', {})
        all_emojis = emojis.get('positive', []) + emojis.get('neutral', []) + emojis.get('gaming', [])
        
        # Emotes de Kick desde messages.json
        kick_emotes = self.messages.get('kick_emotes', {})
        all_kick_emotes = []
        for category, emotes in kick_emotes.items():
            all_kick_emotes.extend(emotes)
        
        # PROB_CHANGE_CASE chance de cambiar case (SOLO el texto del mensaje)
        if random.random() < PROB_CHANGE_CASE:
            if random.choice([True, False]):
                message = message.upper()
            else:
                message = message.lower()
        
        # PROB_ADD_EMOJI_OR_EMOTE chance de agregar algo al final del mensaje
        if random.random() < PROB_ADD_EMOJI_OR_EMOTE:
            # PROB_EMOJI_VS_EMOTE chance de emoji, (1-PROB_EMOJI_VS_EMOTE) chance de emote de Kick
            if random.random() < PROB_EMOJI_VS_EMOTE and all_emojis:
                message += f" {random.choice(all_emojis)}"
            elif all_kick_emotes:
                message += f" {random.choice(all_kick_emotes)}"
        
        # PROB_EXTRA_PUNCTUATION chance de puntuación extra
        if random.random() < PROB_EXTRA_PUNCTUATION:
            punctuation = self.messages.get('punctuation', ["!", "!!"])
            message += random.choice(punctuation)
        
        return message
    
    async def get_channel_info(self, channel_name, token, account_id=None):
        """Obtiene información del canal con proxy asignado"""
        try:
            headers = {
                "Authorization": f"Bearer {token}",
                "Accept": "application/json",
                "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
            }
            
            # Obtener proxy para la cuenta si está disponible
            proxy_url = None
            if account_id:
                proxy = self.assign_proxy_to_account(account_id)
                if proxy:
                    proxy_url = f"http://{proxy['username']}:{proxy['password']}@{proxy['host']}:{proxy['port']}"
            
            # Preparar parámetros de request
            request_params = {
                "method": "GET",
                "url": f"https://kick.com/api/v2/channels/{channel_name}",
                "headers": headers
            }
            
            # Agregar proxy si está disponible
            if proxy_url:
                request_params["proxy"] = proxy_url
            
            response = await self.session.request(**request_params)
            
            if response.status == "200":
                return json.loads(response.body), None
            else:
                return None, f"Error {response.status}"
                
        except Exception as e:
            return None, f"Excepción: {e}"
    
    async def send_message(self, chatroom_id, message, account):
        """Envía mensaje con una cuenta usando proxy asignado"""
        account_id = account.get('user_id', account.get('username', 'unknown'))
        proxy_used = None
        
        try:
            headers = {
                "accept": "application/json",
                "authorization": f"Bearer {account['token']}",
                "content-type": "application/json",
                "user-agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
            }
            
            data = {
                "content": message,
                "type": "message"
            }
            
            # Obtener proxy asignado para esta cuenta
            proxy = self.assign_proxy_to_account(account_id)
            proxy_url = None
            
            if proxy:
                proxy_url = f"http://{proxy['username']}:{proxy['password']}@{proxy['host']}:{proxy['port']}"
                proxy_used = f"{proxy['host']}:{proxy['port']}"
                # Actualizar tiempo de último uso
                proxy['last_used'] = int(time.time())
            
            # Preparar parámetros de request
            request_params = {
                "method": "POST",
                "url": f"https://kick.com/api/v2/messages/send/{chatroom_id}",
                "headers": headers,
                "data": json.dumps(data)
            }
            
            # Agregar proxy si está disponible
            if proxy_url:
                request_params["proxy"] = proxy_url
            
            response = await self.session.request(**request_params)
            
            if response.status == "200":
                response_data = json.loads(response.body)
                # Verificar que el mensaje se envió realmente
                if "data" in response_data and "content" in response_data["data"]:
                    return True, response.status, response_data, proxy_used
                else:
                    return False, "Empty response", response_data, proxy_used
            else:
                # Marcar error en el proxy si es un error de conexión
                if proxy and response.status in ["407", "502", "503", "504"]:
                    self.mark_proxy_error(proxy["id"], f"HTTP {response.status}")
                return False, response.status, response.body, proxy_used
                
        except Exception as e:
            # Marcar error en el proxy si hay excepción de conexión
            if proxy_used and proxy and ("Connection" in str(e) or "timeout" in str(e).lower()):
                self.mark_proxy_error(proxy["id"], str(e))
            return False, "Exception", str(e), proxy_used
    
    async def bot_worker(self, account, channel_name, chatroom_id, bot_id, fixed_message=None, max_messages_override=None):
        """Worker individual para cada bot"""
        username = account['username']
        personality = account['personality']

        # Mostrar modo de mensajes
        if fixed_message is not None:
            print(f"🎭 {username} enviando mensaje personalizado")
        elif self.config['behavior']['only_emotes']:
            print(f"🎭 {username} usando SOLO emotes de Kick")
        else:
            print(f"🎭 {username} usando personalidad: {personality} (con 15% chance de solo emotes)")

        max_messages = max_messages_override or self.config['behavior']['messages_per_session']
        
        print(f"🤖 Bot {bot_id}: {username} ({personality}) iniciado")
        
        sent_count = 0
        retry_count = 0
        max_retries = self.config['safety']['max_retries_per_account']
        
        try:
            # Espera inicial
            initial_delay = random.randint(
                self.config['delays']['initial_wait']['min'],
                self.config['delays']['initial_wait']['max']
            )
            print(f"⏳ {username} esperando {initial_delay}s...")
            await asyncio.sleep(initial_delay)

            while sent_count < max_messages and retry_count < max_retries:
                # Seleccionar mensaje fijo o generado automáticamente
                if fixed_message is not None:
                    final_message = fixed_message
                else:
                    # NUEVA LÓGICA: Usar la función mejorada para seleccionar mensaje
                    final_message = self.get_message_for_account(account, self.current_category)
                
                # Simular escritura
                typing_time = random.uniform(
                    self.config['delays']['typing_simulation']['min'],
                    self.config['delays']['typing_simulation']['max']
                )
                print(f"⌨️ {username} escribiendo '{final_message}'... ({typing_time:.1f}s)")
                await asyncio.sleep(typing_time)
                
                # Enviar mensaje
                success, status, response, proxy_used = await self.send_message(chatroom_id, final_message, account)
                
                if success:
                    sent_count += 1
                    retry_count = 0  # Reset retry count on success
                    proxy_info = f" via {proxy_used}" if proxy_used else " (Sin proxy)"
                    print(f"✅ [{username}]: {final_message}{proxy_info}")
                    print(f"🎉 Mensaje {sent_count}/{max_messages} enviado")

                    # Actualizar estadísticas
                    account['messages_sent'] = account.get('messages_sent', 0) + 1
                    account['last_message_time'] = time.time()
                    self.save_account_stats()

                else:
                    retry_count += 1
                    proxy_info = f" via {proxy_used}" if proxy_used else " (Sin proxy)"
                    print(f"❌ {username} FALLÓ (intento {retry_count}/{max_retries}) - Status: {status}{proxy_info}")
                    
                    # Reportar error específico
                    if "401" in str(status):
                        print(f"   🔑 Causa: Token inválido o expirado")
                        print(f"   💡 Solución: Actualizar token de {username}")
                        break  # No intentar más con este token
                    elif "403" in str(status):
                        print(f"   🚫 Causa: Sin permisos (baneado o restricciones)")
                        print(f"   💡 Solución: Verificar que {username} pueda escribir en este canal")
                        break  # No intentar más en este canal
                    elif "404" in str(status):
                        print(f"   🔍 Causa: Canal/chatroom no encontrado")
                        print(f"   💡 Solución: Verificar nombre del canal")
                        break  # Error de canal, no de cuenta
                    elif "429" in str(status):
                        if self.config['safety']['auto_retry_on_429']:
                            print(f"   ⏰ Causa: Rate limiting")
                            print(f"   💡 Solución: Esperando 2 minutos...")
                            await asyncio.sleep(120)
                            continue  # Reintentar después del delay
                        else:
                            print(f"   ⏰ Rate limiting - auto-retry desactivado")
                            break
                    elif "Empty response" in str(status):
                        print(f"   📭 Causa: Respuesta vacía (token puede estar restringido)")
                        print(f"   💡 Solución: Verificar permisos de {username} en este canal")
                        if retry_count >= max_retries:
                            break
                    else:
                        print(f"   ❓ Causa desconocida: {response}")
                        print(f"   💡 Reintentando... ({retry_count}/{max_retries})")
                        if retry_count >= max_retries:
                            break
                
                # Espera entre mensajes
                if sent_count < max_messages:
                    wait_time = random.randint(
                        self.config['delays']['between_messages']['min'],
                        self.config['delays']['between_messages']['max']
                    )
                    print(f"💤 {username} descansando {wait_time//60}m {wait_time%60}s...")
                    await asyncio.sleep(wait_time)
                    
        except Exception as e:
            print(f"❌ Error en bot {username}: {e}")

        print(f"🏁 Bot {username} completado. Mensajes: {sent_count}")

    async def send_greeting_messages(self, chatroom_id):
        """Envía varios mensajes de saludo antes de iniciar"""
        greetings = self.greeting_templates.get("greetings", [])
        if not greetings:
            return

        count = random.randint(10, 15)
        accounts = self.accounts.copy()
        if self.config['behavior']['randomize_order']:
            random.shuffle(accounts)

        index = 0
        for i in range(count):
            account = accounts[index]
            username = account['username']
            message = random.choice(greetings)

            typing_time = random.uniform(
                self.config['delays']['typing_simulation']['min'],
                self.config['delays']['typing_simulation']['max']
            )
            print(f"👋 {username} saludando '{message}'... ({typing_time:.1f}s)")
            await asyncio.sleep(typing_time)

            success, status, response, proxy_used = await self.send_message(chatroom_id, message, account)
            proxy_info = f" via {proxy_used}" if proxy_used else " (Sin proxy)"
            if success:
                account['messages_sent'] = account.get('messages_sent', 0) + 1
                account['last_message_time'] = time.time()
                self.save_account_stats()
                print(f"✅ [{username}]: {message}{proxy_info}")
            else:
                print(f"❌ {username} FALLÓ al saludar - Status: {status}{proxy_info}")

            index = (index + 1) % len(accounts)
            if i < count - 1:
                await asyncio.sleep(random.randint(2, 5))

    async def send_farewell_messages(self, chatroom_id):
        """Envía mensajes de despedida antes de terminar"""
        farewells = self.greeting_templates.get("farewells", [])
        if not farewells:
            return

        count = random.randint(10, 15)
        accounts = self.accounts.copy()
        if self.config['behavior']['randomize_order']:
            random.shuffle(accounts)

        index = 0
        for i in range(count):
            account = accounts[index]
            username = account['username']
            message = random.choice(farewells)

            typing_time = random.uniform(
                self.config['delays']['typing_simulation']['min'],
                self.config['delays']['typing_simulation']['max']
            )
            print(f"👋 {username} despidiéndose '{message}'... ({typing_time:.1f}s)")
            await asyncio.sleep(typing_time)

            success, status, response, proxy_used = await self.send_message(chatroom_id, message, account)
            proxy_info = f" via {proxy_used}" if proxy_used else " (Sin proxy)"
            if success:
                account['messages_sent'] = account.get('messages_sent', 0) + 1
                account['last_message_time'] = time.time()
                self.save_account_stats()
                print(f"✅ [{username}]: {message}{proxy_info}")
            else:
                print(f"❌ {username} FALLÓ al despedirse - Status: {status}{proxy_info}")

            index = (index + 1) % len(accounts)
            if i < count - 1:
                await asyncio.sleep(random.randint(2, 5))

    async def start_chatbot(self, channel_name):
        """Inicia el chatbot con TODAS las cuentas del kicks.json"""
        print(f"🔥 INICIANDO CHATBOT PERSISTENTE")
        print("=" * 60)
        print(f"📺 Canal: {channel_name}")
        print(f"🤖 Cuentas a usar: {len(self.accounts)}")
        print(f"⚙️ Bots simultáneos: {self.config['behavior']['max_concurrent_bots']}")
        print("=" * 60)
        
        # Obtener info del canal (usar primera cuenta disponible)
        channel_info, error = await self.get_channel_info(channel_name, self.accounts[0]['token'])
        
        if error:
            print(f"❌ Error: {error}")
            return
        
        chatroom_id = channel_info.get('chatroom', {}).get('id')
        if not chatroom_id:
            print("❌ No se pudo obtener chatroom ID")
            return
        
        print(f"✅ Canal: {channel_name} (ID: {channel_info.get('id')})")
        print(f"💬 Chatroom ID: {chatroom_id}")
        print(f"🔴 En vivo: {'Sí' if channel_info.get('livestream') else 'No'}")
        # Intentar obtener la categoría a partir del video en vivo
        category_source = None
        category_name = None
        livestream = channel_info.get('livestream') or {}

        category = livestream.get('category') or {}
        category_name = category.get('name') or category.get('slug')
        if category_name:
            category_source = 'livestream.category'
        else:
            categories = livestream.get('categories') or []
            if categories:
                category_name = categories[0].get('name') or categories[0].get('slug')
                if category_name:
                    category_source = 'livestream.categories[0]'

        if not category_name:
            recent_categories = channel_info.get('recent_categories') or []
            if recent_categories:
                category_name = recent_categories[0].get('name') or recent_categories[0].get('slug')
                if category_name:
                    category_source = 'recent_categories[0]'

        if category_name:
            self.current_category = category_name.lower()
            print(f"🎮 Categoría: {category_name} ({category_source})")
        else:
            self.current_category = None
            print("🎮 Categoría: desconocida")
        
        # Preparar cuentas - USAR TODAS
        accounts_to_use = self.accounts.copy()
        if self.config['behavior']['randomize_order']:
            random.shuffle(accounts_to_use)

        print(f"\n🎬 ¡CHATBOT EN ACCIÓN!")
        print(f"⚠️ Presiona Ctrl+C para detener")
        print("-" * 60)

        try:
            await self.send_greeting_messages(chatroom_id)

            concurrent = self.config['behavior']['max_concurrent_bots']

            # Procesar cuentas en tandas
            for i in range(0, len(accounts_to_use), concurrent):
                batch = accounts_to_use[i:i + concurrent]
                batch_num = (i // concurrent) + 1

                print(f"\n🚀 TANDA {batch_num}: {len(batch)} bots")

                tasks = []
                for j, account in enumerate(batch):
                    bot_id = f"bot_{i+j+1}"
                    task = asyncio.create_task(
                        self.bot_worker(account, channel_name, chatroom_id, bot_id)
                    )
                    tasks.append(task)

                # Esperar que termine esta tanda
                await asyncio.gather(*tasks)

                # Pausa entre tandas
                if i + concurrent < len(accounts_to_use):
                    pause_time = random.randint(
                        self.config['delays']['batch_pause']['min'],
                        self.config['delays']['batch_pause']['max']
                    )
                    print(f"⏸️ Pausa entre tandas: {pause_time//60} minutos")
                    await asyncio.sleep(pause_time)

        except KeyboardInterrupt:
            print("\n🛑 Chatbot detenido por el usuario")
        finally:
            await self.send_farewell_messages(chatroom_id)
        
        # Guardar estadísticas
        try:
            with open("kicks.json", "w") as f:
                json.dump(self.accounts, f, indent=4)
            print("💾 Estadísticas guardadas")
        except:
            print("⚠️ No se pudieron guardar estadísticas")
        
        # Resumen detallado por cuenta
        successful_accounts = []
        failed_accounts = []
        total_sent = 0
        
        for acc in self.accounts:
            msgs_sent = acc.get('messages_sent', 0)
            total_sent += msgs_sent
            
            if msgs_sent > 0:
                successful_accounts.append(f"{acc['username']} ({msgs_sent} msgs)")
            else:
                failed_accounts.append(acc['username'])
        
        print(f"\n📊 RESUMEN FINAL:")
        print(f"   💬 Total mensajes enviados: {total_sent}")
        print(f"   🤖 Cuentas intentadas: {len(self.accounts)}")
        print(f"   ✅ Cuentas exitosas: {len(successful_accounts)}")
        print(f"   ❌ Cuentas fallidas: {len(failed_accounts)}")
        
        if successful_accounts:
            print(f"\n✅ CUENTAS QUE FUNCIONARON:")
            for acc in successful_accounts:
                print(f"   🟢 {acc}")
        
        if failed_accounts:
            print(f"\n❌ CUENTAS QUE FALLARON:")
            for acc in failed_accounts:
                print(f"   🔴 {acc}")

    async def start_manual_message(self, channel_name, message):
        """Envía un mensaje fijo con todas las cuentas"""
        print(f"📤 INICIANDO ENVÍO MANUAL")
        print("=" * 60)
        print(f"📺 Canal: {channel_name}")
        print(f"🤖 Cuentas a usar: {len(self.accounts)}")
        print("=" * 60)

        channel_info, error = await self.get_channel_info(channel_name, self.accounts[0]['token'])
        if error:
            print(f"❌ Error: {error}")
            return

        chatroom_id = channel_info.get('chatroom', {}).get('id')
        if not chatroom_id:
            print("❌ No se pudo obtener chatroom ID")
            return

        accounts_to_use = self.accounts.copy()
        if self.config['behavior']['randomize_order']:
            random.shuffle(accounts_to_use)

        try:
            concurrent = self.config['behavior']['max_concurrent_bots']
            for i in range(0, len(accounts_to_use), concurrent):
                batch = accounts_to_use[i:i + concurrent]
                batch_num = (i // concurrent) + 1
                print(f"\n🚀 TANDA {batch_num}: {len(batch)} bots")

                tasks = []
                for j, account in enumerate(batch):
                    bot_id = f"bot_{i+j+1}"
                    task = asyncio.create_task(
                        self.bot_worker(account, channel_name, chatroom_id, bot_id, fixed_message=message, max_messages_override=1)
                    )
                    tasks.append(task)

                await asyncio.gather(*tasks)

                if i + concurrent < len(accounts_to_use):
                    pause_time = random.randint(
                        self.config['delays']['batch_pause']['min'],
                        self.config['delays']['batch_pause']['max']
                    )
                    print(f"⏸️ Pausa entre tandas: {pause_time//60} minutos")
                    await asyncio.sleep(pause_time)

        except KeyboardInterrupt:
            print("\n🛑 Envío manual detenido por el usuario")

        # Guardar estadísticas
        try:
            with open("kicks.json", "w") as f:
                json.dump(self.accounts, f, indent=4)
            print("💾 Estadísticas guardadas")
        except:
            print("⚠️ No se pudieron guardar estadísticas")

        successful_accounts = []
        failed_accounts = []
        total_sent = 0

        for acc in self.accounts:
            msgs_sent = acc.get('messages_sent', 0)
            total_sent += msgs_sent
            if msgs_sent > 0:
                successful_accounts.append(f"{acc['username']} ({msgs_sent} msgs)")
            else:
                failed_accounts.append(acc['username'])

        print(f"\n📊 RESUMEN FINAL:")
        print(f"   💬 Total mensajes enviados: {total_sent}")
        print(f"   🤖 Cuentas intentadas: {len(self.accounts)}")
        print(f"   ✅ Cuentas exitosas: {len(successful_accounts)}")
        print(f"   ❌ Cuentas fallidas: {len(failed_accounts)}")

        if successful_accounts:
            print(f"\n✅ CUENTAS QUE FUNCIONARON:")
            for acc in successful_accounts:
                print(f"   🟢 {acc}")

        if failed_accounts:
            print(f"\n❌ CUENTAS QUE FALLARON:")
            for acc in failed_accounts:
                print(f"   🔴 {acc}")

    async def start_rotating_chatbot(self, channel_name):
        """Inicia el chatbot con envío rotativo continuo - 1 mensaje por cuenta, sin límite total"""
        print(f"🔄 INICIANDO CHATBOT ROTATIVO CONTINUO")
        print("=" * 60)
        print(f"📺 Canal: {channel_name}")
        print(f"🤖 Cuentas a usar: {len(self.accounts)}")
        print(f"⏰ Intervalo entre mensajes: 5-10 segundos")
        print(f"♾️ Sin límite total de mensajes")
        print("=" * 60)
        
        # Obtener info del canal (usar primera cuenta disponible)
        channel_info, error = await self.get_channel_info(channel_name, self.accounts[0]['token'])
        
        if error:
            print(f"❌ Error: {error}")
            return
        
        chatroom_id = channel_info.get('chatroom', {}).get('id')
        if not chatroom_id:
            print("❌ No se pudo obtener chatroom ID")
            return
        
        print(f"✅ Canal: {channel_name} (ID: {channel_info.get('id')})")
        print(f"💬 Chatroom ID: {chatroom_id}")
        print(f"🔴 En vivo: {'Sí' if channel_info.get('livestream') else 'No'}")

        category_source = None
        category_name = None
        livestream = channel_info.get('livestream') or {}

        category = livestream.get('category') or {}
        category_name = category.get('name') or category.get('slug')
        if category_name:
            category_source = 'livestream.category'
        else:
            categories = livestream.get('categories') or []
            if categories:
                category_name = categories[0].get('name') or categories[0].get('slug')
                if category_name:
                    category_source = 'livestream.categories[0]'

        if not category_name:
            recent_categories = channel_info.get('recent_categories') or []
            if recent_categories:
                category_name = recent_categories[0].get('name') or recent_categories[0].get('slug')
                if category_name:
                    category_source = 'recent_categories[0]'

        if category_name:
            self.current_category = category_name.lower()
            print(f"🎮 Categoría: {category_name} ({category_source})")
        else:
            self.current_category = None
            print("🎮 Categoría: desconocida")

        # Preparar cuentas - USAR TODAS
        accounts_to_use = self.accounts.copy()
        if self.config['behavior']['randomize_order']:
            random.shuffle(accounts_to_use)

        print(f"\n🔄 ¡CHATBOT ROTATIVO EN ACCIÓN!")
        print(f"⚠️ Presiona Ctrl+C para detener")
        print("-" * 60)

        # Contador global de mensajes
        total_messages_sent = 0
        current_account_index = 0

        try:
            await self.send_greeting_messages(chatroom_id)

            while True:  # Bucle infinito - sin límite total
                # Obtener la cuenta actual
                account = accounts_to_use[current_account_index]
                username = account['username']
                personality = account['personality']
                
                # NUEVA LÓGICA: Usar la función mejorada para seleccionar mensaje
                final_message = self.get_message_for_account(account, self.current_category)
                
                # Simular escritura
                typing_time = random.uniform(
                    self.config['delays']['typing_simulation']['min'],
                    self.config['delays']['typing_simulation']['max']
                )
                print(f"⌨️ {username} escribiendo '{final_message}'... ({typing_time:.1f}s)")
                await asyncio.sleep(typing_time)
                
                # Enviar mensaje
                success, status, response, proxy_used = await self.send_message(chatroom_id, final_message, account)
                
                if success:
                    total_messages_sent += 1
                    proxy_info = f" via {proxy_used}" if proxy_used else " (Sin proxy)"
                    print(f"✅ [{username}]: {final_message}{proxy_info}")
                    print(f"🎉 Mensaje #{total_messages_sent} enviado por {username}")
                    
                    # Actualizar estadísticas
                    account['messages_sent'] = account.get('messages_sent', 0) + 1
                    account['last_message_time'] = time.time()
                    self.save_account_stats()

                else:
                    proxy_info = f" via {proxy_used}" if proxy_used else " (Sin proxy)"
                    print(f"❌ {username} FALLÓ - Status: {status}{proxy_info}")
                    
                    # Reportar error específico
                    if "401" in str(status):
                        print(f"   🔑 Causa: Token inválido o expirado")
                        print(f"   💡 Solución: Actualizar token de {username}")
                    elif "403" in str(status):
                        print(f"   🚫 Causa: Sin permisos (baneado o restricciones)")
                        print(f"   💡 Solución: Verificar que {username} pueda escribir en este canal")
                    elif "404" in str(status):
                        print(f"   🔍 Causa: Canal/chatroom no encontrado")
                        print(f"   💡 Solución: Verificar nombre del canal")
                        break  # Error de canal, no de cuenta
                    elif "429" in str(status):
                        print(f"   ⏰ Causa: Rate limiting")
                        print(f"   💡 Solución: Esperando 2 minutos...")
                        await asyncio.sleep(120)
                    elif "Empty response" in str(status):
                        print(f"   📭 Causa: Respuesta vacía (token puede estar restringido)")
                        print(f"   💡 Solución: Verificar permisos de {username} en este canal")
                    else:
                        print(f"   ❓ Causa desconocida: {response}")
                
                # Mover al siguiente account
                current_account_index = (current_account_index + 1) % len(accounts_to_use)
                
                # Espera entre mensajes (1-5 segundos)
                wait_time = random.randint(1, 5)
                next_username = accounts_to_use[current_account_index]['username']
                print(f"⏳ Esperando {wait_time}s... Próximo: {next_username}")
                await asyncio.sleep(wait_time)
                    
        except KeyboardInterrupt:
            print("\n🛑 Chatbot rotativo detenido por el usuario")
        finally:
            await self.send_farewell_messages(chatroom_id)

        # Guardar estadísticas
        try:
            with open("kicks.json", "w") as f:
                json.dump(self.accounts, f, indent=4)
            print("💾 Estadísticas guardadas")
        except:
            print("⚠️ No se pudieron guardar estadísticas")
        
        # Resumen detallado por cuenta
        successful_accounts = []
        failed_accounts = []
        total_sent = 0
        
        for acc in self.accounts:
            msgs_sent = acc.get('messages_sent', 0)
            total_sent += msgs_sent
            
            if msgs_sent > 0:
                successful_accounts.append(f"{acc['username']} ({msgs_sent} msgs)")
            else:
                failed_accounts.append(acc['username'])
        
        print(f"\n📊 RESUMEN FINAL DEL CHATBOT ROTATIVO:")
        print(f"   💬 Total mensajes enviados: {total_sent}")
        print(f"   🤖 Cuentas intentadas: {len(self.accounts)}")
        print(f"   ✅ Cuentas exitosas: {len(successful_accounts)}")
        print(f"   ❌ Cuentas fallidas: {len(failed_accounts)}")
        
        if successful_accounts:
            print(f"\n✅ CUENTAS QUE FUNCIONARON:")
            for acc in successful_accounts:
                print(f"   🟢 {acc}")
        
        if failed_accounts:
            print(f"\n❌ CUENTAS QUE FALLARON:")
            for acc in failed_accounts:
                print(f"   🔴 {acc}")

    def show_proxy_distribution(self):
        """Muestra la distribución actual de proxies entre cuentas"""
        if not self.proxies or not self.proxy_assignments:
            print("📊 No hay asignaciones de proxy para mostrar")
            return
        
        print("\n📊 DISTRIBUCIÓN DE PROXIES:")
        print("=" * 60)
        
        # Contar cuentas por proxy
        proxy_usage = {}
        for account_id, proxy_id in self.proxy_assignments.items():
            if proxy_id not in proxy_usage:
                proxy_usage[proxy_id] = []
            proxy_usage[proxy_id].append(account_id)
        
        # Mostrar estadísticas por proxy
        total_accounts = len(self.proxy_assignments)
        active_proxies = 0
        
        for proxy in self.proxies:
            if proxy["id"] in proxy_usage:
                accounts = proxy_usage[proxy["id"]]
                active_proxies += 1
                status = "🟢" if proxy["active"] else "🔴"
                print(f"{status} Proxy {proxy['id']:2d} | {proxy['host']:15}:{proxy['port']} | {len(accounts):2d} cuentas")
                
                # Mostrar primeras cuentas como ejemplo
                if len(accounts) <= 3:
                    account_list = ", ".join(str(acc) for acc in accounts)
                else:
                    account_list = f"{', '.join(str(acc) for acc in accounts[:3])}, +{len(accounts)-3} más"
                print(f"           └─ Cuentas: {account_list}")
        
        # Estadísticas generales
        avg_accounts_per_proxy = total_accounts / active_proxies if active_proxies > 0 else 0
        print(f"\n📈 ESTADÍSTICAS:")
        print(f"   Total cuentas asignadas: {total_accounts}")
        print(f"   Proxies en uso: {active_proxies}")
        print(f"   Promedio cuentas por proxy: {avg_accounts_per_proxy:.1f}")
        
        # Mostrar proxies sin usar
        unused_proxies = [p for p in self.proxies if p["id"] not in proxy_usage and p["active"]]
        if unused_proxies:
            print(f"   Proxies disponibles sin usar: {len(unused_proxies)}")
    
    def set_proxy_mode(self, mode="shared"):
        """Establece el modo de asignación de proxies"""
        if mode == "shared":
            print("🔄 Modo COMPARTIDO activado - Los proxies se reutilizan entre cuentas")
            # El método assign_proxy_to_account ya usa rotación circular
        elif mode == "unique":
            print("🛡️ Modo ÚNICO activado - Cada cuenta tendrá su proxy exclusivo")
            # Usar assign_proxy_to_account_unique cuando sea necesario
            
    def reassign_all_proxies(self, mode="shared"):
        """Reasigna todos los proxies según el modo especificado"""
        print(f"\n🔄 Reasignando todos los proxies en modo {mode.upper()}...")
        
        # Guardar las cuentas que tenían proxies
        accounts_with_proxies = list(self.proxy_assignments.keys())
        
        # Limpiar asignaciones actuales
        self.proxy_assignments.clear()
        
        # Reasignar según el modo
        if mode == "shared":
            for account_id in accounts_with_proxies:
                proxy = self.assign_proxy_to_account(account_id)
                if not proxy:
                    print(f"⚠️ No se pudo reasignar proxy a cuenta {account_id}")
        elif mode == "unique":
            for account_id in accounts_with_proxies:
                proxy = self.assign_proxy_to_account_unique(account_id)
                if not proxy:
                    print(f"⚠️ No se pudo reasignar proxy único a cuenta {account_id}")
        
        print("✅ Reasignación completada")
        self.show_proxy_distribution()
    
    def manage_proxies(self):
        """Menú de gestión de proxies"""
        while True:
            print("\n🛡️ GESTIÓN DE PROXIES")
            print("=" * 50)
            print("1. 📊 Ver distribución actual")
            print("2. 🔄 Cambiar a modo COMPARTIDO (recomendado)")
            print("3. 🛡️ Cambiar a modo ÚNICO (máximo 20 cuentas)")
            print("4. 🔄 Reasignar todos los proxies")
            print("5. 📈 Estadísticas detalladas")
            print("6. 📊 Estadísticas de tipos de mensajes")
            print("0. ⬅️ Volver al menú principal")
            
            choice = input("\n🔢 Selecciona opción: ").strip()
            
            if choice == "1":
                self.show_proxy_distribution()
            
            elif choice == "2":
                self.set_proxy_mode("shared")
                self.reassign_all_proxies("shared")
            
            elif choice == "3":
                print("\n⚠️ ADVERTENCIA: Modo ÚNICO")
                print("   Solo las primeras 20 cuentas tendrán proxy")
                print("   Las restantes usarán tu IP directa")
                confirm = input("\n¿Continuar? (s/N): ").strip().lower()
                if confirm == 's':
                    self.set_proxy_mode("unique")
                    self.reassign_all_proxies("unique")
            
            elif choice == "4":
                print("\n🔄 REASIGNAR PROXIES")
                print("1. Modo compartido (todas las cuentas protegidas)")
                print("2. Modo único (máximo 20 cuentas)")
                mode_choice = input("Selecciona modo (1/2): ").strip()
                
                if mode_choice == "1":
                    self.reassign_all_proxies("shared")
                elif mode_choice == "2":
                    self.reassign_all_proxies("unique")
                else:
                    print("❌ Opción inválida")
            
            elif choice == "5":
                self.show_detailed_proxy_stats()
            
            elif choice == "6":
                self.show_message_type_stats()
            
            elif choice == "0":
                break
            
            else:
                print("❌ Opción inválida")
    
    def show_detailed_proxy_stats(self):
        """Muestra estadísticas detalladas de proxies"""
        print("\n📊 ESTADÍSTICAS DETALLADAS DE PROXIES")
        print("=" * 60)
        
        if not self.proxies:
            print("❌ No hay proxies configurados")
            return
        
        total_proxies = len(self.proxies)
        active_proxies = len([p for p in self.proxies if p["active"]])
        inactive_proxies = total_proxies - active_proxies
        
        # Proxies por ubicación
        locations = {}
        for proxy in self.proxies:
            loc = proxy.get("location", "Unknown")
            if loc not in locations:
                locations[loc] = {"total": 0, "active": 0}
            locations[loc]["total"] += 1
            if proxy["active"]:
                locations[loc]["active"] += 1
        
        print(f"🌍 DISTRIBUCIÓN GEOGRÁFICA:")
        for location, counts in locations.items():
            print(f"   {location}: {counts['active']}/{counts['total']} activos")
        
        # Cuentas vs Proxies
        total_accounts = len(self.accounts) if hasattr(self, 'accounts') else 0
        accounts_with_proxy = len(self.proxy_assignments)
        
        print(f"\n📈 RESUMEN GENERAL:")
        print(f"   Total proxies: {total_proxies}")
        print(f"   Proxies activos: {active_proxies}")
        print(f"   Proxies inactivos: {inactive_proxies}")
        print(f"   Total cuentas: {total_accounts}")
        print(f"   Cuentas con proxy: {accounts_with_proxy}")
        print(f"   Cuentas sin proxy: {total_accounts - accounts_with_proxy}")
        
        if active_proxies > 0 and total_accounts > 0:
            coverage = (accounts_with_proxy / total_accounts) * 100
            accounts_per_proxy = accounts_with_proxy / active_proxies
            print(f"   Cobertura de proxies: {coverage:.1f}%")
            print(f"   Promedio cuentas/proxy: {accounts_per_proxy:.1f}")
            
            if total_accounts > active_proxies:
                print(f"\n💡 RECOMENDACIÓN:")
                print(f"   Tienes {total_accounts} cuentas y {active_proxies} proxies")
                print(f"   Usa el modo COMPARTIDO para proteger todas las cuentas")
            else:
                print(f"\n✅ SITUACIÓN IDEAL:")
                print(f"   Tienes suficientes proxies para todas las cuentas")
    
    def show_message_type_stats(self):
        """Muestra estadísticas de tipos de mensajes"""
        print("\n📊 ESTADÍSTICAS DE TIPOS DE MENSAJES")
        print("=" * 60)
        
        # Obtener configuración actual
        only_emotes = self.config['behavior'].get('only_emotes', False)
        
        if only_emotes:
            print("🎭 MODO ACTUAL: Solo emotes de Kick (100%)")
            print("   Todos los mensajes son emotes de Kick")
        else:
            print("🎭 MODO ACTUAL: Mensajes mixtos")
            print("   📝 85% - Mensajes de personalidad (+ posibles variaciones)")
            print("   🎭 15% - Solo emotes de Kick")
            print()
            print("📋 DESGLOSE DE VARIACIONES EN MENSAJES DE PERSONALIDAD:")
            print("   📝 70% - Solo texto de personalidad")
            print("   📝 18% - Texto + emoji normal (😄, 🔥, etc.)")
            print("   📝 12% - Texto + emote de Kick")
            print("   📝 15% - Cambio de mayúsculas/minúsculas")
            print("   📝 10% - Puntuación extra")
        
        # Contar emotes disponibles
        kick_emotes = self.messages.get('kick_emotes', {})
        total_emotes = sum(len(emotes) for emotes in kick_emotes.values())
        
        print(f"\n🎭 EMOTES DE KICK DISPONIBLES: {total_emotes} total")
        for category, emotes in kick_emotes.items():
            print(f"   {category}: {len(emotes)} emotes")
        
        # Mostrar configuración de probabilidades
        print(f"\n⚙️ CONFIGURACIÓN ACTUAL:")
        print(f"   Solo emotes de Kick: {'Activado' if only_emotes else 'Desactivado'}")
        print(f"   Probabilidad solo emotes: 15%")
        print(f"   Variaciones humanas: {'Activadas' if self.config['behavior']['add_human_variations'] else 'Desactivadas'}")
    
    def configure_emote_probability(self):
        """Permite configurar la probabilidad de solo emotes"""
        print("\n⚙️ CONFIGURAR PROBABILIDAD DE SOLO EMOTES")
        print("=" * 50)
        print("Actualmente: 15% de probabilidad de enviar solo emotes")
        print("Rango recomendado: 5% - 30%")
        
        try:
            new_prob = float(input("\nNueva probabilidad (5-30): "))
            if 5 <= new_prob <= 30:
                # Aquí podrías agregar la configuración a config.json si quieres
                print(f"✅ Probabilidad configurada a {new_prob}%")
                print("💡 Para aplicar este cambio permanentemente, necesitarías")
                print("   modificar el valor PROB_ONLY_EMOTE en el código")
            else:
                print("❌ Valor fuera del rango recomendado (5-30)")
        except ValueError:
            print("❌ Valor inválido")

async def main():
    """Función principal"""
    print("🔥 KICK.COM CHATBOT - CONFIGURACIÓN PERSISTENTE")
    print("=" * 70)
    print("💾 Configuración se guarda automáticamente")
    print("📝 Mensajes organizados en archivos separados")
    print("=" * 70)
    
    bot = PersistentChatbot()
    
    if not bot.load_accounts():
        print("❌ No se pudieron cargar las cuentas")
        return
    
    if not bot.accounts:
        print("❌ No hay cuentas en kicks.json")
        return
    
    while True:
        print(f"\n🎯 MENÚ PRINCIPAL:")
        print(f"1. 🔥 Iniciar chatbot")
        print(f"2. ⚙️ Configurar delays y comportamiento")
        print(f"3. 📊 Ver configuración actual")
        print(f"4. 📈 Estadísticas de cuentas")
        print(f"5. 📝 Editar mensajes (abrir messages.json)")
        print(f"6. 🛡️ Gestionar proxies")
        print(f"0. ❌ Salir")
        
        choice = input("\n🔢 Selecciona opción: ").strip()
        
        if choice == "1":
            channel = input("📺 Canal (ej: apollo2121): ").strip()
            if channel:
                await bot.start_chatbot(channel)
        
        elif choice == "2":
            bot.modify_config()
        
        elif choice == "3":
            bot.show_config()
        
        elif choice == "4":
            print(f"\n📈 ESTADÍSTICAS DE TODAS LAS CUENTAS:")
            print("-" * 60)
            for acc in bot.accounts:
                msgs = acc.get('messages_sent', 0)
                last_time = acc.get('last_message_time', 0)
                last_msg = datetime.fromtimestamp(last_time).strftime("%H:%M:%S") if last_time > 0 else "Nunca"
                status = "✅ Activa" if msgs > 0 else "⚠️ Sin envíos"
                print(f"   {acc['username']:15} | {status:12} | Mensajes: {msgs:3d} | Último: {last_msg}")
        
        elif choice == "5":
            print(f"\n📝 EDITAR MENSAJES:")
            print(f"   📁 Archivo: messages.json")
            print(f"   🔧 Abre el archivo con tu editor favorito")
            print(f"   💾 Los cambios se aplicarán automáticamente")
            print(f"   🔄 Reinicia el chatbot para aplicar cambios")
        
        elif choice == "6":
            bot.manage_proxies()
        
        elif choice == "0":
            print("👋 ¡Hasta luego!")
            break
        
        else:
            print("❌ Opción inválida")

if __name__ == "__main__":
    asyncio.run(main())
