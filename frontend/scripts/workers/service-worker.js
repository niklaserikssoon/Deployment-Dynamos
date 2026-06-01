const CACHE_NAME = "workout-app-alpha";

const STATIC_ASSETS = [
  "/",
  "/index.html",
  "/src/exerciseBank.html",
  "/styles/style.css",
  "/styles/side-panel.css",
  "/scripts/main.js",
  "/scripts/theme.js",
  "/scripts/side-panel.js",
  "/manifest.json",
  "/assets/gymHero_480.webp",
  "/assets/gymHero_768.webp",
  "/assets/gymHero_1280.webp"
];

self.addEventListener("install", event => {
  event.waitUntil(
    caches.open(CACHE_NAME).then(async cache => {
      for (const url of STATIC_ASSETS) {
        try {
          const response = await fetch(url);
          if (!response.ok) throw new Error(`HTTP ${response.status}`);
          await cache.put(url, response.clone());
          console.log("Cached:", url);
        } catch (err) {
          console.warn("Failed to cache:", url, err);
        }
      }
    })
  );
  self.skipWaiting();
});

self.addEventListener("activate", event => {
  event.waitUntil(
    caches.keys().then(keys =>
      Promise.all(
        keys.filter(key => key !== CACHE_NAME)
            .map(key => caches.delete(key))
      )
    )
  );
  self.clients.claim();
});

self.addEventListener("fetch", event => {
  event.respondWith(
    caches.match(event.request).then(cached => {
      return cached || fetch(event.request);
    })
  );
});