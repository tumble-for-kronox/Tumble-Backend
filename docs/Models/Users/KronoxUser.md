#model #user

---
### `rir:Information` Description
Model for a user that has logged into KronoX. The sessionToken may be passed back to the API when accessing user specific information and the name/username are just data for displaying.
### `fas:List` Fields

`name` : [[string]]
Full name of the user.

`username` : [[string]]
KronoX username. Same as used by the school and may be something like "LAPO0003"

> [!warning] Deprecating
> `sessionToken` : [[string]]
> The login session of the user. This is being deprecated, but is still supported temporarily because the Android app makes use of it.

`refreshToken` : [[string]]
The user's refresh token

### `far:QuestionCircle` Example
```json
{
  "name": "string",
  "username": "string",
  "sessionToken": "string",
  "refreshToken": "string"
}
```