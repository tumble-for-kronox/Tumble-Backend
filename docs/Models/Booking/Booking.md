#model #booking 

---
### `rir:Information` Description
General description of the model
### `fas:List` Fields

`id` : [[string]]

`timeSlot` : [[TimeSlot]]
The timeslot the booking is in. This timeslot will always have its `id` field set to [[null]]. This is because the timeslot id is out of context when a booking has already been made and is only relevant when making a new booking.

`locationId` : [[string]]
The id of the location on which this booking is made.

### `far:QuestionCircle` Example
```json
{
  "id": null,
  "timeSlot": {
    "id": 0,
    "from": "2019-08-24T14:15:22Z",
    "to": "2019-08-24T14:15:22Z",
    "duration": "string"
  },
  "locationId": "string"
}
```