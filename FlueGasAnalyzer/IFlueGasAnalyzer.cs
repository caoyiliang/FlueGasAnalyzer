using ProtocolInterface;

namespace FlueGasAnalyzer;

public interface IFlueGasAnalyzer : IProtocol
{
    Task<Dictionary<string, string>?> Read(string st, string cn, string addr, int tryCount = 0, int timeOut = -1, CancellationToken cancelToken = default);
}
