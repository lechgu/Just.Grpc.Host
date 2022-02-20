using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Just.Grpc;

public class Host
{
    private static WebApplication Build<T>(Action<HostOptions>? opts = null) where T : class
    {
        var config = new HostOptions();
        if (opts is not null)
        {
            opts(config);
        }

        var builder = WebApplication.CreateBuilder();
        if (config.Silent)
        {
            builder.Logging.ClearProviders();
        }

        var listenSetup = (ListenOptions lo) =>
        {
            lo.Protocols = HttpProtocols.Http2;
        };
        if (config.TransportLevelSecuritySettings is not null)
        {
            var cert = X509Certificate2.CreateFromPem(
                config.TransportLevelSecuritySettings.Certificate,
                config.TransportLevelSecuritySettings.PrivateKey
                );
            listenSetup = (ListenOptions lo) =>
            {
                lo.Protocols = HttpProtocols.Http2;
                lo.UseHttps(cert);
            };
        }


        builder.WebHost.ConfigureKestrel(opts =>
        {

            opts.ListenAnyIP(config.Port, listenSetup);
        });


        builder.Services.AddGrpc();
        var app = builder.Build();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<T>();
        });
        if (config.Ready is not null)
        {
            app.Lifetime.ApplicationStarted.Register(config.Ready);
        }
        return app;
    }


    public static Task RunAsync<T>(Action<HostOptions>? opts = null, CancellationToken cancelToken = default) where T : class
    {
        var app = Build<T>(opts);

        if (cancelToken != CancellationToken.None)
        {
            Task.Run(() =>
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                }
                app.Lifetime.StopApplication();
            }, CancellationToken.None);
        }
        return Task.Run(() =>
       {
           app.Run();
       }, cancelToken);
    }

    public static void Run<T>(Action<HostOptions>? opts = null, CancellationToken cancelToken = default) where T : class
    {
        var task = RunAsync<T>(opts, cancelToken);
        task.Wait(CancellationToken.None);
    }
}