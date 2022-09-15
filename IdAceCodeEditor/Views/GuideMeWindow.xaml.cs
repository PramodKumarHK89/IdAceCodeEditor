using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.Devices.Lights;

namespace IdAceCodeEditor.Views
{
    /// <summary>
    /// Interaction logic for GuideMeWindow.xaml
    /// </summary>
    public partial class GuideMeWindow : Window
    {
        public GuideMeWindow(DataSource framework)
        {
            InitializeComponent();
            ObservableCollection<Sample> allSamples = new ObservableCollection<Sample>();
            foreach (var frame in framework.Frameworks)
            {
                foreach (var samp in frame.Samples)
                {
                    allSamples.Add(samp);
                }
            }
            framework.Samples = allSamples;
            this.DataContext = framework;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            var framworks = cmbFramework.ItemsSource;

            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(cmbFramework.Items);
            itemsViewOriginal.Filter = ((o) =>
            {
                return ((Framework)o).Type.Equals(radioButton.Tag);
            });
            itemsViewOriginal.Refresh();

            CollectionView itemsViewOriginal2 = (CollectionView)CollectionViewSource.GetDefaultView(lstScenario.Items);
            itemsViewOriginal2.Filter = ((o) =>
            {
                return ((Sample)o).PlatformType.Equals(radioButton.Tag);
            });
            itemsViewOriginal2.Refresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(cmbFramework.Items);
            itemsViewOriginal.Filter = ((o) =>
            {
                return true;
            });
            itemsViewOriginal.Refresh();
        }       
        private void cmbFramework_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            Framework framework= comboBox.SelectedItem as Framework;

            if (framework != null && comboBox!=null)
            {
                CollectionView itemsViewOriginal2 = (CollectionView)CollectionViewSource.GetDefaultView(lstScenario.Items);
                itemsViewOriginal2.Filter = ((o) =>
                {
                    return ((Sample)o).Type.Contains(framework.Name) ||
                    framework.Name.Contains(((Sample)o).Type);
                });
                itemsViewOriginal2.Refresh();
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        { 
            Framework framework= cmbFramework.SelectedItem as Framework;
            Sample sample = lstScenario.SelectedItem as Sample;

            if (sample != null && framework.Samples != null)
            {
                foreach (var item in framework.Samples)
                {
                    if (item.Name.Equals(sample.Name))
                        item.IsSelected = true;
                }
            }
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {            
            Framework framework = cmbFramework.SelectedItem as Framework;

            if (framework != null )
            {
                CollectionView itemsViewOriginal2 = (CollectionView)CollectionViewSource.GetDefaultView(
                    lstScenario.Items);
                itemsViewOriginal2.Filter = ((o) =>
                {
                    return ((Sample)o).Tags.Any(o=> o.Name.Equals("WithCertificate"));                    
                });
                itemsViewOriginal2.Refresh();
            }
        }
    }
}

