import { initializeApp } from "https://www.gstatic.com/firebasejs/12.11.0/firebase-app.js";
import {
    getFirestore,
    connectFirestoreEmulator,
    collection, doc,
    getDocs, setDoc, deleteDoc,
    query, where,
    Timestamp,
    onSnapshot
} from "https://www.gstatic.com/firebasejs/12.11.0/firebase-firestore.js";
import {
    getAuth,
    connectAuthEmulator,
    signInWithEmailAndPassword,
    signOut as firebaseSignOut
} from "https://www.gstatic.com/firebasejs/12.11.0/firebase-auth.js";

let db = null;
let auth = null;
const subscriptions = {};

window.firebase_interop = {

    init(config) {
        const app = initializeApp(config);
        db = getFirestore(app);
        auth = getAuth(app);
        if (config.isUsingEmulator) {
            const [host, port] = config.emulatorHost.split(":");
            connectFirestoreEmulator(db, host, parseInt(port));
        }
        if (config.authEmulatorHost) {
            connectAuthEmulator(auth, `http://${config.authEmulatorHost}`, { disableWarnings: true });
        }
    },

    // ── Auth ────────────────────────────────────────────────────────────────

    async signIn(email, password) {
        await signInWithEmailAndPassword(auth, email, password);
    },

    async signOut() {
        await firebaseSignOut(auth);
    },

    async hasCurrentUser() {
        await auth.authStateReady();
        return auth.currentUser !== null;
    },

    // ── Children ────────────────────────────────────────────────────────────

    async getChildren() {
        const snap = await getDocs(collection(db, "children"));
        return snap.docs.map(d => ({ id: d.id, ...d.data() }));
    },

    async addChild(child) {
        await setDoc(doc(db, "children", child.id), { name: child.name, avatar: child.avatar });
        return child;
    },

    async deleteChild(childId) {
        await deleteDoc(doc(db, "children", childId));
    },

    // ── Chores ──────────────────────────────────────────────────────────────

    async getChoresForChild(childId) {
        const q = query(collection(db, "chores"), where("assignedTo", "==", childId));
        const snap = await getDocs(q);
        return snap.docs.map(d => ({ id: d.id, ...d.data() }));
    },

    async addChore(chore) {
        await setDoc(doc(db, "chores", chore.id), {
            title: chore.title,
            assignedTo: chore.assignedTo,
            scheduleType: chore.scheduleType
        });
        return chore;
    },

    async deleteChore(choreId) {
        await deleteDoc(doc(db, "chores", choreId));
    },

    // ── Completions ─────────────────────────────────────────────────────────

    async getCompletionsFor(childId, date) {
        // date is "yyyy-MM-dd"; query the completedDate string field to avoid timezone issues
        const q = query(
            collection(db, "completions"),
            where("childId", "==", childId),
            where("completedDate", "==", date)
        );
        const snap = await getDocs(q);
        return snap.docs.map(d => ({
            id: d.id,
            choreId: d.data().choreId,
            childId: d.data().childId,
            completedAt: d.data().completedAt.toDate().toISOString()
        }));
    },

    async completeChore(completion) {
        const completedAt = Timestamp.fromDate(new Date(completion.completedAt));
        const completedDate = completion.completedAt.substring(0, 10); // "yyyy-MM-dd"
        await setDoc(doc(db, "completions", completion.id), {
            choreId: completion.choreId,
            childId: completion.childId,
            completedAt,
            completedDate
        });
        return completion;
    },

    async undoChore(completionId) {
        await deleteDoc(doc(db, "completions", completionId));
    },

    // ── Real-time subscriptions ──────────────────────────────────────────────

    subscribeToChildren(subscriptionId, dotNetRef) {
        const unsub = onSnapshot(collection(db, "children"), snap => {
            const children = snap.docs.map(d => ({ id: d.id, ...d.data() }));
            dotNetRef.invokeMethodAsync("OnDataChanged", children);
        });
        subscriptions[subscriptionId] = unsub;
    },

    subscribeToChores(childId, subscriptionId, dotNetRef) {
        const q = query(collection(db, "chores"), where("assignedTo", "==", childId));
        const unsub = onSnapshot(q, snap => {
            const chores = snap.docs.map(d => ({ id: d.id, ...d.data() }));
            dotNetRef.invokeMethodAsync("OnDataChanged", chores);
        });
        subscriptions[subscriptionId] = unsub;
    },

    subscribeToCompletions(childId, date, subscriptionId, dotNetRef) {
        const q = query(
            collection(db, "completions"),
            where("childId", "==", childId),
            where("completedDate", "==", date)
        );
        const unsub = onSnapshot(q, snap => {
            const completions = snap.docs.map(d => ({
                id: d.id,
                choreId: d.data().choreId,
                childId: d.data().childId,
                completedAt: d.data().completedAt.toDate().toISOString()
            }));
            dotNetRef.invokeMethodAsync("OnDataChanged", completions);
        });
        subscriptions[subscriptionId] = unsub;
    },

    unsubscribe(subscriptionId) {
        if (subscriptions[subscriptionId]) {
            subscriptions[subscriptionId]();
            delete subscriptions[subscriptionId];
        }
    }
};
