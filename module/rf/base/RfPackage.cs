using tool.json;

namespace module.rf
{
    public class RfPackage
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string Meid { set; get; }

        /// <summary>
        /// 会话ID
        /// </summary>
        public int SessionID { get; set; }

        /// <summary>
        /// 功能方法
        /// </summary>
        public string Function { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 错误类型
        /// </summary>
        public string ErrorType { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public string Data { get; set; }

        public string ClientId { set; get; }


        public T GetObject<T>()
        {
            return JsonTool.Deserialize<T>(Data);
        }
    }
}
