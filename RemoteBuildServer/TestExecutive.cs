//////////////////////////////////////////////////////////////////////////////
// TestExecutive.cs - Runs all the Components of the Remote Build Server    //
//                  ver 1.0                                                 //
//--------------------------------------------------------------------------//
//	Author:			Amritbani Sondhi,										//
//					Graduate Student, Syracuse University					//
//					asondhi@syr.edu											//
//	Application:	CSE 681 Project #2, Fall 2017							//
//	Platform:		HP Envy x360, Core i7, Windows 10 Home					//
//  Environment:     C#, Visual Studio 2017 RC                              //
//////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * ===================
 * Runs and calls all the Components of the Remote Build Server
 *
 * Public Methods:
 * ==============
 *      Class TestExecutive -
 *      - Main()  : calls the Rest of the Servers
 *      
 * Build Process:
 * ==============
 *	- Required Files:
 *          TestExecutive.cs, ClientMock.cs, RepoMock.cs, ProjectBuildServer.cs, TestHarnessMock.cs
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Federation;

namespace RemoteBuildServer
{
    class TestExecutive
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n =========================================================================================");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(" From RemoteBuildServer's Test Executive");
            Console.ResetColor();

            // Calls Client and gets it running
            ClientMock.ClientMock clientObj = new ClientMock.ClientMock();
            clientObj.triggerClient();

            // Calls Mock Repository and gets it running
            RepoMock repoObj = new RepoMock();
            repoObj.triggerRepo();

            // Calls Build Server and gets it running
            ProjectBuildServer buildServerObj = new ProjectBuildServer();
            buildServerObj.triggerBuild();

            // Calls Mock Test Harness and gets it running
            TestHarnessMock testHarnessObj = new TestHarnessMock();
            testHarnessObj.triggerTestHarness();

            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(" End of RemoteBuildServer's Test Executive");
            Console.ResetColor();
            Console.WriteLine(" =========================================================================================");

            Console.ReadKey();
        }
    }
}
