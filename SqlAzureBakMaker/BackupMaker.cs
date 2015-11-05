using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlAzureBakMaker
{
    public class BackupMaker
    {
        public static void CopyDatabase(MyArgs parms)
        {
            //remove trailing slash on the path if provided by user
            if (parms.PathToLocalMdf.EndsWith("\\"))
                parms.PathToLocalMdf = parms.PathToLocalMdf.Substring(0, parms.PathToLocalMdf.Length - 1);

            //Set Source SQL Server (SQL Azure)
            Server sourceServer = new Server(new ServerConnection(parms.SourceServer, parms.SourceUser, parms.SourcePassword));
            Database sourceDatabase = sourceServer.Databases[parms.SourceDatabase];

            //Set Destination SQL Server (SQL IaaS)
            Server destServer = new Server(new ServerConnection(parms.DestinationServer, parms.DestinationUser, parms.DestinationPassword));
            Database destDatabase = null;

            //Drop the detination database if it exits
            if (destServer.Databases.Contains(parms.DestinationDatabase))
            {
                Console.Write($"Destintation DB {parms.DestinationDatabase} on {destServer.Name} Exists. Dropping.");
                destServer.KillDatabase(parms.DestinationDatabase);
                Console.WriteLine(" . . . Done!");
            }

            //create the temp database on SQL IaaS
            Console.Write($"Creating Destintation DB {parms.DestinationDatabase} on {destServer.Name}.");
            destDatabase = new Database(destServer, parms.DestinationDatabase);

            var fg = new FileGroup(destDatabase, "PRIMARY");
            destDatabase.FileGroups.Add(fg);

            var df = new DataFile(fg, $"{parms.DestinationDatabase}_data");
            fg.Files.Add(df);
            df.FileName = $"{parms.PathToLocalMdf}\\{parms.DestinationDatabase}.mdf";
            df.IsPrimaryFile = true;
            df.Growth = 10;
            df.GrowthType = FileGrowthType.Percent;

            destDatabase.Create();
            Console.WriteLine(" . . . Done!");

            //Transfer the schema and data from SQL Azure to SQL IaaS
            Console.WriteLine("Starting Transfer...");
            Transfer transfer = new Transfer(sourceDatabase);
            transfer.DataTransferEvent += (object sender, DataTransferEventArgs e) =>
            {
                Console.WriteLine($"{e.DataTransferEventType}: {e.Message}");
            };

            transfer.CopyAllObjects = true;
            transfer.Options.WithDependencies = true;
            transfer.Options.Triggers = true;
            transfer.Options.Indexes = true;
            transfer.Options.ClusteredIndexes = true;
            transfer.Options.Default = true;
            transfer.Options.DriAll = true;
            transfer.CopyData = true;

            transfer.DestinationServer = parms.DestinationServer;
            transfer.DestinationDatabase = parms.DestinationDatabase;

            transfer.TransferData();

            Console.WriteLine("Transfer Complete!");

            //Create a backup credential in the SQL IaaS instance to perform the backup to Azure Blob
            Console.Write("Creating Backup Credential...");
            if (destServer.Credentials.Contains("BackupCred"))
            {
                Console.Write(" Dropping ");
                destServer.Credentials["BackupCred"].Drop();
            }

            Credential credential = new Credential(destServer, "BackupCred");
            credential.Create(parms.StorageAccountName, parms.StorageKey);
            Console.WriteLine(" Complete!");

            string storageEndpoint = $"https://{parms.StorageAccountName}.{parms.StorageEndpointBase}/{parms.StorageContainer}/{parms.StorageFileBase}-{DateTime.UtcNow:yyyy-MM-dd-HH-mm}.bak";

            //Perform the backup from SQL IaaS to Azure Blob
            //https://msdn.microsoft.com/en-us/library/dn435916.aspx
            Console.WriteLine("Starting Backup...");
            Backup backup = new Backup();
            backup.Action = BackupActionType.Database;
            backup.Database = parms.DestinationDatabase;
            backup.Devices.Add(new BackupDeviceItem(storageEndpoint, DeviceType.Url, "BackupCred"));
            backup.CredentialName = "BackupCred";
            backup.Incremental = false;
            backup.SqlBackup(destServer);
            Console.WriteLine("Backup Complete!");

        }
    }
}
