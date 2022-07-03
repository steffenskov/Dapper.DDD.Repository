# WeatherService sample Simplified

This project is a sample of how to use the Dapper.Repository library.

It has been kept extremely minimal, with all domain types and repositories defined in the `WeatherService.Model` project. 
Furthermore the `WeatherService.Api` project directly calls the repositories AND maintains business rules for validation.
This is very much an anti-pattern and should IMHO not be done for any project that is more than a simple Proof-of-Concept. 

However I wanted the focus in this sample to be on the Dapper.Repository library, and to that end I cut quite a few corners.

If you're familiar with [Domain-Driven Design](https://en.wikipedia.org/wiki/Domain-driven_design), [CQRS](https://en.wikipedia.org/wiki/Command%E2%80%93query_separation) and [Onion architecture](https://en.everybodywiki.com/Onion_Architecture) I'd strongly recommend looking at the full-fledged sample project here instead: [WeatherService](https://github.com/steffenskov/Dapper.Repository/tree/main/samples/WeatherService)