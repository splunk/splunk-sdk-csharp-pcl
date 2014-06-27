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
using System.Diagnostics;

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

        private CancellationTokenSource cancelSearchTokenSource;

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
                UserName1.Text = string.Format("User: ");
                UserName2.Text = string.Format(" {0}", loginUser);
                HostName1.Text = string.Format("Server:");
                HostName2.Text = string.Format(" {0}", MainPage.SplunkService.Server.Context.Host);
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
            cancelSearchTokenSource = new CancellationTokenSource();

            string searchStr = SearchInput.Text.Trim();
            if (!searchStr.StartsWith("search ", StringComparison.OrdinalIgnoreCase))
            {
                searchStr = "search " + searchStr;
            }

            titleGrid.Visibility = Visibility.Collapsed;
            this.PageContentSearchInProgress();

            try
            {
                if (TimeSelectComboBox.SelectedIndex == 1)
                {
                    this.DisplaySearchPreviewResult(searchStr);
                }
                else
                {
                    this.DisplaySearchResult(searchStr);
                }
            }
            catch (Exception ex)
            {
                Windows.UI.Popups.MessageDialog messageDialog = new Windows.UI.Popups.MessageDialog(ex.ToString(), "Error in Search");
                messageDialog.ShowAsync();

                titleGrid.Visibility = Visibility.Collapsed;
                this.PageContentReset();
            }
        }

        private async void DisplaySearchPreviewResult(string searchStr)
        {
            int resultCount = 0;
            ObservableCollection<ResultData> resultDatas = new ObservableCollection<ResultData>();
            resultListView.DataContext = new CollectionViewSource { Source = resultDatas };
            JobArgs args = new JobArgs();
            args.EarliestTime = this.searchEarliestTime;
            args.LatestTime = this.searchLatestTime;
            args.SearchMode = SearchMode.RealTime;
            Job realtimeJob = await MainPage.SplunkService.Jobs.CreateAsync(searchStr, args);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            int count = 0;

            do
            {
                using (SearchResultStream stream = await realtimeJob.GetSearchPreviewAsync())
                {
                    titleGrid.Visibility = Visibility.Visible;

                    foreach (SearchResult result in stream)
                    {
                        count++;
                        List<string> results = this.ParseResult(result);
                        resultDatas.Add(new ResultData(++resultCount, results[0], results[1]));
                    }
                    await Task.Delay(1000);
                }

            } while ((count == 0 || count < 2000) && watch.Elapsed.TotalSeconds <= 5 && !this.cancelSearchTokenSource.Token.IsCancellationRequested);

            this.PageContentReset();
            await realtimeJob.CancelAsync();
        }

        private async void DisplaySearchResult(string searchStr)
        {
            int resultCount = 0;
            SearchExportArgs jobArgs = new SearchExportArgs();

            if (this.searchEarliestTime != null)
            {
                jobArgs.EarliestTime = this.searchEarliestTime;
            }

            if (this.searchLatestTime != null)
            {
                jobArgs.LatestTime = this.searchLatestTime;
            }

            ObservableCollection<ResultData> resultDatas = new ObservableCollection<ResultData>();
            resultListView.DataContext = new CollectionViewSource { Source = resultDatas };
            using (SearchResultStream resultStream = await MainPage.SplunkService.ExportSearchResultsAsync(searchStr, jobArgs))
            {
                titleGrid.Visibility = Visibility.Visible;

                int count = 0;
                foreach (SearchResult result in resultStream)
                {
                    List<string> results = this.ParseResult(result);
                    resultDatas.Add(new ResultData(++resultCount, results[0], results[1]));

                    //TODO: need to do paging
                    if (count++ > 2000 || this.cancelSearchTokenSource.Token.IsCancellationRequested) break;
                }

                this.PageContentReset();
            }
        }

        private void SearchCancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancelSearchTokenSource.Cancel();
            SearchCancel.Content = "Cancelling...";
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
            SearchCancel.Content = "Cancel";
            SearchCancel.Visibility = Visibility.Visible;
            SearchSubmit.Content = "Searching";
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

        private void ApplyRelativeTimeSearchClick()
        {
            int relativeTime = 0;
            try
            {
                if ((relativeTime = int.Parse(RelativeEarlistTimeValue.Text.Trim().TrimEnd(' '))) <= 0)
                {
                    throw new Exception("value must be positive value");
                }
            }
            catch (Exception ex)
            {
                string msg = "Invalid input: value must be positive integer";
                Windows.UI.Popups.MessageDialog messageDialog = new Windows.UI.Popups.MessageDialog(msg, "Error in Input");
                messageDialog.ShowAsync();
            }

            string unit = "s";
            switch (RelativeEarlistTimeValueUnit.SelectedIndex)
            {
                case 1: unit = "m"; break;
                case 2: unit = "h"; break;
                case 3: unit = "d"; break;
            }

            this.searchEarliestTime = string.Format("rt-{0}{1}", relativeTime, unit);
            this.searchLatestTime = "rt";
        }

        private void ApplyDateTimeRangeClick()
        {
            DateTime start = new DateTime(EarlistDate.Date.Year, EarlistDate.Date.Month, EarlistDate.Date.Day).AddSeconds(EarlistTime.Time.TotalSeconds);
            DateTime end = new DateTime(LatestDate.Date.Year, LatestDate.Date.Month, LatestDate.Date.Day).AddSeconds(LatestTime.Time.TotalSeconds);
            this.searchEarliestTime = start.ToString("yyyy-MM-ddThh:mm:ss");
            this.searchLatestTime = end.ToString("yyyy-MM-ddThh:mm:ss");
            if (start >= end)
            {
                string msg = "Latest time must be greater than earlist time";
                Windows.UI.Popups.MessageDialog messageDialog = new Windows.UI.Popups.MessageDialog(msg, "Error in Input");
                messageDialog.ShowAsync();
            }
        }

        private void ApplyAdvancedTimeSelectionClick()
        {
            try
            {
                DateTime start = DateTime.Parse(customerEarlistInput.Text.Trim().TrimEnd(' '));
                DateTime end = DateTime.Parse(customerLatestInput.Text.Trim().TrimEnd(' '));
                this.searchEarliestTime = start.ToString("yyyy-MM-ddThh:mm:ss");
                this.searchLatestTime = end.ToString("yyyy-MM-ddThh:mm:ss");
                if (start >= end)
                {
                    throw new Exception("Latest time must be greater than earlist time");
                }
            }
            catch (Exception ex)
            {
                Windows.UI.Popups.MessageDialog messageDialog = new Windows.UI.Popups.MessageDialog(ex.Message, "Error in Input");
                messageDialog.ShowAsync();
            }
        }

        private void TimeSelectComboBox_DropDownClosed(object sender, object e)
        {
            if (TimeSelectComboBox != null)
            {
                if (TimeSelectComboBox.SelectedIndex == 0)
                {
                    //"All Time";
                    this.searchLatestTime = null;
                    this.searchLatestTime = null;
                }

                if (TimeSelectComboBox.SelectedIndex == 1)
                {
                    //Relative time
                    this.ApplyRelativeTimeSearchClick();
                }

                if (TimeSelectComboBox.SelectedIndex == 2)
                {
                    //Date and Time range
                    this.ApplyDateTimeRangeClick();
                }
                else if (TimeSelectComboBox.SelectedIndex == 3)
                {
                    //Advanced
                    this.ApplyAdvancedTimeSelectionClick();
                }
            }
        }
    }
}
