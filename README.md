# Circuit Braker augmented to Circuit Switch
Experimenting with circuit breaker pattern. 

## Why 
Because I was interested in implementing **distractless and quick fallback** with the following desired capabilities : 

* When external service is not available, in case of HTTP request or other, the request must hit the database and return the result.
* When one code path is not available go the alternative service.*
* If an exception is thrown by the first service I do not want to handle it explicitly.
* If no alternative is provided - return silently.
* When an exception occures it must be logged internally by the switch.
* If the alternative service throws exception - the calling code must handle it.
* I do not want my code to be littered with try/catches - it becomes really hairy when I use it in 3-4 methods in a single class.
* I want it to be testable and not an obstacle to the testability of consuming classes.
* I want it to be thread safe because I plan to use it in classes/service which have singleton lifetime.

## At the end
It became not a true circuit breaker so I decided to call it a circuit switch. 

## If you
find it interesting I will be glad to discuss it.
