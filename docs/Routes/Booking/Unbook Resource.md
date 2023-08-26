#route #booking 

---
### `rir:Information` Description
Unbook an already booked time slot, for a given resource id.
`
<mark style="background: #FFB86CA6;">PUT</mark> `api/resources/unbook`
### `rir:ArrowLeftDown` Request

#### Headers

![[x-auth-token#Required|no-h no-title clean]]

#### Query Parameters

![[School Id Parameter|no-title clean]]

`bookingId` : [[string]] -- *required*
Id of the booking (as found in [[Booking#`fas List` Fields]]) that should be unbooked.

### `rir:ArrowRightUp` Response

> [!check]- 200 OK
> *No body*

> [!fail]- 400 Bad Request
![[Error#`fas List` Fields|clean]]

> [!fail]- 401 Unauthorized
![[Error#`fas List` Fields|clean]]

> [!fail]- 404 Not Found
![[Error#`fas List` Fields|clean]]

> [!fail]- 500 Internal Server Error
![[Error#`fas List` Fields|clean]]
