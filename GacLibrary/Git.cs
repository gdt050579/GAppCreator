using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GAppCreator
{
    public class Git
    {
        public enum RepoFileStatus
        {
            UpToDate,
            Modified,
            Deleted,
            Added,
            Ignored,
            Unversioned,
            Unknown,
        };
        private static string gitExecutable = "git.exe";
        private string gitFolder = "";
        private string error = "";
        private string output = "";
        private string errorOutput = "";
        private string[] ExecuteCommandAndGetOutput(string gitParams, string workingDirectory = null, bool ignoreStdError = true,bool showWindow = false)
        {
            Process executionProcess;
            List<string> l = new List<string>();
            error = "";
            output = "";
            errorOutput = "";
            if (workingDirectory == null)
                workingDirectory = gitFolder;

            if (showWindow)
            {
                try
                {
                    executionProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = "/K \""+gitExecutable+"\" "+gitParams,
                            UseShellExecute = false,
                            CreateNoWindow = false,
                            WorkingDirectory = workingDirectory,
                        }
                    };
                    executionProcess.Start();
                    executionProcess.WaitForExit();
                    return l.ToArray();
                }
                catch (Exception exc)
                {
                    error = exc.ToString();
                    return null;
                }
            }

            try
            {
                executionProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = gitExecutable,
                        Arguments = gitParams,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        WorkingDirectory = workingDirectory,
                    }
                };
                AutoResetEvent outputWaitHandle = new AutoResetEvent(false);
                AutoResetEvent errorWaitHandle = new AutoResetEvent(false);
                executionProcess.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        outputWaitHandle.Set();
                    }
                    else
                    {
                        l.Add(e.Data);
                        output += e.Data;
                        output += "\n";
                    }
                };
                executionProcess.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        errorWaitHandle.Set();
                    }
                    else
                    {
                        errorOutput += e.Data;
                        errorOutput += "\n";
                    }
                };
                executionProcess.Start();
                executionProcess.BeginOutputReadLine();
                executionProcess.BeginErrorReadLine();

                while ((executionProcess.HasExited == false))
                {
                    outputWaitHandle.WaitOne(1000);
                    errorWaitHandle.WaitOne(1000);
                }
                if (executionProcess.HasExited == false)
                    executionProcess.Kill();
                // verific erori sigur
                bool isError = false;
                if (errorOutput.ToLower().Contains("\nfatal:"))
                    isError = true;
                if (((errorOutput.Length > 0) && (ignoreStdError == false)) || (isError))
                {
                    error = errorOutput;
                    return null;
                }
                return l.ToArray();
            }
            catch (Exception exc)
            {
                error = exc.ToString();
                return null;
            }
        }
        private string ConvertForUserPassHTTPS(string s)
        {
            string ss = "";
            foreach (char c in s)
            {
                ss += "%" + ((int)c).ToString("x2");
            }
            return ss;
        }
        private string CreateURLWithCredentials(string remoteAddress, string userName, string password)
        {
            if (remoteAddress.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase) == false)
            {
                error = String.Format("Invalid remote address: '{0}' - can not create credential URL for username: '{1}' !", remoteAddress, userName, password);
                return null;
            }
            return "https://" + ConvertForUserPassHTTPS(userName) + ":" + ConvertForUserPassHTTPS(password) + "@" + remoteAddress.Substring(8);
        }
        private string CreateURLWithCredentials(string userName, string password)
        {
            return CreateURLWithCredentials(GetRemoteAddress(), userName, password);
        }
        public static bool IsFolderAGitFolder(string folder)
        {
            if ((folder == null) || (folder.Length == 0))
                return false;
            return (Directory.Exists(Path.Combine(folder, ".git")));
        }
        public static void SetGitExecutable(string gitExecutablePath)
        {
            gitExecutable = gitExecutablePath;
        }
        public string GetLastError()
        {
            return error;
        }
        public string GetLastCommandOutput()
        {
            return output;
        }
        public bool IsAvailable()
        {
            return Git.IsFolderAGitFolder(gitFolder);
        }
        public bool SetGitFolder(string folder)
        {
            if (Git.IsFolderAGitFolder(folder))
            {
                gitFolder = folder;
                return true;
            }
            error = String.Format("Folder '{0}' is not a git folder !", folder);
            return false;
        }
        public string[] GetVersionedFiles()
        {
            if (!IsAvailable())
            {
                error = String.Format("Please set the git folder first !");
                return null;
            }
            return ExecuteCommandAndGetOutput("ls-tree --name-status -r HEAD");
        }
        public bool AddFiles(string[] fileList)
        {
            if (!IsAvailable())
            {
                error = String.Format("Please set the git folder first !");
                return false;
            }
            if (fileList == null)
            {
                error = String.Format("Expecting a non-empty file list");
                return false;
            }
            if (fileList.Length == 0)
            {
                error = String.Format("Expecting a non-empty file list");
                return false;
            }
            string file_list = "";
            foreach (string fname in fileList)
            {
                if (File.Exists(fname) == false)
                {
                    error = String.Format("File {0} does not exist !", fname);
                    return false;
                }
                if (fname.StartsWith(gitFolder, StringComparison.InvariantCultureIgnoreCase) == false)
                {
                    error = String.Format("File {0} should be in current git folder '{1}'", fname, gitFolder);
                    return false;
                }
                file_list += " \"" + fname + "\" ";
                if (file_list.Length>8192)
                {
                    ExecuteCommandAndGetOutput("add " + file_list);
                    if (error.Length > 0)
                        return false;
                    file_list = "";
                }
            }
            if (file_list.Length > 0)
            {
                ExecuteCommandAndGetOutput("add " + file_list);
                if (error.Length > 0)
                    return false;
            }
            return true;
        }
        public bool AddFile(string filePath)
        {
            return AddFiles(new string[] { filePath });
        }
        public bool Commit(string message)
        {
            if (!IsAvailable())
            {
                error = String.Format("Please set the git folder first !");
                return false;
            }
            message = message.Replace("\"", "'");
            string[] l = ExecuteCommandAndGetOutput("commit . -m \"" + message + "\"");
            if (error.Length > 0)
                return false;
            return true;
        }
        public bool CreateRepo(string folder)
        {
            if (folder.Length == 0)
            {
                error = "Please specify the full path of a folder !";
                return false;
            }
            if (folder.ToLower().Equals(gitFolder.ToLower()))
                return true;
            if ((Git.IsFolderAGitFolder(folder)) == true)
            {
                error = String.Format("Folder '{0}' as already a git folder !", folder);
                return false;
            }
            string[] l = ExecuteCommandAndGetOutput("init", folder);
            if (error.Length > 0)
                return false;
            if ((Git.IsFolderAGitFolder(folder)) == false)
            {
                error = String.Format("Unable to add folder '{0}' to a git Repo !", folder);
                return false;
            }
            return SetGitFolder(folder);
        }
        public string GetRemoteAddress()
        {
            if (!IsAvailable())
            {
                error = String.Format("Please set the git folder first !");
                return null;
            }
            string[] l = ExecuteCommandAndGetOutput("config --get remote.origin.url");
            if (error.Length > 0)
                return null;
            if (l.Length == 0)
                return "";
            foreach (string s in l)
                if (s.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                    return s;
            return "";
        }
        public bool SetRemoteAddress(string remoteAddress)
        {
            if (!IsAvailable())
            {
                error = String.Format("Please set the git folder first !");
                return false;
            }
            string ra = GetRemoteAddress();
            if (ra == null)
                return false;
            if (ra.Length > 0)
            {
                error = String.Format("Remote address was already set to : '{0}' !", ra);
                return false;
            }
            remoteAddress = remoteAddress.Trim();
            if (remoteAddress.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase) == false)
            {
                error = String.Format("Expecting a valid URL (http or https) and not '{0}' !", remoteAddress);
                return false;
            }
            string[] l = ExecuteCommandAndGetOutput("remote add origin \"" + remoteAddress + "\" ");
            if (error.Length > 0)
                return false;
            // verific daca s-a setata
            ra = GetRemoteAddress();
            if (ra.Trim().ToLower().Equals(remoteAddress.ToLower()) == true)
                return true;
            error = String.Format("Failed to set remote address - currenta address set to '{0}' !", ra);
            return false;
        }
        public Dictionary<string, RepoFileStatus> GetStatus(bool addUpToDateFiles)
        {
            if (!IsAvailable())
            {
                error = String.Format("Please set the git folder first !");
                return null;
            }
            string[] l = ExecuteCommandAndGetOutput("status --porcelain");
            if (error.Length > 0)
                return null;
            Dictionary<string, RepoFileStatus> d = new Dictionary<string, RepoFileStatus>();
            // parsez listele de la status
            foreach (string info in l)
            {
                if (info.Length < 3)
                    continue;
                string fpath = info.Substring(3).Trim().ToLower().Replace("\\", "/");
                RepoFileStatus r = RepoFileStatus.Unknown;
                if (info[0] == 'A')
                    r = RepoFileStatus.Added;
                else if ((info[0] == 'M') || (info.StartsWith(" M")))
                    r = RepoFileStatus.Modified;
                else if ((info[0] == 'D') || (info[1] == 'D'))
                    r = RepoFileStatus.Deleted;
                else if (info[0] == '!')
                    r = RepoFileStatus.Ignored;
                else if (info[0] == '?')
                    r = RepoFileStatus.Unversioned;
                else
                    r = RepoFileStatus.Unknown;
                d[fpath] = r;
            }
            if (addUpToDateFiles)
            {
                string[] allFiles = GetVersionedFiles();
                if (error.Length > 0)
                    return null;
                foreach (string fname in allFiles)
                {
                    string ffn = fname.Trim().ToLower().Replace("\\","/");
                    if (d.ContainsKey(ffn) == false)
                        d[ffn] = RepoFileStatus.UpToDate;
                }
            }
            // all si good
            return d;
        }
        public bool Push(string userName, string password,bool showWindow)
        {
            if (!IsAvailable())
            {
                error = String.Format("Please set the git folder first !");
                return false;
            }
            string addr = CreateURLWithCredentials(userName, password);
            if (addr == null)
                return false;
            string[] l = ExecuteCommandAndGetOutput("push " + addr + " --porcelain ",null,true,showWindow); //-u origin master 
            if (error.Length > 0)
                return false;
            return true;
        }
        public bool Pull(string userName, string password)
        {
            if (!IsAvailable())
            {
                error = String.Format("Please set the git folder first !");
                return false;
            }
            string addr = CreateURLWithCredentials(userName, password);
            if (addr == null)
                return false;
            string[] l = ExecuteCommandAndGetOutput("pull " + addr + "", null, true);
            if (error.Length > 0)
                return false;
            return true;
        }
        public bool Clone(string remoteAddress, string userName, string password, string folder)
        {
            if (CreateRepo(folder) == false)
                return false;
            if (SetRemoteAddress(remoteAddress) == false)
                return false;
            if (Pull(userName, password) == false)
                return false;
            // presupunand ca totul e ok ar trebui sa il pot seta ca si folder nou
            return SetGitFolder(folder);
        }

    }
}
