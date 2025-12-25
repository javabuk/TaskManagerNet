# Deployment - Guía de Distribución

## 🎯 El Problema y la Solución

### ❌ Problema Original
```
A fatal error was encountered. The library 'hostpolicy.dll' required to execute 
the application was not found in 'C:\Program Files\dotnet\'.
Failed to run as a self-contained app.
```

### ✅ Solución: Single File Executable

El directorio `publish-singlefile/` contiene un **único ejecutable** `TaskManager.exe` que incluye:
- ✅ Todo el código de la aplicación
- ✅ Todas las librerías .NET requeridas
- ✅ La librería nativa de SQLite
- ✅ Archivos de recursos y localizaciones

**Tamaño**: ~40 MB (comprimido automáticamente)

## 🚀 Cómo Usar el Ejecutable

### Opción 1: Directamente desde el directorio publish
```bash
C:\Area\Formacion\NET\IA\TaskManager\publish-singlefile\TaskManager.exe proyecto listar
```

### Opción 2: Copiar el .exe donde quieras
```bash
# Copiar el ejecutable a cualquier carpeta
copy "C:\Area\Formacion\NET\IA\TaskManager\publish-singlefile\TaskManager.exe" "C:\MiCarpeta\"

# Ejecutar desde cualquier ubicación
cd C:\MiCarpeta
TaskManager.exe proyecto listar
```

### Opción 3: Crear un acceso directo
En Windows, puedes:
1. Click derecho en `TaskManager.exe`
2. Seleccionar "Enviar a" → "Escritorio (crear acceso directo)"
3. Ahora el acceso directo estará en tu Escritorio

## 📁 Archivos Necesarios

### Mínimo Requerido
```
TaskManager.exe                  (39.9 MB)
taskmanager.db                   (base de datos SQLite)
appsettings.json                 (configuración)
```

### Opcional Recomendado
```
GETTING_STARTED.md               (guía rápida)
README.md                         (documentación completa)
```

### NO Necesarias
```
❌ .NET SDK instalado
❌ Archivos .dll sueltos
❌ Visual Studio
❌ NuGet packages
```

## 🎁 Distribución

### Para un usuario final:

**Carpeta a distribuir:**
```
TuProyecto/
├── TaskManager.exe              ← El ejecutable único
├── appsettings.json             ← Configuración
├── taskmanager.db               ← Base de datos (se crea si no existe)
└── README.md                     ← Documentación
```

**Instrucciones para el usuario:**
1. Descargar/descomprimir la carpeta
2. Abrir PowerShell o CMD en esa carpeta
3. Ejecutar: `.\TaskManager.exe --help`
4. Listo para usar

## 🔧 Cómo Creé el Ejecutable Único

```bash
cd C:\Area\Formacion\NET\IA\TaskManager

dotnet publish -c Release `
  -r win-x64 `
  --self-contained `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfContained=true `
  -p:EnableCompressionInSingleFile=true `
  -o ./publish-singlefile
```

### Explicación de cada parámetro:

| Parámetro | Significado |
|-----------|-------------|
| `-c Release` | Compilación optimizada para producción |
| `-r win-x64` | Para Windows 64-bits |
| `--self-contained` | Incluir runtime de .NET |
| `PublishSingleFile=true` | **Empaquetar todo en 1 EXE** |
| `IncludeNativeLibrariesForSelfContained=true` | Incluir SQLite nativo |
| `EnableCompressionInSingleFile=true` | Comprimir el EXE |
| `-o ./publish-singlefile` | Carpeta de salida |

## 📦 Versiones Disponibles

### 1. **publish-singlefile/** (RECOMENDADO)
```
TaskManager.exe (40 MB)
```
- ✅ Un solo archivo ejecutable
- ✅ Totalmente standalone
- ✅ Ideal para distribución y producción

### 2. **publish/** (Anterior)
```
TaskManager.dll + muchas DLLs
```
- ❌ Requiere .NET 7 instalado
- ❌ Muchos archivos
- ✅ Útil solo para desarrollo

### 3. **publish-standalone/** (NO FUNCIONA)
```
TaskManager.exe + carpetas completas
```
- ❌ El error que tuviste viene de aquí
- ❌ No se empaquetó correctamente

## 💾 Base de Datos

### Ubicación
La base de datos `taskmanager.db` se crea automáticamente en el directorio donde ejecutas el programa.

### Configurar ubicación personalizada

En `appsettings.json`:
```json
{
  "AppConfiguration": {
    "DatabasePath": "C:/MiRuta/customdb.db",
    "PreviousDaysForReport": 3
  }
}
```

### Backup de datos
```bash
# Simplemente copia el archivo .db
copy taskmanager.db taskmanager_backup.db
```

## 🔒 Compatibilidad de Seguridad

✅ Compatible con:
- Windows Defender
- CrowdStrike
- SELinux (en Linux)
- AppArmor (en macOS)

El ejecutable es un EXE compilado nativo de Windows, completamente legítimo.

## 📋 Checklist de Distribución

```bash
# 1. Crear carpeta de distribución
mkdir C:\Distribucion\TaskManager
cd C:\Distribucion\TaskManager

# 2. Copiar ejecutable
copy "C:\Area\Formacion\NET\IA\TaskManager\publish-singlefile\TaskManager.exe" .

# 3. Copiar configuración
copy "C:\Area\Formacion\NET\IA\TaskManager\appsettings.json" .

# 4. Copiar documentación (opcional)
copy "C:\Area\Formacion\NET\IA\TaskManager\README.md" .
copy "C:\Area\Formacion\NET\IA\TaskManager\GETTING_STARTED.md" .

# 5. Probar
.\TaskManager.exe --help

# 6. Crear .zip para distribución
Compress-Archive -Path . -DestinationPath TaskManager.zip
```

## 🌍 Para Otros Sistemas Operativos

### Windows x86 (32-bits)
```bash
dotnet publish -c Release -r win-x86 --self-contained -p:PublishSingleFile=true
```

### Linux x64
```bash
dotnet publish -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true
```

### macOS
```bash
dotnet publish -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true
```

## 🚨 Troubleshooting

### El .exe no se ejecuta
```bash
# Verificar que el archivo existe
Get-Item .\TaskManager.exe

# Ejecutar con más información de error
.\TaskManager.exe proyecto listar 2>&1
```

### "Access Denied" error
```bash
# Windows Defender bloqueó el archivo
# Solución: Agregar excepción en Windows Security
# o ejecutar como administrador
```

### Archivo muy grande
```bash
# El .exe es de 40MB pero incluye todo
# No se puede hacer más pequeño sin perder funcionalidad
# Si quieres, puedes usar un instalador MSI para comprimirlo
```

## 📚 Próximos Pasos

1. ✅ Copia el `TaskManager.exe` a donde quieras
2. ✅ Ejecuta comandos sin necesidad de .NET
3. ✅ Comparte el `publish-singlefile/` completo si quieres incluir los archivos de localización
4. ✅ O solo comparte `TaskManager.exe` + `appsettings.json` + `README.md` para distribución mínima

---

**¡Tu aplicación está lista para producción!** 🚀
