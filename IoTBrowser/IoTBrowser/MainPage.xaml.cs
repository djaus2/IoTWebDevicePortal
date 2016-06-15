// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Net;
using System.Net.Http;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoTBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            //webView.PermissionRequested += webView_PermissionRequested;
            webView.Navigate(new Uri("ms-appx-web:///login.htm"));

        }

        //private void webView_PermissionRequested(WebView sender, WebViewPermissionRequestedEventArgs args)
        //{
        //    if (args.PermissionRequest.PermissionType == WebViewPermissionType.Geolocation &&
        //        args.PermissionRequest.Uri.Host == "localhost:8080")
        //    {
        //        args.PermissionRequest.Allow();
        //    }
        //}

        private void Go_Web_Click(object sender, RoutedEventArgs e)
        {
            DoWebNavigate();
        }


        private void Web_Address_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                DoWebNavigate();
            }
        }

        private void DoWebNavigate()
        {
            //DismissMessage();

            try
            {
                if (Web_Address.Text.Length > 0)
                {
                    webView.Navigate(new Uri(Web_Address.Text));
                }
                else
                {
                    DisplayMessage("You need to enter a web address.");
                }
            }
            catch (Exception e)
            {
                DisplayMessage("Error: " + e.Message);
            }
        }

        //private void Go_WOD_Click(object sender, RoutedEventArgs e)
        //{
        //    Web_Address.Text = "https://www.windowsondevices.com";
        //    DoWebNavigate();
        //}

        //private void Go_Hackster_Click(object sender, RoutedEventArgs e)
        //{
        //    Web_Address.Text = "https://www.hackster.io/windowsiot";
        //    DoWebNavigate();
        //}

        //private void Go_GitHub_Click(object sender, RoutedEventArgs e)
        //{
        //    Web_Address.Text = "https://github.com/ms-iot";
        //    DoWebNavigate();
        //}

        //private async void Go_DeviceManager_Click(object sender, RoutedEventArgs e)
        //{


        //    HttpClientHandler handler = new HttpClientHandler();
        //    handler.Credentials = new NetworkCredential("administrator", "p@ssw0rd");
        //    HttpClient client = new HttpClient(handler);

        //    string body = await client.GetStringAsync("http://localhost:8080/default.htm");
        //    webView.NavigateToString(body);
        //    Web_Address.Text = "http://localhost:8080/AppManager.htm";
        //    DoWebNavigate();
        //}

        private void DisplayMessage(String message)
        {
            Message.Text = message;
            MessageStackPanel.Visibility = Visibility.Visible;
            webView.Visibility = Visibility.Collapsed;

        }

        private void OnMessageDismiss_Click(object sender, RoutedEventArgs e)
        {
            DismissMessage();
        }

        private void DismissMessage()
        {
            webView.Visibility = Visibility.Visible;
            MessageStackPanel.Visibility = Visibility.Collapsed;
        }

        bool loggedIn = false;
        bool doingLogIn = false;
        private async void webView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (doingLogIn)
                return;
            if (!loggedIn)
            {
                if (args.Uri.AbsolutePath != "/login.htm")
                    return;
                string query = args.Uri.Query;
                string[] queries = query.Split(new char[] { '?', '&' });
                if (queries.Length != 3)
                    return;
                if (queries[0] != "")
                    return;
                string userqry = queries[1];
                string[] usrs = userqry.Split(new char[] { '=' });
                if (usrs.Length != 2)
                    return;
                if (usrs[0] != "usr")
                    return;
                string usr = usrs[1];
                if (usr == "")
                    return;

                string pwdqry = queries[2];
                string[] pwds = pwdqry.Split(new char[] { '=' });
                if (pwds.Length != 2)
                    return;
                if (pwds[0] != "pwd")
                    return;
                string pwd = pwds[1];
                //pwd = pwd.Replace("%40", "&");
                pwd = System.Net.WebUtility.HtmlDecode(pwd);
                pwd = System.Net.WebUtility.UrlDecode(pwd);
                loggedIn = true;
                HttpClientHandler handler = new HttpClientHandler();
                handler.Credentials = new NetworkCredential(usr, pwd); // "administrator", "p@ssw0rd");
                HttpClient client = new HttpClient(handler);

                doingLogIn = true;
                string body = await client.GetStringAsync("http://localhost:8080/default.htm");
                webView.NavigateToString(body);
                Web_Address.Text = "http://localhost:8080/default.htm";
                DoWebNavigate();
            }
            else
                Web_Address.Text = args.Uri.AbsolutePath;
            doingLogIn = false;
        }

        private void Go_Back_Click(object sender, RoutedEventArgs e)
        {
            webView.GoBack();
        }
        private void Go_Fwd_Click(object sender, RoutedEventArgs e)
        {
            webView.GoForward();
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            SecondRow.Height = new Windows.UI.Xaml.GridLength(0);
            Header.Height = new Windows.UI.Xaml.GridLength(65);
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            if (!loggedIn)
                webView.Navigate(new Uri("ms-appx-web:///login.htm"));
            else
                webView.Navigate(new Uri("http://localhost:8080/default.htm"));
        } 
    }
}
