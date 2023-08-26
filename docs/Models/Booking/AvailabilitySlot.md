#model #booking

---
### `rir:Information` Description
Model describing the availability of a certain resource within a specific timeslot.
### `fas:List` Fields

`availability` : [[availability]]
The availability of the slot.

`locationId` : [[string]] | [[null]]
The id of the location this availability slot is for.

`resourceType` : [[string]] | [[null]]
The type of the resource. This is just a descriptive string and doesn't have any meaning outside of showing it to users.

`timeSlotId` : [[string]] | [[null]]
The id of the timeslot this availability slot is related to. The timeslot ids are needed for booking.

`bookedBy` : [[string]] | [[null]]
The name or username of the user who has already booked this slot. Is only non-null if the `availability` field is `2` (Booked)

### `far:QuestionCircle` Example
```json
{
  "availability": 0,
  "locationId": "string",
  "resourceType": "string",
  "timeSlotId": "string",
  "bookedBy": "string"
}
```
