# DunIt — Push Reminders Plan

## Iteration 32: Push Reminders

This plan covers implementing push reminders using Firebase Cloud Messaging in the DunIt PWA.

### Goals
- Add daily reminder notifications for users.
- Handle permission requests cleanly.
- Use Firebase Cloud Messaging (FCM) for notification delivery.
- Schedule reminders locally in the browser.
- Provide settings to enable/disable reminders and select reminder time.
- Support notification display even when the app is in the background.

### Prerequisites & Architecture Decisions
- Use FCM for push notifications.
- Extend the existing `service-worker.js` to handle push events.
- Add FCM token management in the web app.
- Use local scheduling logic for daily reminders.
- Allow users to opt in or opt out via settings.
- Persist reminder settings in browser storage.

### Detailed Iteration Plan

| # | Status | Iteration | Red (test) | Green (code) |
|---|--------|-----------|-----------|--------------|
| 32.1 | ✅ | Notification permission model | `ShouldRequestPermission_WhenAppLoads` / `ShouldHandlePermissionDenied_WhenUserDeclines` | `NotificationService.RequestPermission()` + permission state tracking |
| 32.2 | ✅ | FCM token management | `ShouldGetFcmToken_WhenPermissionGranted` / `ShouldHandleTokenError_WhenFcmFails` | `FcmService.GetToken()` + token storage in localStorage |
| 32.3 | ✅ | Reminder settings model | `ShouldHaveDefaultSettings_WhenNoSettingsSaved` / `ShouldSaveSettings_WhenUserUpdates` | `ReminderSettings` record with enabled/time + localStorage persistence |
| 32.4 | ⬜ | Daily reminder scheduling | `ShouldScheduleReminder_WhenEnabledAndTimeSet` / `ShouldCancelReminder_WhenDisabled` | `ReminderScheduler.ScheduleDaily()` using `setTimeout` |
| 32.5 | ⬜ | Service worker push handler | `ShouldShowNotification_WhenPushReceived` / `ShouldOpenApp_WhenNotificationClicked` | Enhanced `service-worker.js` with `push` and `notificationclick` events |
| 32.6 | ⬜ | FCM message sending | `ShouldSendTestMessage_WhenTokenValid` / `ShouldHandleSendError_WhenTokenInvalid` | `FcmService.SendTestMessage()` for development testing |
| 32.7 | ⬜ | Settings UI component | `ShouldDisplayCurrentSettings_WhenLoaded` / `ShouldUpdateSettings_WhenSaved` | Blazor component for enabling/disabling reminders and setting time |
| 32.8 | ⬜ | Integration with app lifecycle | `ShouldInitializeNotifications_WhenAppStarts` / `ShouldScheduleOnSettingsChange_WhenSettingsUpdated` | Wire up services in `Program.cs` and handle settings changes |
| 32.9 | ⬜ | E2E notification test | `ShouldReceivePushNotification_WhenReminderTriggers` | Playwright test for full notification flow (requires browser permissions) |

### Technical Implementation Notes

- **Firebase Configuration Updates**: Add FCM config to `FirebaseConfig.cs` and `IFirebaseAppSettings.cs`, and update `firebase-interop.js` with FCM imports and token management.
- **Service Worker Enhancements**: Register FCM background message handler; handle `push` events to show notifications; handle `notificationclick` to open the app.
- **Scheduling Logic**: Use client-side scheduling with `setTimeout` for daily reminders; calculate next reminder time based on user settings; persist state across restarts.
- **Testing Strategy**: Unit-test business logic for scheduling and settings; add integration tests for token management; add E2E tests for notification flow and permission handling.
- **Browser Compatibility**: Modern browsers support the Push API and Notifications API; gracefully degrade when unsupported.

### Dependencies
- Firebase Cloud Messaging SDK (included in Firebase JS SDK)
- No additional NuGet packages required.

### Risk Mitigation
- Always check notification permission status before FCM operations.
- Handle FCM token refresh events.
- Ensure reminders work offline by using client-side scheduling.
- Schedule reminders in local time rather than UTC.
