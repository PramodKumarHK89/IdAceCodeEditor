using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IdAceCodeEditor
{
    /// <summary>
    /// Interaction logic for Prerequisites.xaml
    /// </summary>
    public partial class Prerequisites : Window
    {
        public Prerequisites()
        {
            InitializeComponent();
        }
        string mAppType, mReadmeLink, mPath, mquickstartLink;     
        List<Prerequisite> mPrerequisiteList;
        public Prerequisites(string appType,
            string readmeLink,
            string quickLink,
            string localFolderPath,
            List<Prerequisite> prerequisites)
        {
            mAppType = appType;
            mReadmeLink = readmeLink;            
            mquickstartLink = quickLink;
            mPath = localFolderPath;
            mPrerequisiteList = prerequisites;
            InitializeComponent();
        }
        private bool CheckIfVSCodeInstlled()
        {
            var paths = Environment.GetEnvironmentVariable("path");

            foreach (var item in paths!.Split(";"))
            {
                if (item.Contains("Microsoft VS Code"))
                {
                    if (System.IO.File.Exists(System.IO.Path.Combine(item, "code.cmd")))
                        return true;
                }

            }
            return false;
        }

        private bool CheckIfDotNetCoreInstalled(string dotnetVersion = "3.1")
        {
            var paths = Environment.GetEnvironmentVariable("path");

            foreach (var item in paths!.Split(";"))
            {
                if (item.Contains("dotnet"))
                {
                    if (System.IO.Directory.Exists(System.IO.Path.Combine(item, "sdk")))
                    {
                        foreach (var p in System.IO.Directory.GetDirectories(System.IO.Path.Combine(item, "sdk")))
                        {
                            string dirName = new System.IO.DirectoryInfo(p).Name;
                            if (dirName.StartsWith(dotnetVersion))
                                return true;
                        }

                    }
                }
            }
            return false;
        }
        private bool CheckIfNPMInstalled()
        {
            var paths = Environment.GetEnvironmentVariable("path");

            foreach (var item in paths!.Split(";"))
            {
                if (item.Contains("nodejs"))
                {
                    if (System.IO.File.Exists(System.IO.Path.Combine(item, "node.exe")))
                        return true;
                }

            }
            return false;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Components> items = new ObservableCollection<Components>();

            foreach (var item in mPrerequisiteList)
            {
                items.Add(new Components() { Component = item.Name, 
                    IsInstalled = CheckInstallation(item.Name),
                    DownloadLink = item.DownloadLink});

            }           
            lvUsers.ItemsSource = items;
        }
        private bool CheckInstallation(string app)
        {
            bool bRet = false;
            switch (app)
            {
                case "VSCode":
                    bRet = CheckIfVSCodeInstlled();
                    break;
                case "NPM":
                    bRet = CheckIfNPMInstalled();
                    break;
                case "DotNetCore":
                    bRet = CheckIfDotNetCoreInstalled();
                    break;
                case "VisualStudio":
                    bRet = false;
                    break;
                default:
                    break;
            }
            return bRet;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(mPath) { UseShellExecute = true });
        }

        private void msdnLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            readme.NavigateUri = new Uri(mquickstartLink);
            Process.Start(new ProcessStartInfo(mquickstartLink) { UseShellExecute = true });
            e.Handled = true;
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var urlPart = ((Hyperlink)sender).NavigateUri;
            var fullUrl = string.Format("{0}", urlPart);
            Process.Start(new ProcessStartInfo(fullUrl) { UseShellExecute = true });
            e.Handled = true;
        }

        private void readme_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            readme.NavigateUri = new Uri(mReadmeLink);
            Process.Start(new ProcessStartInfo(mReadmeLink) { UseShellExecute = true });
            e.Handled = true;
        }
    }
    public class Components
    {
        public string Component { get; set; }

        public bool IsInstalled { get; set; }

        public bool IsRequired { get; set; }

        public string DownloadLink { get; set; }
    }
}
