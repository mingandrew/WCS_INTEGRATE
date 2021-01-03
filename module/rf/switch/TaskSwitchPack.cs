using System.Collections.Generic;

namespace module.rf
{
    public class TaskSwitchPack
    {
        public List<TaskSwitch> SwitchList { set; get; }

        public void AddSwitch(List<TaskSwitch> list)
        {
            if(SwitchList == null)
            {
                SwitchList = new List<TaskSwitch>();
            }

            SwitchList.AddRange(list);
        }
    }
}
