//////////////////////////////////////////////////////////////////////////////////
// TestHarnessMock.cs - executes the library files send from the Build Server   //
//                      ver 1.1                                                 //
//------------------------------------------------------------------------------//
//	Author:			Amritbani Sondhi,										    //
//					Graduate Student, Syracuse University					    //
//					asondhi@syr.edu											    //
//	Application:	CSE 681 Project #2, Fall 2017						    	//
//	Platform:		HP Envy x360, Core i7, Windows 10 Home					    //
//  Environment:    C#, Visual Studio 2017 RC                                   //
//////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * ===================
 * Receives dll files from the Build Server, executes the library files and generates logs
 * at \RemoteBuildServer\Storage\TestHarnessStorage\TestLogs
 *
 * Public Methods:
 * ==============
 *      Class TestHarnessMock -
 *      - triggerTestHarness()  : calls the execute method
 *      
 * Private Methods:
 * ==============
 *      Class TestHarnessMock -
 *      - getSpecificFiles()    : gets all the files specified in the path for the pattern mentioned
 *      - executeLibraries()    : used for executing the dll files and saving the logs to the testersLocation
 *      
 * Build Process:
 * ==============
 *	- Required Files:
 *          TestHarnessMock.cs, DllLoader.cs    
 * 	- Build commands:
 *		    devenv RemoteBuildServer.sln
 *		    
 * Maintenance History:
 * ===================
 *      ver 1.0 : 05 Oct, 2017
 *          - first release
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DllLoaderDemo;

namespace Federation
{
    public class TestHarnessMock
    {
        // gets all the files specified in the path for the pattern mentioned
        private List<string> getSpecificFiles(string path, string pattern)
        {
            if (pattern == "")
                pattern = "*.*";

            List<string> fileList = new List<string>();

            string[] tempFiles = Directory.GetFiles(path, pattern); // gets all the files using Directory
            for (int i = 0; i < tempFiles.Length; ++i)
            {
                tempFiles[i] = Path.GetFullPath(tempFiles[i]);      // replaces relative paths with the absolute paths
            }
            fileList.AddRange(tempFiles);

            return fileList;
        }

        // used for executing the dll files and saving the logs to the testersLocation
        [STAThread]
        private void executeLibraries()
        {
            // invoke loading the dlls for execution
            DllLoaderExec dllDemObj = new DllLoaderExec();

            // convert testers relative path to absolute path
            DllLoaderExec.testersLocation = Path.GetFullPath(DllLoaderExec.testersLocation);
            Console.Write(" Loading Test Modules from:\n\t {0}", DllLoaderExec.testersLocation);

            // run load and tests and saves the logs
            string result = dllDemObj.loadAndExerciseTesters();

            Console.WriteLine(" {0}", result);
        }

        // calls the execute method
        public void triggerTestHarness()
        {
            Console.WriteLine(" =========================================================================================");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(" Triggering Mock Test Harness!");
            Console.ResetColor();
            Console.WriteLine(" =========================================================================================");

            // Invoke loading the dlls and execute
            executeLibraries();

            Console.WriteLine(" =========================================================================================");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(" Test Harness functionality Completed!");
            Console.WriteLine(" Demonstrated: getting dll files from the Build Server ");
            Console.WriteLine("\t And executing .dll files and creating log files which are present at: ");
            Console.WriteLine("\t RemoteBuildServer --> Storage --> TestHarnessStorage --> TestLogs");
            Console.ResetColor();
            Console.WriteLine(" =========================================================================================");

        }

        static void Main(string[] args)
        {
            TestHarnessMock testHarnessObj = new TestHarnessMock();
            testHarnessObj.triggerTestHarness();
            Console.ReadKey();
        }

    }
}
