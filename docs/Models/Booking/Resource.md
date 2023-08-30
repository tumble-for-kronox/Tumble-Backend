#model #booking 

---
### `rir:Information` Description
Model of a school's resource that is bookable. This comes with information about the resource such as name and id, but also with the timeslots and availabilities for booking.
### `fas:List` Fields

`id` : [[string]]

`name` : [[string]]

`timeSlots` : [[array]]<[[TimeSlot]]> | [[null]]
The timeslots available within this resource.

`locationIds` : [[array]]<[[string]]> | [[null]]
The ids of the locations available within this resource.

`date` : [[string]]
The date the data applies to. The availabilities are specifically fetched for this date.

`availabilities` : [[dict]]<[[string]], [[dict]]<[[string]], [[AvailabilitySlot]]>>
The outer dict's keys are the locationIds found in the `locationIds` field. The inner dict's keys are the ids of the time slots found in the `timeSlots` field.

### `far:QuestionCircle` Example
```json
{
  "id": "string",
  "name": "string",
  "timeSlots": [
    {
      "id": 0,
      "from": "2019-08-24T14:15:22Z",
      "to": "2019-08-24T14:15:22Z",
      "duration": "string"
    }
  ],
  "locationIds": [
    "string"
  ],
  "date": "2019-08-24T14:15:22Z",
  "availabilities": {
    "locationId1": {
      "timeSlotId1": {
        "availability": 0,
        "locationId": "string",
        "resourceType": "string",
        "timeSlotId": "string",
        "bookedBy": "string"
      },
      ...,
      "timeSlotIdN": {
        "availability": 0,
        "locationId": "string",
        "resourceType": "string",
        "timeSlotId": "string",
        "bookedBy": "string"
      }
    },
    ...,
    "locationIdN": {
      "timeSlotId": {
        "availability": 0,
        "locationId": "string",
        "resourceType": "string",
        "timeSlotId": "string",
        "bookedBy": "string"
      }
    }
  }
}
```
[[objectRef]]
