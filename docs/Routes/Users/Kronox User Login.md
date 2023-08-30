#route #user #authorization

---
### `rir:Information` Description
Take a user's KronoX credentials and receive a [[KronoxUser]] in response.

<mark style="background: #FF5582A6;">POST</mark> `api/users/login`
### `rir:ArrowLeftDown` Request

#### Query Parameters

![[School Id Parameter|no-title clean]]
#### Body

`username` : [[string]]

`password` : [[string]]

**Example**
```json
{
    "username": "email@here.com",
    "password": "epicsafety"
}
```

### `rir:ArrowRightUp` Response

> [!check]- 200 OK
> ![[KronoxUser#`fas List` Fields|clean]]

> [!fail]- 401 Unauthorized
![[Error#`fas List` Fields|clean]]

> [!fail]- 404 Not Found
![[Error#`fas List` Fields|clean]]

> [!fail]- 500 Internal Server Error
![[Error#`fas List` Fields|clean]]
