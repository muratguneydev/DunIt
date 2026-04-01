# DunIt — TDD Development Plan

## What We Are Building

DunIt is a family chore tracker for kids. A parent assigns daily chores to each child. Children open the app on their phone, see their chores for the day, and tap to mark each one done. The parent can see each child's progress in real time.

- **4 users:** 1 parent (manages chores and children) + up to 3 children
- **Platform:** Web app (PWA), installable on iPhone and Android — no App Store needed
- **Data:** Firebase Firestore — all phones sync in real time with no server to manage
- **Stack:** Blazor WebAssembly, C# .NET, Firebase (Firestore + Auth + Hosting)

---

Each iteration follows **Red → Green → Review → Refactor → Commit**.

## Phase 1: Domain Models
| # | Iteration | Red (test) | Green (code) |
|---|-----------|-----------|--------------|
| 1 | **Child model** | `ShouldHaveDefaultAvatar_WhenAvatarNotProvided` | `Child` record with Id, Name, Avatar (default 🧒) |
| 2 | **ChoreCompletion model** | no test — no logic | `ChoreCompletion` record: Id, ChoreId, ChildId, CompletedAt (DateTimeOffset) |
| 3 | **Chore scheduling** | `ShouldBeScheduledForToday_WhenTodayIsInDaysOfWeek` / `ShouldNotBeScheduled_WhenTodayIsNotInDaysOfWeek` | `Chore.IsScheduledFor(DateOnly)` method |

## Phase 2: In-Memory Chore Service
| # | Iteration | Red (test) | Green (code) |
|---|-----------|-----------|--------------|
| 4 | **Add chore** | `ShouldAddChore_WhenValidChoreProvided` | `InMemoryChoreService.AddChoreAsync` |
| 5 | **Get chores for child** | `ShouldReturnChores_WhenChildHasAssignedChores` | `GetChoresForChildAsync` |
| 6 | **Complete a chore** | `ShouldRecordCompletion_WhenChoreCompleted` | `CompleteChoreAsync` + `GetCompletionsForDateAsync` |
| 7 | **Undo completion** | `ShouldRemoveCompletion_WhenUndone` | `UndoChoreAsync` |
| 8 | **Delete chore** | `ShouldRemoveChore_WhenDeleted` | `DeleteChoreAsync` |

## Phase 3: In-Memory Child Service
| # | Iteration | Red (test) | Green (code) |
|---|-----------|-----------|--------------|
| 9 | **Add child** | `ShouldAddChild_WhenValidChildProvided` | `InMemoryChildService.AddChildAsync` |
| 10 | **List children** | `ShouldReturnAllChildren_WhenChildrenExist` | `GetChildrenAsync` |
| 11 | **Delete child** | `ShouldRemoveChild_WhenDeleted` | `DeleteChildAsync` |

## Phase 4: Daily Chore View Logic
| # | Iteration | Red (test) | Green (code) |
|---|-----------|-----------|--------------|
| 12 | **Today's chores** | `ShouldReturnOnlyTodaysChores_WhenChoresHaveDifferentSchedules` | `DailyChoreViewModel` combining chores + completions |
| 13 | **Completion status** | `ShouldShowCompleted_WhenChoreHasCompletionForToday` | ViewModel marks done/not-done |
| 14 | **Progress count** | `ShouldCalculateProgress_WhenSomeChoresCompleted` | `3/5 done` style progress |

## Phase 5: Blazor UI (light testing, mostly wiring)
| # | Iteration | Description |
|---|-----------|-------------|
| 15 | **Daily chores page** | List today's chores with checkboxes per child |
| 16 | **Child selector** | Switch between children |
| 17 | **Parent admin page** | Add/remove chores and children |
| 18 | **Mobile CSS** | Responsive layout for phone screens |

## Phase 6: Firebase Integration
| # | Iteration | Description |
|---|-----------|-------------|
| 19 | **Firebase JS interop** | `firebase-interop.js` with Firestore CRUD |
| 20 | **FirebaseChoreService** | Implements `IChoreService` via JS interop |
| 21 | **FirebaseChildService** | Implements `IChildService` via JS interop |
| 22 | **Firebase Auth** | Simple login per family member |
| 23 | **Real-time sync** | Firestore `onSnapshot` listeners for live updates |

## Phase 7: PWA & Polish
| # | Iteration | Description |
|---|-----------|-------------|
| 24 | **Offline support** | Service worker caching for offline use |
| 25 | **Push reminders** | Firebase Cloud Messaging for daily nudges |
| 26 | **Deploy to Firebase Hosting** | `firebase deploy` |
