using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.WPF.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using chd.UI.Base.Extensions;
using System.Collections.Concurrent;

namespace chd.Poomsae.Scoring.WPF.Services
{
    public class TcpServer : IBroadcastClient
    {
        private readonly IOptionsMonitor<SettingDto> _optionsMonitor;

        private readonly TcpListener _server;
        private readonly Guid _id;

        public event EventHandler<ScoreReceivedEventArgs> ResultReceived;
        public event EventHandler<DeviceDto> DeviceFound;
        public event EventHandler<DeviceDto> DeviceDisconnected;
        public event EventHandler<DeviceDto> DeviceDiscovered;
        public event EventHandler ScanTimeout;
        public event EventHandler<DeviceDto> DeviceNameChanged;

        private ConcurrentDictionary<Guid, string> _connectedDevices = [];

        public TcpServer(IOptionsMonitor<SettingDto> optionsMonitor)
        {
            this._optionsMonitor = optionsMonitor;
            this._server = new TcpListener(IPAddress.Any, this._optionsMonitor.CurrentValue.ServerPort);
            this._id = Guid.NewGuid();
        }

        public async Task<List<DeviceDto>> CurrentConnectedDevices(CancellationToken cancellationToken = default)
        => this._connectedDevices.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Select(s => new DeviceDto
        {
            Id = s.Key,
            Name = s.Value
        }).ToList();

        public async Task<bool> DisconnectDeviceAsync(Guid id, CancellationToken cancellationToken = default)
        {
            this._connectedDevices.Clear();
            return true;
        }

        public async Task<bool> StartAutoConnectAsync(CancellationToken cancellationToken = default)
        {
            if (!this._optionsMonitor.CurrentValue.IsServer) { return false; }
            this._server.Start();

            _ = this.StartServerListening(cancellationToken);

            return true;
        }

        private async Task StartServerListening(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var client = await this._server.AcceptTcpClientAsync(cancellationToken);
                this.ScanTimeout?.Invoke(this, EventArgs.Empty);
                _ = this.HandleClient(Guid.NewGuid(), client, cancellationToken);
            }
        }

        private async Task HandleClient(Guid id, TcpClient client, CancellationToken token)
        {
            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            try
            {
                var sb = new StringBuilder();
                sb.AppendLine(this._id.ToString());
                sb.AppendLine(Environment.MachineName);
                await writer.WriteAsync(sb, token);
                while (!token.IsCancellationRequested)
                {
                    var message = await reader.ReadLineAsync(token);
                    await this.HandleMessage(id, message.Trim());
                }
            }
            catch (IOException)
            {
            }
            finally
            {
                client.Close();
                this.DeviceDisconnected?.Invoke(this, new DeviceDto
                {
                    Id = id,
                    Name = string.Empty
                });
            }

        }

        private async Task HandleMessage(Guid id, string message)
        {
            var data = message.Split(":");
            if (data.Length != 2 || !(data[0] == "NAME" || data[0] == "RESULT")) { return; }
            if (data[0] == "NAME")
            {
                if (!this._connectedDevices.ContainsKey(id))
                {
                    this._connectedDevices[id] = data[1];
                    this.DeviceFound?.Invoke(this, new DeviceDto
                    {
                        Id = id,
                        Name = data[1]
                    });
                }
                else if (this._connectedDevices[id] != data[1])
                {
                    this.DeviceNameChanged?.Invoke(this, new DeviceDto
                    {
                        Id = id,
                        Name = data[1]
                    });
                }
            }
            else if (this._connectedDevices.ContainsKey(id)
                && data[0] == "RESULT"
                && data[1].Split(",").Length == 10)
            {
                var value = data[1].Split(",").Select(s => byte.Parse(s)).ToArray();
                var chongResult = value[0] == 0 ? null : new ScoreDto(value.Skip(1).Take(4).ToArray());
                var hongResult = value[5] == 0 ? null : new ScoreDto(value.Skip(6).Take(4).ToArray());

                this.ResultReceived?.Invoke(this, new ScoreReceivedEventArgs
                {
                    Device = new DeviceDto
                    {
                        Id = id,
                        Name = this._connectedDevices[id]
                    },
                    Chong = chongResult,
                    Hong = hongResult,
                });
            }
        }


        public async Task<bool> StartDiscoverAsync(CancellationToken cancellationToken = default)
        {
            return true;
        }
        public async Task<bool> ConnectDeviceAsync(DeviceDto dto, CancellationToken cancellationToken = default)
        {
            return true;
        }
    }
}
