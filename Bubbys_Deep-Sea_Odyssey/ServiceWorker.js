const cacheName = "GA Tech Play _ Learn Lab-Bubby's Deep-Sea Odyssey-1.2";
const contentToCache = [
    "Build/Bubby's Deep-Sea Odyssey - April 2025 Build.loader.js",
    "Build/Bubby's Deep-Sea Odyssey - April 2025 Build.framework.js.unityweb",
    "Build/Bubby's Deep-Sea Odyssey - April 2025 Build.data.unityweb",
    "Build/Bubby's Deep-Sea Odyssey - April 2025 Build.wasm.unityweb",
    "TemplateData/style.css"

];

self.addEventListener('install', function (e) {
    console.log('[Service Worker] Install');
    
    e.waitUntil((async function () {
      const cache = await caches.open(cacheName);
      console.log('[Service Worker] Caching all: app shell and content');
      await cache.addAll(contentToCache);
    })());
});

self.addEventListener('fetch', function (e) {
    e.respondWith((async function () {
      let response = await caches.match(e.request);
      console.log(`[Service Worker] Fetching resource: ${e.request.url}`);
      if (response) { return response; }

      response = await fetch(e.request);
      const cache = await caches.open(cacheName);
      console.log(`[Service Worker] Caching new resource: ${e.request.url}`);
      cache.put(e.request, response.clone());
      return response;
    })());
});
