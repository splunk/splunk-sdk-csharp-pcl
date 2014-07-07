using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Security.Credentials;
using Splunk.Client;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace SplunkSearch
{
    public sealed partial class ConnectSetting : SettingsFlyout
    {
        public static string ResourceName
        {
            get { return "SplunkSearchLogin"; }
        }

        public ConnectSetting()
        {
            this.InitializeComponent();
            this.LoadSettings();
        }

        private async void TestConnection_Click(object sender, RoutedEventArgs e)
        {

            Scheme scheme=Scheme.Https;
            int port = 8089;
            if (CheckInput(ref port, ref scheme))
            {
                try
                {
                    Service SplunkService = new Service(scheme, splunkHost.Text, port);
                    await SplunkService.LogOnAsync(inputUser.Text, inputPassword.Password);
                    OutputInfo.Text = "log on successfully and credential saved!";
                    this.SaveSettings(inputUser.Text, inputPassword.Password, splunkHost.Text, inputPort.Text, scheme);
                }
                catch (Exception ex)
                {
                    OutputInfo.Text = ex.Message;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Scheme scheme = Scheme.Https;
            int port = 8089;
            if (CheckInput(ref port, ref scheme))
            {
                this.SaveSettings(inputUser.Text, inputPassword.Password, splunkHost.Text, inputPort.Text, scheme);
            }
        }

        private bool SaveSettings(string user, string password, string host, string port, Scheme scheme)
        {
            var vault = new PasswordVault();
            foreach (var pwdCredential in vault.RetrieveAll())
            {
                vault.Remove(pwdCredential);
            }

            vault.Add(new PasswordCredential(ResourceName, user, password));

            var res = vault.FindAllByUserName(user);
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["host"] = host;
            localSettings.Values["scheme"] = scheme.ToString();
            localSettings.Values["port"] = port.ToString();       
            MainPage.SplunkService = null;
            return true;
        }

        private bool CheckInput(ref int port, ref Scheme scheme)
        {
            int temp;
            if(inputUser.Text==string.Empty)
            {
                OutputInfo.Text = "User can't be empty";
                return false;
            }
            
            if (int.TryParse(inputPort.Text, out temp))
            {
                port = temp;
            }
            else
            {
                OutputInfo.Text = "Port should to be integer";
                MainPage.SplunkService = null;
                return false;
            }

            scheme = Scheme.Https;
            if (Http.IsChecked == true)
            {
                scheme = Scheme.Http;
            }

            return true;
        }

        private void LoadSettings()
        {
            var vault = new PasswordVault();

            try
            {
                var credentialList = vault.FindAllByResource(ResourceName);

                inputUser.Text = credentialList[0].UserName;
                credentialList[0].RetrievePassword();
                inputPassword.Password = credentialList[0].Password;

                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

                splunkHost.Text = localSettings.Values["host"].ToString();
                inputPort.Text = localSettings.Values["port"].ToString();
                if (string.Equals(localSettings.Values["scheme"].ToString(), "Http", StringComparison.OrdinalIgnoreCase))
                {
                    Http.IsChecked = true;
                }
            }
            catch { }
        }
    }
}
