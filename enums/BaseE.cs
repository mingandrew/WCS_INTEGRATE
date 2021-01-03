namespace enums
{
    /// <summary>
    /// 一般数据类型
    /// </summary>
    public enum ValueTypeE
    {
        Integer,
        Boolean,
        String,
        Double,
        UInteger
    }

    public enum ActionTypeE
    {
        Add,
        Update,
        Delete,
        Finish
    }


    /// <summary>
    /// 数据类型
    /// </summary>
    public enum TypeCodeE
    {
        Empty,
        /// <summary>
        /// 摘要:
        ///    8位(1个字节)，表示值介于 0 到 1 的 二进制数。
        /// </summary>
        Bit8 = 3,
        /// <summary>
        /// 摘要:
        ///    16位(2个字节)，表示值介于 0 到 1 的 二进制数。
        /// </summary>
        Bit16 = 4,
        ///<summary>
        ///摘要:
        ///    整型，表示值介于 -128 到 127 之间的 8 位有符号整数。
        ///</summary>
        SByte = 5,
        ///<summary>
        ///摘要:
        ///    整型，表示值介于 0 到 255 之间的无符号 8 位整数。
        ///</summary>
        Byte = 6,
        ///<summary>
        ///摘要:
        ///    整型，表示值介于 -32768 到 32767 之间的有符号 16 位整数。
        ///</summary>
        Short = 7,
        ///<summary>
        ///摘要:
        ///    整型，表示值介于 0 到 65535 之间的 16 位无符号整数。
        ///</summary>
        UShort = 8,
        ///<summary>
        ///摘要:
        ///    整型，表示值介于 -2147483648 到 2147483647 之间的 32 位有符号整数。
        ///</summary>
        Int32 = 9,
        ///<summary>
        ///摘要:
        ///    整型，表示值介于 0 到 4294967295 之间的 32 位无符号整数。
        ///</summary>
        UInt32 = 10,
        ///<summary>
        ///摘要:
        ///    整型，表示值介于-9223372036854775808 和 9223372036854775807 之间的 64 位有符号整数。
        ///</summary>
        Int64 = 11,
        ///<summary>
        ///摘要:
        ///    整型，表示值介于 0 到 18446744073709551615 之间的 64 位无符号整数。
        ///</summary>
        UInt64 = 12,
    }

}
