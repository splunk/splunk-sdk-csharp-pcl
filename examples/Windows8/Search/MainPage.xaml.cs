using SplunkSearch.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
namespace SplunkSearch
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private string user = string.Empty;
        private string password = string.Empty;
        private string host = string.Empty;
        private int port = 8089;
        private Scheme schem = Scheme.Https;

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        internal static Service SplunkService
        {
            get;
            set;
        }

        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //try to load search page if signin succeed
            this.SigninButton_Click(new Windows.UI.Xaml.Controls.Button(), new Windows.UI.Xaml.RoutedEventArgs());
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion


        private async void SigninButton_Click(object sender, RoutedEventArgs e)
        {
            if (TryLoadSettings())
            {
                if (await ConnectToSplunkServer())
                {
                    this.Frame.Navigate(typeof(SearchPage));
                };
            }
        }

        private bool TryLoadSettings()
        {
            var vault = new PasswordVault();

            try
            {

                var credentialList = vault.FindAllByResource(ConnectSetting.ResourceName);
                user = credentialList[0].UserName;
                credentialList[0].RetrievePassword();
                password = credentialList[0].Password;

                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

                host = localSettings.Values["host"].ToString();
                var portString = localSettings.Values["port"].ToString();
                port = int.Parse(portString);
                if (string.Equals(localSettings.Values["scheme"].ToString(), "Http", StringComparison.OrdinalIgnoreCase))
                {
                    schem = Scheme.Http;
                }

                return true;
            }
            catch (Exception)
            {
                OutputInfo.Text = "Please set connection information from the Settings menu";
                return false;
            }
        }

        private async Task<bool> ConnectToSplunkServer()
        {
            try
            {
                SplunkService = new Service(schem, host, port);

                InProgress.IsActive = true;
                await SplunkService.LogOnAsync(user, password);
                InProgress.IsActive = false;
                SearchPage.loginUser = user;

                return true;
            }
            catch (Exception ex)
            {
                InProgress.IsActive = false;
                SplunkService = null;
                OutputInfo.Text = string.Format("{0}\nCan't connect to {1}://{2}:{3} as user \"{4}\"\nChange Custom Setting in the Settings", ex.Message, schem.ToString(), host, port, user);
            }

            return false;
        }
    }
}
