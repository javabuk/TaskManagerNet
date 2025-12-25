# TaskManager - Gestor de Tareas

Una aplicación de consola .NET 7 para la gestión integral de proyectos, tareas, recursos e impedimentos, con persistencia en SQLite.

## Características

- ✅ Gestión completa de proyectos
- ✅ Gestión de recursos y asignaciones
- ✅ Gestión de tareas con prioridades
- ✅ Tareas diarias (Daily)
- ✅ Registro de impedimentos
- ✅ Generación de reportes en Markdown
- ✅ Filtros avanzados
- ✅ Interfaz de línea de comandos intuitiva

## Requisitos Previos

- .NET SDK 7.0.101 o superior
- Sistema operativo: Windows, macOS o Linux

## Instalación

### 1. Clonar o descargar el repositorio

```bash
git clone <repository-url>
cd TaskManager
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

### 3. Construir la aplicación

```bash
dotnet build
```

## Compilación y Ejecución

### Modo Desarrollo

Para ejecutar la aplicación en modo desarrollo:

```bash
dotnet run -- <comando> [opciones]
```

### Compilación Release

Para crear un ejecutable optimizado:

```bash
dotnet publish -c Release -r win-x64 --self-contained --output ./publish
```

El ejecutable se encontrará en `./publish/TaskManager.exe`

### Comandos Básicos

Ver la ayuda:
```bash
dotnet run
```

## Uso de la Aplicación

### Proyectos

**Crear un nuevo proyecto:**
```bash
dotnet run -- proyecto crear --nombre "Mi Proyecto" --descripcion "Descripción" --tiene-daily 1
```

**Listar todos los proyectos:**
```bash
dotnet run -- proyecto listar
```

**Modificar un proyecto:**
```bash
dotnet run -- proyecto modificar --id 1 --nombre "Nuevo Nombre"
```

### Recursos

**Crear un nuevo recurso:**
```bash
dotnet run -- recurso crear --nombre "Juan Pérez"
```

**Listar todos los recursos:**
```bash
dotnet run -- recurso listar
```

**Modificar un recurso:**
```bash
dotnet run -- recurso modificar --id 1 --nombre "Juan Carlos Pérez" --activo 1
```

### Asignación de Recursos a Proyectos

**Asignar un recurso a un proyecto:**
```bash
dotnet run -- recurso-proyecto crear --id-proyecto 1 --id-recurso 1
```

**Listar asignaciones:**
```bash
dotnet run -- recurso-proyecto listar --id-proyecto 1
```

### Tareas

**Crear una tarea:**
```bash
dotnet run -- tarea crear --id-proyecto 1 --titulo "Mi Tarea" --detalle "Detalles" --prioridad "Alta"
```

**Listar tareas:**
```bash
dotnet run -- tarea listar --id-proyecto 1 --prioridad "Alta"
```

**Modificar una tarea:**
```bash
dotnet run -- tarea modificar --id 1 --titulo "Tarea Actualizada" --fecha-fin "31/12/2025" --activo 1
```

### Tareas Daily

**Crear una tarea daily:**
```bash
dotnet run -- tarea-daily crear --id-proyecto 1 --id-recurso 1 --titulo "Tarea del día"
```

**Listar tareas daily:**
```bash
dotnet run -- tarea-daily listar --id-proyecto 1 --id-recurso 1
```

**Modificar una tarea daily:**
```bash
dotnet run -- tarea-daily modificar --id 1 --fecha-fin "25/12/2025"
```

### Impedimentos Daily

**Registrar un impedimento:**
```bash
dotnet run -- impedimento-daily crear --id-proyecto 1 --id-recurso 1 --impedimento "Sin acceso a servidor" --explicacion "El servidor está caído"
```

**Listar impedimentos:**
```bash
dotnet run -- impedimento-daily listar --id-proyecto 1
```

**Resolver un impedimento:**
```bash
dotnet run -- impedimento-daily modificar --id 1 --activo 0 --fecha-fin "25/12/2025"
```

### Reportes

**Generar reporte del día:**
```bash
dotnet run -- reporte generar
```

**Generar reporte de una fecha específica:**
```bash
dotnet run -- reporte generar --fecha "20/12/2025"
```

**Generar reporte de un proyecto específico:**
```bash
dotnet run -- reporte generar --id-proyecto 1
```

**Generar reporte por nombre de proyecto:**
```bash
dotnet run -- reporte generar --nombre-proyecto "Mi Proyecto"
```

## Estructura de Base de Datos

### Tabla Proyectos
- IdProyecto (INTEGER PRIMARY KEY AUTOINCREMENT)
- NombreProyecto (TEXT NOT NULL)
- Descripcion (TEXT)
- FechaInicio (TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP)
- Activo (INTEGER NOT NULL, 0 o 1)
- TieneDaily (INTEGER NOT NULL, 0 o 1)

### Tabla Recursos
- IdRecurso (INTEGER PRIMARY KEY AUTOINCREMENT)
- NombreRecurso (TEXT NOT NULL)
- Activo (INTEGER NOT NULL, 0 o 1)
- FechaCreacion (TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP)

### Tabla RecursosProyecto
- IdRecursoProyecto (INTEGER PRIMARY KEY AUTOINCREMENT)
- IdProyecto (INTEGER NOT NULL FK)
- IdRecurso (INTEGER NOT NULL FK)
- FechaAsignacion (TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP)
- Índice único: (IdProyecto, IdRecurso)

### Tabla Tareas
- IdTarea (INTEGER PRIMARY KEY AUTOINCREMENT)
- IdProyecto (INTEGER NOT NULL FK)
- Titulo (TEXT NOT NULL)
- Detalle (TEXT)
- FechaCreacion (TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP)
- FechaFIN (TEXT)
- Prioridad (TEXT NOT NULL - Alta, Media, Baja)
- Activo (INTEGER NOT NULL, 0 o 1)

### Tabla RecursosTarea
- IdRecursoTarea (INTEGER PRIMARY KEY AUTOINCREMENT)
- IdTarea (INTEGER NOT NULL FK)
- IdRecurso (INTEGER NOT NULL FK)
- FechaAsignacion (TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP)
- Índice único: (IdTarea, IdRecurso)

### Tabla TareasDaily
- IdTareaDaily (INTEGER PRIMARY KEY AUTOINCREMENT)
- IdProyecto (INTEGER NOT NULL FK)
- IdRecurso (INTEGER NOT NULL FK)
- Titulo (TEXT NOT NULL)
- FechaCreacion (TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP)
- FechaFIN (TEXT)
- Activo (INTEGER NOT NULL, 0 o 1)

### Tabla ImpedimentosDaily
- IdImpedimentoDaily (INTEGER PRIMARY KEY AUTOINCREMENT)
- IdProyecto (INTEGER NOT NULL FK)
- IdRecurso (INTEGER NOT NULL FK)
- Impedimento (TEXT NOT NULL)
- Explicacion (TEXT NOT NULL)
- FechaCreacion (TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP)
- FechaFIN (TEXT)
- Activo (INTEGER NOT NULL, 0 o 1)

## Formato de Fechas

Todas las fechas deben estar en formato **DD/MM/YYYY** (ej: 25/12/2025)

## Configuración

La configuración se puede personalizar en `appsettings.json`:

```json
{
  "AppConfiguration": {
    "DatabasePath": "taskmanager.db",
    "PreviousDaysForReport": 3
  }
}
```

- **DatabasePath**: Ruta donde se guarda la base de datos SQLite
- **PreviousDaysForReport**: Número de días anteriores a incluir en los reportes

## Reportes

Los reportes se generan en formato Markdown (.md) y contienen:

- Tareas finalizadas en los últimos N días
- Tareas programadas para hoy
- Tareas futuras
- Información daily (si el proyecto lo tiene habilitado)
  - Qué se hizo ayer
  - Qué se va a hacer hoy
  - Impedimentos activos

Los reportes se guardan con el nombre: `reporte_YYYY-MM-DD.md`

## Ejecución como Tarea Programada

### Windows (Task Scheduler)

1. Abrir "Programador de tareas"
2. Crear tarea básica
3. Trigger: Diariamente a la hora deseada
4. Acción: Iniciar programa
5. Programa: `C:\ruta\al\TaskManager.exe`
6. Argumentos: `reporte generar`
7. Directorio de inicio: `C:\ruta\al\`

### Linux/macOS (Cron)

```bash
# Agregar a crontab (editar con: crontab -e)
0 9 * * * /ruta/al/TaskManager reporte generar >> /var/log/taskmanager.log 2>&1
```

## Testing

### Ejecutar todos los tests

```bash
dotnet test
```

### Ejecutar tests de un proyecto específico

```bash
dotnet test tests/TaskManager.Tests/
```

### Con cobertura de código

```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=lcov
```

## Seguridad

La aplicación ha sido compilada con las siguientes medidas de seguridad:

- ✅ Análisis estático de código habilitado
- ✅ Compatibilidad con Windows Defender
- ✅ Compatibilidad con CrowdStrike
- ✅ Validación de entrada en todos los comandos
- ✅ Uso de prepared statements (Entity Framework)
- ✅ Inyección de dependencias para mejor seguridad

## Logs

Los logs de la aplicación se generan en la carpeta `logs/`

## Solución de Problemas

### La base de datos no se crea

Asegúrate de tener permisos de escritura en el directorio actual.

### Error al conectar a la base de datos

Verifica que el archivo `taskmanager.db` no esté siendo usado por otro proceso.

### Comando no reconocido

Usa `dotnet run` sin argumentos para ver la ayuda disponible.

## Mejoras Futuras

- [ ] API REST
- [ ] Interfaz gráfica (WinForms/WPF)
- [ ] Exportación a Excel
- [ ] Integración con correo electrónico
- [ ] Dashboard web
- [ ] Sincronización en la nube

## Contribuciones

Las contribuciones son bienvenidas. Por favor:

1. Fork el repositorio
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## Licencia

Este proyecto está bajo la Licencia MIT. Ver archivo `LICENSE` para más detalles.

## Autor

TaskManager Development Team

## Soporte

Para reportar bugs o solicitar features, por favor abre un issue en el repositorio.

## Changelog

### v1.0.0 (2025-12-25)
- Release inicial
- Todas las operaciones CRUD funcionando
- Generador de reportes en Markdown
- Tests unitarios e integración
