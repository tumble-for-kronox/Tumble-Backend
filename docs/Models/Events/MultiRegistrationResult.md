#model #events

---
### `rir:Information` Description
Model containing two lists, one of successful registrations and one of failed registrations. Used as response to automatic exam signup
### `fas:List` Fields

`successfulRegistrations` : [[array]]<[[AvailableUserEvent]]>
User events that the user was successfully registered to. 

`failedRegistrations` : [[array]]<[[AvailableUserEvent]]>
User events that the user was not successfully registered to.

### `far:QuestionCircle` Example
```json
{
  "successfulRegistrations": [
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
  "failedRegistrations": [
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
