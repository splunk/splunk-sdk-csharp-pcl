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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
namespace SplunkSearch
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class search : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private CancellationTokenSource cancelToken;
        private int eventCount = 0;
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


        public search()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            if (MainPage.SplunkService != null)
            {
                UserName.Text =string.Format("User:{0}", loginUser);
                HostName.Text = string.Format("Server:{0}",MainPage.SplunkService.Server.Context.Host);
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            eventCount = 0;
            cancelToken = new CancellationTokenSource();
            string searchStr = SearchInput.Text.Trim();
                        
            ResultRoot.Children.Clear();

            if (!searchStr.StartsWith("search ", StringComparison.OrdinalIgnoreCase))
            {
                searchStr = "search " + searchStr;
            }

            this.PageContentSearchInProgress();

            try
            {
                Job searchJob = await MainPage.SplunkService.SearchAsync(searchStr);
                this.DisplayResultInGrid("Event Time", "Event Details", true);

                while (!cancelToken.IsCancellationRequested)
                {
                    using (SearchResultStream resultStream = await searchJob.GetSearchResultsAsync())
                    {
                                                
                        foreach (Task<SearchResult> resultTask in resultStream)
                        {
                            SearchResult result = await resultTask;
                            List<string> results = this.ParseResult(result);
                            this.DisplayResultInGrid(results[0], results[1], false);
                        }

                        if (searchJob.IsDone)
                        {
                            break;
                        }
                    }
                }

                this.PageContentReset();          
            }
            catch (Exception ex)
            {
                this.PageContentReset();
                TextBlock textBlock = new TextBlock();
                textBlock.Text = ex.ToString();
                textBlock.FontSize = 16;
                ResultRoot.Children.Add(textBlock);
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DisplayResultInGrid(string column1, string column2, bool isGridTitle)
        {
            // display the result title  grid
            Grid resultGrid = new Grid();
            ColumnDefinition colDef = new ColumnDefinition();
            colDef.Width = new GridLength(200);
            resultGrid.ColumnDefinitions.Add(colDef);

            colDef = new ColumnDefinition();
            colDef.Width = new GridLength(500, GridUnitType.Star);
            resultGrid.ColumnDefinitions.Add(colDef);

            SolidColorBrush brush = new SolidColorBrush();
            Windows.UI.Color color;
            if (isGridTitle)
            {
                color = new Windows.UI.Color() { R = 0, G = 0, B = 0, A = 100 };
                Border border = new Border();
                border.BorderBrush = new SolidColorBrush(color);
                border.BorderThickness = new Thickness(1, 2, 1, 2);
                color = new Windows.UI.Color() { R = 155, G = 155, B = 155, A = 100 };
                border.Background = new SolidColorBrush(color);
                Grid.SetColumnSpan(border, 2);
                resultGrid.Children.Add(border);
            }
            else
            {
                color = new Windows.UI.Color() { R = 162, G = 159, B = 159, A = 100 };

                //define time column border
                Border border = new Border();
                border.BorderBrush = new SolidColorBrush(color);
                border.BorderThickness = new Thickness(1, 0, 0, 1);
                Grid.SetColumn(border, 0);
                resultGrid.Children.Add(border);

                //define event column border
                Border border2 = new Border();
                border2.BorderBrush = new SolidColorBrush(color);
                border2.BorderThickness = new Thickness(0, 0, 1, 1);
                Grid.SetColumn(border2, 1);
                resultGrid.Children.Add(border2);
            }

            int fontSize = isGridTitle ? 20 : 16;
            TextBlock textBlock = new TextBlock();
            textBlock.Text = column1;
            textBlock.FontSize = fontSize;
            resultGrid.Children.Add(textBlock);
            Grid.SetColumn(textBlock, 0);

            textBlock = new TextBlock();
            textBlock.Text = column2;
            textBlock.FontSize = fontSize;
            resultGrid.Children.Add(textBlock);
            Grid.SetColumn(textBlock, 1);

            ResultRoot.Children.Add(resultGrid);
        }

        private List<string> ParseResult(SearchResult searchResult)
        {
            List<string> results = new List<string>();
            string rawData = searchResult.SegmentedRaw;

            DateTime time = DateTime.Parse(searchResult["_time"]);
            string format = "yyyy/M/d hh:mm:ss.fff";
            results.Add(string.Format("{0}-{1}",++eventCount, time.ToString(format)));
       
            rawData=rawData.Trim();
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

        private  void SearchCancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancelToken.Cancel();
            SearchCancel.Content = "Cancelling...";
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
            SearchCancel.Content = "Cancel";
            SearchCancel.Visibility = Visibility.Visible;
            searchInProgress.IsActive = true;
        }

    }
}
