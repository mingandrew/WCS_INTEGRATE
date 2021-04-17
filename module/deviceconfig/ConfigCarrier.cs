namespace module.deviceconfig
{
    public class ConfigCarrier
    {
        /// <summary>
        /// 运输车 设备ID
        /// </summary>
        public uint id { set; get; }

        /// <summary>
        /// 前进放货没扫到地标
        /// </summary>
        public bool a_givemisstrack { set; get; }

        /// <summary>
        /// 后退取砖没扫到地标
        /// </summary>
        public bool a_takemisstrack { set; get; }

        /// <summary>
        /// 故障轨道
        /// </summary>
        public uint a_alert_track { set; get; }

        /// <summary>
        /// 库存ID
        /// </summary>
        public uint stock_id { set; get; }

        /// <summary>
        /// 运输顶板长度（脉冲）
        /// </summary>
        public ushort length { set; get; }

        /// <summary>
        /// 运输车负责规格ID集
        /// </summary>
        public string goods_size { set; get; }

        /// <summary>
        /// 运输车是否负责该规格
        /// </summary>
        /// <param name="sizeID"></param>
        /// <returns></returns>
        public bool IsUseGoodsSize(uint sizeID)
        {
            if (sizeID == 0) return false;
            if (string.IsNullOrEmpty(goods_size)) return true;

            return goods_size.Contains(sizeID.ToString());
        }
    }
}
