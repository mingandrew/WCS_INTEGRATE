using System;

namespace socket.process
{
    public struct MsgBuffer
    {
        public byte[] Buffer;
        public UInt16 Size;

        public MsgBuffer(byte[] buffer)
        {
            Buffer = buffer;
            Size = Convert.ToUInt16(buffer.Length);
        }

        public MsgBuffer(byte[] buffer, int size)
        {
            Buffer = buffer;
            Size = Convert.ToUInt16(size);
        }
    }
}
