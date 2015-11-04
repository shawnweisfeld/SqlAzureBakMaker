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
        public static ServerConnection CreateConnection(string instance, string name, string password)
        {
            return new ServerConnection(instance, name, password);
        }
        public static void CopyDatabase(
            ServerConnection sourceServerConnection, string sourceDatabaseName,
            ServerConnection destServerConnection, string destDatabaseName,
            bool includeData, SqlBackupInfo backupInfo)
        {
            //Set Source SQL Server (SQL Azure)
            Server sourceServer = new Server(sourceServerConnection);
            Database sourceDatabase = sourceServer.Databases[sourceDatabaseName];

            //Set Destination SQL Server (SQL IaaS)
            Server destServer = new Server(destServerConnection);
            Database destDatabase = null;

            //Drop the detination database if it exits
            if (destServer.Databases.Contains(destDatabaseName))
            {
                Console.Write($"Destintation DB {destDatabaseName} on {destServer.Name} Exists. Dropping.");
                destServer.KillDatabase(destDatabaseName);
                Console.WriteLine(" . . . Done!");
            }

            //create the temp database on SQL IaaS
            Console.Write($"Creating Destintation DB {destDatabaseName} on {destServer.Name}.");
            destDatabase = new Database(destServer, destDatabaseName);
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
            transfer.CopyData = includeData;

            transfer.DestinationServer = destServerConnection.ServerInstance;
            transfer.DestinationDatabase = destDatabaseName;

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
            credential.Create(backupInfo.StorageAccountName, backupInfo.StorageKey);
            Console.WriteLine(" Complete!");

            //Perform the backup from SQL IaaS to Azure Blob
            //https://msdn.microsoft.com/en-us/library/dn435916.aspx
            Console.WriteLine("Starting Backup...");
            Backup backup = new Backup();
            backup.Action = BackupActionType.Database;
            backup.Database = destDatabaseName;
            backup.Devices.Add(new BackupDeviceItem(backupInfo.StorageEndpoint, DeviceType.Url, "BackupCred"));
            backup.CredentialName = "BackupCred";
            backup.Incremental = false;
            backup.SqlBackup(destServer);
            Console.WriteLine("Backup Complete!");

        }
    }
}
