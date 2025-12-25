# TaskManager - Guía Rápida de Inicio

## Primeros pasos

### 1. Compilar la aplicación

```bash
dotnet build
```

### 2. Ejecutar la aplicación

```bash
# Ver la ayuda
dotnet run

# Crear un proyecto
dotnet run -- proyecto crear --nombre "Proyecto A" --tiene-daily 1

# Listar proyectos
dotnet run -- proyecto listar

# Crear un recurso
dotnet run -- recurso crear --nombre "Juan"

# Asignar recurso a proyecto
dotnet run -- recurso-proyecto crear --id-proyecto 1 --id-recurso 1

# Crear una tarea
dotnet run -- tarea crear --id-proyecto 1 --titulo "Tarea 1" --prioridad "Alta"

# Ver tareas
dotnet run -- tarea listar --id-proyecto 1

# Crear tarea daily
dotnet run -- tarea-daily crear --id-proyecto 1 --id-recurso 1 --titulo "Daily 1"

# Registrar impedimento
dotnet run -- impedimento-daily crear --id-proyecto 1 --id-recurso 1 --impedimento "Sin acceso" --explicacion "BD caída"

# Generar reporte
dotnet run -- reporte generar
```

## Ejecutables Pre-compilados

Hay dos formas de ejecutar la aplicación:

### Opción 1: Con .NET instalado
```bash
cd publish
dotnet TaskManager.dll proyecto listar
```

### Opción 2: Executable Standalone (sin necesidad de .NET)
```bash
cd publish-standalone
TaskManager.exe proyecto listar
```

## Ejemplos Prácticos

### Escenario Completo

```bash
# 1. Crear un proyecto con daily habilitado
dotnet run -- proyecto crear --nombre "WebApp" --descripcion "Aplicación web" --tiene-daily 1

# 2. Crear recursos
dotnet run -- recurso crear --nombre "Dev 1"
dotnet run -- recurso crear --nombre "Dev 2"

# 3. Asignar recursos al proyecto
dotnet run -- recurso-proyecto crear --id-proyecto 1 --id-recurso 1
dotnet run -- recurso-proyecto crear --id-proyecto 1 --id-recurso 2

# 4. Crear tareas
dotnet run -- tarea crear --id-proyecto 1 --titulo "Feature A" --prioridad "Alta"
dotnet run -- tarea crear --id-proyecto 1 --titulo "Bug Fix" --prioridad "Media"

# 5. Crear tareas daily
dotnet run -- tarea-daily crear --id-proyecto 1 --id-recurso 1 --titulo "Implementar auth"
dotnet run -- tarea-daily crear --id-proyecto 1 --id-recurso 2 --titulo "Testing frontend"

# 6. Registrar impedimentos
dotnet run -- impedimento-daily crear --id-proyecto 1 --id-recurso 1 --impedimento "Esperando API" --explicacion "El servidor de API no responde"

# 7. Generar reporte diario
dotnet run -- reporte generar

# 8. Generar reporte de proyecto específico
dotnet run -- reporte generar --id-proyecto 1

# 9. Generar reporte de fecha pasada
dotnet run -- reporte generar --fecha "20/12/2025"
```

## Tests

```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar solo tests unitarios
dotnet test --filter "Category=Unit"

# Ver cobertura de código
dotnet test /p:CollectCoverage=true
```

## Base de Datos

La aplicación crea automáticamente la base de datos `taskmanager.db` en el directorio actual con las siguientes tablas:

- **Proyectos** - Gestión de proyectos
- **Recursos** - Personas o recursos
- **RecursosProyecto** - Asignación de recursos a proyectos
- **Tareas** - Tareas del proyecto
- **RecursosTarea** - Asignación de recursos a tareas
- **TareasDaily** - Tareas del día a día
- **ImpedimentosDaily** - Obstáculos identificados

## Configuración

Edita `appsettings.json` para cambiar:

```json
{
  "AppConfiguration": {
    "DatabasePath": "taskmanager.db",
    "PreviousDaysForReport": 3
  }
}
```

## Reportes Generados

Los reportes se guardan como archivos Markdown con el patrón: `reporte_YYYY-MM-DD.md`

Contenido del reporte:
- Tareas completadas en los últimos N días
- Tareas para hoy
- Tareas futuras
- Información de daily (si está habilitado en el proyecto):
  - Qué se hizo ayer
  - Qué se va a hacer hoy
  - Impedimentos activos

## Cron Jobs / Tareas Programadas

### Windows (Task Scheduler)
```
Programa: C:\ruta\TaskManager.exe
Argumentos: reporte generar
Directorio: C:\ruta\
Activador: Diariamente a las 09:00 AM
```

### Linux/macOS
```bash
# Editar crontab
crontab -e

# Agregar línea (generar reporte diariamente a las 9 AM)
0 9 * * * /ruta/TaskManager reporte generar >> /var/log/taskmanager.log 2>&1
```

## Mejores Prácticas

1. **Nombres únicos**: Los proyectos deben tener nombres únicos para evitar confusiones
2. **Fechas**: Siempre usar formato DD/MM/YYYY
3. **Prioridades**: Usar "Alta", "Media" o "Baja" (sensible a mayúsculas)
4. **Estado activo**: 1=activo, 0=inactivo
5. **Reportes regulares**: Ejecutar el reporte diariamente para seguimiento

## Troubleshooting

### Error: Database is locked
- Cierra todas las instancias de la aplicación
- Elimina `taskmanager.db` y recrea desde cero

### Error: Comando no reconocido
- Ejecuta `dotnet run` sin argumentos para ver la ayuda
- Verifica la ortografía exacta del comando

### La base de datos no se crea
- Asegúrate de tener permisos de escritura en el directorio
- Intenta crear la base de datos manualmente: `dotnet run`

## Ayuda y Documentación

Para documentación completa, consulta el archivo [README.md](README.md)
