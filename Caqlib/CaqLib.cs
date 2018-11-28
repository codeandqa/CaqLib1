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
using log4net;
using log4net.Config;
using System.Drawing;


namespace caqlib
{
    public sealed class CaqLib //: IKeyword
    {
        private static int testTimeout;
        private static IWebDriver _driver;
        private static TestEnvInfo testEInfo = null;
        // private static ScreenCaptureJob job;
       // XmlConfigurator.    
        //private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CaqLib(TestEnvInfo t)
        {
            testEInfo = t;
            testTimeout = Convert.ToInt32(t.ExplicitTimeout());
        }
        public static void OpenURL(string url)
        {
            // // loggerInfo.Instance.LogInfo("Open url: " + url);
            _driver.Navigate().GoToUrl(url);
        }
        public static IWebElement FindElement(LocatorType by, string locator)
        {
            // // loggerInfo.Instance.LogInfo("Find By [ " + by + " ]" + " Locator [ " + locator + " ]");
            IWebElement parentElement = null;
            return FindElement(by, locator, 2, parentElement);//TODO: Ideally we should be using 0 but for now its 2 sec
        }
        public static IWebElement FindElement(LocatorType by, string locator, int timeOut, IWebElement parentElement = null)
        {
            ReadOnlyCollection<IWebElement> SearchElements = FindElements(by, locator, timeOut, parentElement);
            if (SearchElements != null)
            {
                if (SearchElements.Count > 0)
                {
                    // // loggerInfo.Instance.LogInfo("Found Element By [ " + by + " ]" + " Locator [ " + locator + " ]");
                    return SearchElements.First<IWebElement>();
                }
                else
                {
                    PromptAlertMessage("Unable to find Element By [ " + by + " ]" + " Locator [ " + locator + " ]");
                    // // loggerInfo.Instance.LogAppErro(new Exception("Unable to find Element By [ " + by + " ]" + " Locator [ " + locator + " ]"), "", NLog.LogLevel.Error);
                    TakeScreenShot(testEInfo);
                    return null;
                }
            }
            else
            {
                PromptAlertMessage("Unable to Error Message is Element By [ " + by + " ]" + " Locator [ " + locator + " ]");
                // // loggerInfo.Instance.LogAppErro(new Exception("Unable to Error Message is Element By [ " + by + " ]" + " Locator [ " + locator + " ]"), "", NLog.LogLevel.Error);
                TakeScreenShot(testEInfo);
                return null;
            }
        }
        public static IWebElement FindElement(LocatorType by, string locator, string logMessage, IWebElement parentElement = null)
        {
            // // loggerInfo.Instance.LogInfo(logMessage);
            return FindElement(by, locator, testTimeout, parentElement);
        }
        public static ReadOnlyCollection<IWebElement> FindElements(LocatorType by, string locator)
        {
            // // loggerInfo.Instance.LogInfo("Find Elements By [ " + by + " ]" + " Locator [ " + locator + " ]");
            IWebElement parentElement = null;
            return FindElements(by, locator, testTimeout, parentElement);
        }
        public static ReadOnlyCollection<IWebElement> FindElements(LocatorType by, string locator, int timeout, IWebElement parentElement = null)
        {
            // // loggerInfo.Instance.LogInfo("Find Elements By [ " + by + " ]" + " Locator [ " + locator + " ]");
            List<IWebElement> list = new List<IWebElement>();
            ReadOnlyCollection<IWebElement> elements = new ReadOnlyCollection<IWebElement>(list);
            OpenQA.Selenium.By elementPointer = null;
            string baseIdentifier = string.Empty;
            string innerText = string.Empty;
            string[] splittedId = { };
            List<string> containsItemList = new List<string>();
            string[] containtTextArray = { };
            //Split the locator
            if (locator.Contains(":contains"))
            {
                splittedId = locator.Split(new char[] { ':' });
                baseIdentifier = splittedId[0];
                foreach (string item in splittedId)
                {
                    if (item.Contains("contains("))
                    {
                        containsItemList.Add(item.Substring(item.IndexOf('(') + 1, (item.IndexOf(')') - item.IndexOf('(') - 1)));
                    }
                }
                containtTextArray = containsItemList.ToArray();
            }
            else
            {
                baseIdentifier = locator;
            }
            switch (by)
            {
                case LocatorType.ClassName:
                    elementPointer = OpenQA.Selenium.By.ClassName(baseIdentifier);
                    // elements = _driver.FindElements(OpenQA.Selenium.By.ClassName(baseIdentifier));
                    break;

                case LocatorType.CssSelector:
                    elementPointer = OpenQA.Selenium.By.CssSelector(baseIdentifier);
                    break;

                case LocatorType.Id:
                    elementPointer = OpenQA.Selenium.By.Id(baseIdentifier);
                    break;

                case LocatorType.LinkText:
                    elementPointer = OpenQA.Selenium.By.LinkText(baseIdentifier);
                    break;

                case LocatorType.Name:
                    elementPointer = OpenQA.Selenium.By.Name(baseIdentifier);
                    break;

                case LocatorType.PartialLinkText:
                    elementPointer = OpenQA.Selenium.By.PartialLinkText(baseIdentifier);
                    break;

                case LocatorType.TagName:
                    elementPointer = OpenQA.Selenium.By.TagName(baseIdentifier);
                    break;

                case LocatorType.XPath:
                    elementPointer = OpenQA.Selenium.By.XPath(baseIdentifier);
                    break;
            };

            DateTime endTime = DateTime.Now.AddSeconds(10);
            while (elements.Count == 0 && endTime > DateTime.Now)
            {
                try
                {
                    if (parentElement == null)
                    {
                        elements = _driver.FindElements(elementPointer);
                    }
                    else
                    {
                        elements = parentElement.FindElements(elementPointer);
                    }

                }
                catch (Exception e)
                {
                    // // loggerInfo.Instance.Message(e.Message + System.Environment.NewLine);
                    e = new Exception(e.Message);
                    // // loggerInfo.Instance.LogAppErro(e, "Unable to find element: [ " + elementPointer + " ]", NLog.LogLevel.Error);
                    return null;
                }

            }

            if (containtTextArray.Length != 0)
            {
                foreach (IWebElement element in elements)
                {
                    innerText = element.Text;
                    foreach (string contText in containtTextArray)
                    {
                        if (innerText.Contains(contText))
                        {
                            IWebElement elem = element;
                            list.Add(element);
                        }
                    }
                }

                elements = new ReadOnlyCollection<IWebElement>(list);

                // // loggerInfo.Instance.LogInfo("Found '" + elements.Count + "' elements");
            }
            return elements;
        }

