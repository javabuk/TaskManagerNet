# 🚀 Quick Start - Comando "sugerencia" con IA

## En 3 minutos estarás obteniendo sugerencias de IA

### 1️⃣ Verificar que tienes datos

Primero, crea un proyecto y una tarea para que haya datos que analizar:

```bash
# Crear un proyecto
dotnet run -- proyecto crear --nombre "Mi Proyecto"

# Crear una tarea activa
dotnet run -- tarea crear --id-proyecto 1 --titulo "Mi Tarea" --prioridad "Alta"
```

### 2️⃣ Obtener sugerencias de IA

```bash
# Comando básico - ver sugerencias en pantalla
dotnet run -- sugerencia
```

Verás algo como:
```
✨ Recopilando tareas activas de todos los proyectos...
✨ Se encontraron 1 proyecto(s) con tareas activas.
✨ Enviando información a la IA para obtener sugerencias...

📋 Sugerencias del Experto en Project Management:
[Las sugerencias personalizadas aparecerán aquí]
```

### 3️⃣ Guardar sugerencias en archivo (Opcional)

```bash
# Guardar en archivo Markdown
dotnet run -- sugerencia P
```

Se crea un archivo como `20251230211245_Sugerencias.md` con las sugerencias.

## Troubleshooting Rápido

| Problema | Solución |
|----------|----------|
| "Invalid API Key" | Verifica que la clave en `appsettings.local.json` es correcta |
| "model_decommissioned" | Cambia `GroqModel` en `appsettings.json` a un modelo activo |
| No hay sugerencias | Crea al menos un proyecto con tareas activas |
| Error de conexión | Verifica tu conexión a internet y que Groq API esté disponible |

## Comandos Relacionados

```bash
# Ver todos los comandos
dotnet run -- help

# Ver solo la sección de IA
dotnet run -- help | grep -A 10 "INTELIGENCIA"

# Crear tareas diarias para análisis más profundo
dotnet run -- tarea-daily crear --id-proyecto 1 --id-recurso 1 --titulo "Daily"

# Registrar impedimentos
dotnet run -- impedimento-daily crear --id-proyecto 1 --id-recurso 1 --impedimento "Bloqueo" --explicacion "Esperando aprobación"
```

## Configuración Avanzada

Para cambiar el modelo o parámetros, edita `appsettings.json`:

```json
{
  "AIServices": {
    "Temperature": 0.7,           // Creatividad (0-2)
    "MaxCompletionTokens": 2048,  // Límite de palabras
    "TopP": 1.0,                  // Diversidad (0-1)
    "GroqModel": "llama-3-70b"    // Modelo actual
  }
}
```

- **Temperature más alta** (0.7-1.5) = Más creativo
- **Temperature más baja** (0.1-0.4) = Más preciso
- **MaxCompletionTokens más bajo** = Respuesta más corta

---

¿Necesitas ayuda? Ver [SETUP_IA.md](SETUP_IA.md) para configuración detallada.
