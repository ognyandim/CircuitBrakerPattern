# Circuit Braker augmented to Circuit Switch
Experimenting with circuit breaker pattern. 

## Why 
Because I was interested in implementing **distractless and quick fallback** with the following desired capabilities : 

* When one service is not available go the provided alternative. 
* If there is failure by first or second service I do not want to handle it explicitly - it should be retried internally by the Circuit Switch.
* If the problem is not resolved by retring - return null instead of exception. Then the calling call is in charge. Just fail silently. 
* When an exception occures it must be logged by the switch through logging service. Not exception should be thrown over the hedge.
* I do not want my code to be littered with try/catches - it becomes really hairy when I use it in 3-4 methods in a single method. That is why I prefer to return null - I know already that this situation can not be handled by retries by the Circuit Switch. Neither by different services nor by retrying.
* I want it to be testable and not an obstacle to the testability of consuming classes.
* I want it to be thread safe because I plan to use it in classes/service which have singleton lifetime.
* I want to stack multiple services as alternative paths. 
* I want it to be async

## Example case and desired behavior
1. Get the stock quotes from Yahoo if it fails - retry X times with Y seconds configurable back-off
3. otherwise from Google with retry and back-off
4. otherwise from cache (Redis?, Apache.Ignite?) with retry and back-off , 
5. otherwise return null.

## At the end
it became not a true circuit breaker so I decided to call it a Circuit Switch. If you find it interesting I will be glad to discuss it.

## Resources
I found some useful articles and blog posts on my way of studying about Retry and Circuit Breaker patterns.

1. [Transient Fault Handling (Building Real-World Cloud Apps with Azure)](https://www.asp.net/aspnet/overview/developing-apps-with-windows-azure/building-real-world-cloud-apps-with-windows-azure/transient-fault-handling)
2. [Retry pattern](https://msdn.microsoft.com/en-us/library/dn589788.aspx)
3. [Circuit Breaker pattern](https://msdn.microsoft.com/en-us/library/dn589784.aspx)
4. [P&P Transient Fault Handling](https://www.asp.net/aspnet/overview/developing-apps-with-windows-azure/building-real-world-cloud-apps-with-windows-azure/transient-fault-handling)
5. [The Transient Fault Handling Application Block](https://msdn.microsoft.com/library/hh680934(v=pandp.50).aspx)
6. [Martin Fowler`s article on Circuit Breaker](https://martinfowler.com/bliki/CircuitBreaker.html)
7. [Tim Ross - Implementing The Circuit Breaker Pattern In C#](https://timross.wordpress.com/2008/02/10/implementing-the-circuit-breaker-pattern-in-c/)