        public static bool ClickElement(LocatorType by, string locator, bool teardownTestIfFail, string logMessage)
        {
            IWebElement element = FindElement(by, locator, string.Empty);
            if (element != null)
            {
                try
                {
                    // loggerInfo.Instance.Message(logMessage);
                    // loggerInfo.Instance.Message("Click on " + locator);
                    return ClickElement(element, teardownTestIfFail);
                }
                catch (Exception e)
                {
                    // // loggerInfo.Instance.LogAppErro(e, "Unable to Click on Element : [ " + locator + " ]", NLog.LogLevel.Error);
                    return false;
                }
            }
            else
            {
                Exception e = new Exception("Click Element with By [ " + by + " ]" + " Locator [ " + locator + " ]");
                // // loggerInfo.Instance.LogAppErro(e, "", NLog.LogLevel.Error);
                return false;
            }
        }
        public static bool ClickElement(LocatorType by, string locator, string logMessage)
        {
            // // loggerInfo.Instance.LogInfo("Click Element with By [ " + by + " ]" + " Locator [ " + locator + " ]");
            return ClickElement(by, locator, false, logMessage);
        }
        public static bool ClickElement(IWebElement Element, bool teardownTestIfFail)
        {
            try
            {
                Element.Click();
            }
            catch (Exception e)
            {
                // // loggerInfo.Instance.LogAppErro(new Exception(e.Message + ": Unable to click Elements: [" + Element + "]"), "", NLog.LogLevel.Error);// + System.Environment.NewLine);
                EmergencyTeardown(testEInfo);
            }
            return false;
        }

