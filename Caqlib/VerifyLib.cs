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


namespace caqlib
{
    public sealed class VerifyLib
    {
        public static void AssertText(string actualText, string expectedText, StringComparison comparisonType)
        {
            Assert.IsTrue(actualText.Equals(expectedText, comparisonType));
            LoggerInfo.Instance.LogInfo("Verification Point :" + System.Environment.NewLine + "Expected Text : " + expectedText + System.Environment.NewLine +
                                        "Actual Text : " + actualText + System.Environment.NewLine +
                                        "Result : Pass" + System.Environment.NewLine);
        }
        public static void AssertNumber(int actualNum, int expectedNum)
        {
            Assert.IsTrue(actualNum == expectedNum);
            LoggerInfo.Instance.LogInfo("Verification Point: " + System.Environment.NewLine + "Expected Num : " + expectedNum + System.Environment.NewLine +
                                       "Actual Num : " + actualNum + System.Environment.NewLine +
                                       "Result : Pass" + System.Environment.NewLine);
        }
        public static string[] VerifyAndReturnBrokenImage(IWebDriver driver)
        {
            List<string> invalidSrc = new List<string>();
            int i = 1;
            string url = string.Empty;
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.TagName("img"));
            List<string> list = new List<string>();
            List<IWebElement> nullSourceElement = new List<IWebElement>();
            foreach (IWebElement item in elements)
            {
                url = item.GetAttribute("src");
                if (url == null)
                {
                    nullSourceElement.Add(item);
                }
                else
                {
                    list.Add(item.GetAttribute("src"));
                }
            }
            foreach (string item in list)
            {
                if (item != null)
                {
                    HttpWebRequest lxRequest = (HttpWebRequest)WebRequest.Create(item);
                    String lsResponse = string.Empty;
                    try
                    {
                        HttpWebResponse lxResponse = (HttpWebResponse)lxRequest.GetResponse();

                        using (BinaryReader reader = new BinaryReader(lxResponse.GetResponseStream()))
                        {
                            Byte[] lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                            if (lnByte.Length > 0)
                            {
                                LoggerInfo.Instance.Message("Valid image link on current page: " + item);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //404 error
                        if (!invalidSrc.Contains(item))
                        {
                            LoggerInfo.Instance.Message("Invalid image link on current page: " + item);
                            invalidSrc.Add(item);
                        }
                    }
                }
                else
                {
                    // Console.WriteLine("{0}", i);
                    LoggerInfo.Instance.Message("{0} null image found.");
                }
            }
            return invalidSrc.ToArray();

        }
    }
}
