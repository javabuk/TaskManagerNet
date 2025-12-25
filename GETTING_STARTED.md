# TaskManager - Guía de Reproducción Rápida

## 🚀 Inicio Rápido (5 minutos)

### 1. Clonar o descargar el proyecto
```bash
cd c:\Area\Formacion\NET\IA\TaskManager
```

### 2. Compilar
```bash
dotnet build
```

### 3. Ver ayuda
```bash
dotnet run
```

### 4. Crear un proyecto de ejemplo
```bash
dotnet run -- proyecto crear --nombre "Mi Proyecto" --tiene-daily 1
```

### 5. Ver proyectos
```bash
dotnet run -- proyecto listar
```

### 6. Crear un recurso
```bash
dotnet run -- recurso crear --nombre "Juan"
```

### 7. Crear una tarea
```bash
dotnet run -- tarea crear --id-proyecto 1 --titulo "Tarea importante" --prioridad "Alta"
```

### 8. Generar reporte
```bash
dotnet run -- reporte generar
```

### 9. Ver el reporte generado
```bash
cat reporte_2025-12-25.md
```

## 🧪 Ejecutar Tests
```bash
dotnet test
```

## 📦 Crear Ejecutable

### Opción 1: Con .NET instalado
```bash
dotnet publish -c Release -o ./publish
cd publish
dotnet TaskManager.dll proyecto listar
```

### Opción 2: Sin .NET (Standalone)
```bash
dotnet publish -c Release -r win-x64 --self-contained -o ./publish-standalone
cd publish-standalone
TaskManager.exe proyecto listar
```

## 📋 Flujo Completo de Demostración

```bash
#!/bin/bash
cd c:\Area\Formacion\NET\IA\TaskManager

# 1. Crear proyecto
echo "=== Creando proyecto ==="
dotnet run -- proyecto crear --nombre "Proyecto Demo" --descripcion "Demostración del sistema" --tiene-daily 1

# 2. Crear recursos
echo "=== Creando recursos ==="
dotnet run -- recurso crear --nombre "Developer 1"
dotnet run -- recurso crear --nombre "Developer 2"

# 3. Asignar recursos al proyecto
echo "=== Asignando recursos ==="
dotnet run -- recurso-proyecto crear --id-proyecto 1 --id-recurso 1
dotnet run -- recurso-proyecto crear --id-proyecto 1 --id-recurso 2

# 4. Crear tareas
echo "=== Creando tareas ==="
dotnet run -- tarea crear --id-proyecto 1 --titulo "Feature A" --prioridad "Alta"
dotnet run -- tarea crear --id-proyecto 1 --titulo "Bugfix B" --prioridad "Media"

# 5. Crear tareas daily
echo "=== Creando tareas daily ==="
dotnet run -- tarea-daily crear --id-proyecto 1 --id-recurso 1 --titulo "Daily 1"
dotnet run -- tarea-daily crear --id-proyecto 1 --id-recurso 2 --titulo "Daily 2"

# 6. Crear impedimento
echo "=== Creando impedimento ==="
dotnet run -- impedimento-daily crear --id-proyecto 1 --id-recurso 1 \
  --impedimento "Sin acceso a BD" --explicacion "El servidor está caído"

# 7. Listar información
echo "=== Listando información ==="
dotnet run -- proyecto listar
dotnet run -- recurso listar
dotnet run -- tarea listar --id-proyecto 1
dotnet run -- tarea-daily listar --id-proyecto 1

# 8. Generar reporte
echo "=== Generando reporte ==="
dotnet run -- reporte generar

# 9. Ver reporte
echo "=== Contenido del reporte ==="
type reporte_2025-12-25.md
```

## 🔍 Validación Manual

Después de ejecutar los pasos anteriores, puedes verificar:

1. **Base de datos creada**: `taskmanager.db` en el directorio actual
2. **Archivo de reporte**: `reporte_2025-12-25.md` (o la fecha actual)
3. **Salida de consola**: Tablas con información de proyectos, tareas, etc.

