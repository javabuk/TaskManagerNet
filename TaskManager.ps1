# Lanzador simple para TaskManager
# Guardar como: TaskManager.ps1
# Uso: .\TaskManager.ps1 proyecto listar

param(
    [Parameter(ValueFromRemainingArguments=$true)]
    [string[]]$Arguments
)

# Ruta del ejecutable
$exe = Split-Path -Parent $PSCommandPath | Join-Path -ChildPath "TaskManager.exe"

# Verificar si el ejecutable existe
if (-not (Test-Path $exe)) {
    Write-Host "❌ Error: TaskManager.exe no encontrado en:" -ForegroundColor Red
    Write-Host "   $exe" -ForegroundColor Red
    Write-Host "`nVerifica que descargaste los archivos correctamente."
    exit 1
}

# Ejecutar con los argumentos
& $exe @Arguments
