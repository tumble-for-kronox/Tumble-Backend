#route #feedback

---
### `rir:Information` Description
Submit an issue to be sent to the support email address. This is meant for user submitted feedback and issues.

<mark style="background: #FF5582A6;">POST</mark> `api/misc/submitIssue`
### `rir:ArrowLeftDown` Request

#### Body

`title` : [[string]]
Title of the user's feedback.

`description` : [[string]]
Main body of the user's feedback.

**Example**
```json
{
    "title": "Can't open schedule",
    "description": "The schedule with id ... can't be opened."
}
```

### `rir:ArrowRightUp` Response

> [!check]- 200 OK
> *No body*

> [!fail]- 500 Internal Server Error
![[Error#`fas List` Fields|clean]]