        public static void TypeElement(LocatorType by, string locator, string textToType, string logMessage, bool hitEnterAfterType)
        {
            string postString = string.Empty;
            if (logMessage != "")
            {
                // loggerInfo.Instance.Message(logMessage);
            }
            if (hitEnterAfterType)
                postString = System.Environment.NewLine;

            IWebElement element = FindElement(by, locator, string.Empty);
            try
            {
                ClickElement(element, false);
                // loggerInfo.Instance.Message("Type '" + textToType + "' in Elements with By [ " + by + " ]" + " Locator [ " + locator + " ]");
                element.SendKeys(textToType + postString);
            }
            catch (Exception e)
            {
                e = new Exception(e.Message);
                // // loggerInfo.Instance.LogAppErro(e, "Unable to Type '" + textToType + "' in Elements with By [ " + by + " ]" + " Locator [ " + locator + " ]", NLog.LogLevel.Error);
                return;
            }
        }
        public static void TypeElement(LocatorType by, string locator, string textToType, string logMessage)
        {
            TypeElement(by, locator, textToType, logMessage, false);
        }

        public static void FillInFields(params string[] locatorAndInputCombinationWithDollarSignDelimiter)//"Locaotor$dataToFill";
        {
            foreach (string fieldSet in locatorAndInputCombinationWithDollarSignDelimiter)
            {
                if (fieldSet.Contains('['))
                {
                    TypeElement(LocatorType.CssSelector, fieldSet.Split('$')[0], fieldSet.Split('$')[0], "Type " + fieldSet.Split('$')[1] + " in input area " + fieldSet.Split('$')[0]);
                }
                else if (fieldSet.Contains('\\'))
                {
                    TypeElement(LocatorType.XPath, fieldSet.Split('$')[0], fieldSet.Split('$')[0], "Type " + fieldSet.Split('$')[1] + " in input area " + fieldSet.Split('$')[0]);
                }
                else
                {
                    TypeElement(LocatorType.Id, fieldSet.Split('$')[0], fieldSet.Split('$')[0], "Type " + fieldSet.Split('$')[1] + " in input area " + fieldSet.Split('$')[0]);
                }
                //TODO: handle other situation as per case.
            }
        }

        public static bool IsElementVisible(LocatorType by, string locator, string logMessage)
        {
            // // loggerInfo.Instance.LogInfo("Check if Visible: Elements By [ " + by + " ]" + " Locator [ " + locator + " ]");
            if (logMessage != "")
            {
                // loggerInfo.Instance.Message(logMessage);
            }
            IWebElement element = FindElement(by, locator, 5);
            bool result = false;
            try
            {
                result = element.Displayed;
            }
            catch (Exception e)
            {
                e = new Exception(e.Message);
                // // loggerInfo.Instance.LogAppErro(e, "Unable to find Element By [ " + by + " ]" + " Locator [ " + locator + " ]", NLog.LogLevel.Error);
                return false;
            }
            return result;
        }
        public static bool IsElementPresent(LocatorType by, string locator, string logMessage)
        {
            return IsElementVisible(by, locator, logMessage);
        }

        public static bool ElementMouseOver(LocatorType by, string locator, string logMessage)
        {
            return ElementMouseOver(by, locator);
        }
        public static bool ElementMouseOver(LocatorType by, string locator)
        {
            IWebElement targetElement = FindElement(by, locator, testTimeout);
            return ElementMouseOver(targetElement);
        }
        public static bool ElementMouseOver(IWebElement targetElement)
        {
            Size currentWinSize = _driver.Manage().Window.Size;
            _driver.Manage().Window.Maximize();
            OpenQA.Selenium.Interactions.Actions builder = new OpenQA.Selenium.Interactions.Actions(_driver);
            try
            {
                builder.MoveToElement(targetElement).Build().Perform();
                Thread.Sleep(5000);
                _driver.Manage().Window.Size = currentWinSize;
            }
            catch (Exception e)
            {
                // loggerInfo.Instance.Message(e.Message);
                return false;
            }
            return true;
        }

