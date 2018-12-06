#region Please do dont delete or modify this file. If you modify then script may stop
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using caqlib;
using System.Configuration;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using OpenQA.Selenium.Support.PageObjects;
namespace WDTF
{
    class Program
    {
        static void Main(string[] args)
        {
            TestEnvInfo t = new TestEnvInfo();
            AutomationLogging.className = t.testClassName;
            AutomationLogging.currentGuid = t.guid;
            CaqLib nav = new CaqLib(t);
            string browser = string.Empty;
            Program p = new Program();
            List<ITestCaseDefinition> input = new List<ITestCaseDefinition>();
            List<string> inputBrowser = new List<string>();
            string pattern = @"(\w+)(:)(\w+)";
            //ArgData argData = new ArgData();
            List<ArgData> listOfArg = new List<ArgData>();
            ITestCaseDefinition[] testcaseList = WDTF.TestCaseList.TEST_CASES;
            int m = 0;
            t.guid = string.Format("{0:GUIDyyyyMMddmmss}", DateTime.Now);
            
            if (args.Length == 0)
            {
                foreach (ITestCaseDefinition item in testcaseList)
                {
                    input.Add(item);
                    inputBrowser.Add(t.parentBrowser);
                }
                //Browser = t.parentBrowser;
            }
            else
            {
                foreach (string arg in args)
                {
                    string validReg = Regex.Match(arg, pattern).Value;
                    if (validReg !="")

                    foreach (ITestCaseDefinition item in testcaseList)
                    {
                        string namespace_tc = item.ToString();
                        string tc = namespace_tc.Substring(namespace_tc.LastIndexOf('.') + 1);
                        //if (tc == arg)
                        if (tc == validReg.Split(':')[0])
                        {
                                input.Add(item);
                                inputBrowser.Add(validReg.Split(':')[1]);
                        }
                        else
                        {
                            if (arg.ToLower() == "firefox")
                            {
                                browser = arg.ToLower();
                            }
                            else if (arg.ToLower() == "googlechrome")
                            {
                                browser = arg.ToLower();
                            }
                            else if (arg.ToLower() == "iexplore")
                            {
                                browser = arg.ToLower();
                            }
                            else
                            {
                                browser = t.parentBrowser;
                            }
                            // loggerInfo.Instance.Message("Mismatched argument:"+arg); //TODO: handle non matching argument.
                        }
                    }
                }
            }
            if (input.Count == 0)
            {
                foreach (ITestCaseDefinition item in testcaseList)
                {
                    input.Add(item);
                    inputBrowser.Add(t.parentBrowser);
                }
            }
            foreach (ITestCaseDefinition item in input)
            {
                t.parentBrowser = inputBrowser[m].ToLower();
                m++;
                if (t.parentBrowser == "googlechrome" ||t.parentBrowser == "firefox" ||t.parentBrowser == "iexplore")
                {
                    AutomationLogging.className = item.ToString();
                    t.testClassName = AutomationLogging.className;
                    nav.SetupTest(t);
                    item.ExecuteTest(t);
                    nav.TeardownTest(t);
                }
            }

            if (t.sendResult)
            {
                CaqLib.SendEmailUsingGmail();
            }
            
        }
    }
    public interface ITestCaseDefinition
    {
        void ExecuteTest(TestEnvInfo driver);
    }
    struct ArgData
    {
        private ITestCaseDefinition t_testcaseDef;
        public ITestCaseDefinition TestCases
        {
            get
            {
                return t_testcaseDef;
            }
            set
            {
                t_testcaseDef = value;
            }
        }
        private string p_browser;
        public string Browser
        {
            get
            {
                return p_browser;
            }
            set
            {
                p_browser = value;
            }
        }
    }
}
#endregion