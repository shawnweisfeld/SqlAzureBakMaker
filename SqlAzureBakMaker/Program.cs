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
            BackupMaker.CopyDatabase(
                BackupMaker.CreateConnection(parms.SourceServer, parms.SourceUser, parms.SourcePassword), parms.SourceDatabase,
                BackupMaker.CreateConnection(parms.DestinationServer, parms.DestinationUser, parms.DestinationPassword), parms.DestinationDatabase,
                true,
                parms.MdfFile,
                new SqlBackupInfo()
                {
                    StorageAccountName = parms.StorageAccountName,
                    StorageContainer = parms.StorageContainer,
                    StorageFile = $"{parms.StorageFileBase}-{DateTime.UtcNow:yyyy-MM-dd-HH-mm}.bak",
                    StorageKey = parms.StorageKey
                });
            sw.Stop();
            Console.WriteLine($"Done in {sw.Elapsed.TotalMinutes:N} minutes");
            Console.ReadKey();
        }
    }
}