        public static bool PerformDragAndDrop(LocatorType sourceBy, string sourceLocator, LocatorType targetBy, string targetLocator, string logMessage)
        {
            // loggerInfo.Instance.Message(logMessage);
            return PerformDragAndDrop(sourceBy, sourceLocator, targetBy, targetLocator);
        }
        public static bool PerformDragAndDrop(LocatorType sourceBy, string sourceLocator, LocatorType targetBy, string targetLocator)
        {
            IWebElement source = FindElement(sourceBy, sourceLocator, 10);
            IWebElement target = FindElement(targetBy, targetLocator, 10);
            OpenQA.Selenium.Interactions.Actions builder = new OpenQA.Selenium.Interactions.Actions(_driver);
            try
            {
                // loggerInfo.Instance.Message("Perform Drag and Drop for :  " + "Source:{" + sourceLocator + "}   Destination:{" + targetLocator + "}");
                //  builder.DragAndDrop(source, target).Build().Perform();
                builder.ClickAndHold(source).MoveToElement(target).Release(target).Build().Perform();
            }
            catch (Exception e)
            {
                Exception exception = new Exception(e.Message);
                // // loggerInfo.Instance.LogAppErro(exception, "Source:{" + sourceLocator + "}   Destination:{" + targetLocator + "}", NLog.LogLevel.Error);
                return false;
            }
            return true;
        }

        public static bool IsElementEnabled(LocatorType by, string locator, string logMessage)
        {
            // // loggerInfo.Instance.LogInfo("Check Elements By [ " + by + " ]" + " Locator [ " + locator + " ], if enabled");
            if (logMessage != "")
            {
                // loggerInfo.Instance.Message(logMessage);
            }
            IWebElement element = FindElement(by, locator, 5);
            bool result = false;
            try
            {
                result = element.Enabled;
            }
            catch (Exception e)
            {
                e = new Exception(e.Message);
                // // loggerInfo.Instance.LogAppErro(e, "Unable to Find Elements By [ " + by + " ]" + " Locator [ " + locator + " ]", NLog.LogLevel.Error);
                return false;
            }
            return result;
        }

