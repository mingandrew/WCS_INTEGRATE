using enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using module.line;
using resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using wcs.Data.View;

namespace wcs.ViewModel
{
    public class AreaLineSwitchModel : ViewModelBase
    {

        private ObservableCollection<AreaLineView> _list;
        private AreaLineView selectarealine;

        public AreaLineSwitchModel()
        {
            _list = new ObservableCollection<AreaLineView>();

            Messenger.Default.Register<Line>(this, MsgToken.LineSwitchUpdate, LineSwitchUpdate);

            initList();
        }

        private void initList()
        {
            List<Line> list = PubMaster.Area.GetLineList();
            foreach (var item in list)
            {
                List.Add(new AreaLineView(item));
            }
        }

        public ObservableCollection<AreaLineView> List
        {
            get => _list;
            set => Set(ref _list, value);
        }

        public AreaLineView SelectAreaLine
        {
            get => selectarealine;
            set => Set(ref selectarealine, value);
        }

        private void LineSwitchUpdate(Line item)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AreaLineView line = List.FirstOrDefault(c => c.id == item.id);
                if(line != null)
                {
                    line.Update(item);
                }
                else
                {
                    List.Add(new AreaLineView(item));
                }
            });
        }
    }
}
