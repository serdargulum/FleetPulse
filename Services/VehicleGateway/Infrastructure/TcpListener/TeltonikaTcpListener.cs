
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;

namespace VehicleGateway.Infrastructure.TcpListener;

public class TeltonikaTcpListener : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var listener = new System.Net.Sockets.TcpListener(IPAddress.Any, 4000);

        listener.Start();

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync(stoppingToken);

                _ = HandleClientAsync(client, stoppingToken);
            }
        }
        finally
        {
            listener.Stop();
        }
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        using var _ = client;
        using var stream = client.GetStream();

        var buffer = new byte[4096];

        while (!cancellationToken.IsCancellationRequested)
        {
            var bytesRead = await stream.ReadAsync(buffer.AsMemory(), cancellationToken);
            if (bytesRead == 0) break;

            // TODO: Not implemented — Teltonika IMEI handshake.
            // TODO: Not implemented — Teltonika packet parsing.

        }
    }

}
