using System;
using System.Collections.Generic;
using System.Text;

namespace MyJob1.Interfaces
{
    public interface IAppLogger<T>
    {
        void Info(string message, params object[] args);
        void Error(Exception ex, string message, params object[] args);
    }
}
