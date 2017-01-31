# Circuit Braker augmented to Circuit Switch
Experimenting with circuit breaker pattern. 

## Why 
Because I was interested in implementing **distractless and quick fallback** with the following desired capabilities : 

* When one code path is not available go the provided alternative code path. 
E.g. when cache is down - hit the database.
* If an exception is thrown by the first service I do not want to handle it explicitly - it should be retried internally by the Circuit Switch.
* If the first code path is broken and no alternative code path is provided - return null instead of exception. Then the calling call is in charge.
* When an exception occures it must be logged by the switch through logging service.
* If the alternative service throws exception - null is returned. Then the calling call is in charge.
* I do not want my code to be littered with try/catches - it becomes really hairy when I use it in 3-4 methods in a single class. That is why I prefer to return null - I know already that this situation can not be handled by retries by the Circuit Switch - neither by different code path nor by retrying.
* I want it to be testable and not an obstacle to the testability of consuming classes.
* I want it to be thread safe because I plan to use it in classes/service which have singleton lifetime.

## At the end
it became not a true circuit breaker so I decided to call it a Circuit Switch. 

## If you
find it interesting I will be glad to discuss it.
