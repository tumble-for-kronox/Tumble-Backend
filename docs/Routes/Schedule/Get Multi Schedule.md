#route #scheduling 

---
### `rir:Information` Description
Fetch a schedule from a mix of various schedule from various schools, all combined into one schedule.

<mark style="background: #ADCCFFA6;">GET</mark> `api/schedules/multi`
### `rir:ArrowLeftDown` Request

#### Query Parameters

`startDate` : [[iso-date-string]]
Date from which the fetched schedule should start. This is optional and will use the current date if no value is specified.

#### Body

**[[Array]] of**

`schoolId` : [[school-id]]
The id of the school.

`scheduleIds` : [[array]]<[[string]]>
The ids of the schedules to fetch from the given school.

**Example**
```json
[
    {
        "schoolId": "0",
        "scheduleIds": [
            "id1",
            ...,
            "idN"
        ]
    },
    ...,
    {
        "schoolId": "1",
        "scheduleIds": [
            "id1",
            ...,
            "idN"
        ]
    }
]
```

### `rir:ArrowRightUp` Response

> [!check]- 200 OK
> ![[MultiSchedule#`fas List` Fields|no-title clean]]

> [!fail]- 400 Bad Request
![[Error#`fas List` Fields|clean]]

> [!fail]- 404 Not Found
![[Error#`fas List` Fields|clean]]

> [!fail]- 500 Internal Server Error
![[Error#`fas List` Fields|clean]]
