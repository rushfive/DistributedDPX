### Advantages

- each MS is smaller, with a well defined scope (bounded-context). Easier to understand means easier to onboard new developers and get them started working in it.
- Containers startup fast (good for devs). Using docker and orchestration in VS, it's possible to DEBUG more than one container at a time (w/ breakpoints working and all)
- Each MS can be developed independently. It's possible we could go through an entire release cycle w/o having to update a particular one.
- No more monolithic deployments - if the codebase hasn't changed for a MS, it doesn't need to be re-built and re-deployed.
- Testing becomes easier due to the smaller and well-defined scope. This can also lead to better build and deploy times, as we wouldn't have to re-run expensive e2e tests if the MS hasn't changed.
- Better scalability due to the systems being more granular. Horizontally scale up a single MS as needed, while others remain the same.
- Better fault tolerance (errors). It's possible that the system as a whole remains mostly operational if a MS dies, but in a monolithic structure it's an "all or nothing" scenario.
- Choose the right tools for the right MS - if a NoSQL db makes more sense for the Survey Collection MS, while a SQL one fits the identity MS better, we can easily do it.

### Downsides

- more complexity for development. Devs need to start taking into account more isolated services, more databases/schemas, etc. 
- Development time for features increases. More planning is required during the design process as we take into account more moving pieces. There will also be slightly more "boilerplate" type work (creating models, plumbing work for simple CRUD ops, etc.)
- Increased complexity for deployments and infrastructure management (IT ops). It's inherently more difficult to manage tens of distinct apps/services versus a few monolithic apps. There's also an increase in number of DBs to manage - each MS holds it's own local datastore (not a hard and fast rule but we should generally try to follow it when possible)
- Must embrace **Eventual Consistency**. Atomic transactions become a thing of the past to some degree. It becomes more difficult to serve the response *AFTER* all necessary actions have completed, due to the entire backend operating in a mostly asynchronous manner now.
  - There are some ways to work around the absense of strong consistency. These would be crucial to implement for select scenarios like patients registering themselves. Some options:
    - Transactional Outbox pattern (https://microservices.io/patterns/data/transactional-outbox.html)
    - Event Sourcing pattern (https://docs.microsoft.com/en-us/azure/architecture/patterns/event-sourcing)
    - Another idea: the web servers could wait around for a "completed" event (pub/sub or websockets could work for this) before service the HTTP response.