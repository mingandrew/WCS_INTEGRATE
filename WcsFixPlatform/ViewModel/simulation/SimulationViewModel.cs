using GalaSoft.MvvmLight;
using simtask;

namespace wcs.ViewModel
{
    public class SimulationViewModel : ViewModelBase
    {
        public SimulationViewModel()
        {
            SimServer.Init();
        }

        #region[字段]

        private bool simserverrun;
        private bool tileworkstart;

        #endregion

        #region[属性]

        public bool SimServerRun
        {
            get => simserverrun;
            set
            {
                if (Set(ref simserverrun, value))
                {
                    if (SimServer.IsStartServer != value)
                    {
                        if (value)
                        {
                            SimServer.Start();
                        }
                        else
                        {
                            SimServer.Stop();
                        }
                    }
                }
            }
        }

        public bool TileWorkStart
        {
            get => tileworkstart;
            set
            {
                if (Set(ref tileworkstart, value))
                {
                    
                }
            }
        }
        #endregion

        #region[命令]

        #endregion

        #region[方法]

        #endregion
    }
}
