#model #events

---

> [!warning] Missing Functionality
> The [[AvailableUserEvent]] features both signing up for support and picking a location, both in special cases. **This is not supported in this system yet.**
> 
> The fields `supportAvailable` and `requiresChoosingLocation` can be used to determine whether any of these special cases are true for any given user event.
### `rir:Information` Description
Model for user events that are open for registering or already registered (but still open for unregistering). 

The `participatorId` and `supportId` are used in signing up for support on a given event (not currently implemented). `id` is used for registering and unregistering to/from the event.
### `fas:List` Fields

`id` : [[string]] | [[null]]
The id of the user event, used for registering and unregistering. This will be null if it is not possible to unregister. This means registration for the user event has closed.

`title` : [[string]]
The title of the event, as set in KronoX.

`type` : [[string]]
The type of the user event, as set in KronoX. There are no set values this can be, it is simply a string value.

`eventStart` : [[iso-date-string]]

`eventEnd` : [[iso-date-string]]

`lastSignupDate` : [[iso-date-string]]
The last date and time at which the user event can be registered to. After this registration is closed and the `id` field should start returning [[null]].

`participatorId` : [[string]] | [[null]]
The id of the participator.

`supportId` : [[string]] | [[null]]
An id given to people who have access to get support. If a user does not qualify for support this should be [[null]].

`anonymousCode` : [[string]]
An anonymous code from KronoX's system for the user, which can be any string value.

`isRegistered` : [[boolean]]
Whether the user is registered to the user event or not.

`supportAvailable` : [[boolean]]
Whether the user qualifies for support or not. If the user qualifies they should be referred to the KronoX website for signing up for their support.

`requiresChoosingLocation` : [[boolean]]
Whether the user event requires choosing a location on registration. If this is true, the user should be referred to the KronoX website for registration. We cannot register them through the app.

### `far:QuestionCircle` Example
```json
{
  "id": "string",
  "title": "string",
  "type": "string",
  "eventStart": "2019-08-24T14:15:22Z",
  "eventEnd": "2019-08-24T14:15:22Z",
  "lastSignupDate": "2019-08-24T14:15:22Z",
  "participatorId": "string",
  "supportId": "string",
  "anonymousCode": "string",
  "isRegistered": true,
  "supportAvailable": true,
  "requiresChoosingLocation": true
}
```