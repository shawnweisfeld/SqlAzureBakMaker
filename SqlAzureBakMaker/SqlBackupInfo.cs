using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlAzureBakMaker
{
    public class SqlBackupInfo
    {
        public SqlBackupInfo()
        {
            this.StorageEndpointDomain = "blob.core.windows.net";
        }

        /// <summary>
        /// Target Storage account for the backup (just the name not the domain)
        /// </summary>
        public string StorageAccountName { get; set; }

        /// <summary>
        /// Target Storage account container for the backup
        /// </summary>
        public string StorageContainer { get; set; }

        /// <summary>
        /// The file name for the backup (include the .bak part)
        /// </summary>
        public string StorageFile { get; set; }

        /// <summary>
        /// Only set if you are NOT using Azure Commercial (blob.core.windows.net)
        /// </summary>
        public string StorageEndpointDomain { get; set; }

        /// <summary>
        /// The key for the storage account
        /// </summary>
        public string StorageKey { get; set; }

        public string StorageEndpoint { get { return $"https://{this.StorageAccountName}.{this.StorageEndpointDomain}/{this.StorageContainer}/{this.StorageFile}"; } }

    }
}
