A string containing a duration formatted as a string.

**Example**
```
1.01:33:00.000500
[d'.']hh':'mm':'ss['.'fffffff]
```

`d.` will only appear if the duration is longer than a day, it will then be the number of days.
`hh` is the hours.
`mm` is the minutes.
`ss` is the seconds.
`.fffffff` will only appear if milliseconds is relevant.