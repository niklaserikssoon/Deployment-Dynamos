const CACHE_NAME = 'sq-workout-v4'

const FILES_TO_CACHE = [
  '/',
  '/src/index.html',
  '/src/exerciseBank.html',
  '/src/items-list.html',
  '/src/edit-item.html',
  '/styles/base.css',
  '/styles/layout.css',
  '/styles/components.css',
  '/styles/side-panel.css',
  '/styles/edit-item.css',
  '/scripts/main.js',
  '/scripts/theme.js',
  '/scripts/side-panel.js',
  '/scripts/create-Item.js',
  '/scripts/ui/index.js',
  '/scripts/ui/exercise-bank.js',
  '/scripts/ui/workouts-form.js',
  '/scripts/ui/generateWorkout.js',
  '/scripts/storage/exercises.js',
  '/scripts/storage/workouts.js',
  '/scripts/logic/search.js',
  '/scripts/logic/exerciseLogic.js',
]

self.addEventListener('install', (event) => {
  event.waitUntil(
    caches.open(CACHE_NAME).then((cache) => {
      console.log('Service worker: caching files')
      return cache.addAll(FILES_TO_CACHE)
    })
  )
})

self.addEventListener('activate', (event) => {
  event.waitUntil(
    caches.keys().then((keyList) =>
      Promise.all(
        keyList.map((key) => {
          if (key !== CACHE_NAME) {
            console.log('Service worker: removing old cache', key)
            return caches.delete(key)
          }
        })
      )
    )
  )
})

self.addEventListener('fetch', (event) => {
  event.respondWith(
    caches.match(event.request).then((response) => {
      return response || fetch(event.request)
    })
  )
})
