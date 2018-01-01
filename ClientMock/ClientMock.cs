//////////////////////////////////////////////////////////////////////////////
// TestExecutive.cs - Runs all the Components of the Remote Build Server    //
//                  ver 1.0                                                 //
//--------------------------------------------------------------------------//
//	Author:			Amritbani Sondhi,										//
//					Graduate Student, Syracuse University					//
//					asondhi@syr.edu											//
//	Application:	CSE 681 Project #2, Fall 2017							//
//	Platform:		HP Envy x360, Core i7, Windows 10 Home					//
//  Environment:    C#, Visual Studio 2017 RC                               //
//////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * ===================
 * Generates the Test Requests and sends it to the Mock Repository.
 * The test requests contain information about the files which should be compiled
 * and executed as well as the dependency information between them.
 * 
 * Here, for Demo purpose we are generating TestRequest.txt files which are then sent to the Mock Repo
 * and then to the Build Server.
 *
 * Public Methods:
 * ==============
 *      Class ClientMock -
 *      - createBuildRequests()         : creates Build requests to be sent to the MockRepo
 *      - initializeRequestParameters() : initializes all the Request Parameters for the Build Request
 *      - sendBuildRequest()            : sends generated Build Requets files to the Mock Repo
 *      - triggerClient()               : triggers Client to generate the Build Requests and send it to the Mock Repo
 *      
 * Build Process:
 * ==============
 *	- Required Files:
 *          Client.cs
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

using HelpSession;
using System.IO;

namespace ClientMock
{
    public class ClientMock
    {
        // creates Build requests to be sent to the MockRepo
        public void createBuildRequests()
        {
            string savePath = "../../../Storage/MockClientStorage/TestRequests/";

            // checks if the Directory exists at Client, if not then create
            if (!System.IO.Directory.Exists(savePath))
                System.IO.Directory.CreateDirectory(savePath);

            /////////////////////////////////////////////////////////////////////////// For TestRequest1
            string fileName1 = "TestRequest1.txt";

            // constructing full path of the new BuildRequest file to be created
            string fileSpec1 = System.IO.Path.Combine(savePath, fileName1);
            fileSpec1 = System.IO.Path.GetFullPath(fileSpec1);

            // List of code files
            List<string> codeFiles1 = new List<string>();
            codeFiles1.Add("Demo1App1.cs");
            codeFiles1.Add("Demo2App1.cs");

            /////////////////////////////////////////////////////////////////////////// For TestRequest2
            string fileName2 = "TestRequest2.txt";

            // constructing full path of the new BuildRequest file to be created
            string fileSpec2 = System.IO.Path.Combine(savePath, fileName2);
            fileSpec2 = System.IO.Path.GetFullPath(fileSpec2);

            // List of code files
            List<string> codeFiles2 = new List<string>();
            codeFiles2.Add("Demo1App2.cs");
            codeFiles2.Add("Demo2App2.cs");

            /////////////////////////////////////////////////////////////////////////// For TestRequest3
            string fileName3 = "TestRequest3.txt";

            // constructing full path of the new BuildRequest file to be created
            string fileSpec3 = System.IO.Path.Combine(savePath, fileName3);
            fileSpec3 = System.IO.Path.GetFullPath(fileSpec3);

            // List of code files
            List<string> codeFiles3 = new List<string>();
            codeFiles3.Add("Demo1App3.cs");
            codeFiles3.Add("Demo2App3.cs");

            // initialize your parameters for all your Build Requests
            TestRequest tr1 = initializeRequestParameters("ProjectBuilder", "Client", "BuildRequest", "TestDriver1.cs", codeFiles1);
            TestRequest tr2 = initializeRequestParameters("ProjectBuilder", "Client", "BuildRequest", "TestDriver2.cs", codeFiles2);
            TestRequest tr3 = initializeRequestParameters("ProjectBuilder", "Client", "BuildRequest", "TestDriver3.cs", codeFiles3);

            tr1.makeRequest();
            tr2.makeRequest();
            tr3.makeRequest();

            // Saves an xml file on the ClientStorage
            Console.WriteLine("\n Saving Test Request to \"{0}\" \n", fileSpec1);
            tr1.saveXml(fileSpec1);

            Console.WriteLine(" Saving Test Request to \"{0}\" \n", fileSpec2);
            tr2.saveXml(fileSpec2);

            Console.WriteLine(" Saving Test Request to \"{0}\" \n", fileSpec3);
            tr3.saveXml(fileSpec3);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Created an extra 'Success' case Test Request to check if the exceptions and errors in the 
            // past TestRequests doesn't affect the Remote Build Server Federation
            // They do not affect the other TestRequests
        }

