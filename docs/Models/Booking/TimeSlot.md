#model #booking 

---
### `rir:Information` Description
Model describing the different time slots that a given resource has connected to it.
### `fas:List` Fields

`id` : [[string]] | [[null]]
The id of the timeslot, as it is in KronoX's system.

`from` : [[iso-date-string]]
The starting date-time of the timeslot.

`to` : [[iso-date-string]]
The ending date-time of the timeslot.

`duration` : [[duration-string]]
The time duration this timeslot ranges over. This is the time between the `from` and `to` date-times.

### `far:QuestionCircle` Example
```json
{
  "id": 0,
  "from": "2019-08-24T14:15:22Z",
  "to": "2019-08-24T14:15:22Z",
  "duration": "1.01:33:00.5000000"
}
```