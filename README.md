# Foster
Demo for spawning processes under a specified parent PID in C#. Based heavily on the work by Didier Stevens: https://blog.didierstevens.com/2017/03/20/that-is-not-my-child-process/

If you have permissions, you can start a new process as a child process of another process. This can be valuable for evading threat-hunting mechanisms that check for suspicious parent-child relationships such as cmd.exe being a child of an unknown process.

