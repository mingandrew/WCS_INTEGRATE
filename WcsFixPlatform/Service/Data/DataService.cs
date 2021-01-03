using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using wcs.Data.Model;

namespace wcs.Service
{
    public class DataService
    {
        /// <summary>
        /// 获取设置的列表信息
        /// </summary>
        /// <returns></returns>
        internal List<WinCtlModel> GetWinCtlData()
        {
            var itemList = new List<WinCtlModel>();

            try
            {
                var stream = Application.GetResourceStream(new Uri("Data/WinCtlData.json", UriKind.Relative))?.Stream;
                if (stream == null) return itemList;

                string jsonStr;
                using (var reader = new StreamReader(stream))
                {
                    jsonStr = reader.ReadToEnd();
                }

                var jsonObj = JsonConvert.DeserializeObject<dynamic>(jsonStr);
                foreach (var items in jsonObj)
                {
                    foreach (var item in items.WinCtlList)
                    {
                        var key = (string)item[0];
                        var name = (string)item[1];
                        var geometry = (string)item[2];
                        var brush = (string)item[3];
                        var winctlname = (string)item[4];
                        itemList.Add(new WinCtlModel
                        {
                            Key = key,
                            Name = name,
                            Geometry = geometry,
                            Brush = brush,
                            WinCtlName = winctlname
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return itemList;
        }
    }
}