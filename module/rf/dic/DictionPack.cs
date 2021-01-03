using enums;
using module.area;
using module.device;
using module.diction;
using module.goods;
using module.track;
using System;
using System.Collections.Generic;

namespace module.rf
{
    public class DictionPack
    {
        public List<RfDiction> DicList { set; get; }
        public VersionDic VersionDic { set; get; }
        public bool UserLoginFunction { set; get; }

        public void AddDic(RfDiction dic)
        {
            if(DicList == null)
            {
                DicList = new List<RfDiction>();
            }
            DicList.Add(dic);
        }

        public void AddEnum(Type type, string name, string code)
        {
            try
            {
                RfDiction dic = new RfDiction
                {
                    DicName = name,
                    DicCode = code
                };

                Array array = type.GetEnumValues();
                int order = 0;
                foreach (var value in array)
                {
                    dic.AddDtl(new RfDictionDtl()
                    {
                        DtlOrder = order,
                        DtlValue = (int)value,
                        DtlName = value + ""
                    });
                    order++;
                }
                AddDic(dic);
            }catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            
        }

        public void AddArea(List<Area> lists)
        {
            RfDiction dic = new RfDiction
            {
                DicName = "区域字典",
                DicCode = "AreaDic"
            };

            int order = 0;
            foreach (Area item in lists)
            {
                dic.AddDtl(new RfDictionDtl()
                {
                    DtlOrder = order,
                    DtlValue = (int)item.id,
                    DtlName = item.name
                });
                order++;
            }
            AddDic(dic);
        }

        public void AddTrack(List<Track> lists)
        {
            RfDiction dic = new RfDiction
            {
                DicName = "轨道字典",
                DicCode = "TrackDic"
            };

            RfDiction rfdic = new RfDiction
            {
                DicName = "站点字典",
                DicCode = "RfCodeDic"
            };

            int order = 0, rfcodeorder = 0;
            foreach (Track item in lists)
            {
                dic.AddDtl(new RfDictionDtl()
                {
                    DtlOrder = order,
                    DtlValue = (int)item.id,
                    DtlName = item.name
                });

                AddTrackRfDicDtl(ref rfdic, ref rfcodeorder, item);
                order++;
            }
            AddDic(dic);
            AddDic(rfdic);
        }

        private void AddTrackRfDicDtl(ref RfDiction dic, ref int order, Track track)
        {
            if (track != null)
            {
                AddRfCodeDtl(ref dic, ref order, track.name, track.rfid_1);
                AddRfCodeDtl(ref dic, ref order, track.name, track.rfid_2);
                AddRfCodeDtl(ref dic, ref order, track.name, track.rfid_3);
                AddRfCodeDtl(ref dic, ref order, track.name, track.rfid_4);
                AddRfCodeDtl(ref dic, ref order, track.name, track.rfid_5);
                AddRfCodeDtl(ref dic, ref order, track.name, track.rfid_6);
            }
        }

        private void AddRfCodeDtl(ref RfDiction dic, ref int order, string trackname, ushort rfcode)
        {
            if (rfcode > 0)
            {
                dic.AddDtl(new RfDictionDtl()
                {
                    DtlName = trackname,
                    DtlOrder = order,
                    DtlValue = rfcode
                });
                order++;
            }
        }

        public void AddDevice(List<Device> lists)
        {
            RfDiction devdic = new RfDiction
            {
                DicName = "设备字典",
                DicCode = "DeviceDic"
            };

            RfDiction ferrydic = new RfDiction
            {
                DicName = "摆渡字典",
                DicCode = "FerryDic"
            };

            RfDiction carrierdic = new RfDiction
            {
                DicName = "运输车字典",
                DicCode = "CarrierDic"
            };

            RfDiction tilelifterdic = new RfDiction
            {
                DicName = "砖机字典",
                DicCode = "TileLifterDic"
            };

            int order = 0, ferryorder = 0, tileoder = 0, carrierorder =0;
            foreach (Device item in lists)
            {
                devdic.AddDtl(new RfDictionDtl()
                {
                    DtlOrder = order,
                    DtlValue = (int)item.id,
                    DtlName = item.name
                });
                order++;

                switch (item.Type)
                {
                    case DeviceTypeE.上砖机:
                    case DeviceTypeE.下砖机:
                        tilelifterdic.AddDtl(new RfDictionDtl()
                        {
                            DtlOrder = tileoder,
                            DtlValue = (int)item.id,
                            DtlName = item.name
                        });
                        tileoder++;
                        break;
                    case DeviceTypeE.上摆渡:
                    case DeviceTypeE.下摆渡:

                        ferrydic.AddDtl(new RfDictionDtl()
                        {
                            DtlOrder = ferryorder,
                            DtlValue = (int)item.id,
                            DtlName = item.name
                        });
                        ferryorder++;
                        break;
                    case DeviceTypeE.运输车:

                        carrierdic.AddDtl(new RfDictionDtl()
                        {
                            DtlOrder = carrierorder,
                            DtlValue = (int)item.id,
                            DtlName = item.name
                        });
                        carrierorder++;
                        break;
                }
            }

            AddDic(devdic);
            AddDic(tilelifterdic);
            AddDic(ferrydic);
            AddDic(carrierdic);
        }

        public void AddGood(List<Goods> lists)
        {
            RfDiction dic = new RfDiction
            {
                DicName = "规格字典",
                DicCode = "GoodDic"
            };

            int order = 0;
            foreach (Goods item in lists)
            {
                dic.AddDtl(new RfDictionDtl()
                {
                    DtlOrder = order,
                    DtlValue = (int)item.id,
                    DtlName = item.name
                });
                order++;
            }
            AddDic(dic);
        }


        public void AddVersion(string dictag, int version)
        {
            VersionDic = new VersionDic()
            {
                Name = dictag,
                Version = version
            };
        }

        public void AddFerry(List<Device> lists)
        {
            RfDiction dic = new RfDiction
            {
            };

            int order = 0;
            foreach (Device item in lists)
            {
                dic.AddDtl(new RfDictionDtl()
                {
                    DtlOrder = order,
                    DtlValue = (int)item.id,
                    DtlName = item.name
                });
                order++;
            }
            AddDic(dic);
        }

        public void AddCarrier(List<Device> lists)
        {
            RfDiction dic = new RfDiction
            {
                DicName = "运输车字典",
                DicCode = "CarrierDic"
            };

            int order = 0;
            foreach (Device item in lists)
            {
                dic.AddDtl(new RfDictionDtl()
                {
                    DtlOrder = order,
                    DtlValue = (int)item.id,
                    DtlName = item.name
                });
                order++;
            }
            AddDic(dic);
        }
    }
}
