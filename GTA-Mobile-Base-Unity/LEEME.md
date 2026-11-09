# Base GTA Mobile (Unity) – Argentina

## Armado (10 min)
1. Unity Hub → Unity 2022.3 LTS → plantilla **3D (URP)** (con módulo Android).
2. Copiá la carpeta `Assets/Scripts` de este zip a tu proyecto.
3. Escena nueva vacía → GameObject vacío → agregale **GameBootstrap**. Guardá la escena y agregala en File > Build Settings.
4. Edit > Project Settings > Player > Other Settings: **Active Input Handling = Both**, Scripting Backend **IL2CPP**, **ARM64**, Min API 24, Auto Graphics API OFF (Vulkan + OpenGLES3).
5. Probá en el editor: WASD, Espacio, E (subir/bajar del taxi), click derecho = cámara.

## APK con GitHub Actions
Subí el proyecto Unity completo (Assets, Packages, ProjectSettings) a GitHub junto con `.github/workflows/build-android.yml`.
Cargá los secrets `UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD` (guía: game.ci/docs/github/activation).
Actions > Build Android APK > Run workflow → bajás el APK de Artifacts.

## Optimizaciones incluidas
- Ciudad por chunks de 80 m, 1 chunk nuevo por frame, descarga automática
- Cada chunk = ~8 draw calls (mallas combinadas, materiales con instancing)
- Calidad por RAM del celu + escala de render dinámica para mantener 60 FPS
- Niebla que tapa el pop-in, sombras duras solo en edificios

## Ampliación (todo se activa solo con GameBootstrap, no hay que configurar nada nuevo)
- Obelisco de 67 m en el cruce 0,0 · ciclo día/noche (12 min) con faros del taxi de noche
- Tráfico con autos y colectivos que respetan carril derecha, giran y frenan ante obstáculos
- Peatones en las veredas y billetes para juntar ($)
- Misiones automáticas: taxi, delivery de empanadas y carrera de 5 checkpoints (con baliza amarilla)
- Policía: chocar o atropellar sube "BUSCADO"; te multan si te alcanzan, o los perdés alejándote
- HUD: plata en pesos, vida, hora, radar con calles, mensajes · botón Bocina · menú de pausa (FPS 30/60, distancia, sombras)
- Sonidos generados por código (motor, bocina, choque, monedas) · progreso guardado (plata)
- Cambios en archivos viejos: solo se agregaron miembros (MobileUI, PlayerVehicleInteractor, PerformanceManager)

## Edición San Miguel de Tucumán (basada en información real)
- Plaza Independencia con la estatua de la Libertad, Casa de Gobierno (columnas dóricas), Catedral de Nuestra Señora de la Encarnación,
  Basílica de San Francisco, Casa Histórica de la Independencia (blanca, puerta azul) y Parque 9 de Julio con laguna
- Cerro San Javier de fondo (al oeste) con el Cristo Bendicente, y arbolado de vereda con lapachos rosas, tipas y naranjos
- Calles reales (25 de Mayo, 24 de Septiembre, Laprida, Congreso, Mendoza…) en el HUD; misiones de taxi y de empanadas tucumanas a esos lugares
- Reemplaza al Obelisco (Landmarks.cs queda sin usar). Las ubicaciones son estilizadas, no un mapa exacto.

## Pasar el juego por GitHub (sin abrir Unity)
El proyecto ya incluye Packages, ProjectSettings y `Assets/Editor/CIBuild.cs`, que arma la escena y la configuración de Android solo.
1. Creá un repo en GitHub y subí este .zip (o las carpetas descomprimidas) a la rama `main`.
2. Creá el archivo `.github/workflows/build-android.yml` con el texto del workflow (si subiste solo el zip, tiene que estar así, a mano).
3. Settings > Secrets and variables > Actions: `UNITY_LICENSE` (contenido de tu Unity_lic.ulf), `UNITY_EMAIL`, `UNITY_PASSWORD`.
4. Actions > Build Android APK > Run workflow. El APK queda en Artifacts (apk-tucuman).
Nota: Unity ya no permite activación manual de licencias Personal; el .ulf sale de activar Unity Hub en una PC.
