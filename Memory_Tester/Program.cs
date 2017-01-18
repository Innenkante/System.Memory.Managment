using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Memory.Managment;
using System.Diagnostics;

namespace Memory_Tester
{
    class Program
    {   
        static void Main(string[] args)
        {

            Pointer<object>.ProcessHandle = HackProcess.GetProcessHandle("iw5mp");
            Pointer<int> p1 = new Pointer<int>(0xDEADBEEF, 4);
            Console.WriteLine(p1.ReadValue());
            p1.SetValue(0);
            Console.ReadKey();
        }
    }
}
