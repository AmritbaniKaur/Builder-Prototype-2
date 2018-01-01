//////////////////////////////////////////////////////////////////////////////
// RepoMock.cs -    Demonstrate a few mock repo operations                  //
//                  ver 2.0                                                 //
//--------------------------------------------------------------------------//
//  Source:         Prof. Jim Fawcett, CST 4-187, jfawcett@twcny.rr.com     //
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
 *      Simulates basic Repository operations. The Repository is supposed to
 *      get BuildRequests from Client. Then it should send the code files and
 *      build requests to the Build Server for Compilation
 * 
 * Public Methods:
 * ==============
 *      Class RepoMock -
 *      - RepoMock()        : initializes RepoMock Storage
 *      - getFiles()        : find all the files in RepoMock.storagePath
 *      - sendFile()        : copy file to RepoMock.receivePath
 *      - triggerRepo()     : triggers the repository to start it's functions and send files to the Build Server
 *      
 * Private Methods:
 * ==============
 *      Class RepoMock -
 *      - getFilesHelper()  : private helper function for RepoMock.getFiles
 *      
 * Build Process:
 * ==============
 *	- Required Files:
 *          RepoMock.cs
 * 	- Build commands:
 *		    devenv RemoteBuildServer.sln
 *		    
 * Maintenance History:
 * ===================
 *      ver 1.0 : 07 Sep 2017
 *          - first release
 *      ver 2.0 : 05 Oct, 2017
 *      
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Federation
{
    ///////////////////////////////////////////////////////////////////
    // RepoMock class
    // - begins to simulate basic Repo operations

    public class RepoMock
    {
        public string storagePath { get; set; } = "../../../Storage/RepositoryStorage";
        public string receivePath { get; set; } = "../../../Storage/BuilderStorage";
        public List<string> files { get; set; } = new List<string>();

        /*----< initialize RepoMock Storage>---------------------------*/

        public RepoMock()
        {
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);
            if (!Directory.Exists(receivePath))
                Directory.CreateDirectory(receivePath);
        }
        /*----< private helper function for RepoMock.getFiles >--------*/

        private void getFilesHelper(string path, string pattern)
        {
            string[] tempFiles = Directory.GetFiles(path, pattern);
            for (int i = 0; i < tempFiles.Length; ++i)
            {
                tempFiles[i] = Path.GetFullPath(tempFiles[i]);
            }
            files.AddRange(tempFiles);

            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                getFilesHelper(dir, pattern);
            }
        }
        /*----< find all the files in RepoMock.storagePath >-----------*/
        /*
        *  Finds all the files, matching pattern, in the entire 
        *  directory tree rooted at repo.storagePath.
        */
        public void getFiles(string pattern)
        {
            files.Clear();
            getFilesHelper(storagePath, pattern);
        }

        /*---< copy file to RepoMock.receivePath >---------------------*/
        /*
        *  Will overwrite file if it exists. 
        */
        public bool sendFile(string fileSpec)
        {
            try
            {
                string fileName = Path.GetFileName(fileSpec);
                string destSpec = Path.Combine(receivePath, fileName);
                File.Copy(fileSpec, destSpec, true);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n--{0}--", ex.Message);
                return false;
            }
        }

        // triggers the repository to start it's functions and send files to the Build Server
        public void triggerRepo()
        {
            Console.WriteLine("\n =========================================================================================");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(" Triggering Mock Repository!");
            Console.ResetColor();
            Console.WriteLine(" =========================================================================================");

            string fileSpec = "";
            string fileName = "";
            // populates the files property
            getFiles("*.*");

            Console.WriteLine(" TestRequests.xml are the Request Files which were already present in the Mock Repository");
            Console.WriteLine(" They contain hardcoded configuration details, without which the code won't be able to compile");
            Console.WriteLine(" and, TestRequests.txt which are newly created and sent from the Client do not ");
            Console.WriteLine(" configuration details");

            Console.WriteLine(" Sending the following files to \"{0}\" \n", receivePath);

            // send the List of all the files from Repository to the Build Server
            foreach (string file in files)
            {
                fileSpec = file;
                fileName = Path.GetFileName(fileSpec);
                Console.WriteLine("\t \"{0}\"", fileName);
                sendFile(fileSpec);
            }

            Console.WriteLine("\n =========================================================================================");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(" Mock Repo functionality Completed!");
            Console.WriteLine(" The Mock Repository sent all the code files and the Build requests to the Build Server");
            Console.ResetColor();
            Console.Write(" =========================================================================================");

        }

        static void Main(string[] args)
        {
            RepoMock repo = new RepoMock();
            repo.triggerRepo();
            Console.ReadKey();
        }
    }
}
