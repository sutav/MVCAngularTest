using System;

namespace MVCAngularTest.BusinessLogic.Log
{
    public interface ILogManager
    {        
        void LogError(Exception ex, string message);
        void LogError(Exception ex, string message, params object[] propertyValues);
        void LogError(string message, params object[] propertyValues);
        void LogInfo(string message, params object[] propertyValues);        
    }
}
