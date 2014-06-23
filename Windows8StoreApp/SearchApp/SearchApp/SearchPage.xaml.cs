using SplunkSearch.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Splunk.Client;
using System.Threading;
using System.Collections.ObjectModel;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
namespace SplunkSearch
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private CancellationTokenSource tokenSource;

        //private string searchTimeConstraint = "All Time";
        private string searchEarliestTime = null;
        private string searchLatestTime = null;

        private List<object> comboBoxItems = new List<object>();
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
        
        public SearchPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            if (MainPage.SplunkService != null)
            {
                UserName.Text = string.Format("User:{0}", loginUser);
                HostName.Text = string.Format("Server:{0}", MainPage.SplunkService.Server.Context.Host);
            }
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

        public static string loginUser { get; set; }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            tokenSource = new CancellationTokenSource();
            string searchStr = SearchInput.Text.Trim();

            titleGrid.Visibility = Visibility.Collapsed;

            if (!searchStr.StartsWith("search ", StringComparison.OrdinalIgnoreCase))
            {
                searchStr = "search " + searchStr;
            }

            this.PageContentSearchInProgress();

            CancellationToken token = tokenSource.Token;

            try
            {
                ObservableCollection<ResultData> resultDatas = new ObservableCollection<ResultData>();
                resultListView.DataContext = new CollectionViewSource { Source = resultDatas };

                SearchExportArgs jobArgs = new SearchExportArgs();

                if (TimeSelectComboBox.SelectedIndex == 1)
                {
                    if (this.searchEarliestTime != null)
                    {
                        jobArgs.EarliestTime = this.searchEarliestTime;
                    }

                    if (this.searchLatestTime != null)
                    {
                        jobArgs.LatestTime = this.searchLatestTime;
                    }
                }

                titleGrid.Visibility = Visibility.Visible;

                using (SearchResultStream resultStream = await MainPage.SplunkService.ExportSearchResultsAsync(searchStr, jobArgs))
                {
                    int resultCount = 0;

                    try
                    {
                        foreach (SearchResult result in resultStream)
                        {
                            if (!tokenSource.IsCancellationRequested)
                            {
                                List<string> results = this.ParseResult(result);
                                resultDatas.Add(new ResultData(++resultCount, results[0], results[1]));
                            }
                        }

                        if (resultStream.IsFinal)
                        {
                            this.PageContentReset();
                        }
                    }
                    catch
                    { }
                }
            }
            catch (Exception ex)
            {
                Windows.UI.Popups.MessageDialog messageDialog = new Windows.UI.Popups.MessageDialog(ex.ToString(), "Error in Search");
                messageDialog.Content = ex.ToString();
                messageDialog.ShowAsync().GetResults();
                titleGrid.Visibility = Visibility.Collapsed;
                this.PageContentReset();
            }
        }
    
        private void SearchCancelButton_Click(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
            SearchCancel.Content = "Cancelling...";
        }

        private void SearchTimeButton_Click(object sender, RoutedEventArgs e)
        {
        }
        private void backButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private List<string> ParseResult(SearchResult searchResult)
        {
            List<string> results = new List<string>();
            string rawData = searchResult.SegmentedRaw;

            //DateTime time = DateTime.Parse(searchResult["_time"]);
            //string format = "yyyy/M/d hh:mm:ss.fff";
            //results.Add(string.Format("{0}-{1}", ++eventCount, time.ToString(format)));

            string time = searchResult.GetValue("_time");
            time = time.Replace("Pacific Summer Time", "PST");
            results.Add(string.Format("{0}", time));

            rawData = rawData.Trim();
            //remove <v xml:space="preserve" trunc="0">
            if (rawData.StartsWith("<v xml:space="))
            {
                rawData = rawData.Remove(0, 34);
            }

            //remove </v>
            if (rawData.EndsWith("</v>"))
            {
                rawData = rawData.Substring(0, rawData.Length - 4);
            }

            results.Add(rawData);

            return results;
        }

        private void PageContentReset()
        {
            SearchSubmit.Content = "Search";
            SearchCancel.Content = "Cancel";
            SearchCancel.Visibility = Visibility.Collapsed;
            searchInProgress.IsActive = false;

        }

        private void PageContentSearchInProgress()
        {
            SearchSubmit.Content = "Searching";
            //SearchCancel.Content = "Cancel";
            //SearchCancel.Visibility = Visibility.Visible;
            searchInProgress.IsActive = true;
        }

        private class ResultData
        {
            public string Time { get; set; }
            public string Event { get; set; }
            public int Index { get; set; }

            public ResultData(int index, string time, string theEvent)
            {
                this.Index = index;
                this.Time = time;
                this.Event = theEvent;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TimeSelectComboBox != null)
            {
                if (TimeSelectComboBox.SelectedIndex == 0)
                {
                    //this.searchTimeConstraint = "All Time";
                    this.searchLatestTime = null;
                    this.searchLatestTime = null;
                }
                else if (TimeSelectComboBox.SelectedIndex == 1)
                {
                    this.searchEarliestTime = new DateTime(EarlistDate.Date.Year, EarlistDate.Date.Month, EarlistDate.Date.Day).AddSeconds(EarlistTime.Time.TotalSeconds).ToString("yyyy-MM-ddThh:mm:ss");
                    this.searchLatestTime = new DateTime(LatestDate.Date.Year, LatestDate.Date.Month, LatestDate.Date.Day).AddSeconds(LatestTime.Time.TotalSeconds).ToString("yyyy-MM-ddThh:mm:ss");
                }
            }
        }

        private void EarlistDate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            this.searchEarliestTime = new DateTime(EarlistDate.Date.Year, EarlistDate.Date.Month, EarlistDate.Date.Day).AddSeconds(EarlistTime.Time.TotalSeconds).ToString("yyyy-MM-ddThh:mm:ss");
        }

        private void EarlistTime_DateChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            this.searchEarliestTime = new DateTime(EarlistDate.Date.Year,EarlistDate.Date.Month,EarlistDate.Date.Day).AddSeconds(EarlistTime.Time.TotalSeconds).ToString("yyyy-MM-ddThh:mm:ss");             
        }

        private void LatestDate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            this.searchLatestTime = new DateTime(LatestDate.Date.Year, LatestDate.Date.Month, LatestDate.Date.Day).AddSeconds(LatestTime.Time.TotalSeconds).ToString("yyyy-MM-ddThh:mm:ss");
        }

        private void LatestTime_DateChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            this.searchLatestTime = new DateTime(LatestDate.Date.Year,LatestDate.Date.Month,LatestDate.Date.Day).AddSeconds(LatestTime.Time.TotalSeconds).ToString("yyyy-MM-ddThh:mm:ss");
        }
    }
}
