# 🔍 Guía de Debugging con Logs - TaskManager

## ¿Qué es el Archivo de Log?

El archivo `taskmanager.log` es un registro automático de **TODAS las operaciones y errores** que ocurren en la aplicación.

Se crea automáticamente en la misma carpeta que el ejecutable (ubicación configurable).

---

## 📝 Contenido del Log

### Ejemplo de un log típico:

```
[2025-12-25 12:29:05.388] [INFO] === TaskManager iniciado ===
[2025-12-25 12:29:05.408] [COMMAND] proyecto listar
[2025-12-25 12:29:07.651] [INFO] === Ejecución completada exitosamente ===
[2025-12-25 12:29:18.880] [INFO] === TaskManager iniciado ===
[2025-12-25 12:29:18.880] [COMMAND] proyecto crear --nombre Test Project --tiene-daily 1
[2025-12-25 12:29:20.934] [INFO] === Ejecución completada exitosamente ===
```

### Estructura de cada línea:

```
[TIMESTAMP] [NIVEL] MENSAJE
```

- **TIMESTAMP**: Fecha y hora exacta (incluyendo milisegundos)
- **NIVEL**: Tipo de evento (INFO, COMMAND, ERROR, WARNING, SUCCESS)
- **MENSAJE**: Descripción del evento

---

## 🔴 Niveles de Log

### INFO - Información General
```
[2025-12-25 12:29:05.388] [INFO] === TaskManager iniciado ===
[2025-12-25 12:29:20.934] [INFO] === Ejecución completada exitosamente ===
```

**Uso:** Eventos normales de la aplicación

---

### COMMAND - Comandos Ejecutados
```
[2025-12-25 12:29:05.408] [COMMAND] proyecto listar
[2025-12-25 12:29:18.880] [COMMAND] proyecto crear --nombre Test Project --tiene-daily 1
[2025-12-25 12:29:21.365] [COMMAND] tarea listar --id-proyecto 1 --prioridad Alta
```

**Uso:** Rastrear qué comandos se ejecutaron y con qué parámetros

---

### ERROR - Errores con Detalles Completos
```
[2025-12-25 13:45:22.156] [ERROR] Error en comando tarea | Exception: ArgumentException - ID requerido | StackTrace: ...
```

**Contenido:**
- Mensaje de error
- Tipo de excepción
- Stack trace completo para debugging

**Uso:** Diagnosticar problemas

---

### WARNING - Advertencias
```
[2025-12-25 12:30:15.445] [WARNING] Comando no reconocido: comando_invalido
```

**Uso:** Eventos anormales pero no críticos

---

### SUCCESS - Operaciones Completadas
```
[TIMESTAMP] [SUCCESS] Proyecto creado: ID=1
```

**Uso:** Confirmación de acciones importantes

---

## 🔧 Cómo Consultar el Log

### Opción 1: Abrir en Editor de Texto
```bash
# Windows - Notepad
notepad taskmanager.log

# Windows - WordPad
wordpad taskmanager.log
```

### Opción 2: Ver en PowerShell

**Ver todo el log:**
```powershell
Get-Content taskmanager.log
```

**Ver últimas N líneas:**
```powershell
Get-Content taskmanager.log -Tail 50
```

**Ver en tiempo real (actualización continua):**
```powershell
Get-Content taskmanager.log -Wait
```

### Opción 3: Buscar en el Log

**Buscar todos los errores:**
```powershell
Select-String "ERROR" taskmanager.log
```

**Buscar comandos de un tipo específico:**
```powershell
Select-String "proyecto" taskmanager.log
Select-String "tarea" taskmanager.log
Select-String "recurso" taskmanager.log
```

**Buscar un comando específico con fecha:**
```powershell
Select-String "tarea crear" taskmanager.log
```

**Ver solo líneas con error y contexto:**
```powershell
Select-String -Context 2,2 "ERROR" taskmanager.log
```

### Opción 4: Análisis Avanzado

**Contar total de comandos ejecutados:**
```powershell
(Select-String "COMMAND" taskmanager.log).Count
```

**Ver todos los comandos ejecutados hoy:**
```powershell
$fecha = Get-Date -Format "yyyy-MM-dd"
Select-String "\[$fecha" taskmanager.log | Select-String "COMMAND"
```

**Buscar errores en un período:**
```powershell
# Errores entre dos timestamps
Select-String "\[2025-12-25 1[23]:" taskmanager.log | Select-String "ERROR"
```

---

## 🐛 Problemas Comunes y Cómo Debuggearlos

### Problema 1: "Proyecto no encontrado"

**Comando que falla:**
```bash
TaskManager.exe proyecto modificar --id 999 --nombre "Nuevo"
```

**Búsqueda en log:**
```powershell
Select-String "proyecto modificar" taskmanager.log
Select-String "ERROR" taskmanager.log | Select-String -A 3 "proyecto"
```

**Qué buscar:**
```
[...] [ERROR] Error en comando proyecto | Exception: ...
```

---

### Problema 2: "No aparecen los datos que creé"

**Pasos:**
1. Buscar el comando de creación en el log:
```powershell
Select-String "proyecto crear" taskmanager.log
```

2. Verificar si completó exitosamente:
```powershell
Select-String -A 1 "proyecto crear" taskmanager.log
```

3. Si hay error, revisar la excepción completa:
```powershell
Select-String "ERROR" taskmanager.log | Select-String "Ejecución completada"
```

---

### Problema 3: "La aplicación se comporta extraño"

