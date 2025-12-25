# TaskManager - Ejemplos de Uso Avanzado

## Filtrado Avanzado

### Filtrar tareas por proyecto y prioridad
```bash
dotnet run -- tarea listar --id-proyecto 1 --prioridad "Alta"
```

### Filtrar tareas por título (búsqueda parcial)
```bash
dotnet run -- tarea listar --titulo "Feature"
```

### Filtrar tareas por estado
```bash
# Solo tareas activas
dotnet run -- tarea listar --activo 1

# Solo tareas inactivas
dotnet run -- tarea listar --activo 0
```

### Filtrar tareas daily por proyecto y recurso
```bash
dotnet run -- tarea-daily listar --id-proyecto 1 --id-recurso 1 --titulo "Daily"
```

### Filtrar impedimentos activos
```bash
dotnet run -- impedimento-daily listar --id-proyecto 1 --activo 1
```

## Reportes Avanzados

### Generar reporte de proyecto específico
```bash
dotnet run -- reporte generar --id-proyecto 1
```

### Generar reporte por nombre de proyecto (búsqueda)
```bash
dotnet run -- reporte generar --nombre-proyecto "WebApp"
```

### Generar reporte histórico de una fecha anterior
```bash
dotnet run -- reporte generar --fecha "15/12/2025"
```

### Combinar filtros de reporte
```bash
dotnet run -- reporte generar --fecha "20/12/2025" --id-proyecto 1
```

## Gestión de Ciclo de Vida

### Marcar tarea como completada
```bash
dotnet run -- tarea modificar --id 1 --fecha-fin "25/12/2025" --activo 0
```

### Cambiar prioridad de tarea
```bash
dotnet run -- tarea modificar --id 1 --prioridad "Baja"
```

### Desactivar recurso
```bash
dotnet run -- recurso modificar --id 1 --activo 0
```

### Desactivar proyecto
```bash
dotnet run -- proyecto modificar --id 1 --activo 0
```

### Resolver impedimento
```bash
dotnet run -- impedimento-daily modificar --id 1 --activo 0 --fecha-fin "25/12/2025"
```

## Flujos de Trabajo Comunes

### Workflow: Inicio de Sprint

```bash
#!/bin/bash

# 1. Crear proyecto del sprint
dotnet run -- proyecto crear --nombre "Sprint 1" --descripcion "Sprint del mes" --tiene-daily 1

# 2. Asignar recursos
dotnet run -- recurso crear --nombre "Developer 1"
dotnet run -- recurso crear --nombre "Developer 2"
dotnet run -- recurso-proyecto crear --id-proyecto 1 --id-recurso 1
dotnet run -- recurso-proyecto crear --id-proyecto 1 --id-recurso 2

# 3. Crear tareas del sprint
dotnet run -- tarea crear --id-proyecto 1 --titulo "Feature 1" --prioridad "Alta"
dotnet run -- tarea crear --id-proyecto 1 --titulo "Feature 2" --prioridad "Media"
dotnet run -- tarea crear --id-proyecto 1 --titulo "Bugfix 1" --prioridad "Alta"

# 4. Asignar recursos a tareas
dotnet run -- recurso-tarea crear --id-tarea 1 --id-recurso 1
dotnet run -- recurso-tarea crear --id-tarea 2 --id-recurso 2
dotnet run -- recurso-tarea crear --id-tarea 3 --id-recurso 1

# 5. Crear tareas daily
dotnet run -- tarea-daily crear --id-proyecto 1 --id-recurso 1 --titulo "Dev 1 - Daily"
dotnet run -- tarea-daily crear --id-proyecto 1 --id-recurso 2 --titulo "Dev 2 - Daily"
```

### Workflow: Daily Standup

```bash
#!/bin/bash

# 1. Crear entrada de tarea daily
dotnet run -- tarea-daily crear --id-proyecto 1 --id-recurso 1 --titulo "Implementación feature X"

# 2. Si hay impedimentos, registrarlos
dotnet run -- impedimento-daily crear --id-proyecto 1 --id-recurso 1 \
  --impedimento "Bloqueado en API" \
  --explicacion "La API no responde - contactar DevOps"

# 3. Generar reporte del día
dotnet run -- reporte generar

# 4. Al finalizar el día, marcar tareas como completadas
dotnet run -- tarea-daily modificar --id 1 --fecha-fin "25/12/2025" --activo 0
```

### Workflow: Cierre de Sprint

```bash
#!/bin/bash

# 1. Listar todas las tareas del sprint
dotnet run -- tarea listar --id-proyecto 1

# 2. Marcar tareas completadas
dotnet run -- tarea modificar --id 1 --fecha-fin "25/12/2025" --activo 0
dotnet run -- tarea modificar --id 2 --fecha-fin "25/12/2025" --activo 0

# 3. Generar reporte final
dotnet run -- reporte generar --id-proyecto 1

# 4. Desactivar proyecto si está terminado
dotnet run -- proyecto modificar --id 1 --activo 0
```

## Scripts en Batch (Windows)

