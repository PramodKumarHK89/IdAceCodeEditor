using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IdAceCodeEditor
{
    /// <summary>
    /// Interaction logic for ManualAppdetailsCollection.xaml
    /// </summary>
    public partial class ManualAppdetailsCollection : Window
    {
        public ManualAppdetailsCollection()
        {
            InitializeComponent();
        }
        public ManualAppdetailsCollection(string clonnedPath, Project project)
        {
            InitializeComponent();
            project.ProjectPath = System.IO.Path.Combine(clonnedPath,project.ProjectPath);
            this.DataContext = project;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
