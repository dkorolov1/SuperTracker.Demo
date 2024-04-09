

# SuperTracker App 🚀
The project was built as a test solution for further practical exploration of the capabilities of MassTransit in combination with RabbitMQ, as well as .NET8 Minimal API and FluentAssertions with xUnit.

The solution consists of two services **SuperTracker.PixelApi** and **SuperTracker.Storage** communicating with each other using [MassTransit](https://masstransit.io/) and [RabbitMQ Transport](https://masstransit.io/documentation/transports/rabbitmq) underneath.

## SuperTracker.PixelApi
.NET 8 Minimal API with just one endpoint, `GET /track`, which captures certain information from the request (`Referrer ` & `User-Agent` headers + `Visitor IP`), publishes the corresponding notification in MassTransit containing this information, and returns a 'dummy' 1-pixel GIF image as a response.

## SuperTracker.Storage
.NET 8 service responsible for consuming and processing notifications from the **SuperTracker.PixelApi** service, ensuring the thread-safe storage of the received information in a log file.

## Run It 🏃

 1. Make sure you have RabbitMQ up and running.
 2. Replace the corresponding RabbitMQ settings in `src/SuperTracker.PixelApi/appsettings.json` and `src/SuperTracker.Storage/appsettings.json`.
 3. Run the Api service: `dotnet run --project "src/SuperTracker.PixelApi"`
 4. Run the Storage service: `dotnet run --project "src/SuperTracker.Storage"`
 5. Open `requests/PixelTrack/PixelTrack.http` and send requests to `GET /track` directly from VS Code ([REST Client Extension](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) is needed).

![enter image description here](https://i.postimg.cc/P5d2DvtG/run-it.gif)

...Or with just a single command using [docker-compose](https://docs.docker.com/compose/): `docker-compose up`

![enter image description here](https://i.postimg.cc/Vs7xTMfB/2024-04-09-12-28-58.gif)

## Test It 📝

Both services are covered by unit tests using xUnit. Other types of tests were omitted for simplicity. To run the tests, execute the following commands:


 - `dotnet test "tests/SuperTracker.PixelApi.UnitTests"`
 - `dotnet test "tests/SuperTracker.Storage.UnitTests"`

 ![enter image description here](https://i.postimg.cc/PJXNkrnH/test-it.gif)
 
## Docker It 🏭

Both services contain Dockerfiles with build and runtime split into two stages. To build Docker images, execute the following commands:

 - `docker build -t supertracker.storage -f "src/SuperTracker.Storage/Dockerfile" .`
 - `docker build -t supertracker.pixelapi -f "src/SuperTracker.PixelApi/Dockerfile" .`

 ![enter image description here](https://i.postimg.cc/nrRQ4vZs/run-it-in-docker.gif)