        public static bool WaitForElement(LocatorType by, string locator, string logMessage)
        {
            ReadOnlyCollection<IWebElement> SearchElements = FindElements(by, locator, testTimeout);
            if (SearchElements.Count > 0)
            {
                // // loggerInfo.Instance.LogInfo("Found Elements By [ " + by + " ]" + " Locator [ " + locator + " ]");
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void EmergencyTeardown(TestEnvInfo t)
        {
            if (!t.EndToEnd)
            {
                TakeScreenShot(t);
                CaqLib wd = new CaqLib(t);
                // // loggerInfo.Instance.LogWarning("killing the process in middle of the test");
                IWebElement currEl = _driver.SwitchTo().ActiveElement();
                wd.TeardownTest(t);
                Process p = Process.GetCurrentProcess();
                p.Kill();
            }
        }

        public static void ExecuteJavaScript(IWebElement targetElement)
        {
            // loggerInfo.Instance.Message("Executing JavaScript");
            string javaScript = "var evObj = document.createEvent('MouseEvents');" +
                                "evObj.initMouseEvent(\"mouseover\",true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);" +
                                "arguments[0].dispatchEvent(evObj);";
            IJavaScriptExecutor js = _driver as IJavaScriptExecutor;
            js.ExecuteScript(javaScript, targetElement);
        }

        public static void TakeScreenShot(TestEnvInfo testInfo)
        {
            if (AutomationLogging.countOfError > 0)
            {
                Screenshot ss = ((ITakesScreenshot)_driver).GetScreenshot();
                string screenshot = ss.AsBase64EncodedString;
                byte[] screenshotAsByteArray = ss.AsByteArray;
                //ss.SaveAsFile(AutomationLogging.newLocationInResultFolder + "\\" + testInfo.testClassName + "_" + AutomationLogging.countOfError.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                string pageSource = _driver.PageSource;
                using (StreamWriter outfile = new StreamWriter(AutomationLogging.newLocationInResultFolder + "\\" + testInfo.testClassName + "_" + AutomationLogging.countOfError.ToString() + ".html"))
                {
                    outfile.Write(pageSource.ToString());
                }
            }
        }

        #region Frames and Windows
        public static void SwitchToTop()
        {
            try
            {
                _driver.SwitchTo().DefaultContent();
            }
            catch (Exception e)
            {
                // // loggerInfo.Instance.LogAppErro(new Exception(e.Message), "Unable to to to Top frame", NLog.LogLevel.Error);
                throw;
            }
        }
        public static void SwitchFrame(int frameIndex)
        {
            try
            {
                _driver.SwitchTo().Frame(frameIndex);
            }
            catch (Exception e)
            {
                // // loggerInfo.Instance.LogAppErro(new Exception(e.Message), "Unable to switch frame", NLog.LogLevel.Error);
                throw;
            }
        }
        public static void SwitchFrame(string frameName)
        {
            try
            {
                _driver.SwitchTo().Frame(frameName);
            }
            catch (Exception e)
            {
                // // loggerInfo.Instance.LogAppErro(new Exception(e.Message), "Unable to switch to  frame: " + frameName, NLog.LogLevel.Error);
                throw;
            }
        }
        public static void SwitchFrame(IWebElement webElement)
        {
            try
            {
                _driver.SwitchTo().Frame(webElement);
            }
            catch (Exception e)
            {
                // // loggerInfo.Instance.LogAppErro(new Exception(e.Message), "Unable to switch to  frame: " + webElement.ToString(), NLog.LogLevel.Error);
                throw;
            }
        }
        public static void SwitchWindow(string windowName)
        {
            try
            {
                _driver.SwitchTo().Window(windowName);
            }
            catch (Exception e)
            {
                // // loggerInfo.Instance.LogAppErro(new Exception(e.Message), "Unable to switch to  Window: " + windowName, NLog.LogLevel.Error);
                throw;
            }
        }
        public static void AlertAccept()
        {
            try
            {
                _driver.SwitchTo().Alert().Accept();
            }
            catch (Exception e)
            {
                // // loggerInfo.Instance.LogAppErro(new Exception(e.Message), "Unable to accept in alert", NLog.LogLevel.Error);
                throw;
            }
        }
        public static void AlertDismiss()
        {
            try
            {
                _driver.SwitchTo().Alert().Dismiss();
            }
            catch (Exception e)
            {
                // // loggerInfo.Instance.LogAppErro(new Exception(e.Message), "Unable to accept in alert", NLog.LogLevel.Error);
                throw;
            }
        }
        #endregion

        #region Recording methods
        /*
        public static void StartRecordingVideo()
        {
            if (testEInfo.isRecording)
            {
                job = new ScreenCaptureJob();
                job.CaptureRectangle = Screen.PrimaryScreen.Bounds;
                job.ShowFlashingBoundary = true;
                job.OutputPath = AutomationLogging.newLocationInResultFolder;
                job.Start();
            }
        }

        public static void StopRecordingVideo()
        {
            if (testEInfo.isRecording)
            {
                string filename = job.ScreenCaptureFileName;
                job.Stop();
                if (AutomationLogging.countOfError > 0)
                {
                    MediaItem src = new MediaItem(filename);
                    Job jb = new Job();
                    jb.MediaItems.Add(src);
                    jb.ApplyPreset(Presets.VC1HD720pVBR);
                    jb.OutputDirectory = AutomationLogging.newLocationInResultFolder;
                    string output = ((Microsoft.Expression.Encoder.JobBase)(jb)).ActualOutputDirectory;
                    jb.Encode();
                }

                File.Delete(filename);
            }
        }
        */
        #endregion

        #region Script Executor
        [SetUp]
        public void SetupTest(TestEnvInfo testEnvInfo)
        {
            StringBuilder verificationErrors = new StringBuilder();
            verificationErrors.AppendLine(System.Environment.NewLine);
            verificationErrors.AppendLine("#####################  Test Header ####################");
            verificationErrors.AppendLine(" baseURL:            " + testEInfo.testClassName);
            verificationErrors.AppendLine(" baseURL:            " + testEnvInfo.baseURL);
            verificationErrors.AppendLine(" GUID:               " + testEnvInfo.guid);
            verificationErrors.AppendLine(" CurrentBrowser:     " + testEnvInfo.parentBrowser);
            verificationErrors.AppendLine(" TestUser:           " + testEnvInfo.testUser);
            verificationErrors.AppendLine(" TestPassword:       " + testEnvInfo.testPassword);
            verificationErrors.AppendLine(" TestEmail:          " + testEnvInfo.email);
            verificationErrors.AppendLine(" Timeout:            " + testEnvInfo.implicitTimeout + " sec");
            verificationErrors.AppendLine(" StartTime:          " + DateTime.Now.ToString());
            verificationErrors.AppendLine("#######################################################");
            //loggerInfo loggerInfo = new WDGL.loggerInfo();

            // // loggerInfo.Instance.LogInfo(verificationErrors.ToString());
            // // loggerInfo.Instance.LogInfo("Start SetupTest");
            //StartRecordingVideo();
            _driver = testEnvInfo.StartDriver();
            //_driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(Convert.ToInt16(testEnvInfo.implicitTimeout)));
            string baseURL = testEnvInfo.GetURL();
        }

        [TearDown]
        public void TeardownTest(TestEnvInfo testEnvInfo)
        {
            StringBuilder verificationErrors = new StringBuilder();
            verificationErrors.AppendLine(System.Environment.NewLine);
            //StopRecordingVideo();
            // loggerInfo.Instance.Message("TearDown the Test");
            try
            {
                _driver = testEnvInfo.Driver;
                if (AutomationLogging.countOfError > 0)
                {
                    verificationErrors.AppendLine("******************Result:Fail with {" + AutomationLogging.countOfError + "} Error(s)******************");
                    // // loggerInfo.Instance.LogInfo(verificationErrors.ToString());

                }
                else
                {
                    verificationErrors.AppendLine("******************Result:Pass with No Error******************");
                    // // loggerInfo.Instance.LogInfo(verificationErrors.ToString());
                }
                //TakeScreenShot(testEnvInfo);
                AutomationLogging.countOfError = 0;
                _driver.Quit();
                _driver.Dispose();
            }
            catch (Exception)
            {

                //// loggerInfo.Instance.Message("Unable to TearDown the Test");
            }

            //SendEmailUsingGmail();
        }
        #endregion

        public static void SendEmailUsingGmail()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(AutomationLogging.newLocationInResultFolder);
            string[] extensions = new[] { ".jpg" };

            FileInfo[] files = dirInfo.GetFiles()
                                      .Where(f => extensions.Contains(f.Extension.ToLower()))
                                      .ToArray();
            if (testEInfo.sendResult || files.Length > 0)
            {
                // // loggerInfo.Instance.LogInfo("Email Results and result files to " + testEInfo.reportingEmail);
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("shahi.adityas@gmail.com");
                mail.To.Add("shahi.aditya@gmail.com");
                mail.Subject = "WebDriver Test Result: " + "[" + files.Length + "] Error/Failed TestCase: " + testEInfo.testClassName;
                mail.Body = "This is for testing SMTP mail from GMAIL";
                System.Net.Mail.Attachment attachment;
                dirInfo = new DirectoryInfo(AutomationLogging.newLocationInResultFolder);
                extensions = new[] { ".jpg", ".html", ".txt" };

                files = dirInfo.GetFiles()
                                          .Where(f => extensions.Contains(f.Extension.ToLower()))
                                          .ToArray();
                foreach (FileInfo item in files)
                {
                    attachment = new System.Net.Mail.Attachment(item.FullName);
                    mail.Attachments.Add(attachment);
                }
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(testEInfo.email, testEInfo.password);
                SmtpServer.EnableSsl = true;
                try
                {
                    SmtpServer.Send(mail);
                }
                catch
                {

                }
            }
        }

        public static void PromptAlertMessage(string message)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "C:\\WDTF\\ThirdPartyTools\\AlertMessage.exe",
                Arguments = message
            };
            Process.Start(startInfo);
        }
    }

}
