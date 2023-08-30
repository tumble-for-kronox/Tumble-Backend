#route #booking 

---
### `rir:Information` Description
Fetch the full dataset for a given resource. No attributes in the returned Resource object should be [[null]].

<mark style="background: #ADCCFFA6;">GET</mark> `api/resources/{resourceId}`
### `rir:ArrowLeftDown` Request

#### Headers

![[x-auth-token#Required|no-h no-title clean]]

#### Path Parameters

`resourceId` : [[string]] -- *required*
The ID of the resource to fetch data for.

#### Query Parameters

![[School Id Parameter|no-title clean]]

`date` : [[iso-date-string]]
The date to get resource data from. If nothing is passed the current date is used as a default value.

### `rir:ArrowRightUp` Response

> [!check]- 200 OK
> ![[Resource#`fas List` Fields|clean]]

> [!fail]- 400 Bad Request
![[Error#`fas List` Fields|clean]]

> [!fail]- 401 Unauthorized
![[Error#`fas List` Fields|clean]]

> [!fail]- 404 Not Found
![[Error#`fas List` Fields|clean]]

> [!fail]- 500 Internal Server Error
![[Error#`fas List` Fields|clean]]
