#route

---
### `rir:Information` Description
Get all the bookings a user currently has.

<mark style="background: #ADCCFFA6;">GET</mark> `api/resources/userbookings`
### `rir:ArrowLeftDown` Request

#### Headers

![[x-auth-token#Required|no-h no-title clean]]

#### Query Parameters

![[School Id Parameter|no-title clean]]

### `rir:ArrowRightUp` Response

> [!check]- 200 OK
> **[[Array]] of**
> ![[Booking#`fas List` Fields|clean]]

> [!fail]- 400 Bad Request
![[Error#`fas List` Fields|clean]]

> [!fail]- 401 Unauthorized
![[Error#`fas List` Fields|clean]]

> [!fail]- 500 Internal Server Error
![[Error#`fas List` Fields|clean]]
