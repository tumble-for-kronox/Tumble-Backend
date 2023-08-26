#route #booking 

---
### `rir:Information` Description
Book a time slot at a given date, for a given resource.

<mark style="background: #FFB86CA6;">PUT</mark> `api/resources/book`
### `rir:ArrowLeftDown` Request

#### Headers

![[x-auth-token#Required|no-h no-title clean]]

#### Query Parameters

![[School Id Parameter|no-title clean]]

#### Body

`resourceId` : [[string]]
Id of the resource.

`date` : [[iso-date-string]]
The date on which to make the booking.

`availabilitySlot` : [[AvailabilitySlot]]
The availability slot that should be booked. This will also change the state of this (when re-fetched) to booked.

**Example**
```json
{
  "resourceId": "string",
  "date": "2019-08-24T14:15:22Z",
  "availabilitySlot": {
    "availability": 0,
    "locationId": "string",
    "resourceType": "string",
    "timeSlotId": "string",
    "bookedBy": "string"
  }
}
```

### `rir:ArrowRightUp` Response

> [!check]- 200 OK
> *No body*

> [!fail]- 400 Bad Request
![[Error#`fas List` Fields|clean]]

> [!fail]- 401 Unauthorized
![[Error#`fas List` Fields|clean]]

> [!fail]- 403 Forbidden
![[Error#`fas List` Fields|clean]]

> [!fail]- 409 Conflict
![[Error#`fas List` Fields|clean]]

> [!fail]- 500 Internal Server Error
![[Error#`fas List` Fields|clean]]
