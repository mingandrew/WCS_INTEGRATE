namespace simserver.simsocket
{
    public class SimSocketConst
    {
        #region[公共]

        /// <summary>
        /// 读取长度
        /// </summary>
        internal const int BUFFER_SIZE = 1024;

        /// <summary>
        /// 尾部值
        /// </summary>
        internal const ushort TAIL_KEY = 0xFFFE;
        #endregion

        #region[运输车]

        internal const ushort CARRIER_HEAD_KEY = 0x9701;
        internal const ushort CARRIER_STATUS_SIZE = 43;
        internal const ushort CARRIER_CMD_HEAD_KEY = 0x9601;
        internal const byte CARRIER_CMD_SIZE = 19;
        #endregion

        #region[摆渡车]

        internal const ushort FEERY_STATUS_HEAD_KEY = 0x9501;
        internal const ushort FEERY_SPEED_HEAD_KEY = 0x9502;
        internal const ushort FEERY_SITE_HEAD_KEY = 0x9503;
        internal const ushort FERRY_CMD_HEAD_KEY = 0x9401;

        internal const byte FERRY_STATUS_SIZE = 20;
        internal const byte FERRY_SPEED_SIZE = 20;
        internal const byte FERRY_SITE_SIZE = 20;
        internal const byte FERRY_CMD_SIZE = 13;

        #endregion

        #region[上下砖机]

        internal const ushort TILELIFTER_STATUS_HEAD_KEY = 0x9101;
        internal const ushort TILELIFTER_CMD_HEAD_KEY = 0x9001;
        internal const byte TILELIFTER_STATUS_SIZE =38;

        internal const byte TILELIFTER_CMD_SIZE = 13;

        #endregion
    }
}
