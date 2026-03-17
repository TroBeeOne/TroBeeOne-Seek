self.addEventListener('install', (event) => {
  self.skipWaiting();
});

self.addEventListener('activate', (event) => {
  event.waitUntil(clients.claim());
});

self.addEventListener('fetch', (event) => {
  const isSameOrigin = event.request.url.startsWith(self.location.origin);
  
  event.respondWith(
    fetch(event.request, {
      credentials: isSameOrigin ? 'include' : 'same-origin'
    }).catch(() => {
      // Fallback logic
      return new Response('Network error occurred', { status: 408, headers: { 'Content-Type': 'text/plain' } });
    })
  );
});