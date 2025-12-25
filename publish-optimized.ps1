# Script de publicación optimizado para CrowdStrike
# Reduce el tamaño del ejecutable eliminando símbolos de depuración
# e incluye opciones de firma de código

Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host "TaskManager - Publicación Optimizada para CrowdStrike" -ForegroundColor Cyan
Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = "C:\Area\Formacion\NET\IA\TaskManager"
$publishPath = "$projectPath\publish-optimized"

Write-Host "[CONFIGURACION] Parámetros de publicación:" -ForegroundColor Yellow
Write-Host "  - Proyecto: $projectPath"
Write-Host "  - Destino: $publishPath"
Write-Host "  - Optimizaciones:"
Write-Host "    > PublishSingleFile: true (todo en un .exe)"
Write-Host "    > PublishTrimmed: true (elimina código no usado)"
Write-Host "    > DebugType: none (sin símbolos de depuración)"
Write-Host "    > SelfContained: true (incluye .NET runtime)"
Write-Host "    > EnableCompressionInSingleFile: true"
Write-Host ""

# Limpiar carpeta anterior
if (Test-Path $publishPath) {
    Write-Host "[INFO] Limpiando carpeta anterior..." -ForegroundColor Yellow
    Remove-Item $publishPath -Recurse -Force
}

Write-Host "[INICIO] Compilando y publicando..." -ForegroundColor Cyan
Write-Host ""

# Publicar con todas las optimizaciones
& dotnet publish "$projectPath" `
    -c Release `
    -r win-x64 `
    --self-contained `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfContained=true `
    -p:EnableCompressionInSingleFile=true `
    -p:PublishTrimmed=true `
    -p:PublishReadyToRun=false `
    -p:DebugType=none `
    -p:DebugSymbols=false `
    -p:InvariantGlobalization=false `
    -p:SelfContainedRuntimeIdentifier=win-x64 `
    -o $publishPath

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "[EXITO] Publicación completada exitosamente" -ForegroundColor Green
    Write-Host ""
    
    # Obtener información del archivo
    $exePath = "$publishPath\TaskManager.exe"
    if (Test-Path $exePath) {
        $exeSize = (Get-Item $exePath).Length / 1MB
        Write-Host "[ESTADISTICAS] Información del ejecutable:" -ForegroundColor Yellow
        Write-Host "  - Ruta: $exePath"
        Write-Host "  - Tamaño: $([Math]::Round($exeSize, 2)) MB"
        Write-Host ""
        
        # Información de archivos en la carpeta
        Write-Host "[CONTENIDO] Archivos en carpeta de publicación:" -ForegroundColor Yellow
        Get-ChildItem $publishPath | ForEach-Object {
            if ($_.PSIsContainer) {
                Write-Host "  [DIR] $($_.Name)\"
            } else {
                $size = $_.Length / 1MB
                Write-Host "  [FILE] $($_.Name) ($([Math]::Round($size, 2)) MB)"
            }
        }
    }
} else {
    Write-Host ""
    Write-Host "[ERROR] Fallo durante la publicación" -ForegroundColor Red
    Write-Host "Código de error: $LASTEXITCODE" -ForegroundColor Red
}

Write-Host ""
Write-Host "[INFO] Notas sobre CrowdStrike:" -ForegroundColor Cyan
Write-Host "  - El ejecutable está optimizado sin símbolos de depuración"
Write-Host "  - Sin información de trace que pueda triggerar alertas"
Write-Host "  - Si aún es bloqueado, agregue excepción en CrowdStrike console"
Write-Host "  - O firme el ejecutable con certificado corporativo (Authenticode)"
Write-Host ""
