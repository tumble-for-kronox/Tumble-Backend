#model #events

---
### `rir:Information` Description
A collection of user events, sorted into lists depending on their statuses. Contains a list of upcoming events, events that the user is already signed up for, and events that are available for the user to sign up for.
### `fas:List` Fields

`upcomingEvents` : [[array]]<[[UpcomingUserEvent]]>
User events that the user have coming up.

`registeredEvents` : [[array]]<[[AvailableUserEvent]]>
User events that the user is already registered to.

`availableEvents` : [[array]]<[[AvailableUserEvent]]>
User events that are open for registration, but the user is not currently registered to.

### `far:QuestionCircle` Example
```json
{
  "upcomingEvents": [
    {
      "title": "string",
      "type": "string",
      "eventStart": "2019-08-24T14:15:22Z",
      "eventEnd": "2019-08-24T14:15:22Z",
      "firstSignupDate": "2019-08-24T14:15:22Z"
    }
  ],
  "registeredEvents": [
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
  ],
  "availableEvents": [
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
  ]
}
```
