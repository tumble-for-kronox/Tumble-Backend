#route #booking 

---
### `rir:Information` Description
When a booking comes up the user must confirm that they still wish to keep the booking. This is done here.

<mark style="background: #FFB86CA6;">PUT</mark> `api/resources/confirm`
### `rir:ArrowLeftDown` Request

#### Headers

![[x-auth-token#Required|no-h no-title clean]]

#### Query Parameters

![[School Id Parameter|no-title clean]]

#### Body

`resourceId` : [[string]]
This is a description of the field above. It's quite long and very extensive.

`bookingId` : [[string]]
This is a description of the field above. It's quite long and very extensive.

**Example**
```json
{
  "resourceId": "string",
  "bookingId": "string"
}
```

### `rir:ArrowRightUp` Response

> [!check]- 200 OK
> *No body*

> [!fail]- 401 Unauthorized
![[Error#`fas List` Fields|clean]]

> [!fail]- 404 Not Found
![[Error#`fas List` Fields|clean]]

> [!fail]- 500 Internal Server Error
![[Error#`fas List` Fields|clean]]
