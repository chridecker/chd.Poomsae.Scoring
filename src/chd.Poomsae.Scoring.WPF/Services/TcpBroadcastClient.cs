using chd.Poomsae.Scoring.Contracts.Dtos;
using chd.Poomsae.Scoring.Contracts.Dtos.Base;
using chd.Poomsae.Scoring.Contracts.Enums;
using chd.Poomsae.Scoring.Contracts.Interfaces;
using chd.Poomsae.Scoring.WPF.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.WPF.Services
{
    public class TcpBroadcastClient : IBroadCastService
    {
        private readonly IOptionsMonitor<SettingDto> _optionsMonitor;
        private readonly ISettingManager _settingManager;

        private TcpClient _client;

        public TcpBroadcastClient(IOptionsMonitor<SettingDto> optionsMonitor, ISettingManager settingManager)
        {
            this._optionsMonitor = optionsMonitor;
            this._settingManager = settingManager;
        }

        public async Task BroadcastNameChange()
        {
            if (this._optionsMonitor.CurrentValue.IsServer || this._client is null || !this._client.Connected) { return; }
            var name = await this._settingManager.GetName();
            var text = $"NAME:{name}\r\n";
            this._client.Client.Send(Encoding.ASCII.GetBytes(text));
        }

        public void BroadcastResult(RunDto run)
        {
            if (this._optionsMonitor.CurrentValue.IsServer || this._client is null || !this._client.Connected) { return; }
            try
            {
                var resultData = new byte[10];
                if (run is EliminationRunDto elimination)
                {
                    resultData = [1, __dataConvert(elimination.ChongScore.Accuracy), __dataConvert(elimination.ChongScore.SpeedAndPower ?? 0m), __dataConvert(elimination.ChongScore.RhythmAndTempo ?? 0m), __dataConvert(elimination.ChongScore.ExpressionAndEnergy ?? 0m), 2, __dataConvert(elimination.HongScore.Accuracy), __dataConvert(elimination.HongScore.SpeedAndPower ?? 0m), __dataConvert(elimination.HongScore.RhythmAndTempo ?? 0m), __dataConvert(elimination.HongScore.ExpressionAndEnergy ?? 0m)];
                }
                else if (run is SingleRunDto singleRun)
                {
                    if (singleRun.Color is EScoringButtonColor.Blue)
                    {
                        resultData = [1, __dataConvert(singleRun.Score.Accuracy), __dataConvert(singleRun.Score.SpeedAndPower ?? 0m), __dataConvert(singleRun.Score.RhythmAndTempo ?? 0m), __dataConvert(singleRun.Score.ExpressionAndEnergy ?? 0m), 0, 0, 0, 0, 0];
                    }
                    else if (singleRun.Color is EScoringButtonColor.Red)
                    {
                        resultData = [0, 0, 0, 0, 0, 2, __dataConvert(singleRun.Score.Accuracy), __dataConvert(singleRun.Score.SpeedAndPower ?? 0m), __dataConvert(singleRun.Score.RhythmAndTempo ?? 0m), __dataConvert(singleRun.Score.ExpressionAndEnergy ?? 0m)];
                    }
                }
                var text = $"RESULT:{string.Join(",", resultData)}\r\n";

                this._client.Client.Send(Encoding.ASCII.GetBytes(text));
            }
            catch { }
            byte __dataConvert(decimal d) => (byte)(d * 10);
        }

        public void ResetScore()
        {
            try
            {
                if (this._optionsMonitor.CurrentValue.IsServer) { return; }
                var data = new byte[10];
                var text = $"RESULT:{string.Join(",", data)}\r\n";

                this._client.Client.Send(Encoding.ASCII.GetBytes(text));
            }
            catch { }
        }

        public async Task StartAsync(CancellationToken token)
        {
            if (this._optionsMonitor.CurrentValue.IsServer) { return; }

            while (!token.IsCancellationRequested)
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        this._client = new TcpClient();
                        this._client.Connect(IPAddress.Parse(this._optionsMonitor.CurrentValue.ServerAddress), this._optionsMonitor.CurrentValue.ServerPort);
                        break;
                    }
                    catch { }
                    await Task.Delay(TimeSpan.FromSeconds(5), token);
                }

                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        await this.BroadcastNameChange();
                    }
                    catch
                    {
                        break;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(5), token);
                }
            }
        }
    }
}
