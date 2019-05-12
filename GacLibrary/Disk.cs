using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;

namespace GAppCreator
{
    public class Disk
    {
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool CreateHardLink(string lpFileName,string lpExistingFileName,IntPtr lpSecurityAttributes);

        public static bool CreateFolder(string path,ErrorsContainer ec)
        {
            if (Directory.Exists(path))
                return true;
            try
            {
                Directory.CreateDirectory(path);
                return Directory.Exists(path);
            }
            catch (Exception e)
            {
                if (ec!=null)
                    ec.AddException("Unable to create folder: '" + path+"'", e);
                return false;
            }
        }
        public static bool SaveFile(string fileName, string content,ErrorsContainer ec)
        {
            if (CreateFolder(Path.GetDirectoryName(fileName),ec) == false)
                return false;
            try
            {
                File.WriteAllText(fileName, content);
                return File.Exists(fileName);
            }
            catch (Exception e)
            {
                if (ec!=null)
                    ec.AddException("Unable to save data to file: " + fileName, e);
                return false;
            }
        }
        public static bool SaveFile(string fileName, byte[] buffer, ErrorsContainer ec)
        {
            string path = Path.GetDirectoryName(fileName);
            if ((path!=null) && (path.Length>0))
            {
                if (CreateFolder(path, ec) == false)
                    return false;
            }
            try
            {
                File.WriteAllBytes(fileName, buffer);             
                return File.Exists(fileName);
            }
            catch (Exception e)
            {
                if (ec!=null)
                    ec.AddException("Unable to save data to file: " + fileName, e);
                return false;
            }
        }
        public static byte[] ReadFile(string fileName, ErrorsContainer ec)
        {
            try
            {
                byte[] data = File.ReadAllBytes(fileName);
                return data;
            }
            catch (Exception e)
            {
                if (ec != null)
                    ec.AddException("Unable to read data from file: " + fileName, e);
                return null;
            }
        }
        public static string ReadFileAsString(string fileName, ErrorsContainer ec)
        {
            try
            {
                string ss = File.ReadAllText(fileName);
                return ss;
            }
            catch (Exception e)
            {
                if (ec!=null)
                    ec.AddException("Unable to read string from file: " + fileName, e);
                return null;
            }
        }

        public static bool SaveImage(string fileName, Image img, ErrorsContainer ec)
        { 
                try
                {
                    img.Save(fileName);
                    if (File.Exists(fileName)==false)
                    {
                        ec.AddError("Internal error - image: " + fileName + " was not saved !");
                        return false;
                    }
                    return true;
                }
                catch (Exception e)
                {
                    if (ec != null)
                        ec.AddException("Unable to save local image to: \n" + fileName, e);
                    return false;
                }
        }

        public static bool CleanDirectory(string dirName, ErrorsContainer ec)
        {
            try
            {
                if (Directory.Exists(dirName))
                {
                    Directory.Delete(dirName, true);
                }
                Directory.CreateDirectory(dirName);
                return true;
            }
            catch (Exception e)
            {
                if (ec != null)
                    ec.AddException("Unable to clean directory: " + dirName, e);
                return false;
            }
        }
        public static bool DeleteDirectory(string dirName, ErrorsContainer ec)
        {
            try
            {
                if (Directory.Exists(dirName))
                {
                    Directory.Delete(dirName, true);
                }
                return true;
            }
            catch (Exception e)
            {
                if (ec != null)
                    ec.AddException("Unable to delete directory: " + dirName, e);
                return false;
            }
        }


        public static bool CreateHardLink(string hardLink, string original, ErrorsContainer ec)
        {
            bool res = CreateHardLink(hardLink, original, IntPtr.Zero);
            if (res == false)
            {
                if (ec != null)
                    ec.AddError("Unable to create hard link from '"+original+"' to '"+hardLink+"'");
            }
            return res;
        }
        public static bool DeleteFile(string fileName, ErrorsContainer ec)
        {
            if (File.Exists(fileName) == false)
                return true;
            try
            {
                File.SetAttributes(fileName, FileAttributes.Normal);
                File.Delete(fileName);
                if (File.Exists(fileName) == true)
                {
                    if (ec != null)
                        ec.AddError("Unable to delete file (unknown reason) : " + fileName);
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                if (ec != null)
                    ec.AddException("Unable to delete file : " + fileName, e);
                return false;
            }
        }
        public static bool MoveFile(string source,string dest, ErrorsContainer ec)
        {
            try
            {
                File.Move(source, dest);
                return true;
            }
            catch (Exception e)
            {
                if (ec != null)
                    ec.AddException("Unable to move file '" + source + "' to '" + dest + "'", e);
                return false;
            }
        }
        private static bool GetRecursiveAllFiles(string folder,List<string> files,ErrorsContainer ec)
        {
            try
            {
                string[] fs = null;
                fs = Directory.GetFiles(folder);
                if (fs != null)
                {
                    foreach (string s in fs)
                        files.Add(s);
                }
                fs = Directory.GetDirectories(folder);
                if (fs != null)
                {
                    foreach (string s in fs)
                    {
                        if (GetRecursiveAllFiles(s, files, ec) == false)
                            return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                if (ec != null)
                    ec.AddException("Unable to read files from  : " + folder, e);
                return false;
            }
        }
        public static bool GetAllFiles(string folder,bool recursive, List<string> files,ErrorsContainer ec)
        {
            if (recursive)
            {
                files.Clear();
                return GetRecursiveAllFiles(folder, files, ec);
            }
            try
            {
                string[] fs = null; 
                fs = Directory.GetFiles(folder);
                files.Clear();
                if (fs!=null)
                {
                    foreach (string s in fs)
                        files.Add(s);
                }
                return true;
            }
            catch (Exception e)
            {
                if (ec != null)
                    ec.AddException("Unable to read files from  : " + folder, e);
                return false;
            }
        }
        public static long GetFileSize(string fileName, ErrorsContainer ec)
        {
            try
            {
                FileInfo f = new FileInfo(fileName);
                return f.Length;
            }
            catch (Exception e)
            {
                if (ec != null)
                    ec.AddException("Unable to compute file size for: " + fileName, e);
                return 0;
            }
        }
        public static bool Copy(string source, string dest, ErrorsContainer ec)
        {
            try
            {
                File.Copy(source,dest,true);
                if (File.Exists(dest) == false)
                {
                    if (ec!=null)
                        ec.AddError("Unable to copy file (unknown reason) from '" + source + "' to '" + dest + "'");
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                if (ec!=null)
                    ec.AddException("Unable to copy file '" + source+"' to '"+dest+"'", e);
                return false;
            }
        }

        public static bool MoveFolder(string source,string dest, ErrorsContainer ec)
        {
            try
            {
                Directory.Move(source, dest);
                return true;
            }
            catch (Exception e)
            {
                if (ec != null)
                    ec.AddException("Unable to move folder '" + source + "' to '" + dest + "'", e);
                return false;
            }
        }
    }
}
