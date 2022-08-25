using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.Networking.NetworkOperators;
using Application = Microsoft.Graph.Application;

namespace IdAceCodeEditor
{
    /// <summary>
    /// Interaction logic for AppList.xaml
    /// </summary>
    public partial class AppList : Window
    {
        string[] LISTCOPES = new string[] { "user.read", "Application.Read.All" };

        public AuthenticationResult authResult;
        public Application app;
        public Project _project;
        Dictionary<string, string> _persistData;
        ConfigureAzureAdApp objConfigureApp;
        AppListViewModel objViewModel;
        public AppList()
        {
            InitializeComponent();
        }
        public AppList(Dictionary<string, string> persistData, string clonnedPath, Project project)
        {
            _persistData = persistData;
            _project = project;
            _project.AbsoluteProjectPath = System.IO.Path.Combine(clonnedPath, project.ProjectPath);
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            objConfigureApp = new ConfigureAzureAdApp(_persistData, _project);
            authResult = await objConfigureApp.AuthenticateWithAzureAd(LISTCOPES, this);

            objViewModel = new AppListViewModel(_project, objConfigureApp, _persistData);
            var listApps = await objConfigureApp.ListAppRegistrations(authResult.AccessToken);
            
            ObservableCollection<Application> myCollection = new ObservableCollection<Application>(listApps);
            objViewModel.Applications = myCollection;
            this.DataContext = objViewModel;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvUsers.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("CreatedDateTime", ListSortDirection.Descending));
            view.SortDescriptions.Add(new SortDescription("DisplayName", ListSortDirection.Ascending));

        }
        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;
        private void lvUsers_Click(object sender, RoutedEventArgs e)
        {


            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string header = headerClicked.Column.Header as string;
                    Sort(header, direction);

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }
        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(lvUsers.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var listApps = await objConfigureApp.ListAppRegistrations(authResult.AccessToken);

            ObservableCollection<Application> myCollection = new ObservableCollection<Application>(listApps);
            objViewModel.Applications = myCollection;
         
        }
    }
}
