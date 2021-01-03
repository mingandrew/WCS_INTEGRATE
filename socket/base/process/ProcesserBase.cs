using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace socket.process
{
    public class ProcesserBase : IProcesser
    {
        protected readonly DateTime m_CreatedUTC;
        private static readonly byte _fillChar = 0;//the fill character
        private readonly string timeformat = "yyyy-MM-dd HH:mm:ss:ffff";

        /// <summary>
        /// 接收数据构造函数
        /// </summary>
        /// <param name="msgtype"></param>
        /// <param name="functype"></param>
        public ProcesserBase()
        {
            m_CreatedUTC = DateTime.Now;
        }

        public DateTime CreatedUTC
        {
            get
            {
                return m_CreatedUTC;
            }
        }


        public virtual MsgBuffer ToAciMsgBuffer()
        {
            throw new NotImplementedException();
        }

        protected T BufferToStruct<T>(byte[] buffer)
        {
            GCHandle packet = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            T msg = (T)Marshal.PtrToStructure(packet.AddrOfPinnedObject(), typeof(T));
            packet.Free();

            return msg;
        }

        protected byte[] StructToBuffer<T>(T msg)
        {
            int size = Marshal.SizeOf(msg);

            return StructToBuffer<T>(msg, size);
        }

        protected byte[] StructToBuffer<T>(T msg, int size)
        {
            byte[] data = new byte[size];
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(msg, handle.AddrOfPinnedObject(), false);
            }
            catch (Exception)
            {
                // don't handle error
            }
            finally
            {
                handle.Free();
            }
            return data;
        }

        protected ushort ShiftBytes(ushort value)
        {
            byte[] b = BitConverter.GetBytes(value);

            return BitConverter.ToUInt16(new byte[] { b[1], b[0] }, 0);
        }

        internal byte[] ShiftBytes(byte[] buffer)
        {
            return buffer.Reverse().ToArray();
        }

        protected bool ShiftBytes(byte value)
        {
            return value == 1;
        }

        protected int ShiftBytes(int value)
        {
            byte[] b = BitConverter.GetBytes(value);

            return BitConverter.ToInt32(new byte[] { b[3], b[2], b[1], b[0] }, 0);
        }

        protected uint ShiftBytes(uint value)
        {
            byte[] b = BitConverter.GetBytes(value);
            return BitConverter.ToUInt32(new byte[] { b[3], b[2], b[1], b[0] }, 0);

        }

        public byte ShiftByte(bool value)
        {
            return value ? (byte)1 : (byte)0;
        }

        //convert string to byte array in Ascii with length is len        
        protected byte[] StringBytes(string str, int len)
        {
            if (string.IsNullOrEmpty(str))
            {
                str = string.Empty;
            }

            byte[] result = new byte[len];
            byte[] strBytes = Encoding.Default.GetBytes(str);

            //copy the array converted into result, and fill the remaining bytes with 0
            for (int i = 0; i < len; i++)
                result[i] = ((i < strBytes.Length) ? strBytes[i] : _fillChar);

            return result;
        }


        protected byte[] TimeToBytes(DateTime? dateTime)
        {
            string time = "";
            if (dateTime != null)
            {
                time = dateTime?.ToString(timeformat);
            }
            return StringBytes(time, 24);

        }

        protected DateTime? BytesToTime(byte[] datetime)
        {
            DateTime? date = null;
            try
            {

                string times = Encoding.ASCII.GetString(datetime);
                date = DateTime.ParseExact(times, timeformat, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {

            }

            return date;
        }

        protected string GString(byte[] b)
        {
            return Encoding.Default.GetString(b).Replace("\0", "");
        }
    }
}
