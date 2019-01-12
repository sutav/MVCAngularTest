using Serilog;
using System;

namespace MVCAngularTest.BusinessLogic.Log
{
    public class LogManager : ILogManager
    {
      
        private readonly ILogger _logger;
     
        public LogManager( ILogger logger)
        {
            
            _logger = logger;
       
        }   
   
 
        public void LogError(Exception ex, string message) {
            _logger.Error(ex, message);
        }
        public void LogError(Exception ex, string message, params object[] propertyValues)
        {
            _logger.Error(ex, message, propertyValues);
        }
        public void LogError(string message, params object[] propertyValues)
        {
            _logger.Error( message, propertyValues);
        }
        public void LogInfo(string message, params object[] propertyValues)
        {
            _logger.Information(message, propertyValues);
        }

      
    }
}