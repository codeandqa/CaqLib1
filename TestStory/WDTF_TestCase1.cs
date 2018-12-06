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
    public class WDTF_TestCase1 : ITestCaseDefinition
    {
        // private readonly IWebDriver driver;

        [TestCase]
        public void ExecuteTest(TestEnvInfo testEnv)
        {
            //driver = testEnv.Driver;//This is going to be there in code.
            //loggerInfo.Instance.Message("Description of Test............");

            // Start Coding here.
            // Example:'\
            // open website
            CaqLib.OpenURL("https://www.lampsplus.com/");
            // click chandelier
            CaqLib.ClickElement(LocatorType.Id, "aChandeliers", "click on chandeliers");
            // click traditional
            // Click specific one.
            // click add to cart.
            // Click to checkout now
            // CLick Edit order
            // Click remove
            // click contniue shoppping.
            // verify Traditional Chandeliers


            // loggerInfo.Instance.Message("End statement of Test..........");
        }

    }
}
