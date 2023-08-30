#route #scheduling 

---
### `rir:Information` Description
Get a specific schedule from a specific school. Also supports specifying a starting date to only fetch data from that date onwards.

<mark style="background: #ADCCFFA6;">GET</mark> `api/schedule/{scheduleId}`
### `rir:ArrowLeftDown` Request

#### Headers

![[x-auth-token#Optional|no-h no-title clean]]

#### Path Parameters

`scheduleId` : [[string]] -- *required*
The id of the schedule to fetch.

#### Query Parameters

![[School Id Parameter#^a93917|clean]]

`startDate` : [[date-string]]
Date from which the fetched schedule should start. This is optional and will use the first date of the current week if no value is specified.

![[Session Token Parameter|no-title clean]]

### `rir:ArrowRightUp` Response

> [!check]- 200 OK
![[Schedule#`fas List` Fields|clean]]

> [!fail]- 400 Bad Request
![[Error#`fas List` Fields|clean]]

> [!fail]- 403 Forbidden
![[Error#`fas List` Fields|clean]]

> [!fail]- 403 Not Found
![[Error#`fas List` Fields|clean]]

> [!fail]- 500 Internal Server Error
![[Error#`fas List` Fields|clean]]
