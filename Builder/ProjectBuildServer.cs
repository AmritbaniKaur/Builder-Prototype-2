//////////////////////////////////////////////////////////////////////////////////////
// ProjectBuildServer.cs :  builds projects using .csproj or .xml config files      //
//                          ver 2.0                                                 //
//----------------------------------------------------------------------------------//
//  Source:                 Ammar Salman, EECS Department, Syracuse University      //
//                          (313)-788-4694, hoplite.90@hotmail.com                  //
//	Author:			        Amritbani Sondhi,										//
//					        Graduate Student, Syracuse University					//
//					        asondhi@syr.edu											//
//	Application:	        CSE 681 Project #2, Fall 2017							//
//	Platform:		        HP Envy x360, Core i7, Windows 10 Home					//
//  Environment:            C#, Visual Studio 2017 RC                               //
//////////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * ===================
 * This package demonstrates parsing the .xml files and then building the respective code 
 * files according to the configurations provided in the build request files. 
 * The default config in the .xml file is set as Debug/AnyCPU
 * It then sends the generated .dll files to the Mock Test Harness for Execution
 * It also generates Logs on the Console window and creates a build log file
 * stored at \RemoteBuildServer\Storage\BuilderStorage\BuildLogs
 * 
 * Here, we have Hardcoded the TestRequest.xml files, as it is difficult to include the
 * dependencies between the files. For demo purposes, the TestRequest.txt files are sent from
 * the Mock Client to the Mock Repo to the Build Server to fulfill the requirements of the Project
 * 
 * Methods:
 * ==============
 *      Class TestHarnessMock -
 *      - ProjectBuildServer()      : checks if Directories are present or not
 *      - createLogFile()           : creates a Build Log File
 *      - BuildXml()                : build a .xml file
 *      - startBuild()              : triggers the Build Process from the xml file
 *      - getSpecificFiles()        : gets all the files specified in the path for the pattern mentioned
 *      - triggerBuild()            : triggers the Build Server to work
 *      - sendLibraryFiles()        : sends all the dll files created, to the TestHarness
 *      - sendFile()                : copies a file from source to destination
 *      
 * Build Process:
 * ==============
 *	- Required Files:
 *          ProjectBuildServer.cs
 * 	- Build commands:
 *		    devenv RemoteBuildServer.sln
 *		    
 * Maintenance History:
 * ===================
 *      ver 2.0 : 05 Oct, 2017
 */

using System;
using System.Collections.Generic;

using Microsoft.Build.BuildEngine;
using Microsoft.Build.Framework;
using Microsoft.Build.Execution;
using System.IO;
using System.Security;

namespace Federation
{
  public class ProjectBuildServer
  {
        /* 
         * This method uses MSBuild to build a .csproj file. The csproj file is configured to build as Debug/AnyCPU
         * Therefore, there is no need to specify the parameters here. This is useful for the build server because it should be as
         * general as it can get. The build server shouldn't have to specify different build parameters for each project. 
         * Instead, the csproj/xml file sets the configuration settings.
         * 
         * In the csproj file, the OutputPath is set to "csproj_Debug" for the Debug configuration, and "csproj_Release" for the
         * Release configuration. Moreover, if Debug was selected, the project will be build into an x86 library (DLL), while if Release
         * was selected, the project will build into an x64 executable (EXE)
         * 
         * To change the default configuration, the first PropertyGroup in the ..\..\..\files\Builder.csproj must be modified.
         */

        string storagePath = "../../../Storage/BuilderStorage";
        string toLibraryPath = "../../../Storage/TestHarnessStorage";

        public static string logLocation { get; set; } = "../../../Storage/BuilderStorage/BuildLogs/";
        public static string logFile { get; set; } = "BuildLog_";

        public int Count
        {
            get { return Count; }
            set { Count = 1; }
        }

        // checks if Directories are present or not
        public ProjectBuildServer()
        {
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);
            if (!Directory.Exists(toLibraryPath))
                Directory.CreateDirectory(toLibraryPath);
            if (!Directory.Exists(logLocation))
                Directory.CreateDirectory(logLocation);
        }

        // creates a Build Log File
        static void createLogFile()
        {
            // Create a LogFile Path
            logLocation = System.IO.Path.GetFullPath(logLocation);
            string dtTime = DateTime.Now.ToString();
            dtTime = dtTime.Replace(":", "-");
            dtTime = dtTime.Replace("/", "-");

            string logFileName = "BuildLog_";
            logFileName = logFileName + dtTime + ".txt";

            logFile = System.IO.Path.Combine(logLocation, logFileName);
        }