## 💡 Ejemplos de Filtrado

```bash
# Tareas de alta prioridad
dotnet run -- tarea listar --prioridad "Alta"

# Tareas de un proyecto específico
dotnet run -- tarea listar --id-proyecto 1

# Tareas con "Feature" en el título
dotnet run -- tarea listar --titulo "Feature"

# Impedimentos activos
dotnet run -- impedimento-daily listar --activo 1

# Tareas daily de un recurso
dotnet run -- tarea-daily listar --id-recurso 1
```

## 📊 Generación de Reportes

```bash
# Reporte del día actual
dotnet run -- reporte generar

# Reporte de una fecha específica
dotnet run -- reporte generar --fecha "20/12/2025"

# Reporte de un proyecto
dotnet run -- reporte generar --id-proyecto 1

# Reporte por nombre de proyecto
dotnet run -- reporte generar --nombre-proyecto "Proyecto Demo"

# Combinación de filtros
dotnet run -- reporte generar --fecha "20/12/2025" --id-proyecto 1
```

## 🔧 Modificación de Datos

```bash
# Modificar proyecto
dotnet run -- proyecto modificar --id 1 --nombre "Nuevo Nombre"

# Completar una tarea
dotnet run -- tarea modificar --id 1 --fecha-fin "25/12/2025" --activo 0

# Cambiar prioridad
dotnet run -- tarea modificar --id 2 --prioridad "Baja"

# Resolver impedimento
dotnet run -- impedimento-daily modificar --id 1 --activo 0 --fecha-fin "25/12/2025"
```

## 📱 Automatización Simple

### Windows (PowerShell)
```powershell
# Generar reporte automáticamente
$date = Get-Date -Format "dd/MM/yyyy"
& "C:\ruta\TaskManager\TaskManager.exe" reporte generar
Get-Content "reporte_*.md" | Out-File "reportes\reporte_$date.txt"
```

### Linux/macOS (Bash)
```bash
#!/bin/bash
DATE=$(date +%d/%m/%Y)
/ruta/TaskManager/TaskManager reporte generar --fecha "$DATE"
cat reporte_*.md >> reportes.log
```

## 🐛 Troubleshooting Rápido

### El comando no se reconoce
```bash
# Verificar que .NET está instalado
dotnet --version

# Verificar que estás en el directorio correcto
pwd  # Linux/Mac
cd   # Windows (ver directorio actual)
```

### Error de base de datos
```bash
# Elimina la BD para recrearla
rm taskmanager.db  # Linux/Mac
del taskmanager.db # Windows

# Vuelve a ejecutar un comando
dotnet run -- proyecto listar
```

### Puerto o recurso en uso
```bash
# En Windows, detén la aplicación anterior
taskkill /F /IM TaskManager.exe

# En Linux/Mac
killall TaskManager
```

## ✨ Tips Útiles

1. **Guardar script completo**: Copia el flujo de demostración en un archivo `.sh` o `.bat`
2. **Automatizar reportes**: Usa Task Scheduler (Windows) o Cron (Linux)
3. **Monitorear cambios**: Genera reportes diarios
4. **Exportar datos**: Los reportes son Markdown, puedes convertir con Pandoc
5. **Integración**: Agrega a Git/GitHub para control de versiones

## 📖 Documentación Completa

- **README.md** - Documentación detallada
- **QUICKSTART.md** - Ejemplos rápidos
- **ADVANCED.md** - Uso avanzado y scripts
- **VALIDATION.md** - Checklista de validación
- **IMPLEMENTATION_SUMMARY.md** - Resumen del proyecto

## 🎯 Próximos Pasos

Después de familiarizarte con el sistema:

1. Integra con Git: `git init && git add . && git commit -m "Initial commit"`
2. Crea tareas programadas para reportes automáticos
3. Personaliza `appsettings.json` según tus necesidades
4. Explora el código fuente para entender la arquitectura
5. Considera extensiones futuras (API REST, UI web, etc.)

---

¡Listo para usar! La aplicación está completamente funcional y lista para producción. 🚀
