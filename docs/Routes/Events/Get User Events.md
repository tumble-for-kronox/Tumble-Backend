#route #user #events 

---
### `rir:Information` Description
Get all events from a specified user.

<mark style="background: #ADCCFFA6;">GET</mark> `api/users/events`
### `rir:ArrowLeftDown` Request

#### Headers

![[x-auth-token#Required|no-h no-title clean]]

#### Query Parameters

![[School Id Parameter|no-title clean]]

### `rir:ArrowRightUp` Response

> [!check]- 200 OK
> ![[UserEventCollection#`fas List` Fields|clean]]

> [!fail]- 401 Unauthorized
![[Error#`fas List` Fields|clean]]

> [!fail]- 404 Not Found
![[Error#`fas List` Fields|clean]]

> [!fail]- 500 Internal Server Error
![[Error#`fas List` Fields|clean]]
