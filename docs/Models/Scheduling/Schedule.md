#model #scheduling 

---
### `rir:Information` Description
Model of a given schedule with details about when it was last cached, what its id is, and a list of days containing the events in the schedule.
### `fas:List` Fields

`id` : [[string]]
Id of the schedule as it appears in KronoX's system.

`cachedAt` : [[iso-date-string]]
When this schedule was cached in our mongoDB.

`days` : [[array]]<[[Day]]>
The days included in the schedule with the events nested inside them.

### `far:QuestionCircle` Example
```json
{
  "cachedAt": "string",
  "id": "string",
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