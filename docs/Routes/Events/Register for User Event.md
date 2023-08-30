#route #user #events 

---
### `rir:Information` Description
Register a user on a currently available event.

<mark style="background: #FFB86CA6;">PUT</mark> `api/users/events/register/{eventId}`
### `rir:ArrowLeftDown` Request

#### Headers

![[x-auth-token#Required|no-h no-title clean]]

#### Path Parameters

`eventId` : [[string]] -- *required*
The id of the event to register for.

#### Query Parameters

![[School Id Parameter|no-title clean]]

### `rir:ArrowRightUp` Response

> [!check]- 200 OK
> *No body*

> [!fail]- 401 Unauthorized
![[Error#`fas List` Fields|clean]]

> [!fail]- 404 Not Found
![[Error#`fas List` Fields|clean]]

> [!fail]- 500 Internal Server Error
![[Error#`fas List` Fields|clean]]
