# CircuitBrakerPattern
Experimenting with circuit breaker pattern. 

## Why 
I was interested in implementing **distractless and quick fallback** with the following desired capabilities : 

* When external service is not available, in case of HTTP request or other, the request must hit the database and return the result.
* When one code path is not available go the alternative service.*
* If an exception is thrown by the first service I do not want to handle it explicitly.
* If no alternative is provided - return silently.
* When an exception occures it must be logged internally by the switch.
* If the alternative service throws exception - the calling code must handle it.

It became not a true circuit breaker so I decided to call it circuit switch.
