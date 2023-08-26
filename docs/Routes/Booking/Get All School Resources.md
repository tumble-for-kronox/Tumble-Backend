#route #booking 

---
### `rir:Information` Description
endpoint for fetching the list of different resources available from a given school. Returns a list of Resource objects with their names and IDs. The IDs can then be used to fetch full data as needed.

<mark style="background: #ADCCFFA6;">GET</mark> `api/resources`
### `rir:ArrowLeftDown` Request

#### Headers

![[x-auth-token#Required|no-h no-title clean]]

#### Query Parameters

![[School Id Parameter|no-title clean]]

### `rir:ArrowRightUp` Response

> [!check]- 200 OK
> **[[Array]] of**
> ![[Resource#`fas List` Fields|clean]]

> [!fail]- 400 Bad Request
![[Error#`fas List` Fields|clean]]

> [!fail]- 401 Unauthorized
![[Error#`fas List` Fields|clean]]

> [!fail]- 500 Internal Server Error
![[Error#`fas List` Fields|clean]]
