using System.Text;

namespace FlueGasAnalyzer.Response
{
    internal class ReadRsp
    {
        public Dictionary<string, string> RecData { get; internal set; }

        public ReadRsp(byte[] reqBytes, byte[] rspBytes)
        {
            var req = Encoding.ASCII.GetString([.. reqBytes.Skip(6)]);
            var rsp = Encoding.ASCII.GetString([.. rspBytes.Skip(6)]);
            var datalist = rsp.Split([";", ",", "&&"], StringSplitOptions.RemoveEmptyEntries).Where(item => item.Contains('=') && !item.Contains("CP"));
            var rspST = datalist.FirstOrDefault(item => item.Contains("ST"));
            var reqST = datalist.FirstOrDefault(item => item.Contains("ST"));
            if (rspST != reqST)
            {
                throw new Exception("Response ST does not match Request ST");
            }
            var rspAddr = datalist.FirstOrDefault(item => item.Contains("Addr"));
            var reqAddr = datalist.FirstOrDefault(item => item.Contains("Addr"));
            if (rspAddr != reqAddr)
            {
                throw new Exception("Response Addr does not match Request Addr");
            }
            RecData = [];
            foreach (var item in datalist)
            {
                var keyValue = item.Split('=', StringSplitOptions.RemoveEmptyEntries);
                if (keyValue.Length == 2)
                {
                    RecData[keyValue[0]] = keyValue[1];
                }
            }
        }
    }
}
