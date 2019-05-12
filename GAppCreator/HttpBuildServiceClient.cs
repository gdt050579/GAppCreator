using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace GAppCreator
{
    public class HttpBuildServiceClient
    {
        private WebClient Client;
        private Project prj;
        private byte[] result;
        private string resultString;
          
        
        public HttpBuildServiceClient(Project proj)
        {
            prj = proj;
            Client = new WebClient();
            /*
            Client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
            Client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(Client_DownloadFileCompleted);
            Client.UploadProgressChanged += new UploadProgressChangedEventHandler(Client_UploadProgressChanged);
            Client.UploadFileCompleted += new UploadFileCompletedEventHandler(Client_UploadFileCompleted);
             */
        }
        /*
        void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            paralelOpDone = true;
        }

        void Client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            result = e.Result;
            paralelOpDone = true;
        }

        void Client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            if (Callback!=null)
                Callback.Update(e.ProgressPercentage);            
        }
        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (Callback != null)
                Callback.Update(e.ProgressPercentage);
        }
        */
        private string GetServiceURL(string command,Dictionary<string,string> parameters)
        {
            string serviceURL;
            serviceURL = prj.Settings.BuildServerAddress;
            if (serviceURL.EndsWith("/") == false)
                serviceURL += "/";
            serviceURL += "service.php?user=" + prj.Settings.UserName;
            serviceURL += "&cmd=" + command;
            if (parameters != null)
            {
                foreach (string key in parameters.Keys)
                    serviceURL += "&" + key + "=" + parameters[key];
            }
            return serviceURL;
        }
        private Uri GetServiceUri(string command, Dictionary<string, string> parameters)
        {
            return new Uri(GetServiceURL(command, parameters));
        }
        private void ClearState()
        {
            result = null;
            resultString = "";
        }
        private void UpdateError(string value,string text)
        {
            if (value == null)
            {
                prj.EC.AddError("Internal server error. Error code was incorectly formated !\n"+text);
                return;
            }
            if (text != null)
            {
                text = text.Replace("\r", "");
                text = text.Replace("\\n", "\n");
                text = text.Replace("\n", "\r\n");
            }
            switch (value)
            {
                case "1": prj.EC.AddError("Invalid connection method. Please update your GACCreator !\n"+text); break;
                case "2": prj.EC.AddError("Invalid user name. Your user is not registered on the server !\n" + text); break;
                case "3": prj.EC.AddError("Unknown command request. Check if you have the latest version of GACCreator !\n" + text); break;
                case "4": prj.EC.AddError("Internal server error. Not files were specified for upload!\n" + text); break;
                case "5": prj.EC.AddError("Internal server error. Error uploading files ...!\n" + text); break;
                case "6": prj.EC.AddError("Internal server error. Upload package is too big !\n" + text); break;
                case "7": prj.EC.AddError("There is already an existing build running for the current project.\n" + text); break;
                case "8": prj.EC.AddError("Builder execution error\n" + text); break;
                case "9": prj.EC.AddError("Unable to create server folder.\n" + text); break;
                case "14": prj.EC.AddError("Missing update file from the server.\n" + text); break;
                default: prj.EC.AddError("Unknown error: "+value+"\n"+text); break;
            }
        }
        private void PreprocessResult()
        {
            if (result != null)
            {
                resultString = "";
                if (result.Length < 0x10000)
                {
                    resultString = System.Text.Encoding.Default.GetString(result);
                    if (resultString.StartsWith("[ERROR_CODE_"))
                    {
                        if (resultString.Contains(']'))
                        {
                            int index = resultString.IndexOf(']');
                            UpdateError(resultString.Substring(12, index - 12),resultString.Substring(index+1).Trim());
                        }
                        else
                        {
                            UpdateError(null, resultString);
                        }
                    }
                }
            }
        }
        public bool Query(string command,Dictionary<string,string> parameters)
        {
            ClearState();
            try
            {
                result = Client.DownloadData(GetServiceURL(command, parameters));
                if (result == null)
                {
                    prj.EC.AddError("Invalid result (NULL) when qurying command: " + command);
                    return false;
                }
                PreprocessResult();
                return !prj.EC.HasErrors();
            }
            catch (Exception e)
            {
                prj.EC.AddException("Unable to query command: " + command, e);
                return false;
            }
        }
        public bool Upload(string command, string fileName,Dictionary<string, string> parameters)
        {
            ClearState();
            try
            {
                result = Client.UploadFile(GetServiceURL(command, parameters),fileName.ToLower());
                //paralelOpDone = false;
                //Client.UploadFileAsync(GetServiceUri(command, parameters), fileName);
                //while (paralelOpDone) { System.Threading.Thread.Sleep(1000); }

                
                
                if (result == null)
                {
                    prj.EC.AddError("Invalid result (NULL) when uploading file '"+fileName+"' with command: " + command);
                    return false;
                }
                PreprocessResult();
                return !prj.EC.HasErrors();
            }
            catch (Exception e)
            {
                prj.EC.AddException("Unable to upload file '"+fileName+"' with command: " + command, e);
                return false;
            }
        }
        public byte[] GetResult()
        {
            return result;
        }
        public string GetStringResult()
        {
            return resultString;
        }
    }
}
