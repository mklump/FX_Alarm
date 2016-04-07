// -----------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="PIPs for Heaven, LLC">
//     Copyright (c) PIPs for Heaven, LLC.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FX_Alarm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Awesomium.Core;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Xml;
    using System.Text;
    using System.Security;
    using System.Windows.Data;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using System.Security.Cryptography;
    using System.Runtime.InteropServices;

    /// <summary>
    /// MainViewModel is the primary ViewModel
    /// The User Interface is bound to properties in this class.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Reference to the MainWindow.
        /// </summary>
        private MainWindow win;

        /// <summary>
        /// SecureString member strUserName for login.
        /// </summary>
        private SecureString strUserName;

        /// <summary>
        /// SecureString member strPassword for login.
        /// </summary>
        private SecureString strPassword;

        /// <summary>
        /// String member fxArrowAlert for initiating arrow alert response.
        /// </summary>
        private string fxArrowAlert;

        /// <summary>
        /// String member fxBackupAlert for initiating backup alert response.
        /// </summary>
        private string fxBackupAlert;

        /// <summary>
        /// String member fxAlertSite for determining which FX alert site is responding.
        /// </summary>
        private string fxAlertSite;

        /// <summary>
        /// String member loginStatus for specifying the status of login.
        /// </summary>
        private string loginStatus;

        /// <summary>
        /// String member loginLabel for specifying the content labeling of the Login Button.
        /// </summary>
        private string loginLabel;

        /// <summary>
        /// String member isRefreshOn for determining if the refresh continous check box is checked or not.
        /// </summary>
        private bool isRefreshOn;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            strUserName = new SecureString();
            strPassword = new SecureString();
            fxArrowAlert = "";
            fxBackupAlert = "";
            fxAlertSite = "";
            LoginStatus = "Pending login.";

            // Commands
            this.commandLogin = new Command((o) => this.LogIn());
            this.commandAbout = new Command((o) => this.OpenAbout());
            this.commandRefresh = new Command((o) => this.RefreshNow());

            // Control initializations
            LoginLabel = "Log In";
        }

        /// <summary>
        /// Setter property for this MainWindow reference.
        /// </summary>
        public MainWindow Win
        {
            get { return win; }
            set { win = value; }
        }

        /// <summary>
        /// Backing field property for the strUserName to login
        /// </summary>
        public SecureString StrUserName
        {
            get { return this.strUserName; }
            set
            {
                if (this.strUserName != value)
                {
                    this.strUserName = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Backing field property for the strPassword to login
        /// </summary>
        public SecureString StrPassword
        {
            get { return this.strPassword; }
            set
            {
                if (this.strPassword != value)
                {
                    this.strPassword = value;
                    //this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Backing field property for the loginStatus class member
        /// </summary>
        public string LoginStatus
        {
            get { return this.loginStatus; }
            set
            {
                if (this.loginStatus != value)
                {
                    this.loginStatus = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Backing field property for the fxArrowAlert for initiation access
        /// </summary>
        public string FXArrowAlert
        {
            get { return fxArrowAlert; }
            set
            {
                if (this.fxArrowAlert != value)
                {
                    fxArrowAlert = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Backing field property for the fxBackupAlert for initiation access
        /// </summary>
        public string FXBackUpAlert
        {
            get { return fxBackupAlert; }
            set
            {
                if (this.fxBackupAlert != value)
                {
                    fxBackupAlert = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Backing field property for the fxAlertSite for responsiveness determination
        /// </summary>
        public string FXAlertSite
        {
            get { return fxAlertSite; }
            set
            {
                if (this.fxAlertSite != value)
                {
                    fxAlertSite = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Backing field property for the content label of the Login button
        /// </summary>
        public string LoginLabel
        {
            get { return loginLabel; }
            set
            {
                if (this.loginLabel != value)
                {
                    loginLabel = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Backing field property for the isRefreshOn checkbox continous refresh
        /// </summary>
        public bool IsRefreshOn
        {
            get { return isRefreshOn; }
            set
            {
                if (this.isRefreshOn != value)
                {
                    isRefreshOn = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Get or sets the command to login.
        /// </summary>
        public Command commandLogin { get; set; }

        /// <summary>
        /// Gets or sets the command to open the about dialogue box
        /// </summary>
        public Command commandAbout { get; set; }

        /// <summary>
        /// Gets or sets the command to Navigate the primary web browser control to ArrowAlert
        /// </summary>
        public Command commandRefresh { get; set; }

        /// <summary>
        /// Gets or sets the command for the Navigating event handler web browswer control
        /// </summary>
        public Command commandNavigating { get; set; }

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// NotifyPropertyChanged triggers the PropertyChanged event for the specified property
        /// </summary>
        /// <param name="propertyName">Name of property that has changed</param>
        private void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Retrieves the AlertUrlData written previously to FXAlertData.dat, and loads them here.
        /// </summary>
        private void GetAlertUrlData()
        {
            StreamReader reader = null;
            string secureFile = GetProjectVar() + "\\FXAlertData.dat";
            try
            {
                reader = new StreamReader(File.OpenRead(secureFile));
                fxArrowAlert = reader.ReadLine();
                fxBackupAlert = reader.ReadLine();
            }
            catch (Exception e)
            {
                LoginStatus = "Login failed and web test did not execute.";
                throw e;
            }
            finally
            {
                if (null != reader)
                    reader.Close();
            }
        }

        /// <summary>
        /// Executes login command to forex alert website.
        /// </summary>
        private void LogIn()
        {
            if (false == win.btnRefreshNow.IsEnabled)
            {
                // Get login credentials
                string secureFile = GetLogin(), alertSite = null;
                // Execute browse to alert website
                this.ExecuteGetUrlCoded();
                // Get the alert url data to begin viewing alerts
                this.GetAlertUrlData();
                // Test responsiveness, if not switch to backup
                string[] sites = new string[]
                {
                    this.fxBackupAlert,
                    "http://www.forexearlywarning.com/get_v4.php",
                    this.fxArrowAlert
                };
                foreach (string site in sites)
                {
                    alertSite = this.GetFXAlertRespose(site);
                    if (null != alertSite) break;
                }
                if (null == alertSite)
                {
                    LoginStatus = "Operationg failed. Neither the Primary or Secondary site is responding.";
                    MessageBox.Show(LoginStatus, "SITE NAVIGATION ERROR", MessageBoxButton.OK,
                        MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                }
                // Load FX alert site
                this.NavigateUrl(alertSite);
                //this.NavigateUrl(GetProjectVar() + "lastFXAlertPage.html");
                if (true == File.Exists(secureFile))
                    File.Delete(secureFile);
                win.btnRefreshNow.IsEnabled = true;
                win.chkRefreshContinous.IsEnabled = true;
                LoginLabel = "Log Out";
            }
            else
                this.LogOut();
        } // End of private void LogIn()

        #region Testing Awesomium sample code to get data
        /// <summary>
        /// Executes command to open the about dialogue box.
        /// Temporarily used for testing for data retrieval.
        /// Command funtion bound to property commandAbout
        /// </summary>
        private void OpenAbout()
        {
            try
            {
                //string html;
                //byte[] bytes = null;
                //using (var wc = new WebClient())
                //{
                //    bytes = wc.DownloadData(FXAlertSite);
                //    wc.DownloadFile(FXAlertSite, ".\\DataDump.txt");
                //}

                //document.write(unescape("<script language="JavaScript">document.write('<iframe src="http://204.12.52.120/hm9485488/" height="1300" width="820" frameborder="0" scrolling="auto" target="_blank"></iframe>');</script>"));

                using (WebSession session = WebCore.CreateWebSession(new WebPreferences() { }))
                {
                    using (WebView view = WebCore.CreateWebView(1100, 600, session))
                    {
                        // Load a URL.
                        view.Source = FXAlertSite.ToUri();

                        // Print loaded page information
                        MessageBox.Show(String.Format("Page Title: {0}", view.Title));
                        MessageBox.Show(String.Format("Loaded URL: {0}", view.Source));

                        // Check if the WebCore is already automatically updating.
                        // This check is only here for demonstration. Console applications
                        // are not UI applications and have no synchronization context.
                        // Without a valid synchronization context, we need to call Run.
                        // This will tell the WebCore to create an Awesomium-specific
                        // synchronization context and start an update loop.
                        // The current thread will be blocked until WebCore.Shutdown
                        // is called. You can use the same model by creating a dedicated
                        // thread for Awesomium. For details about the new auto-updating
                        // and synchronization model of Awesomium.NET, read the documentation
                        // of WebCore.Run.
                        if (WebCore.UpdateState == WebCoreUpdateState.NotUpdating)
                            // The point of no return. This will only exit
                            // when we call Shutdown.
                            WebCore.Run();

                        try
                        {
                            //Test webview object HERE
                            if (true == view.IsJavascriptEnabled)
                            {
                                view.Reload(false);
                                XmlDocument fxjs = new XmlDocument();
                                fxjs.LoadXml(view.HTML);
                                XmlNodeList nodeList = fxjs.ChildNodes;
                            }
                        }
                        catch (Exception error)
                        {
                            MessageBox.Show("WEB SITE ERROR PAGE: " + FXAlertSite + ". Exception details: "
                                + error.ToString(), "SITE NAVIGATION ERROR", MessageBoxButton.OK, MessageBoxImage.Stop,
                                MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        }
                    } // End using (WebView view = WebCore.CreateWebView(1100, 600, session))
                }

                // /html/body/div/div[1]
                // /html/body/div/div[2]/div[2]
                //StreamWriter write = new StreamWriter(File.Open(".\\DataDump.txt", FileMode.Create, FileAccess.Write, FileShare.Read));
                //write.Write(bytes);
                //write.Close();
            }
            catch (WebException error)
            {
                MessageBox.Show("WEB SITE ERROR PAGE: " + FXAlertSite + ". Exception details: "
                    + error.ToString(), "SITE NAVIGATION ERROR", MessageBoxButton.OK, MessageBoxImage.Stop,
                    MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
        #endregion

        /// <summary>
        /// Gets the login username and password credentials from UI controls.
        /// </summary>
        /// <returns>Secure file location.</returns>
        public string GetLogin()
        {
            if (strUserName.Length == 0)
            {
                foreach (char c in win.txtUN.Text)
                    strUserName.AppendChar(c);
                strUserName.MakeReadOnly();
            }
            if (strPassword.Length == 0)
            {
                foreach (char c in win.pwBox.Password)
                    strPassword.AppendChar(c);
                strPassword.MakeReadOnly();
            }
            string secureFile = GetProjectVar() + "securedata.dat";
            StreamWriter stream = null;
            try
            {
                stream = new StreamWriter(File.OpenWrite(secureFile));
                stream.WriteLine(
                    Encrypt(ConvertToUnsecureString(strUserName), "@s%^3H8!"));
                stream.WriteLine(
                    Encrypt(ConvertToUnsecureString(strPassword), "@s%^3H8!"));
            }
            catch (Exception e) { throw e; }
            finally { if (null != stream) stream.Close(); }
            return secureFile;
        }

        /// <summary>
        /// Executes command to open the primary web browser control to the specified location
        /// </summary>
        /// <param name="navigate">Website Url of which to go to.</param>
        private void NavigateUrl(string navigate)
        {
            try
            {
                FXAlertSite = navigate;
                this.RefreshNow();
            }
            catch (Exception error)
            {
                MessageBox.Show("404 ERROR. THE SPECIFIED PAGE: " + navigate + ", DOES NOT EXIST. Exception details: "
                    + error.ToString(), "SITE NAVIGATION ERROR", MessageBoxButton.OK, MessageBoxImage.Stop,
                    MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        /// <summary>
        /// Returns the first responsive first FX alert site.
        /// Switches to backup if the first FX alert website fails.
        /// </summary>
        /// <param name="siteToTest">FX alert web site that is currently under test</param>
        /// <returns>First responsive FX alert site.</returns>
        private string GetFXAlertRespose(string siteToTest)
        {
            WebResponse response = null;
            StreamReader reader = null;
            string result = null;
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(siteToTest);
                httpRequest.Method = "GET";
                try
                {
                    response = httpRequest.GetResponse();
                    reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    result = reader.ReadToEnd();
                }
                catch (WebException error) { result = LoginStatus = error.Message; }
                if (false == string.IsNullOrEmpty(siteToTest) &&
                    result != "Error: System error. Please contact support." &&
                    result != "Please login to system." ||
                    result != "The remote server returned an error: (400) Bad Request.")
                {
                    this.NavigateUrl(siteToTest);
                    LoginStatus = siteToTest + " site is responding";
                    return siteToTest;
                }
                else
                {
                    LoginStatus = "Primary failed, switching to Secondary site";
                    MessageBox.Show("SESSION ERROR. THE SPECIFIED PAGE: " + siteToTest +
                        ", HAS EXPIRED SESSION OR LOGGED OUT.",
                        "SITE NAVIGATION ERROR", MessageBoxButton.OK, MessageBoxImage.Stop,
                        MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "SITE NAVIGATION ERROR", MessageBoxButton.OK,
                    MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (response != null)
                    response.Close();
            }
            return null;
        }

        /// <summary>
        /// Immediately refreshes browserFXAlert web browser control
        /// </summary>
        private void RefreshNow()
        {
            win.browserFXAlert.Navigate(FXAlertSite);
            if (null != win.browserFXAlert.Source && true == win.btnRefreshNow.IsEnabled)
                win.browserFXAlert.Refresh(false);
        }

        /// <summary>
        /// Log out the current active session
        /// </summary>
        private void LogOut()
        {
            WebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.forexearlywarning.com/logout.php");
                request.Method = "GET";
                //request.Expect = "http://www.forexearlywarning.com/login.php?err=2";
                //request.Headers.Add("Referer", "http://www.forexearlywarning.com/heatmap.php");
                response = request.GetResponse();
                //FXAlertSite = response.ResponseUri.PathAndQuery;
                this.RefreshNow();
                win.btnRefreshNow.IsEnabled = false;
                win.chkRefreshContinous.IsEnabled = false;
                LoginLabel = "Log In";
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "SITE NAVIGATION ERROR", MessageBoxButton.OK,
                    MessageBoxImage.Stop, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
            finally
            {
                if (response != null)
                    response.Close();
                // TODO: Uncomment file delete before release
                //if (File.Exists(GetProjectVar() + "\\FXAlertData.dat"))
                //    File.Delete(GetProjectVar() + "\\FXAlertData.dat");
            }
        }

        /// <summary>
        /// Convert a System.SecureString to an unsecure System.String
        /// </summary>
        /// <param name="secureString">SecureString to convert</param>
        /// <returns>Converted System.String</returns>
        public string ConvertToUnsecureString(SecureString secureString)
        {
            if (secureString == null)
                throw new ArgumentNullException("secureString");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        /// <summary>
        /// For use only with visual studio, this gets the project path environment variable
        /// NOTE: If %FXALARMPROJDIR% environment variable is not defined, then the directory where this application
        /// FX_Alarm.exe lives and contents COPIED UNDER the %USERPROFILE% user data files path will be used to set
        /// the environment variable %FXALARMPROJDIR%.
        /// </summary>
        /// <returns>The project path.</returns>
        private string GetProjectVar()
        {
            string projPath = null;
            if (!Directory.Exists(Environment.GetEnvironmentVariable(
                    "FXALARMPROJDIR", EnvironmentVariableTarget.Machine)))
            {
                string[] strDirs = Directory.GetDirectories(Environment.GetEnvironmentVariable("USERPROFILE",
                    EnvironmentVariableTarget.Process), "*", SearchOption.TopDirectoryOnly),
                    dirs = null, files = null;
                foreach (string str in strDirs)
                {
                    try
                    {
                        dirs = Directory.GetDirectories(str, "FX_Alarm", SearchOption.AllDirectories);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    foreach (string subPath in dirs)
                        if (true == subPath.EndsWith("FX_Alarm"))
                        {
                            projPath = subPath;
                            try
                            {
                                files = Directory.GetFiles(projPath, "FX_Alarm.exe", SearchOption.AllDirectories);
                                DirectoryInfo info = Directory.GetParent(files[0]);
                                if (null != projPath && string.Empty != projPath && projPath == info.FullName)
                                {
                                    Environment.SetEnvironmentVariable("FXALARMPROJDIR", projPath, EnvironmentVariableTarget.Machine);
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    if (string.Empty != projPath && null != projPath)
                        break;
                }
            }
            return Environment.GetEnvironmentVariable("FXALARMPROJDIR", EnvironmentVariableTarget.Machine) + "\\";
        }

        /// <summary>
        /// Executes the GetUrlCoded web performance test.
        /// NOTE: This function also activly searches for the %MSTest% environment variable, sets it if it
        /// does not exist, and then finally searches for the existance of the %MSTest% environment variable
        /// within the %Path% global environment variable and sets it for machine level path string expansion.
        /// </summary>
        /// <returns>Error code execution status for this GetUrlCoded web performance test.</returns>
        private int ExecuteGetUrlCoded()
        {
            int errorCode = -1;
            string dirMSTest = "", pathVarGlobal = "";
            try
            {
                if (!File.Exists(Environment.GetEnvironmentVariable(
                    "MSTest", EnvironmentVariableTarget.Machine) +
                    "MSTest.exe"))
                {
                    string[] dirsMSTest = Directory.GetDirectories(Environment.GetEnvironmentVariable("ProgramFiles(x86)",
                        EnvironmentVariableTarget.Process), "*Visual Studio*", SearchOption.TopDirectoryOnly);
                    foreach (string strPath in dirsMSTest)
                    {
                        string[] subpaths = Directory.GetFiles(strPath, "MSTest.exe", SearchOption.AllDirectories);
                        if (subpaths.Length > 0)
                            dirMSTest = subpaths[0];
                        if (true == dirMSTest.EndsWith("MSTest.exe"))
                            break;
                    }
                    dirMSTest = dirMSTest.Remove(dirMSTest.Length - "MSTest.exe".Length);
                    Environment.SetEnvironmentVariable("MSTest", dirMSTest, EnvironmentVariableTarget.User);
                    pathVarGlobal = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
                    Environment.SetEnvironmentVariable("Path", pathVarGlobal + dirMSTest, EnvironmentVariableTarget.User);
                }
                errorCode = 0;
            }
            catch (Exception e)
            {
                errorCode = -1;
                throw e;
            }
            Process msTest = new Process();
            msTest.StartInfo.UseShellExecute = true;
            msTest.StartInfo.CreateNoWindow = true;
            msTest.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            dirMSTest = msTest.StartInfo.WorkingDirectory = Environment.GetEnvironmentVariable("MSTest", EnvironmentVariableTarget.User);
            msTest.StartInfo.FileName = dirMSTest + "MSTest.exe";
            // Specifies to MSTest where to save the test results for the AccessFXAlarm_WebTest webtest.
            string saveTestResultFolder = GetProjectVar() + "TestResults";
            if (!Directory.Exists(saveTestResultFolder))
                Directory.CreateDirectory(saveTestResultFolder);
            try
            {
                // Check for existance of VS2012 old test result files, if yes, delete them.
                string projPath = GetProjectVar();
                List<string> testResultFiles = new List<string>(Directory.GetFiles(projPath, "TestResult*", SearchOption.TopDirectoryOnly)),
                    localMachineResults = new List<string>(Directory.GetDirectories(projPath,
                        '*' + Dns.GetHostName() + '*', SearchOption.TopDirectoryOnly));
                testResultFiles.AddRange(Directory.GetDirectories(projPath, "TestResults\\*", SearchOption.AllDirectories));
                foreach (string resultFile in testResultFiles)
                {
                    if (true == File.Exists(resultFile))
                        File.Delete(resultFile);
                    if (true == Directory.Exists(resultFile))
                        Directory.Delete(resultFile, true);
                }
                // Get other VS2012 saved result file locations and delete them.
                foreach (string resultDir in localMachineResults)
                    if (true == Directory.Exists(resultDir))
                        Directory.Delete(resultDir, true);
                msTest.StartInfo.Arguments = "/testcontainer:\"" + GetProjectVar() + "AccessFXAlarm_WebTest.dll\"" +
                    " /resultsfile:\"" + saveTestResultFolder + "\"";
                msTest.Start();
                msTest.WaitForExit();
                errorCode = 0;
            }
            catch (Exception e)
            {
                errorCode = -1;
                throw e;
            }
            return errorCode;
        }

        /// <summary>
        /// Security encrypts sensitive data in a string.
        /// </summary>
        /// <param name="strText">Sensitive data to encrypt</param>
        /// <param name="strEncrypt">Encryption key</param>
        /// <returns>Decrypted data</returns>
        private string Encrypt(string strText, string strEncrypt)
        {
            byte[] byKey = new byte[20];
            byte[] dv = { 0x30, 0x52, 0x74, 0x96, 0xB2, 0xC9, 0xEB, 0xFD };
            try
            {
                byKey = System.Text.Encoding.UTF8.GetBytes(strEncrypt.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputArray = System.Text.Encoding.UTF8.GetBytes(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, dv), CryptoStreamMode.Write);
                cs.Write(inputArray, 0, inputArray.Length); cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    } // End of public class MainViewModel : INotifyPropertyChanged
} // End of namespace FX_Alarm