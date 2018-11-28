using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using caqlib;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using OpenQA.Selenium.Support.PageObjects;
using System.IO;
using System.Net;

using System.Drawing;

namespace WDTF
{
    public class WDTF_TestCase1 : TestCaseDefinition
    {
        // private readonly IWebDriver driver;

        [TestCase]
        public void ExecuteTest(TestEnvInfo testEnv)
        {
            //driver = testEnv.Driver;//This is going to be there in code.
            //loggerInfo.Instance.Message("Description of Test............");
            
            // Start Coding here.
            // Example:
            CaqLib.OpenURL(testEnv.GetURL());
            CaqLib.ClickElement(LocatorType.Id, "aChandeliers", "");
            CaqLib.ClickElement(LocatorType.Id, "aCeilingLighting", "");
            CaqLib.ClickElement(LocatorType.Id, "aLamps", "");
            CaqLib.ClickElement(LocatorType.Id, "aWallLights", "");
            
           // loggerInfo.Instance.Message("End statement of Test..........");
        }

    }
}
