#model #events

---
### `rir:Information` Description
Model of a user event that is not yet available for signing up, but is soon to be an [[AvailableUserEvent]].
### `fas:List` Fields

`title` : [[string]]
The title of the event, as set in KronoX.

`type` : [[string]]
The type of the user event, as set in KronoX. There are no set values this can be, it is simply a string value.

`eventStart` : [[iso-date-string]]

`eventEnd` : [[iso-date-string]]

`firstSignupDate` : [[iso-date-string]]
The first date and time it is possible to sign up for the user event. Also the date and time on which the user event should become an [[AvailableUserEvent]].

### `far:QuestionCircle` Example
```json
{
  "title": "string",
  "type": "string",
  "eventStart": "2019-08-24T14:15:22Z",
  "eventEnd": "2019-08-24T14:15:22Z",
  "firstSignupDate": "2019-08-24T14:15:22Z"
}
```
