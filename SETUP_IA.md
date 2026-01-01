# Configuración de la IA - Groq API

## Requisitos

Para usar el comando `sugerencia` con IA, necesitas una API key válida de Groq.

## Pasos de Configuración

### 1. Obtener API Key de Groq

1. Ve a https://console.groq.com
2. Crea una cuenta o inicia sesión
3. Ve a la sección de "API Keys"
4. Copia tu API key

### 2. Configurar la API Key Localmente

#### Windows

Edita el archivo `appsettings.local.json` en la raíz del proyecto:

```json
{
  "AIServices": {
    "GroqApiKey": "gsk_TU_API_KEY_AQUI"
  }
}
```

Reemplaza `gsk_TU_API_KEY_AQUI` con tu API key real.

**⚠️ IMPORTANTE:** Este archivo está en `.gitignore` y NO se debe subir a Git.

### 3. Seleccionar el Modelo Correcto

Edita `appsettings.json` y verifica el modelo disponible:

```json
{
  "AIServices": {
    "GroqApiUrl": "https://api.groq.com/openai/v1/chat/completions",
    "GroqApiKey": "",
    "GroqModel": "mixtral-8x7b-32768",
    "Temperature": 0.6,
    "MaxCompletionTokens": 2048,
    "TopP": 1.0
  }
}
```

**Modelos disponibles (verificar en https://console.groq.com/docs/models):**
- `mixtral-8x7b-32768` - Modelo rápido y poderoso
- `llama-3.1-70b-versatile` - Modelo versátil de Llama 3
- `llama2-70b-4096` - Modelo anterior de Llama 2
- Otros modelos según tu plan

Si recibis un error `model_decommissioned`, contacta a Groq para confirmar qué modelos están disponibles para tu cuenta.

## Verificar la Configuración

Ejecuta el comando para verificar que funciona:

```bash
dotnet run -- sugerencia
```

Deberías ver:
1. "Recopilando tareas activas..."
2. "Se encontraron X proyecto(s) con tareas activas"
3. Las sugerencias de IA en pantalla

## Solución de Problemas

### Error: "Invalid API Key"

- Verifica que has copiado correctamente la clave
- Asegúrate de que es una clave válida (comienza con `gsk_`)
- Verifica que la clave no haya expirado en la consola de Groq

### Error: "model_decommissioned"

- El modelo que especificaste ya no está disponible
- Verifica los modelos activos en https://console.groq.com/docs/models
- Actualiza `GroqModel` en `appsettings.json` con un modelo disponible

### Error: "model_not_found"

- El nombre del modelo es incorrecto
- Asegúrate de escribir el nombre exactamente como aparece en la documentación
- Nota: Los nombres pueden cambiar, verifica siempre la documentación oficial

### Error: "temperature: number must be at most 2"

- Este error generalmente indica que el parámetro no se está parseando correctamente
- Asegúrate de que `appsettings.json` tiene valores numéricos válidos (0.6, no "0,6")
- Si cambias CultureInfo del sistema a una región europea, asegúrate de usar punto decimal en JSON

## Uso del Comando

### Mostrar sugerencias en pantalla

```bash
dotnet run -- sugerencia
```

### Guardar sugerencias en archivo Markdown

```bash
dotnet run -- sugerencia P
```

Esto creará un archivo `yyyyMMddHHmmss_Sugerencias.md` en el directorio actual.

## Costos

Groq ofrece una capa gratuita con límites razonables para desarrollo. Verifica tu uso en:
https://console.groq.com/account

## Documentación Oficial

- Documentación: https://console.groq.com/docs/
- Modelos disponibles: https://console.groq.com/docs/models
- API Reference: https://console.groq.com/docs/api-reference
- Deprecations: https://console.groq.com/docs/deprecations

---

**Última actualización:** 30/12/2025
