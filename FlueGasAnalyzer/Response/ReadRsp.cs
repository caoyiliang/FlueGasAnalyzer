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
            var rspCN = datalist.FirstOrDefault(item => item.Contains("CN"));
            var reqCN = datalist.FirstOrDefault(item => item.Contains("CN"));
            if (rspCN != reqCN)
            {
                throw new Exception("Response CN does not match Request CN");
            }
            RecData = new Dictionary<string, string>();
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
