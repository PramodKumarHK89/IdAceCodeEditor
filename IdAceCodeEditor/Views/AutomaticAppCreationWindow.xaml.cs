using IdAceCodeEditor.ViewModels;
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

namespace IdAceCodeEditor.Views
{
    /// <summary>
    /// Interaction logic for AutomaticAppCreationWindow.xaml
    /// </summary>
    public partial class AutomaticAppCreationWindow : Window
    {
        public AutomaticAppCreationWindow()
        {
            InitializeComponent();
        }
        Project _project;
        public AutomaticAppCreationWindow(Project project)
        {
            _project = project;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AutoViewModel objViewModel = new AutoViewModel(_project);
            this.DataContext = objViewModel;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
