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
    public enum LocatorType
    {
        ClassName,
        CssSelector,
        Id,
        LinkText,
        Name,
        PartialLinkText,
        TagName,
        XPath
    };
    enum OutputLogType
    {
        Email,
        ColoredConsole,
        Console,
        Database,
        HTMLfile,
        Log
    }
    public enum MouseEvent
    {
        click,
        contextmenu,
        dblclick,
        mousedown,
        mousemove,
        mouseout,
        mouseover,
        mouseup,
        mousewheel
    };
    public enum Eresult
    {
        TRUE,
        FALSE
    };
    
    
    public sealed class TestEnvInfo
    {
        private IWebDriver driver;

        //private string _guid = string.Empty;
        //private string _timout = string.Empty;
        //private string _parentBrowser = string.Empty;
        //private string _testUser = string.Empty;
        //private string _testPassword = string.Empty;
        //private string _testEmail = string.Empty;
        //private string _networkLocation = string.Empty;
        //private string _baseURL = string.Empty;
        //private string _killOtherBrowser = string.Empty;
        //private string _deleteOldLogFiles = string.Empty;
        //private string _fileType = string.Empty;
        public  string testClassName = string.Empty;
        public string guid = System.Configuration.ConfigurationManager.AppSettings["UniqueGUID"];
        public string implicitTimeout = System.Configuration.ConfigurationManager.AppSettings["ImplicitWaits"];
        public string explicitTimeout = System.Configuration.ConfigurationManager.AppSettings["ExplicitWait"];
        public string parentBrowser = System.Configuration.ConfigurationManager.AppSettings["ParentBrowser"];
        public string testUser = System.Configuration.ConfigurationManager.AppSettings["TestUser"];
        public string testPassword = System.Configuration.ConfigurationManager.AppSettings["TestPassword"];
        public string testUserName = System.Configuration.ConfigurationManager.AppSettings["TestUserName"];
        public string email = System.Configuration.ConfigurationManager.AppSettings["Email"];
        public string password = System.Configuration.ConfigurationManager.AppSettings["Password"];
        public string reportingEmail = System.Configuration.ConfigurationManager.AppSettings["ResultReportingEmail"];
        public string networkLocation = System.Configuration.ConfigurationManager.AppSettings["NetworkLocation"];
        public string baseURL = System.Configuration.ConfigurationManager.AppSettings["baseURL"];
        public bool sendResult = System.Configuration.ConfigurationManager.AppSettings["sendlog"] == "true" ? true : false;
        public string deleteOldLogFiles = System.Configuration.ConfigurationManager.AppSettings["DeleteOldLogfiles"];
        public bool isRecording = System.Configuration.ConfigurationManager.AppSettings["RecordingWhileFailure"]=="true"? true : false;
        public bool EndToEnd = System.Configuration.ConfigurationManager.AppSettings["EndToEndTesting"] == "true" ? true : false;

        public IWebDriver Driver { get => driver; set => driver = value; }

        public string GetBrowser()
        {
            return parentBrowser;
        }
        public IWebDriver GetDriver()
        {
            return Driver;
        }
        public IWebDriver StartDriver()
        {
            if (parentBrowser.ToLower().Equals("Firefox".ToLower()))
            {
                FirefoxProfile firefoxProfile = new FirefoxProfile(@"", false);
                firefoxProfile.AddExtension(@"C:\WDTF\ThirdPartyTools\firebug-1.9.2.xpi");
                firefoxProfile.SetPreference("extensions.firebug.currentVersion", "1.9.2"); // Avoid startup screen
                firefoxProfile.EnableNativeEvents = true;
                this.Driver = new FirefoxDriver();
            }
            else if (parentBrowser.ToLower().Equals("googlechrome".ToLower()))
            {
                this.Driver = new ChromeDriver();
            }
            else if (parentBrowser.ToLower().Equals("Iexplore".ToLower()))
            {
                this.Driver = new InternetExplorerDriver();
            }
            else if (parentBrowser.ToLower().Equals("Android".ToLower()))
            {
               // this.driver =new  AndroidDriver();
                
            }
            else
            {
                this.Driver = null;
            }

            return Driver;
        }
        public int ImplicitTimeout()
        {
            return Convert.ToInt32(implicitTimeout);
        }
        public int ExplicitTimeout()
        {
            return Convert.ToInt32(explicitTimeout);
        }
        public string GetURL()
        {
            return baseURL;
        }
        public string GetTestUserName()
        {
            return testUser;
        }
        public string GetTestUserPassword()
        {
            return testPassword;
        }
        public string GetTestEmail()
        {
            return email;
        }
        public string GetNetworkLocation()
        {
            return networkLocation;
        }
    }
    
    

    
}
