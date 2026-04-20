// During development, always fetch from the network
self.addEventListener('fetch', () => { });

self.addEventListener('push', event => {
    const payload = event.data?.json() ?? {};
    const title = payload.title ?? 'DunIt';
    const body = payload.body ?? 'Time to check your chores!';

    event.waitUntil(
        self.registration.showNotification(title, { body })
    );
});

self.addEventListener('notificationclick', event => {
    event.notification.close();
    event.waitUntil(
        clients.matchAll({ type: 'window', includeUncontrolled: true }).then(windowClients => {
            for (const client of windowClients) {
                if ('focus' in client) return client.focus();
            }
            return clients.openWindow('/');
        })
    );
});
