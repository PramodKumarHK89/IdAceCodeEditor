using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using Microsoft.Identity.Client;
using System.Windows.Interop;
using Microsoft.Graph;
using System.Net.Http.Headers;
using Application = Microsoft.Graph.Application;
using LibGit2Sharp;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Windows.Storage.Streams;
using Microsoft.Extensions.Configuration;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace IdAceCodeEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
       

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string jsonString = System.IO.File.ReadAllText(@"Config\appsettings.json");
            var frameworks = JsonConvert.DeserializeObject<DataSource>(jsonString);
            this.DataContext = frameworks;
        }
    }
}
