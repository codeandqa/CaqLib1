using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using System.Reflection;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using System.Configuration;

using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Safari;

//using Microsoft.Expression.Encoder.ScreenCapture;
using System.Drawing;
//using Microsoft.Expression.Encoder.Profiles;
//using Microsoft.Expression.Encoder;

namespace caqlib
{
    public sealed class LoggerInfo
    {
        public static int countOfError = 0;
        private static AutomationLogging _logger;
        public static AutomationLogging Instance
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new AutomationLogging();
                }

                return _logger;
            }
        }
    }
    public class AutomationLogging
    {
        public static string currentGuid = string.Empty;
        public static string logfileName = string.Empty;
        public static string resultdirectory = string.Empty;
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public static string _logFolder;
        public static string className = string.Empty;
        public static StringBuilder stringbuilder = null;
        public static List<string> list = new List<string>();
        public static int countOfError = 0;
        public static string newLocationInResultFolder = string.Empty;
        public AutomationLogging()
        {

            string resultFolder = @"C:/WDTF/TestResult";
            string asm = Assembly.GetCallingAssembly().FullName;
            string logFormat = string.Format("{0:yyyy-MM-dd-hh-mm-ss}", DateTime.Now);
            newLocationInResultFolder = resultFolder + "/" + currentGuid + "_" + logFormat;
            DirectoryInfo directoryInfo = new DirectoryInfo(newLocationInResultFolder);
            if (!directoryInfo.Exists)
            {
                System.IO.Directory.CreateDirectory(newLocationInResultFolder);
            }
            LoggingConfiguration config = new LoggingConfiguration();


            //===========================================================================================//             
            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget
            {
                Layout = "${time} | ${level}  | ${stacktrace::topFrames=2}|${message} "
            };
            config.AddTarget("console", consoleTarget);
            LoggingRule consoleInfo = new LoggingRule("*", NLog.LogLevel.Info, consoleTarget);
            config.LoggingRules.Add(consoleInfo);
            //===========================================================================================//
            FileTarget fileTarget = new FileTarget
            {
                Layout = "${time} | ${level}  | ${stacktrace:topFrames=2} | ${message} ",
                FileName = newLocationInResultFolder + "/" + className + "_" + logFormat + DateTime.Now.Second + ".log"
            };
            config.AddTarget("file", fileTarget);
            LoggingRule fileInfo = new LoggingRule("*", NLog.LogLevel.Info, fileTarget);
            config.LoggingRules.Add(fileInfo);
            //===========================================================================================//
            TraceTarget traceTarget = new TraceTarget
            {
                Layout = "${time} | ${level}  | ${stacktrace:topFrames=2} | ${message} "
            };

            //===========================================================================================//
            MailTarget mailTarget = new MailTarget
            {
                Name = "gmail",
                SmtpServer = "smtp.gmail.com",
                SmtpPort = 465,
                SmtpAuthentication = SmtpAuthenticationMode.Basic,
                SmtpUserName = "donethedeal@gmail.com",
                SmtpPassword = "passwd@123",
                EnableSsl = true,
                From = "donethedeal@gmail.com",
                To = "shahi.aditya@gmail.com",
                CC = ""
            };
            LoggingRule mailInfo = new LoggingRule("*", NLog.LogLevel.Info, mailTarget);
            config.LoggingRules.Add(mailInfo);

            //===========================================================================================//
            DatabaseTarget dbTarget = new DatabaseTarget();
            //===========================================================================================//

            // Step 4. Define rules
            LogManager.Configuration = config;
        }
        public void LogDebug(string Message)
        {
            logger.Debug(Message);
        }
        public void LogInfo(string Message)
        {
            logger.Info(Message);
        }
        public void LogWarning(string Message)
        {
            logger.Warn(Message);
        }
        public void LogAppErro(Exception e, string appLocation, NLog.LogLevel logLevel)
        {

            logger.Error("Error Message is: {0}", e.Message);
            countOfError += 1;
        }
        public void Message(string message)
        {
            logger.Info(message);
        }
        public void Email(string message)
        {
        }
    }
}

//https://www.eyecatch.no/blog/logging-with-log4net-in-c/ need to replace with log4net
