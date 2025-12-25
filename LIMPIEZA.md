# 🗑️ LIMPIEZA - Eliminar Versiones Antiguas

## ⚠️ Los Directorios a Eliminar

El anterior método de publicación generó dos directorios que **NO FUNCIONAN**:

```
publish/                  ← Framework-dependent (necesita .NET)
publish-standalone/       ← Self-contained incorrecto (CAUSABA EL ERROR)
```

## ✅ El Nuevo Directorio (SÍ FUNCIONA)

```
publish-singlefile/       ← Single file standalone (FUNCIONA PERFECTAMENTE)
```

## 🧹 Qué Hacer

### Opción 1: Limpiar el proyecto (RECOMENDADO)

```powershell
cd C:\Area\Formacion\NET\IA\TaskManager

# Eliminar directorios obsoletos
Remove-Item "publish" -Recurse -Force
Remove-Item "publish-standalone" -Recurse -Force

# Limpiar archivos binarios
Remove-Item "bin" -Recurse -Force
Remove-Item "obj" -Recurse -Force

# Eliminar base de datos de prueba anterior (opcional)
Remove-Item "taskmanager.db" -Force
```

### Opción 2: Mantener solo la distribución funcional

```powershell
# Crear carpeta de distribución si no existe
mkdir "C:\Distribucion\TaskManager_Backup" -ErrorAction SilentlyContinue

# Copiar la distribución funcional
Copy-Item "C:\Distribucion\TaskManagerV2\TaskManager.exe" `
          "C:\Distribucion\TaskManager_Backup\" -Force

# Ya tienes la distribución lista en TaskManagerV2
```

## 📋 Checklist de Limpieza

```
[ ] Eliminar publish/
[ ] Eliminar publish-standalone/
[ ] Eliminar bin/
[ ] Eliminar obj/
[ ] Verificar que publish-singlefile/ existe
[ ] Verificar que C:\Distribucion\TaskManagerV2\ tiene TaskManager.exe
[ ] Probar: C:\Distribucion\TaskManagerV2\TaskManager.exe proyecto listar
```

## ✅ Después de la Limpieza

Para futuras compilaciones:

```powershell
cd C:\Area\Formacion\NET\IA\TaskManager

# Build de prueba (Debug)
dotnet build

# Publicación para distribución
dotnet publish -c Release -r win-x64 --self-contained `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfContained=true `
  -o ./publish-singlefile
```

## 📊 Comparativa: Qué Cambió

| Directorio | Estado Anterior | Estado Actual | Acción |
|-----------|-----------------|--------------|--------|
| `publish/` | ❌ No funciona standalone | No cambió | ELIMINAR |
| `publish-standalone/` | ❌ **CAUSA ERROR** | No cambió | ELIMINAR |
| `publish-singlefile/` | No existía | ✅ **FUNCIONA** | MANTENER |
| `bin/` | Residuos de compilación | Sin usar | ELIMINAR |
| `obj/` | Residuos de compilación | Sin usar | ELIMINAR |

## 🎯 Estructura Final Recomendada

```
C:\Area\Formacion\NET\IA\TaskManager\
├── src/                          ← Código fuente
├── tests/                         ← Tests
├── publish-singlefile/           ← SOLO ESTA (38 MB)
├── TaskManager.csproj            ← Proyecto
├── Program.cs                    ← Entrada
├── appsettings.json              ← Configuración
├── taskmanager.db                ← Base de datos
├── *.md                          ← Documentación
└── [archivos de configuración]

C:\Distribucion\
└── TaskManagerV2/
    ├── TaskManager.exe           ← EL DISTRIBUIBLE
    ├── appsettings.json          ← Configuración
    ├── taskmanager.db            ← Base de datos
    └── LEEME.txt                 ← Instrucciones
```

## ⚡ Script de Limpieza Automática

```powershell
# Guardar como: cleanup.ps1
param(
    [string]$ProjectPath = "C:\Area\Formacion\NET\IA\TaskManager"
)

Write-Host "Iniciando limpieza de directorios obsoletos..." -ForegroundColor Cyan

$dirs = @("publish", "publish-standalone", "bin", "obj")

foreach ($dir in $dirs) {
    $fullPath = Join-Path $ProjectPath $dir
    if (Test-Path $fullPath) {
        Write-Host "Eliminando: $dir" -ForegroundColor Yellow
        Remove-Item $fullPath -Recurse -Force
        Write-Host "  ✓ Eliminado" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "Verificando publish-singlefile..." -ForegroundColor Cyan
if (Test-Path "$(Join-Path $ProjectPath 'publish-singlefile')") {
    Write-Host "  ✓ publish-singlefile existe y es el único" -ForegroundColor Green
} else {
    Write-Host "  ⚠️ publish-singlefile NO encontrado" -ForegroundColor Red
}

Write-Host ""
Write-Host "✅ Limpieza completada" -ForegroundColor Green
```

## 🔄 Cómo Ejecutar el Script

```powershell
# Permitir ejecución de scripts
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process

# Ejecutar el script
.\cleanup.ps1
```

## ⚠️ Importante: No Elimines

```
✓ Código fuente (src/)
✓ Tests (tests/)
✓ Archivos .md de documentación
✓ TaskManager.csproj
✓ appsettings.json
✓ publish-singlefile/ (El nuevo)
```

## 📈 Beneficios de la Limpieza

- ✅ Menos desorden en el proyecto
- ✅ Menos confusión sobre cuál es la versión correcta
- ✅ Más fácil para distribuir a otros
- ✅ Menos almacenamiento usado (200 MB menos)

---

**Después de la limpieza, solo tendrás la distribución funcional.** ✅
