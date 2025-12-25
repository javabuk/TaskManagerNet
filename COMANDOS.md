# 📖 Referencia Completa de Comandos - TaskManager

## Tabla de Contenidos

1. [Información General](#información-general)
2. [Proyectos](#proyectos)
3. [Recursos](#recursos)
4. [Recursos en Proyectos](#recursos-en-proyectos)
5. [Tareas](#tareas)
6. [Recursos en Tareas](#recursos-en-tareas)
7. [Tareas Diarias](#tareas-diarias)
8. [Impedimentos Diarios](#impedimentos-diarios)
9. [Reportes](#reportes)
10. [Ejemplos Prácticos](#ejemplos-prácticos)

---

## Información General

### Help / Ayuda

```bash
TaskManager.exe help
TaskManager.exe --help
TaskManager.exe -h
```

Muestra la ayuda interactiva con todos los comandos disponibles.

---

## Proyectos

### Crear Proyecto

```bash
TaskManager.exe proyecto crear --nombre <texto> [opciones]
```

**Parámetros:**
- `--nombre` (REQUERIDO): Nombre del proyecto
- `--descripcion`: Descripción adicional
- `--fecha-inicio`: Fecha de inicio (formato: dd/MM/yyyy)
- `--activo`: Estado (0=inactivo, 1=activo, default: 1)
- `--tiene-daily`: Si tiene Daily tracking (0 o 1, default: 0)

**Ejemplos:**
```bash
TaskManager.exe proyecto crear --nombre "Backend API"
TaskManager.exe proyecto crear --nombre "Frontend Web" --descripcion "Interfaz de usuario" --tiene-daily 1
TaskManager.exe proyecto crear --nombre "Testing" --fecha-inicio "25/12/2025" --activo 1
```

### Listar Proyectos

```bash
TaskManager.exe proyecto listar
```

Muestra todos los proyectos registrados en forma de tabla.

### Modificar Proyecto

```bash
TaskManager.exe proyecto modificar --id <número> [opciones]
```

**Parámetros:**
- `--id` (REQUERIDO): ID del proyecto a modificar
- `--nombre`: Nuevo nombre
- `--descripcion`: Nueva descripción
- `--activo`: Nuevo estado (0 o 1)

**Ejemplos:**
```bash
TaskManager.exe proyecto modificar --id 1 --nombre "Backend API v2"
TaskManager.exe proyecto modificar --id 2 --activo 0
```

---

## Recursos

### Crear Recurso

```bash
TaskManager.exe recurso crear --nombre <texto> [opciones]
```

**Parámetros:**
- `--nombre` (REQUERIDO): Nombre del recurso (persona, equipo, etc.)
- `--activo`: Estado (0=inactivo, 1=activo, default: 1)

**Ejemplos:**
```bash
TaskManager.exe recurso crear --nombre "María"
TaskManager.exe recurso crear --nombre "Juan" --activo 1
TaskManager.exe recurso crear --nombre "Equipo Backend"
```

### Listar Recursos

```bash
TaskManager.exe recurso listar
```

Muestra todos los recursos disponibles.

### Modificar Recurso

```bash
TaskManager.exe recurso modificar --id <número> [opciones]
```

**Parámetros:**
- `--id` (REQUERIDO): ID del recurso
- `--nombre`: Nuevo nombre
- `--activo`: Nuevo estado (0 o 1)

**Ejemplos:**
```bash
TaskManager.exe recurso modificar --id 1 --nombre "María García"
TaskManager.exe recurso modificar --id 3 --activo 0
```

---

## Recursos en Proyectos

### Asignar Recurso a Proyecto

```bash
TaskManager.exe recurso-proyecto crear --id-proyecto <número> --id-recurso <número>
```

**Parámetros:**
- `--id-proyecto` (REQUERIDO): ID del proyecto
- `--id-recurso` (REQUERIDO): ID del recurso

**Ejemplos:**
```bash
TaskManager.exe recurso-proyecto crear --id-proyecto 1 --id-recurso 1
TaskManager.exe recurso-proyecto crear --id-proyecto 2 --id-recurso 3
```

### Listar Asignaciones de Recursos a Proyectos

```bash
TaskManager.exe recurso-proyecto listar [opciones]
```

**Parámetros opcionales:**
- `--id-proyecto`: Filtrar por proyecto
- `--id-recurso`: Filtrar por recurso

**Ejemplos:**
```bash
TaskManager.exe recurso-proyecto listar
TaskManager.exe recurso-proyecto listar --id-proyecto 1
TaskManager.exe recurso-proyecto listar --id-recurso 2
```

### Modificar Asignación

```bash
TaskManager.exe recurso-proyecto modificar --id <número>
```

**Parámetros:**
- `--id` (REQUERIDO): ID de la asignación

---

## Tareas

### Crear Tarea

```bash
TaskManager.exe tarea crear --id-proyecto <número> --titulo <texto> [opciones]
```

**Parámetros:**
- `--id-proyecto` (REQUERIDO): ID del proyecto
- `--titulo` (REQUERIDO): Título de la tarea
- `--detalle`: Descripción detallada
- `--prioridad`: Alta, Media o Baja (default: Media)

**Ejemplos:**
```bash
TaskManager.exe tarea crear --id-proyecto 1 --titulo "Implementar login"
TaskManager.exe tarea crear --id-proyecto 1 --titulo "Corregir bug de validación" --prioridad "Alta" --detalle "Error en formulario de registro"
TaskManager.exe tarea crear --id-proyecto 2 --titulo "Documentación" --prioridad "Baja"
```

### Listar Tareas

```bash
TaskManager.exe tarea listar [opciones]
```

**Parámetros opcionales:**
- `--id-proyecto`: Filtrar por proyecto
- `--titulo`: Buscar por título (búsqueda parcial, case-insensitive)
- `--prioridad`: Filtrar por prioridad (Alta, Media, Baja)
- `--activo`: Filtrar por estado (0 o 1)

**Ejemplos:**
```bash
TaskManager.exe tarea listar
TaskManager.exe tarea listar --id-proyecto 1
TaskManager.exe tarea listar --prioridad "Alta"
TaskManager.exe tarea listar --titulo "bug"
TaskManager.exe tarea listar --id-proyecto 1 --prioridad "Alta" --activo 1
```

### Modificar Tarea

```bash
TaskManager.exe tarea modificar --id <número> [opciones]
```

**Parámetros:**
- `--id` (REQUERIDO): ID de la tarea
- `--titulo`: Nuevo título
- `--fecha-fin`: Fecha de finalización (dd/MM/yyyy)
- `--prioridad`: Nueva prioridad
- `--activo`: Nuevo estado (0 o 1)

**Ejemplos:**
```bash
TaskManager.exe tarea modificar --id 1 --titulo "Implementar autenticación OAuth"
TaskManager.exe tarea modificar --id 5 --fecha-fin "28/12/2025" --activo 0
TaskManager.exe tarea modificar --id 3 --prioridad "Baja"
```

---

## Recursos en Tareas

### Asignar Recurso a Tarea

```bash
TaskManager.exe recurso-tarea crear --id-tarea <número> --id-recurso <número>
```

**Parámetros:**
- `--id-tarea` (REQUERIDO): ID de la tarea
- `--id-recurso` (REQUERIDO): ID del recurso

**Ejemplos:**
```bash
TaskManager.exe recurso-tarea crear --id-tarea 1 --id-recurso 2
```

### Listar Asignaciones

```bash
TaskManager.exe recurso-tarea listar [opciones]
```

**Parámetros opcionales:**
- `--id-tarea`: Filtrar por tarea
- `--id-recurso`: Filtrar por recurso

**Ejemplos:**
```bash
TaskManager.exe recurso-tarea listar
TaskManager.exe recurso-tarea listar --id-tarea 1
```

### Modificar Asignación

```bash
TaskManager.exe recurso-tarea modificar --id <número>
```

---

## Tareas Diarias

### Crear Tarea Diaria

```bash
TaskManager.exe tarea-daily crear --id-proyecto <número> --id-recurso <número> --titulo <texto>
```

**Parámetros:**
- `--id-proyecto` (REQUERIDO): ID del proyecto
- `--id-recurso` (REQUERIDO): ID del recurso
- `--titulo` (REQUERIDO): Título de la tarea

**Ejemplos:**
```bash
TaskManager.exe tarea-daily crear --id-proyecto 1 --id-recurso 1 --titulo "Daily stand-up"
TaskManager.exe tarea-daily crear --id-proyecto 2 --id-recurso 3 --titulo "Testing de módulo de pagos"
```

### Listar Tareas Diarias

```bash
TaskManager.exe tarea-daily listar [opciones]
```

**Parámetros opcionales:**
- `--id-proyecto`: Filtrar por proyecto
- `--id-recurso`: Filtrar por recurso
- `--titulo`: Buscar por título

**Ejemplos:**
```bash
TaskManager.exe tarea-daily listar
TaskManager.exe tarea-daily listar --id-proyecto 1
TaskManager.exe tarea-daily listar --id-recurso 2
```

### Modificar Tarea Diaria

```bash
TaskManager.exe tarea-daily modificar --id <número> [opciones]
```

**Parámetros:**
- `--id` (REQUERIDO): ID de la tarea diaria
- `--titulo`: Nuevo título
- `--fecha-fin`: Fecha de finalización

**Ejemplos:**
```bash
TaskManager.exe tarea-daily modificar --id 1 --titulo "Daily actualizado"
TaskManager.exe tarea-daily modificar --id 2 --fecha-fin "25/12/2025"
```

---

## Impedimentos Diarios

### Crear Impedimento

```bash
TaskManager.exe impedimento-daily crear --id-proyecto <número> --id-recurso <número> --impedimento <texto> --explicacion <texto>
```

**Parámetros:**
- `--id-proyecto` (REQUERIDO): ID del proyecto
- `--id-recurso` (REQUERIDO): ID del recurso
- `--impedimento` (REQUERIDO): Descripción breve del impedimento
- `--explicacion` (REQUERIDO): Explicación detallada

**Ejemplos:**
```bash
TaskManager.exe impedimento-daily crear --id-proyecto 1 --id-recurso 1 --impedimento "Servidor caído" --explicacion "BD no responde, esperando al DevOps"
TaskManager.exe impedimento-daily crear --id-proyecto 2 --id-recurso 2 --impedimento "Bloqueo por revisión" --explicacion "PR pendiente de aprobación"
```

### Listar Impedimentos

```bash
TaskManager.exe impedimento-daily listar [opciones]
```

**Parámetros opcionales:**
- `--id-proyecto`: Filtrar por proyecto
- `--id-recurso`: Filtrar por recurso
- `--activo`: Filtrar por estado

**Ejemplos:**
```bash
TaskManager.exe impedimento-daily listar
TaskManager.exe impedimento-daily listar --id-proyecto 1
TaskManager.exe impedimento-daily listar --activo 1
```

### Modificar Impedimento

```bash
TaskManager.exe impedimento-daily modificar --id <número> [opciones]
```

**Parámetros:**
- `--id` (REQUERIDO): ID del impedimento
- `--activo`: Nuevo estado (0 para resuelto, 1 para activo)

**Ejemplos:**
```bash
TaskManager.exe impedimento-daily modificar --id 1 --activo 0
```

---

## Reportes

### Generar Reporte

```bash
TaskManager.exe reporte generar [opciones]
```

**Parámetros opcionales:**
- `--fecha`: Fecha específica (dd/MM/yyyy)
- `--id-proyecto`: Filtrar por ID de proyecto
- `--nombre-proyecto`: Filtrar por nombre de proyecto

**Ejemplos:**
```bash
TaskManager.exe reporte generar
TaskManager.exe reporte generar --id-proyecto 1
TaskManager.exe reporte generar --nombre-proyecto "Backend API"
TaskManager.exe reporte generar --fecha "25/12/2025"
TaskManager.exe reporte generar --fecha "25/12/2025" --id-proyecto 1
```

**Salida:** Genera un archivo `reporte_YYYY-MM-DD.md` con formato Markdown.

---

## Ejemplos Prácticos

### Ejemplo 1: Crear y Gestionar un Proyecto Completo

```bash
# 1. Crear un proyecto
TaskManager.exe proyecto crear --nombre "E-commerce" --descripcion "Plataforma de ventas" --tiene-daily 1

# 2. Crear recursos
TaskManager.exe recurso crear --nombre "Carlos"
TaskManager.exe recurso crear --nombre "Elena"
TaskManager.exe recurso crear --nombre "Marco"

# 3. Asignar recursos al proyecto
TaskManager.exe recurso-proyecto crear --id-proyecto 1 --id-recurso 1
TaskManager.exe recurso-proyecto crear --id-proyecto 1 --id-recurso 2
TaskManager.exe recurso-proyecto crear --id-proyecto 1 --id-recurso 3

# 4. Crear tareas
TaskManager.exe tarea crear --id-proyecto 1 --titulo "Diseño de BD" --prioridad "Alta"
TaskManager.exe tarea crear --id-proyecto 1 --titulo "Implementar API REST" --prioridad "Alta"
TaskManager.exe tarea crear --id-proyecto 1 --titulo "Frontend con React" --prioridad "Alta"
TaskManager.exe tarea crear --id-proyecto 1 --titulo "Testing" --prioridad "Media"

# 5. Crear tareas diarias
TaskManager.exe tarea-daily crear --id-proyecto 1 --id-recurso 1 --titulo "Qué hago hoy"
TaskManager.exe tarea-daily crear --id-proyecto 1 --id-recurso 2 --titulo "Mi trabajo de hoy"

# 6. Crear impedimentos si es necesario
TaskManager.exe impedimento-daily crear --id-proyecto 1 --id-recurso 1 --impedimento "Esperando BD" --explicacion "No tenemos credentials de producción"

# 7. Generar reporte
TaskManager.exe reporte generar --id-proyecto 1
```

### Ejemplo 2: Workflow de Sprint Diario

```bash
# Mañana: Crear tareas diarias
TaskManager.exe tarea-daily crear --id-proyecto 1 --id-recurso 1 --titulo "Daily Standup"
TaskManager.exe tarea-daily crear --id-proyecto 1 --id-recurso 2 --titulo "Revisión de PR"

# A mitad de día: Reportar un impedimento
TaskManager.exe impedimento-daily crear --id-proyecto 1 --id-recurso 1 --impedimento "Merge conflict" --explicacion "Dos desarrolladores modificaron el mismo archivo"

# Al final del día: Ver tareas completadas
TaskManager.exe tarea listar --id-proyecto 1 --activo 1

# Generar reporte diario
TaskManager.exe reporte generar --fecha "25/12/2025" --id-proyecto 1
```

### Ejemplo 3: Búsquedas y Filtros

```bash
# Todas las tareas de alta prioridad
TaskManager.exe tarea listar --prioridad "Alta"

# Tareas que contienen "bug" en el título
TaskManager.exe tarea listar --titulo "bug"

# Tareas no completadas de un proyecto
TaskManager.exe tarea listar --id-proyecto 1 --activo 1

# Impedimentos activos del proyecto
TaskManager.exe impedimento-daily listar --id-proyecto 1 --activo 1

# Todas las asignaciones de un recurso
TaskManager.exe recurso-tarea listar --id-recurso 1
```

---

## Formatos Especiales

### Fechas

- **Formato**: `dd/MM/yyyy`
- **Ejemplos**: 25/12/2025, 01/01/2025, 15/06/2025

### Estados Booleanos

- `0` = Inactivo / No / Falso
- `1` = Activo / Sí / Verdadero

### Prioridades

- `Alta` - Urgente, debe hacerse pronto
- `Media` - Normal, se puede esperar
- `Baja` - Puede posponer

---

## Archivo de Log

Todas las operaciones se registran en `taskmanager.log` (ubicación configurable en `appsettings.json`).

**Consultar el log:**
```bash
# Windows PowerShell
Get-Content taskmanager.log -Tail 50

# Windows CMD
type taskmanager.log | more
```

---

## Configuración (appsettings.json)

```json
{
  "AppConfiguration": {
    "DatabasePath": "taskmanager.db",
    "PreviousDaysForReport": 3,
    "LogFilePath": "taskmanager.log"
  }
}
```

- **DatabasePath**: Ubicación del archivo de base de datos
- **PreviousDaysForReport**: Días anteriores a incluir en reportes
- **LogFilePath**: Ubicación del archivo de log

---

**¡Última actualización:** 25/12/2025