**Revisar:**
1. Ver los últimos comandos ejecutados:
```powershell
Get-Content taskmanager.log -Tail 30
```

2. Ver si hay errores ocultos:
```powershell
Select-String "ERROR\|WARNING" taskmanager.log
```

3. Ver la secuencia completa de hoy:
```powershell
$fecha = Get-Date -Format "yyyy-MM-dd"
Select-String "\[$fecha" taskmanager.log
```

---

## 📊 Ejemplos de Análisis Útiles

### Ver todos los proyectos creados

```powershell
Select-String "proyecto crear" taskmanager.log | ForEach-Object {
    if ($_ -match "--nombre\s+(\S+)") {
        $matches[1]
    }
}
```

### Contar operaciones por tipo

```powershell
$log = Get-Content taskmanager.log
Write-Host "Total comandos: $(@($log | Select-String 'COMMAND').Count)"
Write-Host "Total errores: $(@($log | Select-String 'ERROR').Count)"
Write-Host "Total advertencias: $(@($log | Select-String 'WARNING').Count)"
```

### Ver tiempo promedio entre operaciones

```powershell
$timestamps = Select-String "\[[\d\-]+ ([\d:\.]+)\]" taskmanager.log -AllMatches | 
    ForEach-Object { $_.Matches.Groups[1].Value }
"Primero: $($timestamps[0])"
"Último: $($timestamps[-1])"
```

---

## ⚙️ Configurar Ubicación del Log

Editar `appsettings.json`:

```json
{
  "AppConfiguration": {
    "DatabasePath": "taskmanager.db",
    "PreviousDaysForReport": 3,
    "LogFilePath": "taskmanager.log"
  }
}
```

### Ejemplos de ubicaciones:

**En una carpeta specific:**
```json
"LogFilePath": "C:\\Logs\\taskmanager.log"
```

**Log por fecha:**
```json
"LogFilePath": "logs\\taskmanager_2025-12-25.log"
```

**En carpeta temporal del sistema:**
```json
"LogFilePath": "%TEMP%\\taskmanager.log"
```

**Log con nombre por día:**
```json
"LogFilePath": "logs/app_${date}.log"
```

> **Nota:** La carpeta se crea automáticamente si no existe.

---

## 🔐 Limpieza del Log

### Archivar log anterior
```powershell
# Renombrar el log actual
Rename-Item taskmanager.log "taskmanager_$(Get-Date -Format 'yyyy-MM-dd').log"

# El próximo comando creará un log nuevo
```

### Borrar log
```powershell
Remove-Item taskmanager.log
```

### Guardar backup antes de limpiar
```powershell
Copy-Item taskmanager.log "taskmanager_backup_$(Get-Date -Format 'yyyy-MM-dd_HHmmss').log"
```

---

## 📈 Monitoreo Continuo

### Ver log en vivo durante pruebas
```powershell
# En PowerShell, ver actualizaciones en tiempo real
Get-Content taskmanager.log -Wait
```

```powershell
# En otra ventana de PowerShell, ejecutar comandos
.\TaskManager.exe proyecto listar
.\TaskManager.exe tarea crear --id-proyecto 1 --titulo "Test"
```

---

## 🎯 Casos de Uso Típicos

### Case 1: Auditoría de Actividad
```powershell
# Quién hizo qué (comandos ejecutados)
Select-String "COMMAND" taskmanager.log | Select-Object -Last 20
```

### Case 2: Debugging de Error
```powershell
# Obtener error completo
Select-String "ERROR" taskmanager.log -A 5
```

### Case 3: Validación de Integridad
```powershell
# Verificar que cada comando se completó
(Select-String "COMMAND" taskmanager.log).Count
(Select-String "Ejecución completada" taskmanager.log).Count
# Si son iguales, todo ok
```

### Case 4: Performance
```powershell
# Ver operaciones lentas (diferencia grande entre timestamps)
Get-Content taskmanager.log | Select-String "iniciado|completada"
```

---

## 💡 Tips de Debugging

1. **Siempre revisar ERROR primero**
   ```powershell
   Select-String "ERROR" taskmanager.log
   ```

2. **Ordenar por timestamp para secuencia correcta**
   ```powershell
   Get-Content taskmanager.log | Sort-Object
   ```

3. **Usar colores para mejor legibilidad en PowerShell**
   ```powershell
   Get-Content taskmanager.log | Select-String "ERROR" | Format-Table -AutoSize
   ```

4. **Exportar para análisis en Excel**
   ```powershell
   Select-String "COMMAND" taskmanager.log | 
   Export-Csv -Path "comandos.csv" -NoTypeInformation
   ```

5. **Buscar patrones con regex**
   ```powershell
   Select-String "\[ERROR\].*tarea.*" taskmanager.log
   ```

---

## 📚 Herramientas Recomendadas

| Herramienta | Uso | Comando |
|-----------|-----|---------|
| Notepad | Lectura simple | `notepad taskmanager.log` |
| PowerShell | Búsquedas avanzadas | `Select-String` |
| grep (si tienes WSL) | Búsquedas potentes | `grep "ERROR" taskmanager.log` |
| VS Code | Editor avanzado | Abrir archivo |
| Excel | Análisis de datos | Importar como CSV |

---

## 🔗 Ver También

- [COMANDOS.md](COMANDOS.md) - Referencia de comandos
- [MEJORAS_V2.md](MEJORAS_V2.md) - Detalles de logging
- [README.md](README.md) - Documentación general

---

**¡Los logs son tu mejor herramienta para entender qué está pasando en la aplicación!** 🔍
