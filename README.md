# SqlAzureBakMaker
This project is designed to provide a simple framework to create a .BAK File from a SQL Azure database. 

##How it works
The project utilized a bit of SQL Server Management Objects (SMO) magic to automate the following steps:

1. Create a local database on SQL IaaS
2. Copy the schema from SQL Azure to SQL IaaS
3. Copy the data from SQL Azure to SQL IaaS
4. Create a backup of the SQL IaaS system to Azure Blob Storage

##Getting Started

1. (optional) Use Point in Time Restore to generate a copy of your transactional SQL Azure Database (https://azure.microsoft.com/en-us/blog/azure-sql-database-point-in-time-restore/)
2. Create a D series VM in the SAME Azure Region as your SQL Azure Database (Use an image that contains SQL Server already installed)
3. Open the port in the Azure SQL DB so that the VM can access
4. Create a local SQL Account on that box and turn on SQL Server Authentication  (will require a restart of the SQL Service)
5. Run this tool
6. Destroy the D series VM and the Point in Time restore (if you created one)

##Example command line to run tool
SqlAzureBakMaker.exe -SourceServer "" -SourceUser "" -SourcePassword "" -SourceDatabase "" -DestinationServer "." -DestinationUser "" -DestinationPassword "" -DestinationDatabase "" -StorageAccountName "" -StorageContainer "" -StorageFileBase "" -StorageKey "" -MdfFile ""
(fill in the stuff between the quotes)

##How it was tested
This project was tested against the sample AdventureWorks database that the Azure portal will inject into a new database for you. 

##Warrantee
This code is a sample, use at own risk. Please submit a pull request if you find a bug. 

## Copyright and License
- Copyright (c) Microsoft Corporation
- Released under the MIT License (MIT)
- Please see LICENSE for more information.
