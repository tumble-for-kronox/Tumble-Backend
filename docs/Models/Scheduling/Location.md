#model #scheduling

---
### `rir:Information` Description
Model containing information about a KronoX location, such as a classroom or lecture hall.
### `fas:List` Fields

`id` : [[string]]

`name` : [[string]]

`building` : [[string]]
Name of the building the location is inside.

`floor` : [[string]]
Floor of the building on which the location is.

`maxSeats` : [[string]]
The total number of seats available in the location.

### `far:QuestionCircle` Example
```json
{
    "id": "21-324",
    "name": "Orkanen",
    "floor": "2",
    "maxSeats": "34"
}
```