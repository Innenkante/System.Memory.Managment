using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Memory.Managment
{

    public class MemoryBase
    {
        [DllImport("kernel32.dll")]
        protected static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        protected static extern Int32 WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesWritten);

        protected byte[] Read(IntPtr handle,IntPtr address,int length)
        {
            try
            {
                byte[] buf = new byte[length];
                IntPtr numOfBytes = IntPtr.Zero;
                ReadProcessMemory(handle, address, buf, (uint)buf.Length, out numOfBytes);
                return buf;
            }
            catch(Exception e)
            {
                throw new Exception("Exception in reading the memory: " + e.Message);
            }
        }

        protected void Write(IntPtr handle,IntPtr address,byte[] value)
        {
            try
            {
                IntPtr numOfBytes = IntPtr.Zero;
                WriteProcessMemory(handle, address, value, (uint)value.Length, out numOfBytes);
            }
            catch (Exception e)
            {
                throw new Exception("Exception in writing the memory: " + e.Message);
            }
        }
    }
        
    public abstract class HandlerBase<T> : MemoryBase
    {
        public abstract void Write(Pointer<T> pointer,T value);
        public abstract T Read(Pointer<T> pointer);
        
    }

    public class Pointer<T> : MemoryBase
    {
        private static readonly Dictionary<Type, object> Handler = new Dictionary<Type, object>();

        public static IntPtr ProcessHandle;

        static Pointer()
        {
            Handler.Add(typeof(string), new StringHandler());
            Handler.Add(typeof(int), new IntHandler());
            Handler.Add(typeof(float), new FloatHandler());
            Handler.Add(typeof(double), new DoubleHandler());
            Handler.Add(typeof(long), new LongHandler());
            Handler.Add(typeof(byte[]), new ByteArrayHandler());
            Handler.Add(typeof(char), new CharHandler());
        }

        public Pointer(ulong address,int size = 4)
        {
            Address = (IntPtr)address;
            Size = size;
        }

        public Pointer(IntPtr address,int size = 4)
        {
            Address = address;
            Size = size;
        }

        public IntPtr Address;

        public int Size;


        public void SetValue(T value)
        {
            ((HandlerBase<T>)Handler[typeof(T)]).Write(this,value);
        }

        public T ReadValue()
        {
            return ((HandlerBase<T>)Handler[typeof(T)]).Read(this);
        }

        public void SetNOP(int length)
        {
            byte[] buffer = new byte[length];
            for(int i = 0;i<length;i++)
            {
                buffer[i] = 0x90;
            }
            Write(Pointer<object>.ProcessHandle, Address, buffer);
        }

        public static Pointer<T> operator + (Pointer<T> p1,Pointer<T> p2) => 
            new Pointer<T>((ulong)p1.Address + (ulong)p2.Address);

        public static Pointer<T> operator -(Pointer<T> p1, Pointer<T> p2) =>
            new Pointer<T>((ulong)p1.Address - (ulong)p2.Address);

        public static Pointer<T> operator *(Pointer<T> p1, Pointer<T> p2) =>
            new Pointer<T>((ulong)p1.Address * (ulong)p2.Address);

        public static Pointer<T> operator /(Pointer<T> p1, Pointer<T> p2) =>
            new Pointer<T>((ulong)p1.Address / (ulong)p2.Address);

    }

    class StringHandler : HandlerBase<string>
    {
        public override string Read(Pointer<string> pointer)
        {
            return new ASCIIEncoding().GetString(Read(Pointer<object>.ProcessHandle, pointer.Address, pointer.Size));
        }

        public override void Write(Pointer<string> pointer,string value)
        {
            Write(Pointer<object>.ProcessHandle, pointer.Address, new ASCIIEncoding().GetBytes(value));
        }
    }

    class IntHandler : HandlerBase<int>
    {
        public override int Read(Pointer<int> pointer)
        {
            return BitConverter.ToInt32(Read(Pointer<object>.ProcessHandle, pointer.Address, pointer.Size),0);
        }

        public override void Write(Pointer<int> pointer, int value)
        {
            Write(Pointer<object>.ProcessHandle, pointer.Address, BitConverter.GetBytes(value));
        }
    }

    class FloatHandler : HandlerBase<float>
    {
        public override float Read(Pointer<float> pointer)
        {
            return BitConverter.ToSingle(Read(Pointer<object>.ProcessHandle, pointer.Address, pointer.Size), 0);
        }

        public override void Write(Pointer<float> pointer, float value)
        {
            Write(Pointer<object>.ProcessHandle, pointer.Address, BitConverter.GetBytes(value));
        }
    }

    class DoubleHandler : HandlerBase<double>
    {
        public override double Read(Pointer<double> pointer)
        {
            return BitConverter.ToDouble(Read(Pointer<object>.ProcessHandle, pointer.Address, pointer.Size), 0);
        }

        public override void Write(Pointer<double> pointer, double value)
        {
            Write(Pointer<object>.ProcessHandle, pointer.Address, BitConverter.GetBytes(value));
        }
    }

    class LongHandler : HandlerBase<long>
    {
        public override long Read(Pointer<long> pointer)
        {
            return BitConverter.ToInt64(Read(Pointer<object>.ProcessHandle, pointer.Address, pointer.Size), 0);
        }

        public override void Write(Pointer<long> pointer, long value)
        {
            Write(Pointer<object>.ProcessHandle, pointer.Address, BitConverter.GetBytes(value));
        }
    }

    class ByteArrayHandler : HandlerBase<byte[]>
    {
        public override byte[] Read(Pointer<byte[]> pointer)
        {
            return Read(Pointer<object>.ProcessHandle, pointer.Address, pointer.Size);
        }

        public override void Write(Pointer<byte[]> pointer, byte[] value)
        {
            Write(Pointer<object>.ProcessHandle, pointer.Address, value);
        }
    }

    class CharHandler : HandlerBase<char>
    {
        public override char Read(Pointer<char> pointer)
        {
            return BitConverter.ToChar(Read(Pointer<object>.ProcessHandle, pointer.Address, pointer.Size), 0);
        }

        public override void Write(Pointer<char> pointer, char value)
        {
            Write(Pointer<object>.ProcessHandle, pointer.Address, BitConverter.GetBytes(value));
        }
    }

    public class HackProcess
    {
        public static IntPtr GetProcessHandle(string processname)
        {
            try
            {
                Process[] ProcList = Process.GetProcessesByName(processname);
                if(ProcList.Count() == 0)
                    throw new Exception("No process found");
                return ProcList.First().Handle;
            }
            catch (Exception e)
            {
                throw new Exception("Exception in getting the process handle: "+ e.Message);
            }
        }
    }
}
