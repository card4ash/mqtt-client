using Elmah;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Logging
{
    public class Logger 
    {
        public static ILog _logger;
        static Logger()
        {
            var log4NetConfigDirectory = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var log4NetConfigFilePath = Path.Combine(log4NetConfigDirectory, "log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(log4NetConfigFilePath));
            //log4net.GlobalContext.Properties["LogFileName"] = @"D:\\file1";
            _logger = LogManager.GetLogger("");
        }

        public static void LogError(Exception ex, string contextualMessage = null)
        {
            try
            {

                if (contextualMessage != null)
                {

                    var annotatedException = new Exception(contextualMessage + " error type is " + ex.GetType().Name, ex);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(annotatedException);


                }
                else
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }

            }
            catch (Exception)
            {

            }
        }

        public void Log(string contextualMessage = null)
        {
            try
            {

                if (contextualMessage != null)
                {


                }
                else
                {
                    //connect log4 and write message.+contextualMessage
                }

            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to write log file by used log4.");
            }
        }




        public static void Fatal(string errorMessage)
        {
            if (_logger.IsFatalEnabled)
                _logger.Fatal(errorMessage);
        }

        public static void Error(string errorMessage)
        {
            if (_logger.IsErrorEnabled)
                _logger.Error(errorMessage);
        }

        public static void Warn(string message)
        {
            if (_logger.IsWarnEnabled)
                _logger.Warn(message);
        }

        public static void Info(string message)
        {
            if (_logger.IsInfoEnabled)
                _logger.Info(message);
        }

        public static void Debug(string message)
        {
            if (_logger.IsDebugEnabled)
                _logger.Debug(message);
        }
    }
}