        // initializes all the Request Parameters for the Build Request
        public TestRequest initializeRequestParameters(string to, string from, string type, string testDriver, List<string> testFiles)
        {
            TestRequest tr = new TestRequest();

            // The Client passes the parameters and it gets initialized for the TestRequest class
            tr.toRequest = to;
            tr.fromRequest = from;
            tr.typeOfRequest = type;
            tr.testDriver = testDriver;

            foreach (string codeFile in testFiles)
                tr.testedFiles.Add(codeFile);

            return tr;
        }

        // sends generated Build Requets files to the Mock Repo
        public bool sendBuildRequest(string from, string to)
        {
            bool status = false;

            // checks if the Directory exists
            if (!Directory.Exists(from))
                Console.WriteLine("Invalid 'from' Directory. Please provide a valid Directory Name.");
            if (!Directory.Exists(to))
                Directory.CreateDirectory(to);

            List<string> files = new List<string>();

            // get all files into a List
            string[] tempFiles = Directory.GetFiles(from, "*.*");
            for (int i = 0; i < tempFiles.Length; ++i)
            {
                tempFiles[i] = Path.GetFullPath(tempFiles[i]);
            }
            files.AddRange(tempFiles);

            Console.WriteLine(" ----------------------------------------------------------------------------------------------");

            // print all the files in the list
            foreach (string f in files)
            {
                string fileSpec = f;
                Console.WriteLine("\n Sending \"{0}\" \n to \n \"{1}\"", fileSpec, to);
                string fileName = "";

                try
                {
                    // configure Destination path
                    fileName = Path.GetFileName(fileSpec);
                    string destSpec = Path.Combine(to, fileName);

                    File.Copy(fileSpec, destSpec, true);
                    status = true;
                }

                catch (Exception ex)
                {
                    Console.WriteLine(" --{0}--", ex.Message);
                    status = false;
                }

            }
            Console.WriteLine(" All files saved successfully");
            files.Clear();
            return status;
        }

        // triggers Client to generate the Build Requests and send it to the Mock Repo
        public void triggerClient()
        {
            Console.WriteLine("\n =========================================================================================");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(" Triggering Mock Client");
            Console.ResetColor();
            Console.WriteLine(" =========================================================================================");

            // Creates Build Requests and saves it to the RepositoryStorage
            // Here, we are creating 2 BuildRequests for now
            createBuildRequests();

            // Save Build requests from Client to Repository
            // Saves all the files contained in the MockClientStorage Folder
            string fromStorage = "../../../Storage/MockClientStorage/TestRequests";
            string toStorage = "../../../Storage/RepositoryStorage";

            string from = System.IO.Path.GetFullPath(fromStorage);
            string to = System.IO.Path.GetFullPath(toStorage);
            sendBuildRequest(from, to);

            Console.WriteLine("\n =========================================================================================");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(" Client functionality Completed!");
            Console.WriteLine(" The Client Created the Build Requests and Sent them to the Mock Repository");
            Console.ResetColor();
            Console.Write(" =========================================================================================");
        }

        static void Main(string[] args)
            {
                ClientMock client = new ClientMock();
                client.triggerClient();
                Console.ReadKey();
            }
    }

}
