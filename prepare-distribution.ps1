# Script para crear paquete de distribución
# Guardar como: prepare-distribution.ps1
# Uso: .\prepare-distribution.ps1

param(
    [string]$OutputPath = "C:\Distribucion\TaskManager"
)

Write-Host "=== Preparando paquete de distribucion ===" -ForegroundColor Cyan

# Crear carpeta de salida
if (Test-Path $OutputPath) {
    Write-Host "Carpeta ya existe, eliminando contenido..." -ForegroundColor Yellow
    Remove-Item "$OutputPath\*" -Recurse -Force
}
else {
    Write-Host "Creando carpeta: $OutputPath" -ForegroundColor Green
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

# Rutas fuente
$projectPath = "C:\Area\Formacion\NET\IA\TaskManager"
$publishPath = "$projectPath\publish-singlefile"

# Archivos a copiar
$filesToCopy = @(
    @{
        Source = "$publishPath\TaskManager.exe"
        Dest = "$OutputPath\TaskManager.exe"
        Description = "Ejecutable principal"
    },
    @{
        Source = "$projectPath\appsettings.json"
        Dest = "$OutputPath\appsettings.json"
        Description = "Configuración"
    },
    @{
        Source = "$projectPath\README.md"
        Dest = "$OutputPath\README.md"
        Description = "Documentación principal"
    },
    @{
        Source = "$projectPath\GETTING_STARTED.md"
        Dest = "$OutputPath\GETTING_STARTED.md"
        Description = "Guía rápida"
    },
    @{
        Source = "$projectPath\DEPLOYMENT.md"
        Dest = "$OutputPath\DEPLOYMENT.md"
        Description = "Guía de distribución"
    },
    @{
        Source = "$projectPath\TaskManager.bat"
        Dest = "$OutputPath\TaskManager.bat"
        Description = "Lanzador .bat"
    },
    @{
        Source = "$projectPath\TaskManager.ps1"
        Dest = "$OutputPath\TaskManager.ps1"
        Description = "Lanzador PowerShell"
    }
)

# Copiar archivos
Write-Host ""
Write-Host "Copiando archivos:" -ForegroundColor Cyan
foreach ($file in $filesToCopy) {
    if (Test-Path $file.Source) {
        Copy-Item -Path $file.Source -Destination $file.Dest -Force
        $size = (Get-Item $file.Dest).Length / 1MB
        Write-Host "  [OK] $($file.Description) ($([Math]::Round($size, 2)) MB)" -ForegroundColor Green
    }
    else {
        Write-Host "  [ERROR] $($file.Description) - NO ENCONTRADO" -ForegroundColor Red
    }
}

# Crear archivo README de instalación
Write-Host ""
Write-Host "Creando guia de instalacion..." -ForegroundColor Cyan

$installGuide = @"
# TaskManager - Guía de Instalación Rápida

## Requisitos
- Windows 7 o superior
- 100 MB de espacio libre

## Instalación

1. **Descargar**: Obtener los archivos de distribución
2. **Descomprimir**: Si está en un .zip, descomprimir en tu carpeta preferida
3. **Ejecutar**: Doble click en `TaskManager.exe` o en `TaskManager.bat`

## Uso Básico

```bash
# Ver comandos disponibles
TaskManager.exe

# Crear un proyecto
TaskManager.exe proyecto crear --nombre "Mi Proyecto"

# Ver proyectos
TaskManager.exe proyecto listar

# Crear un recurso
TaskManager.exe recurso crear --nombre "Juan"

# Crear una tarea
TaskManager.exe tarea crear --id-proyecto 1 --titulo "Mi tarea"

# Generar reporte
TaskManager.exe reporte generar
```

## Archivos en esta carpeta

- **TaskManager.exe** - La aplicación (no requiere .NET instalado)
- **taskmanager.db** - Base de datos (se crea automáticamente)
- **appsettings.json** - Archivo de configuración
- **README.md** - Documentación completa
- **GETTING_STARTED.md** - Ejemplos rápidos
- **DEPLOYMENT.md** - Información técnica

## Solución de Problemas

### El .exe no se ejecuta
- Verifica que Windows Defender no lo está bloqueando
- Si está bloqueado, haz click en "Más información" → "Ejecutar de todas formas"

### Error con la base de datos
- Elimina el archivo `taskmanager.db` si está corrupto
- Se creará uno nuevo automáticamente

### Cambiar configuración
- Abre `appsettings.json` con Notepad
- Cambia los valores y guarda
- Ejecuta el programa de nuevo

## Documentación

Para información detallada, consulta:
- **GETTING_STARTED.md** - Empezar rápido
- **README.md** - Documentación completa
- **DEPLOYMENT.md** - Información técnica

## Soporte

Si tienes problemas, consulta los archivos .md incluidos.

---
**Versión**: 1.0
**Fecha**: $((Get-Date).ToString('dd/MM/yyyy'))
**Plataforma**: Windows 64-bits
"@

$installGuide | Out-File -FilePath "$OutputPath\INSTALL.md" -Encoding UTF8
Write-Host "  ✓ Guía de instalación creada" -ForegroundColor Green

# Crear .bat de inicio rápido
Write-Host ""
Write-Host "Creando lanzadores..." -ForegroundColor Cyan

$quickStart = @"
@echo off
REM Quick Start - Abrir TaskManager con lista de proyectos
cd /d "%~dp0"
TaskManager.exe proyecto listar
pause
"@

$quickStart | Out-File -FilePath "$OutputPath\Quick-Start.bat" -Encoding ASCII
Write-Host "  [OK] Lanzador Quick-Start.bat creado" -ForegroundColor Green

# Mostrar tamaño total
Write-Host ""
Write-Host "Informacion del paquete:" -ForegroundColor Cyan
$totalSize = (Get-ChildItem -Path $OutputPath -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
$fileCount = (Get-ChildItem -Path $OutputPath -Recurse -File).Count
Write-Host "  Carpeta: $OutputPath"
Write-Host "  Archivos: $fileCount"
Write-Host "  Tamaño total: $([Math]::Round($totalSize, 2)) MB"

# Crear .zip opcional
Write-Host ""
$createZip = Read-Host "Deseas crear un archivo .zip para distribucion? (S/N)"
if ($createZip -eq "S" -or $createZip -eq "s") {
    $zipPath = "$OutputPath\..\TaskManager.zip"
    Write-Host "Creando .zip..." -ForegroundColor Cyan
    Compress-Archive -Path $OutputPath -DestinationPath $zipPath -Force
    $zipSize = (Get-Item $zipPath).Length / 1MB
    Write-Host "  [OK] $zipPath ($([Math]::Round($zipSize, 2)) MB)" -ForegroundColor Green
}

Write-Host ""
Write-Host "Paquete de distribucion listo!" -ForegroundColor Green
Write-Host ""
Write-Host "Proximos pasos:" -ForegroundColor Yellow
Write-Host "  1. Abre File Explorer: explorer.exe $OutputPath"
Write-Host "  2. Haz doble click en TaskManager.exe"
Write-Host "  3. O ejecuta desde CMD: cd $OutputPath && TaskManager.exe proyecto listar"
Write-Host ""