        /* 
         * This method uses MSBuild to build a .xml file. The xml file is configured to build as Debug/AnyCPU
         * In the xml file, the OutputPath is set to "\RemoteBuildServer\Storage\BuilderStorage" and 
         * will be build into a DLL library
         */
        static void BuildXml(string fileToBuild)
        {
            try
            {
                string projectFileName = fileToBuild;

                ConsoleLogger logger = new ConsoleLogger();

                Dictionary<string, string> GlobalProperty = new Dictionary<string, string>();
                BuildRequestData BuildRequest = new BuildRequestData(projectFileName, GlobalProperty, null, new string[] { "Rebuild" }, null);
                BuildParameters bp = new BuildParameters();
                bp.Loggers = new List<ILogger>{logger};
                BuildResult buildResult = BuildManager.DefaultBuildManager.Build(bp, BuildRequest);

                FileLogger fileLogger = new FileLogger();
                fileLogger.Parameters = logFile;
                fileLogger.Verbosity = LoggerVerbosity.Normal;

                BuildRequestData BuildLogRequest = new BuildRequestData(projectFileName, GlobalProperty, null, new string[] { "Rebuild" }, null);
                BuildParameters bpLog = new BuildParameters();
                bpLog.Loggers = new List<ILogger> { fileLogger };
                BuildResult buildLogResult = BuildManager.DefaultBuildManager.Build(bpLog, BuildLogRequest);

                string sourceLogMSBuild = "BuildLog_" + Path.GetFileName(fileToBuild).ToString() + ".txt";
                string destFile = logLocation.ToString() + sourceLogMSBuild.ToString();
                sourceLogMSBuild = System.IO.Path.GetFullPath(sourceLogMSBuild);

                // if File Exists then delete the old files
                if (File.Exists(destFile))
                    File.Delete(destFile);
                if (File.Exists(sourceLogMSBuild))
                    File.Delete(sourceLogMSBuild);

                System.IO.File.Move("msbuild.log", sourceLogMSBuild);

                File.Copy(sourceLogMSBuild, destFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // triggers the Build Process from the xml file
        static void startBuild(List<string> testRequestFileList)
        {
        // Build all the TestRequest xml files
        foreach (string file in testRequestFileList)
        {
            try
                {
                    Console.WriteLine("\n Building {0}", file);
                    BuildXml(file);
                    Console.WriteLine("\n --------------------------------------------------------------------------------------------------");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n An error occured while trying to build the xml file: {0}", file);
                    Console.WriteLine(" Details: {0}\n\n", ex.Message);
                }
            }
    }

        //gets all the files specified in the path for the pattern mentioned
        private List<string> getSpecificFiles(string path, string pattern)
        {
            List<string> fileList = new List<string>();

            string[] tempFiles = Directory.GetFiles(path, pattern);
            for (int i = 0; i < tempFiles.Length; ++i)
            {
                tempFiles[i] = Path.GetFullPath(tempFiles[i]);
            }
            fileList.AddRange(tempFiles);

            return fileList;
        }

        // gets all the files to compile, builds them, displays the logs stores them and
        // sends the files to the Test Harness
        public void triggerBuild()
        {
            Console.WriteLine("\n =========================================================================================");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(" Triggering ProjectBuildServer!");
            Console.ResetColor();
            Console.WriteLine(" =========================================================================================");

            List<string> buildRequestFileList = new List<string>();

            // get all the .xml files ie. BuildRequests with their full paths
            buildRequestFileList = getSpecificFiles(storagePath, "*.xml");

            // create a file for Saving Build Logs
            createLogFile();

            // Build all the TestRequest xml files
            startBuild(buildRequestFileList);

            sendLibraryFiles(storagePath, toLibraryPath);

            Console.WriteLine("\n =========================================================================================");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(" Project Build Server functionality Completed!");
            Console.WriteLine(" Demonstrated: building .cs files with .xml containing config details");
            Console.WriteLine(" \tAlso, it sent the .dll files to TestHarnessMock for executing them");
            Console.WriteLine("\n \tAnd Saved log files at: ");
            Console.WriteLine(" RemoteBuildServer --> Storage --> BuildStorage --> BuildLogs");
            Console.ResetColor();
            Console.WriteLine(" =========================================================================================");
        }

        // sends all the dll files created, to the TestHarness
        private void sendLibraryFiles(string fromPath, string toLibraryPath)
        {
            List<string> libraryFileList = new List<string>();
            string fromLibraryPath = Path.GetFullPath(fromPath);

            // get all the .dll files ie. library files with their full paths
            libraryFileList = getSpecificFiles(fromLibraryPath, "*.dll");

            // send the List of all dll files from ProjectBuildServer to the TestHarness
            Console.WriteLine("\n Sending the following files to \"{0}\"", toLibraryPath);
            foreach (string file in libraryFileList)
            {
                string fileSpec = file;
                string fileName = Path.GetFileName(fileSpec);
                Console.WriteLine("\t \"{0}\"", fileName);
                sendFile(fileSpec);
            }
        }

        // copies a file from source to destination
        private bool sendFile(string fileSpec)
        {
            try
            {
                string fileName = Path.GetFileName(fileSpec);
                string destSpec = Path.Combine(toLibraryPath, fileName);
                File.Copy(fileSpec, destSpec, true);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n--{0}--", ex.Message);
                return false;
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine(" Building project Build Server");
            Console.WriteLine(" ======================");
            Console.ReadLine();
        }
  }

}
