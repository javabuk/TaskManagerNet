# Configuración de Modelos IA - Guía de Referencia Rápida

## 📌 Introducción

Este documento es una guía de referencia rápida para configurar, agregar y personalizar modelos IA en TaskManager. Para una explicación detallada de los cambios arquitectónicos, ver `Explicaciones_2026-01-04_12-35-17.md`.

---

## 🔧 Estructura de Configuración

### Archivo: `appsettings.json`

```json
{
  "AIServiceProviders": [
    {
      "Id": "identificador-unico",
      "Model": "proveedor/nombre-modelo",
      "ApiUrl": "https://api.proveedor.com/v1/chat/completions",
      "ApiKey": "",
      "Temperature": 0.6,
      "MaxCompletionTokens": 4096,
      "TopP": 0.95,
      "ReasoningEffort": "default",
      "Stop": null,
      "CustomParams": {}
    }
  ],
  "DefaultAIModel": "moonshotai/kimi-k2-instruct-0905"
}
```

### Descripción de Propiedades

| Propiedad | Tipo | Requerido | Descripción |
|-----------|------|-----------|-------------|
| `Id` | string | ✅ Sí | Identificador único para el modelo (sin espacios) |
| `Model` | string | ✅ Sí | Nombre del modelo (formato: proveedor/modelo) |
| `ApiUrl` | string | ✅ Sí | URL base de la API |
| `ApiKey` | string | ✅ Sí | Clave de autenticación (debe estar en appsettings.local.json) |
| `Temperature` | decimal | ⚠️ Opcional | Creatividad (0.0 = determinista, 2.0 = creativo) |
| `MaxCompletionTokens` | int | ⚠️ Opcional | Máximo de tokens en respuesta |
| `TopP` | decimal | ⚠️ Opcional | Probabilidad acumulativa para muestreo |
| `ReasoningEffort` | string | ⚠️ Opcional | Nivel de esfuerzo de razonamiento (ej: "default", "medium") |
| `Stop` | string | ⚠️ Opcional | Token de parada (null para no usar) |
| `CustomParams` | object | ⚠️ Opcional | Parámetros específicos del modelo/proveedor |

---

## ➕ Agregar un Nuevo Modelo

### Paso 1: Preparar Configuración en `appsettings.json`

```json
{
  "Id": "mi-nuevo-modelo",
  "Model": "provider/modelo-nuevo",
  "ApiUrl": "https://api.provider.com/v1/chat/completions",
  "ApiKey": "",
  "Temperature": 0.7,
  "MaxCompletionTokens": 2048,
  "TopP": 0.95,
  "Stop": null,
  "CustomParams": {}
}
```

### Paso 2: Configurar API Key en `appsettings.local.json`

```json
{
  "AIServiceProviders": [
    {
      "Id": "mi-nuevo-modelo",
      "Model": "provider/modelo-nuevo",
      "ApiKey": "sk_xxxxx_tu_api_key"
    }
  ]
}
```

### Paso 3: Usar el Modelo

```bash
# Via línea de comandos
dotnet run -- sugerencia --modelo "provider/modelo-nuevo"

# Con archivo de salida
dotnet run -- sugerencia --modelo "provider/modelo-nuevo" P

# Con proyecto específico
dotnet run -- sugerencia --id-proyecto 1 --modelo "provider/modelo-nuevo"
```

---

## 🎨 Ejemplos Prácticos

### Ejemplo 1: Modelo Simple (Groq Mixtral)

```json
{
  "Id": "mixtral-simple",
  "Model": "mixtral-8x7b-32768",
  "ApiUrl": "https://api.groq.com/openai/v1/chat/completions",
  "ApiKey": "",
  "Temperature": 0.6,
  "MaxCompletionTokens": 2048,
  "TopP": 1.0,
  "Stop": null,
  "CustomParams": {}
}
```

### Ejemplo 2: Modelo con Herramientas (Groq Compound)

```json
{
  "Id": "groq-compound",
  "Model": "groq/compound",
  "ApiUrl": "https://api.groq.com/openai/v1/chat/completions",
  "ApiKey": "",
  "Temperature": 1.0,
  "MaxCompletionTokens": 1024,
  "TopP": 1.0,
  "Stop": null,
  "CustomParams": {
    "compound_custom": {
      "tools": {
        "enabled_tools": [
          "web_search",
          "code_interpreter",
          "visit_website"
        ]
      }
    }
  }
}
```

### Ejemplo 3: Modelo con Reasoning (GPT-OSS)

```json
{
  "Id": "gpt-oss-reasoning",
  "Model": "openai/gpt-oss-120b",
  "ApiUrl": "https://api.groq.com/openai/v1/chat/completions",
  "ApiKey": "",
  "Temperature": 1.0,
  "MaxCompletionTokens": 8192,
  "TopP": 1.0,
  "ReasoningEffort": "medium",
  "Stop": null,
  "CustomParams": {}
}
```

---

## 🎯 Casos de Uso Recomendados

### Para Análisis Profundo
- **Modelos Recomendados:**
  - `openai/gpt-oss-120b` (ReasoningEffort: "medium")
  - `qwen/qwen3-32b`
- **Configuración:**
  - `Temperature`: 0.6-0.8 (consistencia moderada)
  - `MaxCompletionTokens`: 4096+ (respuestas largas)

