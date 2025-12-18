using System.Text;
using TopPortLib.Interfaces;
using Utils;

namespace FlueGasAnalyzer.Request
{
    internal class ReadReq : IByteStream
    {
        private string _st;
        private string _cn;
        private string _addr;

        public ReadReq(string st, string cn, string addr)
        {
            _st = st;
            _cn = cn;
            _addr = addr;
        }

        public byte[] ToBytes()
        {
            var rs = $"QN={DateTime.Now:yyyyMMddHHmmssfff};ST={_st};CN={_cn};Addr={_addr};CP=&&&&";
            return Encoding.ASCII.GetBytes(GetGbCmd(rs));
        }

        internal string GetGbCmd(string rs)
        {
            var brs = Encoding.ASCII.GetBytes(rs);
            return $"##{rs.Length.ToString().PadLeft(4, '0')}{rs}{StringByteUtils.BytesToString(CRC.GBcrc16(brs, brs.Length)).Replace(" ", "")}\r\n";
        }
    }
}
