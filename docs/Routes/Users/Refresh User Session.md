#route #user #authorization 

---
> [!warning] Deprecating
> This route is used in the flutter app, but should not be used further. This is replaced with the [[x-auth-token]] header on all routes.
### `rir:Information` Description
Returns a fresh session token.

<mark style="background: #ADCCFFA6;">GET</mark> `api/users/refresh`

### `rir:ArrowLeftDown` Request

#### Headers

`authorization` -- *required*
User's refresh token.

#### Query Parameters

![[School Id Parameter#^a93917|clean ]]

### `rir:ArrowRightUp` Response

> [!check]- 200 OK
> ![[KronoxUser#`fas List` Fields|clean]]

> [!fail]- 400 Bad Request
![[Error#`fas List` Fields|clean]]

> [!fail]- 401 Unauthorized
![[Error#`fas List` Fields|clean]]

> [!fail]- 500 Internal Server Error
![[Error#`fas List` Fields|clean]]
