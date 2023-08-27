#route #scheduling 

---
### `rir:Information` Description
Fetch a set number of events from a [[MultiSchedule]].

<mark style="background: #ADCCFFA6;">GET</mark> `api/schedules/nevents`
### `rir:ArrowLeftDown` Request

#### Query Parameters

`n_events` : [[integer]]
The number of events to return.

`startDate` : [[iso-date-string]]
Date from which the fetched schedule should start. This is optional and will use the current date if no value is passed.

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
> ### `fas:List` Fields
>  
> `fieldName` : [[string]]
> This is a description of the field above. It's quite long and very extensive.
> 
> `fieldName` : [[boolean]]
> This is a description of the field above. It's quite long and very extensive.

> [!fail]- 400 Bad Request
![[Error#`fas List` Fields|clean]]

> [!fail]- 404 Not Found
![[Error#`fas List` Fields|clean]]
