using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Memory.Managment;
using System.Diagnostics;

namespace Memory_Tester
{
    ///Example of using the abstract base to add a new datatype 
    class Vector //Our custom type
    {
        public Vector(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public float x;
        public float y;
        public float z;
    }

    class VectorHandler : HandlerBase<Vector> //Our custom handler
    {
        public override Vector Read(Pointer<Vector> pointer)
        {
            return new Vector(new Pointer<float>(pointer.Address).ReadValue(),new Pointer<float>(pointer.Address + 4).ReadValue(),new Pointer<float>(pointer.Address + 8).ReadValue());
        }

        public override void Write(Pointer<Vector> pointer, Vector value)
        {
            new Pointer<float>(pointer.Address).SetValue(value.x);
            new Pointer<float>(pointer.Address + 4).SetValue(value.y);
            new Pointer<float>(pointer.Address + 8).SetValue(value.z);
        }
    }

    class Program
    {   
        static void Main(string[] args)
        {
            //Example of the usage:
            Pointer<object>.ProcessHandle = HackProcess.GetProcessHandle("whatever"); //Getting the process handle, it is static to the class, just using object because of lulZ
            Pointer<int> p1 = new Pointer<int>(0xDEADBEEF); //Pointer object to 0xDEADBEEF with the type int to 0xDEADBEEF
            Pointer<string> p2 = new Pointer<string>(0xDEADBEAF, 16); //Pointer object of the type string to 0xDEADBEAF with the size of 16 chars
            Pointer<object> p3 = new Pointer<object>(0xFFFFFFFF); //Pointer object with they of object , so basically for everything
            p1.SetValue(100); //Writes the certain value to the Pointer
            p2.ReadValue(); //Reads the certain value of the pointer in the size of 16 chars
            p3.SetNOP(16); //Sets 0x90 aka no executable code to the adress range -> size

            Pointer<object>.Handler.Add(typeof(Vector), new VectorHandler()); //Adding our handler for our custom type to the dictionary
            Pointer<Vector> p4 = new Pointer<Vector>(0xDEADBAAF); //Making an instance of the pointer object with our custom type
            p4.ReadValue(); 
            p4.SetValue(new Vector(0, 0, 0)); 
            Console.ReadKey();
        }
    }
}
