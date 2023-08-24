#route

---
### `rir:Information` Description
General description of the route

<mark style="background: #BBFABBA6;">GET</mark> $\qquad$ `api/something/more`
<mark style="background: #FFB86CA6;">PUT</mark> $\qquad$ `api/something/more`
<mark style="background: #FF5582A6;">POST</mark> $\qquad$ `api/something/more`
### `rir:ArrowLeftDown` Request

#### Headers

`fieldName` -- *required*
This is a description of the field above. It's quite long and very extensive.

#### Query Parameters

`fieldName` : [[string]] -- *required*
This is a description of the field above. It's quite long and very extensive.

`fieldName` : [[integer]] -- *required*
This is a description of the field above. It's quite long and very extensive.

#### Body

`fieldName` : [[string]]
This is a description of the field above. It's quite long and very extensive.

`fieldName` : [[boolean]]
This is a description of the field above. It's quite long and very extensive.

**Example**
```json
{
    "key1": 0,
    "key2": "value1",
    "key3": {objectRef},
    "key4": [list1, list2, list3]
}
```
[[objectRef]]

### `rir:ArrowRightUp` Response

> [!check] 200 OK
#### Body

`fieldName` : [[string]]
This is a description of the field above. It's quite long and very extensive.

`fieldName` : [[boolean]]
This is a description of the field above. It's quite long and very extensive.

**Example**
```json
{
    "key1": 0,
    "key2": "value1",
    "key3": {objectRef},
    "key4": [list1, list2, list3]
}
```
[[objectRef]]

> [!fail] 400 Bad Request

![[Error]]