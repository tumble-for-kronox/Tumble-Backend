#model #scheduling 

---
### `rir:Information` Description
Model wrapping events within a given day. The model carries calendar information regarding the day along with some ease-of-use strings.
### `fas:List` Fields

`name` : [[weekday-string]]
The name of the weekday.

`date` : [[string]]
A pre-formatted date and month string of the date: `"30/05"` (May 30th).

`isoString` : [[iso-date-string]]

`weekNumber` : [[integer]]
Week number of the week the day is in.

`events` : [[array]]<[[Event]]>
List of events occurring on the day.

### `far:QuestionCircle` Example
```json
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
```
