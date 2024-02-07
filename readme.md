# Introduction
Dear reviewer,
in this folder you will find a Visual Studio solution implementing problem described in "BE-DEVELOPER-PANGEA.pdf". 

# Build
Build the solution using Visual Studio. Use VS2022 or later.

# Test
- There are unit tests covering the core classes.
- To verify the API functionality there are three basic ways:

### Swagger
Swagger UI is exposed under http://localhost:5176/swagger/index.html (it should open also if you run the WebApi project with the only execution profile).

### .http file
Visual Studio supports executing HTTP requests from the included **RFETest.WebApi.http** file. Similar to the Swagger option, you have to launch the WebApi project or host it somewhere.

### Integration test
The **RFETest.WebApi.IntegrationTests** project includes test scenarios that call the API. The application is hosted in the test process, so there is no need to launch anything else.

# Limitations of the provided solution
- Database is only implemented as in-memory, so data is not persisted anywhere when the application is restarted. I hope this is fine - I considered using an out-of-process database to be out of scope of this project.
- The requirement to accept a base64 encoded string makes manual testing cumbersome. Accepting plain json alongside the base64 encoded version might simplify things. It was not clear to me whether the app should accept only **application/custom** or if it can also accept plain json. I believe the latter is better and the application behaves accordingly.
- Depending on usage patterns it might be better to store the resulting diff early (i.e. when both values are available), and then reuse the value instead of recalculating it every time the **http get diff** endpoint is called. But this could also add extra complexity which has to be justified - so I have not implemented that.