### Para Respuestas Rápidas
- **Modelos Recomendados:**
  - `llama-3.1-8b-instant`
  - `groq/compound`
- **Configuración:**
  - `Temperature`: 0.7-0.9
  - `MaxCompletionTokens`: 1024-2048

### Para Análisis Creativo
- **Modelos Recomendados:**
  - `qwen/qwen3-32b` (Temperature: 0.8-1.0)
  - `moonshotai/kimi-k2-instruct-0905` (Temperature: 0.8-1.0)
- **Configuración:**
  - `Temperature`: 1.0-1.5
  - `TopP`: 0.9-1.0

### Para Control de Seguridad
- **Modelos Recomendados:**
  - `meta-llama/llama-guard-4-12b`
- **Configuración:**
  - `Temperature`: 0.2-0.5 (determinista)
  - `MaxCompletionTokens`: 1024

---

## 🔄 Parámetros Personalizados Comunes

### Para Proveedores que Soportan Web Search

```json
"CustomParams": {
  "web_search": {
    "enabled": true,
    "max_results": 5
  }
}
```

### Para Modelos con Code Interpreter

```json
"CustomParams": {
  "code_interpreter": {
    "enabled": true,
    "languages": ["python", "javascript"]
  }
}
```

### Para Modelos Multimodales

```json
"CustomParams": {
  "vision": {
    "enabled": true,
    "max_image_size": "10MB"
  }
}
```

---

## 📋 Checklist para Agregar Modelo

- [ ] Obtener API key del proveedor
- [ ] Identificar todas las propiedades soportadas
- [ ] Crear entrada en `appsettings.json` con valores por defecto
- [ ] Establecer valores correctos para Temperature, MaxCompletionTokens, TopP
- [ ] Agregar CustomParams si es necesario
- [ ] Documentar en `appsettings.local.json` con API key real
- [ ] Probar con comando: `dotnet run -- sugerencia --modelo "provider/modelo"`
- [ ] Verificar logs en `taskmanager.log`
- [ ] Ajustar parámetros según resultados

---

## 🐛 Solución de Problemas

### Modelo No Encontrado
```
Error: El modelo 'provider/modelo' no está configurado.
Modelos disponibles: moonshotai/kimi-k2-instruct-0905, openai/gpt-oss-120b
```

**Solución:**
- Verificar que el nombre del modelo en línea de comandos coincide exactamente con el valor "Model" en appsettings.json
- Usar comando: `dotnet run -- sugerencia --modelo "moonshotai/kimi-k2-instruct-0905"`

### API Key No Configurada
```
Error: API Key no está configurada para el modelo 'provider/modelo'
```

**Solución:**
- Agregar el modelo en `appsettings.local.json` con ApiKey válida
- Verificar que la ApiKey no esté vacía ("")
- Confirmar que la ApiKey es válida para ese proveedor

### Parámetros Personalizados No Funcionan
```
Error: CustomParams no se incluye en la solicitud
```

**Solución:**
- Verificar que `CustomParams` no esté vacío en la configuración
- Los CustomParams se agregan automáticamente al JSON de solicitud
- Consultar documentación del proveedor para nombres exactos de parámetros

---

## 📊 Tabla de Modelos Preconfigurados

| ID | Modelo | Temperatura | Max Tokens | TopP | Reasoning | Herramientas |
|----|--------|------------|-----------|------|-----------|--------------|
| qwen | qwen/qwen3-32b | 0.6 | 4096 | 0.95 | default | ❌ |
| groq-compound | groq/compound | 1.0 | 1024 | 1.0 | ❌ | ✅ |
| llama-instant | llama-3.1-8b-instant | 1.0 | 1024 | 1.0 | ❌ | ❌ |
| llama-guard | meta-llama/llama-guard-4-12b | 1.0 | 1024 | 1.0 | ❌ | ❌ |
| gpt-oss | openai/gpt-oss-120b | 1.0 | 8192 | 1.0 | medium | ❌ |
| kimi-default | moonshotai/kimi-k2-instruct-0905 | 0.6 | 4096 | 1.0 | ❌ | ❌ |

---

## 🔗 Enlaces Útiles

### Proveedores de API
- **Groq:** https://console.groq.com
- **OpenAI:** https://platform.openai.com/api-keys
- **Otros:** Consultar documentación del proveedor

### Documentación
- **Parámetros OpenAI-compatible:** https://platform.openai.com/docs/api-reference/chat
- **Explicaciones detalladas:** Ver `Explicaciones_2026-01-04_12-35-17.md`
- **README:** Ver `README.md` para uso general

---

## 💡 Tips y Mejores Prácticas

1. **Prueba antes de producción:** Siempre prueba un nuevo modelo con un proyecto pequeño
2. **Documenta cambios:** Si modifica temperaturas o parámetros, documentalo en un comentario
3. **Monitorea costos:** Algunos modelos pueden ser más caros; mantén registros en logs
4. **Usa el modelo adecuado:** No todos los modelos son adecuados para todos los casos
5. **Mantén secrets seguros:** API keys siempre en `appsettings.local.json`, nunca en Git
6. **Versiona modelos:** Considera agregar fecha o versión en el Id para rastrear cambios

---

**Última actualización:** 2026-01-04
