# Just.Grpc.Host

Just.Grpc.Host is meant to be a simple and straightforward way to host gRPC service applications written in .net, without drilling too deeply inside the gRPC stack as implemented in Asp.net core.

The following examples assume that you have a `CalculatorService` class implementing the gRPC contract, as defined in the `calculator.proto` file.

To host the service using default settings, do

```
using Just.Grpc;
Host.Run<CalculatorService>();
```

This will start the service on port 8080.

For the C# client, it is recommended to use the [Grpc.Net.Client package](https://www.nuget.org/packages/Grpc.Net.Client)
with the code like this:

```
using Grpc.Net.Client;

var channel = GrpcChannel.ForAddress("http://localhost:8080");
var client = new Calculator.CalculatorClient(channel);
// ... call the service
```

Because of the gRPC cross-language compatibility, it is possible to call the service from the other languages. Refer to the gRPC documentation for your language.

To change the port the service listens on, use

```
using Just.Grpc;
Host.Run<CalculatorService>(opts => {
    opts.Port = 7070;
});
```

To disable the console logging,

```
using Just.Grpc;
Host.Run<CalculatorService>(opts => {
    opts.Silent = true;
});
```

To be able to cancel the running service,

```
using System.Threading;
AncellationTokenSource tokenSource = new();
tokenSource.CancelAfter(5000); // will stop the service after 5 seconds
Host.Run<CalculatorService>(opts => {
    opts.Port = 9090;
}, tokenSource.Token);
```

To run the service over the https, as opposed to the http, provide the certificate and the associated private key.

```
Host.Run<CalculatorService>(opts => {
    opts.TransportLevelSecuritySettings = new TransportLevelSecuritySettings
    {
        Certificate = File.ReadAllText("cert.pem"),
        PrivateKey = File.ReadAllText("key.pem"),
    };
});
```

Note that this will work only on Linux and Windows, during the platform limitations on Mac.

To detect when the service is ready to accept connections,

```
Host.Run<CalculatorService>(opts => {
    opts.Ready = () => {
        // now it is safe to call the service
    }
});
```
