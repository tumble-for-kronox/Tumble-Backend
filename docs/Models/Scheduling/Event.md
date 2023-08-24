#model #scheduling 

---
### `rir:Information` Description
Model containing information regarding one scheduled event.
### `fas:List` Fields

`id` : [[uuid-string]]
Id of event coming from the KronoX system.

`title` : [[string]]

`course` : [[Course]]

`timeStart` : [[iso-date-string]]

`timeEnd` : [[iso-date-string]]

`locations` : [[array]]<[[Location]]>

`teachers` : [[array]]<[[Location]]>

`isSpecial` : [[boolean]]
An event can be marked special in KronoX system for various reasons, but is the most likely indicator of something being an exam or not. As such: `special == exam` is the assumption used in this system.

`lastModified` : [[iso-date-string]]
Last time of modification.

### `far:QuestionCircle` Example
```json
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
```
