#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
🧹 SCRIPT DE LIMPIEZA - ELIMINA ARCHIVOS INNECESARIOS
Mantiene solo los archivos esenciales del chatbot
"""

import os
import shutil

def cleanup_project():
    """Elimina archivos innecesarios del proyecto"""
    
    # Lista de archivos a conservar (ESENCIALES)
    keep_files = {
        # Archivos principales del chatbot
        "kick_chatbot_persistent.py",     # Chatbot con configuración persistente
        "main_chatbot.py",                # Launcher principal 
        "kicks.json",                     # Cuentas de usuario
        "config.json",                    # Configuración persistente
        "messages.json",                  # Mensajes por personalidad
        
        # Utilidades importantes
        "aiocurl.py",                     # HTTP client
        "test_specific_token.py",         # Probar tokens individualmente
        "account_manager.py",             # Gestionar cuentas
        
        # Documentación
        "README_CHATBOT.md",              # Guía de uso
        "requirements.txt",               # Dependencias
        
        # Sistema
        "cleanup_files.py",               # Este script
        ".gitignore",                     # Git
        "LICENSE"                         # Licencia
    }
    
    # Archivos a eliminar (INNECESARIOS)
    remove_files = [
        # Versiones antiguas del chatbot
        "kick_chatbot.py",
        "kick_chatbot_final.py", 
        "kick_chatbot_fixed_final.py",
        "kick_chatbot_final_working.py",
        "kick_chatbot_multi_accounts.py",
        "kick_chatbot_debug.py",
        "chatbot_fixed.py",
        "chatbot_simulator.py",
        "real_kick_chatbot.py",
        "kick_chatbot_final_working.py",
        
        # Scripts de prueba obsoletos
        "channel_diagnostics.py",
        "smart_chatbot.py",
        "quick_test_chat.py",
        "follow_channels.py",
        "security_guide.py",
        "manual_token_guide.py",
        "create_real_account.py",
        "demo_setup.py",
        "fix_problematic_account.py",
        
        # Archivos de configuración obsoletos
        "config.py",
        
        # Documentación duplicada
        "SOLUCION_CANALES.md",
        "INSTRUCCIONES_KASADA.md",
        
        # Generador de cuentas original (problemático)
        "accgen.py",
        "account_creator.py",
        
        # Tokens de prueba específicos
        "token_extractor.py",
        
        # Archivos temporales si existen
        "temp_*",
        "test_*",
        "debug_*"
    ]
    
    print("🧹 LIMPIEZA DEL PROYECTO")
    print("=" * 50)
    
    # Mostrar archivos que se conservarán
    print("✅ ARCHIVOS QUE SE CONSERVAN:")
    existing_keep = []
    for file in keep_files:
        if os.path.exists(file):
            existing_keep.append(file)
            file_size = os.path.getsize(file)
            print(f"   📁 {file:<30} ({file_size:,} bytes)")
    
    print(f"\n📊 Total archivos a conservar: {len(existing_keep)}")
    
    # Mostrar archivos que se eliminarán
    print("\n❌ ARCHIVOS QUE SE ELIMINARÁN:")
    files_to_remove = []
    for file in remove_files:
        if os.path.exists(file):
            files_to_remove.append(file)
            file_size = os.path.getsize(file)
            print(f"   🗑️ {file:<30} ({file_size:,} bytes)")
    
    if not files_to_remove:
        print("   ✨ ¡No hay archivos innecesarios!")
        return
    
    print(f"\n📊 Total archivos a eliminar: {len(files_to_remove)}")
    
    # Calcular espacio liberado
    total_size = sum(os.path.getsize(f) for f in files_to_remove)
    print(f"💾 Espacio a liberar: {total_size:,} bytes ({total_size/1024:.1f} KB)")
    
    # Confirmar eliminación
    print(f"\n⚠️ CONFIRMACIÓN:")
    print(f"   Se eliminarán {len(files_to_remove)} archivos")
    print(f"   Los archivos esenciales se conservarán")
    
    confirm = input("\n🔢 ¿Proceder con la limpieza? (sí/no): ").strip().lower()
    
    if confirm not in ['sí', 'si', 'yes', 's', 'y']:
        print("❌ Limpieza cancelada")
        return
    
    # Proceder con la eliminación
    print(f"\n🗑️ ELIMINANDO ARCHIVOS...")
    removed_count = 0
    errors = []
    
    for file in files_to_remove:
        try:
            os.remove(file)
            print(f"   ✅ Eliminado: {file}")
            removed_count += 1
        except Exception as e:
            error_msg = f"Error eliminando {file}: {e}"
            errors.append(error_msg)
            print(f"   ❌ {error_msg}")
    
    # Resumen final
    print(f"\n📊 RESUMEN DE LIMPIEZA:")
    print(f"   ✅ Archivos eliminados: {removed_count}")
    print(f"   ❌ Errores: {len(errors)}")
    print(f"   💾 Espacio liberado: {total_size:,} bytes")
    
    if errors:
        print(f"\n⚠️ ERRORES ENCONTRADOS:")
        for error in errors:
            print(f"   ❌ {error}")
    
    print(f"\n✨ ¡LIMPIEZA COMPLETADA!")
    print(f"📁 Archivos conservados: {len(existing_keep)}")
    
    # Mostrar estructura final
    print(f"\n📂 ESTRUCTURA FINAL DEL PROYECTO:")
    print(f"   🔥 kick_chatbot_persistent.py  (PRINCIPAL)")
    print(f"   🚀 main_chatbot.py            (LAUNCHER)")
    print(f"   ⚙️ config.json               (CONFIGURACIÓN)")
    print(f"   💬 messages.json             (MENSAJES)")
    print(f"   👥 kicks.json                (CUENTAS)")
    print(f"   📚 README_CHATBOT.md         (DOCUMENTACIÓN)")
    print(f"   🔧 + archivos de utilidades")

def show_current_files():
    """Muestra todos los archivos actuales del proyecto"""
    print("📂 ARCHIVOS ACTUALES EN EL PROYECTO:")
    print("=" * 50)
    
    files = [f for f in os.listdir('.') if os.path.isfile(f)]
    files.sort()
    
    total_size = 0
    for file in files:
        size = os.path.getsize(file)
        total_size += size
        
        # Categorizar archivo
        if file.endswith('.py'):
            icon = "🐍"
        elif file.endswith('.json'):
            icon = "📋"
        elif file.endswith('.md'):
            icon = "📚"
        elif file.endswith('.txt'):
            icon = "📝"
        else:
            icon = "📄"
        
        print(f"   {icon} {file:<35} ({size:,} bytes)")
    
    print(f"\n📊 Total: {len(files)} archivos ({total_size:,} bytes)")

def main():
    """Función principal"""
    print("🧹 HERRAMIENTA DE LIMPIEZA DEL PROYECTO")
    print("=" * 60)
    print("🎯 Elimina archivos innecesarios y mantiene solo lo esencial")
    print("=" * 60)
    
    while True:
        print(f"\n🎯 OPCIONES:")
        print(f"1. 🧹 Limpiar proyecto (eliminar archivos innecesarios)")
        print(f"2. 📂 Ver archivos actuales")
        print(f"3. 📋 Ver lista de archivos esenciales")
        print(f"0. ❌ Salir")
        
        choice = input("\n🔢 Selecciona opción: ").strip()
        
        if choice == "1":
            cleanup_project()
        
        elif choice == "2":
            show_current_files()
        
        elif choice == "3":
            print(f"\n📋 ARCHIVOS ESENCIALES:")
            print(f"   🔥 kick_chatbot_persistent.py - Chatbot principal")
            print(f"   🚀 main_chatbot.py           - Launcher fácil de usar")
            print(f"   ⚙️ config.json              - Configuración persistente")
            print(f"   💬 messages.json            - Mensajes por personalidad")
            print(f"   👥 kicks.json               - Cuentas de usuario")
            print(f"   🌐 aiocurl.py               - Cliente HTTP")
            print(f"   🧪 test_specific_token.py   - Probar tokens")
            print(f"   📚 README_CHATBOT.md        - Documentación")
            print(f"   📦 requirements.txt         - Dependencias")
        
        elif choice == "0":
            print("👋 ¡Hasta luego!")
            break
        
        else:
            print("❌ Opción inválida")

if __name__ == "__main__":
    main()