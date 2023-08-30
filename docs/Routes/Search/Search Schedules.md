#route #search

---
### `rir:Information` Description
Search a school's KronoX system for a specific keyword. This makes use of KronoX's search function, which is a little limited so these limitations unfortunately follow into this system.

<mark style="background: #ADCCFFA6;">GET</mark> `api/schedules/search`
### `rir:ArrowLeftDown` Request

#### Headers

![[x-auth-token#Optional|no-h no-title clean]]

#### Query Parameters

![[School Id Parameter#^a93917|clean]]

`search` : [[string]] -- *required*
The string to search for on KronoX.

![[Session Token Parameter|no-title clean]]

### `rir:ArrowRightUp` Response

> [!check]- 200 OK
> ![[Search Result#`fas List` Fields|clean]]

> [!fail]- 400 Bad Request
![[Error#`fas List` Fields|clean]]
