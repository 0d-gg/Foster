using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Foster
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Foster <program path> <parent process id>");
                return;
            }

            if (!Int32.TryParse(args[1], out int ppid))
            {
                Console.WriteLine("Invalid ppid");                
                return;
            }

            if (Process.GetProcessById(ppid) == null)
            {
                Console.WriteLine("No such pid");
                return;
            }

            try
            {
                var ret = Foster.StartProcessWithParent(args[0], "", ppid);
                if (ret == IntPtr.Zero)
                    Console.WriteLine("failed to start process.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problem starting process: " + ex.Message);
            }
        }
    }
}
