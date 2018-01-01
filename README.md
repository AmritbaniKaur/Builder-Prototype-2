=============================================================================================
SMA, Fall 2017, Project 2- Remote Build Server
=============================================================================================
1. The Storage Folder Structure contains:
	- BuilderStorage : it can be completely erased except BuildLogs
		- RemoteBuildServer\bin\Debug\msbuild.log : log files will be saved here
	- MockClientStorage
		- TestRequests : it can be completely erased
	- RepositoryStorage : only the .txt(if present) files can be erased.
			The rest of the .cs and .xml files are needed for demonstrating the working of 
			the Remote Build Server
			Just in case, the test files get erased, a copy of the whole folder is kept at:
				\Project 2- Amritbani Sondhi\RemoteBuildServer\
	- TestHarnessStorage : can be completely erased except the TestLogs Folder
		- TestLogs : log files will be saved here