using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GAppCreator
{
    public class ErrorsContainer
    {
        public enum ErrorType
        {
            Error,
            Warning,
            Exception,
        };
        public class ErrorInfo
        {
            public ErrorType Type;
            public string Module = "";
            public string Error = "";
            public string Exception = "";
        }
        List<ErrorInfo> list = new List<ErrorInfo>();
        string defaultModule = "";
        public void Reset()
        {
            list.Clear();
            defaultModule = "";
        }
        public void SetDefaultModule(string name)
        {
            defaultModule = name;
        }
        public bool HasErrors()
        {
            return (list.Count > 0);
        }
        public void AddError(string module, string error)
        {
            ErrorInfo ei = new ErrorInfo();
            ei.Type = ErrorType.Error;
            ei.Module = module;
            ei.Error = error;
            list.Add(ei);
        }
        public void AddError( string error)
        {
            AddError(defaultModule, error);
        }
        public void AddWarning(string module, string error)
        {
            ErrorInfo ei = new ErrorInfo();
            ei.Type = ErrorType.Warning;
            ei.Module = module;
            ei.Error = error;
            list.Add(ei);
        }
        public void AddWarning(string error)
        {
            AddWarning(defaultModule, error);
        }
        public void AddException(string module, string error, Exception e)
        {
            ErrorInfo ei = new ErrorInfo();
            ei.Type = ErrorType.Exception;
            ei.Module = module;
            ei.Error = error;
            ei.Exception = e.ToString();
            list.Add(ei);
        }
        public void AddException(string error, Exception e)
        {
            AddException(defaultModule, error, e);
        }
        public int GetCount()
        {
            return list.Count;
        }
        public ErrorInfo Get(int id)
        {
            return list[id];
        }
    }
}
