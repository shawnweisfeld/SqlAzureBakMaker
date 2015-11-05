using PowerArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlAzureBakMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            var parms = Args.Parse<MyArgs>(args);

            Console.WriteLine("Starting");
            var sw = new Stopwatch();
            sw.Start();
            BackupMaker.CopyDatabase(parms);
            sw.Stop();
            Console.WriteLine($"Done in {sw.Elapsed.TotalMinutes:N} minutes");
            Console.ReadKey();
        }
    }
}
