const version = '20240909224407';
const cacheName = `static::${version}`;

const buildContentBlob = () => {
  return ["/CoralQuest/2024/09/03/Singleton.html","/CoralQuest/2024/09/02/MetodosExtensao.html","/CoralQuest/2024/09/01/Raycasts.html","/CoralQuest/2024/08/31/Update-FixedUpdate.html","/CoralQuest/2024/08/30/GetComponent.html","/CoralQuest/creditos/","/CoralQuest/","/CoralQuest/about/","/CoralQuest/tutoriais/","/CoralQuest/manifest.json","/CoralQuest/assets/search.json","/CoralQuest/assets/styles.css","/CoralQuest/redirects.json","/CoralQuest/feed.xml","/CoralQuest/sitemap.xml","/CoralQuest/robots.txt","", "/CoralQuest/assets/default-offline-image.png", "/CoralQuest/assets/scripts/fetch.js"
  ]
}

const updateStaticCache = () => {
  return caches.open(cacheName).then(cache => {
    return cache.addAll(buildContentBlob());
  });
};

const clearOldCache = () => {
  return caches.keys().then(keys => {
    // Remove caches whose name is no longer valid.
    return Promise.all(
      keys
        .filter(key => {
          return key !== cacheName;
        })
        .map(key => {
          console.log(`Service Worker: removing cache ${key}`);
          return caches.delete(key);
        })
    );
  });
};

self.addEventListener("install", event => {
  event.waitUntil(
    updateStaticCache().then(() => {
      console.log(`Service Worker: cache updated to version: ${cacheName}`);
    })
  );
});

self.addEventListener("activate", event => {
  event.waitUntil(clearOldCache());
});

self.addEventListener("fetch", event => {
  let request = event.request;
  let url = new URL(request.url);

  // Only deal with requests from the same domain.
  if (url.origin !== location.origin) {
    return;
  }

  // Always fetch non-GET requests from the network.
  if (request.method !== "GET") {
    event.respondWith(fetch(request));
    return;
  }

  // Default url returned if page isn't cached
  let offlineAsset = "/offline/";

  if (request.url.match(/\.(jpe?g|png|gif|svg)$/)) {
    // If url requested is an image and isn't cached, return default offline image
    offlineAsset = "/CoralQuest/assets/default-offline-image.png";
  }

  // For all urls request image from network, then fallback to cache, then fallback to offline page
  event.respondWith(
    fetch(request).catch(async () => {
      return (await caches.match(request)) || caches.match(offlineAsset);
    })
  );
  return;
});