### scriptDailyReport.bat
```batch
@echo off
REM Script para generar reporte diario
cd C:\ruta\TaskManager
TaskManager.exe reporte generar
echo Reporte generado en: %DATE%
pause
```

### scriptCreateDaily.bat
```batch
@echo off
REM Script para crear entrada de daily
setlocal enabledelayedexpansion
set /p project="Ingresa ID del proyecto: "
set /p resource="Ingresa ID del recurso: "
set /p title="Ingresa título de la tarea: "

cd C:\ruta\TaskManager
TaskManager.exe tarea-daily crear --id-proyecto %project% --id-recurso %resource% --titulo "%title%"
echo Tarea creada exitosamente
pause
```

## Scripts en Bash (Linux/macOS)

### scriptDailyReport.sh
```bash
#!/bin/bash
# Script para generar reporte diario

cd /ruta/TaskManager
./TaskManager reporte generar
echo "Reporte generado el $(date)"
```

### scriptCreateDaily.sh
```bash
#!/bin/bash
# Script para crear entrada de daily interactivamente

read -p "Ingresa ID del proyecto: " project
read -p "Ingresa ID del recurso: " resource
read -p "Ingresa título de la tarea: " title

/ruta/TaskManager tarea-daily crear \
  --id-proyecto $project \
  --id-recurso $resource \
  --titulo "$title"

echo "Tarea creada exitosamente"
```

## Consultas Comunes

### Ver todas las tareas sin completar de un proyecto
```bash
dotnet run -- tarea listar --id-proyecto 1 --activo 1
```

### Ver recursos asignados a un proyecto
```bash
dotnet run -- recurso-proyecto listar --id-proyecto 1
```

### Ver impedimentos activos de un recurso
```bash
dotnet run -- impedimento-daily listar --id-recurso 1 --activo 1
```

### Ver tareas de una persona específica
```bash
dotnet run -- recurso-tarea listar --id-recurso 1
```

## Análisis y Reportes

### Análisis de carga de trabajo
```bash
# Ver todas las tareas activas
dotnet run -- tarea listar --activo 1

# Ver tareas de alta prioridad
dotnet run -- tarea listar --prioridad "Alta"

# Ver tareas sin fecha de término
# (Nota: necesitaría filtro adicional en el código)
```

## Integración con Herramientas Externas

### Exportar reporte a Word
```powershell
# Instalar pandoc
choco install pandoc

# Convertir Markdown a Word
pandoc reporte_2025-12-25.md -o reporte_2025-12-25.docx
```

### Enviar reporte por correo (PowerShell)
```powershell
$EmailFrom = "taskmanager@empresa.com"
$EmailTo = "equipo@empresa.com"
$Subject = "Reporte Diario"
$BodyFile = Get-Content "reporte_$(Get-Date -Format 'yyyy-MM-dd').md"

Send-MailMessage -From $EmailFrom -To $EmailTo `
  -Subject $Subject -Body $BodyFile `
  -SmtpServer "smtp.empresa.com"
```

### Integración con Git
```bash
# Guardar reportes en repositorio
git add reporte_*.md
git commit -m "Reportes diarios actualizados"
git push
```

## Monitoreo y Seguimiento

### Script de monitoreo diario
```bash
#!/bin/bash
# Ejecutarse cada mañana para verificar estado

PROJECT_ID=1
REPORTE_HOY=$(date +%Y-%m-%d)

# Generar reporte
/ruta/TaskManager reporte generar --id-proyecto $PROJECT_ID

# Contar tareas completadas
echo "=== Estadísticas del día ==="
echo "Tareas activas:"
/ruta/TaskManager tarea listar --id-proyecto $PROJECT_ID --activo 1 | wc -l

echo "Impedimentos activos:"
/ruta/TaskManager impedimento-daily listar --id-proyecto $PROJECT_ID --activo 1 | wc -l

# Guardar en log
echo "Monitoreo completado: $(date)" >> /var/log/taskmanager-monitor.log
```

## Troubleshooting de Automatización

### Problema: Script no encuentra el ejecutable
```bash
# Usar ruta completa
/usr/local/bin/TaskManager reporte generar

# O establecer PATH
export PATH="/usr/local/bin:$PATH"
TaskManager reporte generar
```

### Problema: Permisos de ejecución (Linux)
```bash
chmod +x /ruta/TaskManager
chmod +x /ruta/scripts/*.sh
```

### Problema: Variable de entorno
```bash
# Definir variable permanente en .bashrc o .bash_profile
export TASKMANAGER_HOME="/ruta/TaskManager"
alias tm="$TASKMANAGER_HOME/TaskManager"

# Luego usar
tm reporte generar
```

## Mejores Prácticas de Automatización

1. **Usar timestamps**: Agregar marca de tiempo a los reportes
2. **Rotación de logs**: Guardar reportes históricos organizados por fecha
3. **Validación**: Verificar que los IDs existan antes de crear registros
4. **Error handling**: Capturar errores y notificar
5. **Backup**: Hacer backup regular de la base de datos
