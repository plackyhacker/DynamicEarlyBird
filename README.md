<img width="1052" alt="Screenshot 2023-11-24 at 14 17 08" src="https://github.com/plackyhacker/DynamicEarlyBird/assets/42491100/1639cb43-8bd9-41e5-aef8-113dc6343106">

# Dynamic Early Bird
An example of a Dynamic Invoke shellcode injection technique using C#.

If you do use any of the code in these repositories **keep it legal!**

## Introduction

I have been researching different techniques on injecting shellcode into memory as part of a staged attack. It looked a bit like this:

**PowerShell cradle -> Disable PowerShell Logs -> AMSI Bypass -> Load .Net Harness -> Inject Stage 1 Shellcode**

I had generally been using P/Invoke, and occassionally Syscalls, but I wanted to try out Dynamic Invoke with obfuscation as part of my Anti-Malware evasion strategies.

### D/Invoke

D/Invoke is a technique used in offensive security, particularly within the context of the Windows platform. D/Invoke (Dynamic Invoke) is a library that allows us to dynamically invoke native Windows API functions directly from memory, avoiding the need for importing functions from DLLs at compile time.

I first read about D/Invoke On [The Wovers Blog](https://thewover.github.io/Dynamic-Invoke/). The code for the [DInvoke.cs](https://github.com/plackyhacker/DynamicEarlyBird/blob/main/DInvoke.cs) file was taken from [Red Team Cafe](https://www.redteam.cafe/red-team/shellcode-injection/process-hollowing-dinvoke). I am not sure of the origins of this code, if I find it I will update the reference.

### Earlybird

The Early Bird shellcode injection technique involves injecting malicious shellcode into a process's memory space before the process properly initialises or executes. This method allows the injected code to run early in the process's execution.

It aims to evade detection and increase the chances of successful exploitation by executing malicious actions at an early stage of process execution.

This exampe shows how we can use D/Invoke to start a new process in a suspended state, inject our shellcode and queue a User APC.

A User APC (Asynchronous Procedure Call) refers to a mechanism in Windows operating systems where a user-mode application can schedule a user-mode function or procedure to execute asynchronously within a specific thread context.

When the target thread enters an alertable state (when we resume the thread) it executes the User APC. We queue our User APC using the `QueueUserAPC` Win32 API:

### The Code

There are three classes implementing this example code:

[Program.cs](https://github.com/plackyhacker/DynamicEarlyBird/blob/main/Program.cs): This is the main program that calls the function pointers/delegates.

[DInvoke.cs](https://github.com/plackyhacker/DynamicEarlyBird/blob/main/DInvoke.cs): This is where the magic happens, and where we need to declare our delegates.

[Encrypt.cs](https://github.com/plackyhacker/DynamicEarlyBird/blob/main/Encrypt.cs): This is just a very basic XOR implementation, any encryption algorythm could be used.

**Enjoy!**
