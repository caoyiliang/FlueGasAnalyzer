using Communication;
using Communication.Bus.PhysicalPort;
using Communication.Exceptions;
using FlueGasAnalyzer.Request;
using FlueGasAnalyzer.Response;
using LogInterface;
using Parser;
using Parser.Parsers;
using System.Text;
using TopPortLib;
using TopPortLib.Interfaces;
using Utils;

namespace FlueGasAnalyzer
{
    public class FlueGasAnalyzer : IFlueGasAnalyzer
    {
        private static readonly ILogger _logger = Logs.LogFactory.GetLogger<FlueGasAnalyzer>();
        private readonly ICrowPort _crowPort;
        private bool _isConnect = false;
        public bool IsConnect => _isConnect;

        /// <inheritdoc/>
        public event DisconnectEventHandler? OnDisconnect { add => _crowPort.OnDisconnect += value; remove => _crowPort.OnDisconnect -= value; }
        /// <inheritdoc/>
        public event ConnectEventHandler? OnConnect { add => _crowPort.OnConnect += value; remove => _crowPort.OnConnect -= value; }

        public FlueGasAnalyzer(SerialPort serialPort, int defaultTimeout = 5000)
        {
            _crowPort = new CrowPort(new TopPort(serialPort, new HeadLengthParser([0x23, 0x23], d =>
            {
                if (d.Length < 15) return Task.FromResult(new GetDataLengthRsp() { StateCode = Parser.StateCode.LengthNotEnough });
                if (int.TryParse(Encoding.UTF8.GetString(d, 2, 4), out var rs))
                {
                    return Task.FromResult(new GetDataLengthRsp() { Length = rs + 8, StateCode = Parser.StateCode.Success });
                }
                else
                {
                    return Task.FromResult(new GetDataLengthRsp() { Length = 4, StateCode = Parser.StateCode.Success });
                }
            })), defaultTimeout);
            _crowPort.OnSentData += CrowPort_OnSentData;
            _crowPort.OnReceivedData += CrowPort_OnReceivedData;
            _crowPort.OnConnect += CrowPort_OnConnect;
            _crowPort.OnDisconnect += CrowPort_OnDisconnect;
        }

        public FlueGasAnalyzer(ICrowPort crowPort)
        {
            _crowPort = crowPort;
            _crowPort.OnConnect += CrowPort_OnConnect;
            _crowPort.OnDisconnect += CrowPort_OnDisconnect;
        }

        private async Task CrowPort_OnDisconnect()
        {
            _isConnect = false;
            await Task.CompletedTask;
        }

        private async Task CrowPort_OnConnect()
        {
            _isConnect = true;
            await Task.CompletedTask;
        }

        private async Task CrowPort_OnReceivedData(byte[] data)
        {
            _logger.Trace($"FlueGasAnalyzer Rec:<-- {StringByteUtils.BytesToString(data)}");
            await Task.CompletedTask;
        }

        private async Task CrowPort_OnSentData(byte[] data)
        {
            _logger.Trace($"FlueGasAnalyzer Send:--> {StringByteUtils.BytesToString(data)}");
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task OpenAsync()
        {
            _isConnect = _crowPort.PhysicalPort.IsOpen;
            return _crowPort.OpenAsync();
        }

        /// <inheritdoc/>
        public async Task CloseAsync(bool closePhysicalPort)
        {
            if (closePhysicalPort) await _crowPort.CloseAsync();
        }

        public async Task<Dictionary<string, string>?> Read(string st, string cn, string addr, int tryCount = 0, int timeOut = -1, CancellationToken cancelToken = default)
        {
            if (!_isConnect) throw new NotConnectedException();
            Func<Task<ReadRsp>> func = () => _crowPort.RequestAsync<ReadReq, ReadRsp>(new ReadReq(st, cn, addr), timeOut);
            return (await func.ReTry(tryCount, cancelToken))?.RecData;
        }
    }
}
