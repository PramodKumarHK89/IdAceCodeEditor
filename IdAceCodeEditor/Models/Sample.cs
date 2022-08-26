using IdAceCodeEditor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace IdAceCodeEditor
{
    public class Sample
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }

        public string WorkingFolder { get; set; }
        public GitHubRepo GitHubRepoSettings{ get; set; }
        public ObservableCollection<Project> Projects { get; set; }
        public List<Prerequisite> Prerequisites { get; set; }
        
        public List<Tag> Tags{ get; set; }

        //public Dictionary<string, string> persistData = new Dictionary<string, string>();

        private ICommand _configureWithAzureAdCommand;
        
        public ICommand ConfigureWithAzureAdCommand
        {
            get
            {
                if (_configureWithAzureAdCommand == null)
                {
                    _configureWithAzureAdCommand = new RelayCommand(
                        param => this.ConfigureWithAzureAd(param),
                        param => this.CanConfigurewithAzureAd()
                    );
                }
                return _configureWithAzureAdCommand;
            }
        }

        private bool CanConfigurewithAzureAd()
        {
            return true;
            // Verify command can be executed here
        }

        private void ConfigureWithAzureAd(object o)
        {
            //clone
            CloneService objCloneService = new CloneService();
            var clonnedPath = //@"C:\t\Debug\netcoreapp3.1\LocalRepo\ASP.NETCORE\Scenario 2\2022-25-5--21-20-27";
                objCloneService.CloneRepo(this.GitHubRepoSettings);

            bool isCorrect = true;
            //show automatic window
            Dictionary<string, string> persistData = new Dictionary<string, string>();

            foreach (var item in Projects)
            {
                AppList objWindow = new AppList(persistData, clonnedPath, item);
                objWindow.Owner = (Window)o;
                if (objWindow.ShowDialog() == false)
                {
                    isCorrect = false;
                }
            }           
            // List prerequitess 
            if (isCorrect)
            {
                Prerequisites obWindow = new Prerequisites(Type,
                                                                  GitHubRepoSettings.ReadMeLink,
                                                                  GitHubRepoSettings.TutorialLink,
                                                                  System.IO.Path.Combine(clonnedPath,WorkingFolder),
                                                                  Prerequisites);
                obWindow.ShowDialog();
            }
            foreach (var item in Projects)
            {
                ClearMapping(item.ReplacementFields);
            }

            // Save command execution logic
        }
        private ICommand _configureManuallyCommand;

        public ICommand ConfigureManuallyCommand
        {
            get
            {
                if (_configureManuallyCommand == null)
                {
                    _configureManuallyCommand = new RelayCommand(
                        param => this.ConfigureManually(param),
                        param => this.CanConfigureManually()
                    );
                }
                return _configureManuallyCommand;
            }
        }

        private bool CanConfigureManually()
        {
            return true;
            // Verify command can be executed here
        }

        private void ConfigureManually(object o)
        {

            //clone
            CloneService objCloneService = new CloneService();
            var clonnedPath =
               //@"C:\t\Debug\netcoreapp3.1\LocalRepo\ASP.NETCORE\Scenario 2\2022-25-5--21-20-27";
            objCloneService.CloneRepo(this.GitHubRepoSettings);

            //show manual window
            bool isCorrect = true;
            foreach (var item in Projects)
            {
                ManualAppdetailsCollection objWindow = new ManualAppdetailsCollection(clonnedPath, item);
                objWindow.Owner = (Window)o;
                if (objWindow.ShowDialog() == false)
                {
                    isCorrect = false;
                }
            }

            //list prerequites
            if (isCorrect)

            {
                Prerequisites obWindow = new Prerequisites(Type,
                                                          GitHubRepoSettings.ReadMeLink,
                                                          GitHubRepoSettings.TutorialLink,
                                                          System.IO.Path.Combine(clonnedPath, WorkingFolder),
                                                          Prerequisites);
                obWindow.ShowDialog();
            }
            foreach (var item in Projects)
            {
                ClearMapping(item.ReplacementFields);
            }

        }
        private void ClearMapping(List<ReplacementField> replacementFields)
        {
            foreach (var item in replacementFields)
            {
                item.Value = String.Empty;
            }
        }
            }
}
