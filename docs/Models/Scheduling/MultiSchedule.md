#model #scheduling 

---
### `rir:Information` Description
Version of the [[Schedule]] model which combines a list of KronoX schedule ids into one schedule. This is used in the web application for now.

This model is never cached, so the `cachedAt` field does not serve any importance. Caching is disabled because these schedules can be any strange combination of individually cached schedules. Functionality may be extended to make use caching in the future.
### `fas:List` Fields

`ids` : [[string]]
Ids of the schedules as it appears in KronoX's system.

`cachedAt` : [[iso-date-string]]
When this schedule was cached in our mongoDB.

`days` : [[array]]<[[Day]]>
The days included in the schedule with the events nested inside them.

### `far:QuestionCircle` Example
```json
{
  "cachedAt": "string",
  "ids": [
    "string"
  ],
  "days": [
    {
      "name": "string",
      "date": "string",
      "isoString": "2019-08-24T14:15:22Z",
      "weekNumber": 0,
      "events": [
        {
          "id": "497f6eca-6276-4993-bfeb-53cbbbba6f08",
          "title": "string",
          "course": {
            "id": "string",
            "swedishName": "string",
            "englishName": "string"
          },
          "timeStart": "2019-08-24T14:15:22Z",
          "timeEnd": "2019-08-24T14:15:22Z",
          "locations": [
            {
              "id": "string",
              "name": "string",
              "building": "string",
              "floor": "string",
              "maxSeats": "string"
            }
          ],
          "teachers": [
            {
              "id": "string",
              "firstName": "string",
              "lastName": "string"
            }
          ],
          "isSpecial": true,
          "lastModified": "2019-08-24T14:15:22Z"
        }
      ]
    }
  ]
}
```