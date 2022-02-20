using System.Threading;
using Grpc.Net.Client;
using Xunit;

namespace Just.Grpc.Test;

public class HostTests
{
    [Fact]
    public void TestBasic()
    {

        var ready = new AutoResetEvent(false);
        var tokenSource = new CancellationTokenSource();
        var serviceTask = Host.RunAsync<CalculatorService>(opts =>
        {
            opts.Silent = true;
            opts.Ready = () =>
            {
                ready.Set();
            };
        });
        ready.WaitOne();
        RunClient("http://localhost:8080");
        tokenSource.Cancel();
    }

    [Fact]
    public void TestPort()
    {

        var ready = new AutoResetEvent(false);
        var tokenSource = new CancellationTokenSource();
        var serviceTask = Host.RunAsync<CalculatorService>(opts =>
        {
            opts.Silent = true;
            opts.Port = 9090;
            opts.Ready = () =>
            {
                ready.Set();
            };
        });
        ready.WaitOne();
        RunClient("http://localhost:9090");
        tokenSource.Cancel();
    }


    private void RunClient(string url)
    {
        var channel = GrpcChannel.ForAddress(url);
        var client = new Calculator.CalculatorClient(channel);
        var req = new CalculateRequest
        {
            X = 17,
            Y = 25,
            Op = "+"
        };
        var resp = client.Calculate(req);
        Assert.Equal(42, resp.Result);
    }
}