#route #user #events 

---
### `rir:Information` Description
Attempt to register a user for all the currently available, unregistered events the user has. Returns a list of successfully 

<mark style="background: #FFB86CA6;">PUT</mark> `api/users/events/register/all`
### `rir:ArrowLeftDown` Request

#### Headers

![[x-auth-token#Required|no-h no-title clean]]

#### Query Parameters

![[School Id Parameter|no-title clean]]

### `rir:ArrowRightUp` Response

> [!check]- 200 OK
> ![[MultiRegistrationResult#`fas List` Fields|clean]]

> [!fail]- 401 Unauthorized
![[Error#`fas List` Fields|clean]]

> [!fail]- 500 Internal Server Error
![[Error#`fas List` Fields|clean]]
