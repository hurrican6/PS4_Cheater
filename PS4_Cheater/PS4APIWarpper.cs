using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using librpc;

namespace PS4_Cheater
{
    public interface PS4APIWarpper
    {
        void Connect();
        ProcessInfo GetProcessInfo(int pid);
        ProcessList GetProcessList();
    }
}
