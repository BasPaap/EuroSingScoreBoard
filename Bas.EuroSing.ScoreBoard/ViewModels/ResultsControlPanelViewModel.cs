using Bas.EuroSing.ScoreBoard.Messages;
using Bas.EuroSing.ScoreBoard.Services;
using Bas.EuroSing.ScoreBoard.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bas.EuroSing.ScoreBoard.ViewModels
{
    internal class ResultsControlPanelViewModel : ViewModelBase
    {
        private IDataService dataService;
        public RelayCommand SettingsCommand { get; set; }

        public ObservableCollection<CountryResultsViewModel> Countries { get; set; }

        private ResultsView resultsView;

        private ResultsState state = ResultsState.SplashScreen;

        public RelayCommand NextCommand { get; set; }
        public RelayCommand ClickCommand { get; set; }

        public ResultsControlPanelViewModel(IDataService dataService)
        {
            this.dataService = dataService;
            SettingsCommand = new RelayCommand(() => MessengerInstance.Send(new GenericMessage<Message>(Message.ShowSettings)));

            Countries = new ObservableCollection<CountryResultsViewModel>(from c in this.dataService.GetAllCountries()
                                                                          select new CountryResultsViewModel(c, this.dataService));
            NextCommand = new RelayCommand(OnNextCommand, CanNextCommandExecute);

            Messenger.Default.Register<CountryResultClickedMessage>(this, (message) => NextCommand.RaiseCanExecuteChanged());
        }

        private bool CanNextCommandExecute()
        {   
            if (this.state == ResultsState.None ||
                this.state == ResultsState.SplashScreen ||
                this.state == ResultsState.TwelvePoints ||
                GetSelectedCountry() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnNextCommand()
        {
            SetNextState();
            //resultsView = new ResultsView();
            //this.resultsView.Show();
        }

        private void SetNextState()
        {
            this.state = this.state != ResultsState.TwelvePoints ? this.state + 1 : ResultsState.ScoreOverview;

            if (this.state == ResultsState.ScoreOverview)
            {
                var selectedCountry = GetSelectedCountry();
                if (selectedCountry != null)
                {
                    selectedCountry.IsSelected = false;
                    selectedCountry.IsInQueue = false;

                    if (Countries.Count(c => c.IsInQueue) > 1)
                    {
                        Countries.First(c => c.IsInQueue).IsSelected = true;
                    }
                }
            }

            NextCommand.RaiseCanExecuteChanged();
        }

        private CountryResultsViewModel GetSelectedCountry()
        {
            return (from c in Countries
                    where c.IsSelected
                    select c).FirstOrDefault();
        }
    }
}
