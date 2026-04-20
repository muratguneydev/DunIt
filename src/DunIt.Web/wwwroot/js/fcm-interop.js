import { initializeApp, getApps } from "https://www.gstatic.com/firebasejs/12.11.0/firebase-app.js";
import {
    getMessaging,
    getToken
} from "https://www.gstatic.com/firebasejs/12.11.0/firebase-messaging.js";

let messaging = null;

function ensureMessaging() {
    if (messaging) return messaging;
    const app = getApps()[0];
    if (!app) throw new Error("Firebase app not initialized");
    messaging = getMessaging(app);
    return messaging;
}

window.fcm_interop = {

    async getToken(vapidKey) {
        const m = ensureMessaging();
        return await getToken(m, { vapidKey });
    },

    async sendTestMessage(token, serverKey) {
        const response = await fetch("https://fcm.googleapis.com/fcm/send", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `key=${serverKey}`
            },
            body: JSON.stringify({
                to: token,
                notification: {
                    title: "DunIt Reminder",
                    body: "Time to check your chores!"
                }
            })
        });
        if (!response.ok) {
            const text = await response.text();
            throw new Error(`FCM send failed: ${response.status} ${text}`);
        }
    }
};
