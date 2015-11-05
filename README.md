# SqlAzureBakMaker
This project is designed to provide a simple framework to create a .BAK File from a SQL Azure database. 

##How it works
The project utilized a bit of SQL Server Management Objects (SMO) magic to automate the following steps:

1. Create a local database on SQL IaaS
2. Copy the schema from SQL Azure to SQL IaaS
3. Copy the data from SQL Azure to SQL IaaS
4. Create a backup of the SQL IaaS system to Azure Blob Storage

##Getting Started

###Setup
1. (optional) Use Point in Time Restore to generate a copy of your transactional SQL Azure Database (https://azure.microsoft.com/en-us/blog/azure-sql-database-point-in-time-restore/)
  * While this step is optional, using a restored copy of the soruce database will ensure that the source system is not changing during the processes and reduce the impact that this process will have on your source system.  
2. Create a D series VM in the SAME Azure Region as your SQL Azure Database
  * (recommended) Use an image that contains SQL Server already installed
3. Open the port in the Azure SQL DB so that the VM can access
4. Create a local SQL Account on your VM and turn on SQL Server Authentication
  * This will require a restart of the SQL Service
5. Using SSMS on the VM test that you can log into the SQL Azure Database and the Local SQL Server

###Run this tool
1. Download the source from this repo, compile it
2. Execute the following command
SqlAzureBakMaker.exe -SourceServer "" -SourceUser "" -SourcePassword "" -SourceDatabase "" -DestinationServer "." -DestinationUser "" -DestinationPassword "" -DestinationDatabase "" -StorageAccountName "" -StorageContainer "" -StorageFileBase "" -StorageKey "" -MdfFile ""

  * -SourceServer: The full server name for the source SQL Azure DB (ex. something.database.windows.net,1433)
  * -SourceUser: The username for the source SQL Azure DB
  * -SourcePassword: The password for the soruce SQL Azure DB
  * -SourceDatabase: The database name for the source SQL Azure DB
  * -DestinationServer: The destination server (this will typically be the local SQL IaaS instance that you are creating a temporary copy of your database on, this DB will be used to create the .bak file, so you can just use a ".")
  * -DestinationUser: The username for the destination server 
  * -DestinationPassword: The password for the destination server
  * -DestinationDatabase: The database name you want to use on the destination server
    * CAUTION: If this database already exits it will be dropped!
  * -StorageAccountName: This is the name of the storage account (note: just the name not the full domain)
  * -StorageContainer: This is the name of the conatiner in the storage account you want to put the .bak file in
  * -StorageFileBase: this will be the prefix used for the .bak file, appended to the end will be the date the bak was created
  * -StorageKey: the key for the storage account
  * -MdfFile: the full path and name to where you want to put the MDF file on the IaaS VM (NOTE: put this somewhere on the D drive if you are using a D Series VM since it is an SSD)

###Cleanup
1. Destroy the D series VM and the Point in Time restored SQL Azure DB (if you created one)

##How it was tested
This project was tested against the sample AdventureWorks database that the Azure portal will inject into a new database for you. 

##Warrantee
This code is a sample, use at own risk. Please submit a pull request if you find a bug. 

## Copyright and License
- Copyright (c) Microsoft Corporation
- Released under the MIT License (MIT)
- Please see LICENSE for more information.
