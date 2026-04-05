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
| # | Status | Iteration | Red (test) | Green (code) |
|---|--------|-----------|-----------|--------------|
| 1 | ✅ | **Child model** | `ShouldHaveDefaultAvatar_WhenAvatarNotProvided` | `Child` record with Id, Name, Avatar (default 🧒) |
| 2 | ✅ | **ChoreCompletion model** | no test — no logic | `ChoreCompletion` record: Id, ChoreId, ChildId, CompletedAt (DateTimeOffset) |
| 3 | ✅ | **Chore scheduling** | `ShouldBeScheduledForToday_WhenTodayIsInDaysOfWeek` / `ShouldNotBeScheduled_WhenTodayIsNotInDaysOfWeek` | `Chore.IsScheduledFor(DateTimeOffset)` method |

## Phase 2: In-Memory Chore Repository
| # | Status | Iteration | Red (test) | Green (code) |
|---|--------|-----------|-----------|--------------|
| 4 | ✅ | **Add chore** | `ShouldAddChore_WhenValidChoreProvided` | `InMemoryChoreRepository.AddChore` |
| 5 | ✅ | **Get chores for child** | `ShouldReturnChores_WhenChildHasAssignedChores` | `GetChoresForChild` |
| 6 | ✅ | **Complete a chore** | `ShouldRecordCompletion_WhenChoreCompleted` | `CompleteChore` + `GetCompletionsFor` |
| 7 | ✅ | **Undo completion** | `ShouldRemoveCompletion_WhenUndone` | `UndoChore` |
| 8 | ✅ | **Delete chore** | `ShouldRemoveChore_WhenDeleted` | `DeleteChore` |

## Phase 3: In-Memory Child Repository
| # | Status | Iteration | Red (test) | Green (code) |
|---|--------|-----------|-----------|--------------|
| 9 | ✅ | **Add child** | `ShouldAddChild_WhenValidChildProvided` | `InMemoryChildRepository.AddChild` |
| 10 | ✅ | **List children** | `ShouldReturnAllChildren_WhenChildrenExist` | `GetChildren` |
| 11 | ✅ | **Delete child** | `ShouldRemoveChild_WhenDeleted` | `DeleteChild` |

## Phase 4: Daily Chore View Logic
| # | Status | Iteration | Red (test) | Green (code) |
|---|--------|-----------|-----------|--------------|
| 12 | ✅ | **Today's chores** | `ShouldReturnOnlyTodaysChores_WhenChoresHaveDifferentSchedules` | `DailyChoreViewModel` combining chores + completions |
| 13 | ✅ | **Completion status** | `ShouldShowCompleted_WhenChoreHasCompletionForToday` | ViewModel marks done/not-done |
| 14 | ✅ | **Progress count** | `ShouldCalculateProgress_WhenSomeChoresCompleted` | `3/5 done` style progress |

## Phase 5: Blazor UI (light testing, mostly wiring)
| # | Status | Iteration | Description |
|---|--------|-----------|-------------|
| 15 | ✅ | **Daily chores page** | List today's chores with checkboxes per child |
| 16 | ✅ | **Child selector** | Switch between children |
| 17 | ✅ | **Parent admin page** | Add/remove chores and children |
| 18 | ✅ | **Mobile CSS** | Responsive layout for phone screens |

## Phase 6: Firebase Integration
| # | Status | Iteration | Description |
|---|--------|-----------|-------------|
| 19 | ⬜ | **Firebase JS interop** | `firebase-interop.js` with Firestore CRUD |
| 20 | ⬜ | **FirebaseChoreRepository** | Implements `IChoreRepository` via JS interop |
| 21 | ⬜ | **FirebaseChildRepository** | Implements `IChildRepository` via JS interop |
| 22 | ⬜ | **Firebase Auth** | Simple login per family member |
| 23 | ⬜ | **Real-time sync** | Firestore `onSnapshot` listeners for live updates |

## Phase 7: End-to-End Tests (Playwright)
| # | Status | Iteration | Description |
|---|--------|-----------|-------------|
| 27 | ✅ | **Playwright setup** | `DunIt.IntegrationTests` project + Playwright Docker image in Compose |
| 28 | ✅ | **Daily chores E2E** | Navigate to page, verify chores listed per child |
| 29 | ✅ | **Complete/undo E2E** | Tap chore → moves to completed; tap again → moves back |
| 30 | ✅ | **Child selector E2E** | Switch children, verify correct chores shown |

## Phase 8: PWA & Polish
| # | Status | Iteration | Description |
|---|--------|-----------|-------------|
| 31 | ⬜ | **Offline support** | Service worker caching for offline use |
| 32 | ⬜ | **Push reminders** | Firebase Cloud Messaging for daily nudges |
| 33 | ⬜ | **Deploy to Firebase Hosting** | `firebase deploy` |
