using Bas.EuroSing.ScoreBoard.Messages;
using Bas.EuroSing.ScoreBoard.Services;
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

        public ObservableCollection<CountryResultsControlViewModel> Countries { get; set; }

        private ResultsState state = ResultsState.None;

        public RelayCommand NextCommand { get; set; }
        public RelayCommand ClickCommand { get; set; }
        public RelayCommand BackCommand { get; set; }

        public ResultsControlPanelViewModel(IDataService dataService)
        {
            this.dataService = dataService;
            SettingsCommand = new RelayCommand(() => MessengerInstance.Send(new GenericMessage<Message>(Message.ShowSettings)));

            Countries = new ObservableCollection<CountryResultsControlViewModel>(from c in this.dataService.GetAllCountries()
                                                                          select new CountryResultsControlViewModel(c, this.dataService));
            NextCommand = new RelayCommand(OnNextCommand, CanNextCommandExecute);
            BackCommand = new RelayCommand(OnBackCommand);

            Messenger.Default.Register<CountryResultClickedMessage>(this, (message) =>
            {
                NextCommand.RaiseCanExecuteChanged();
                Messenger.Default.Send(new SetNextCountryMessage(GetSelectedCountry()));
            });
            Messenger.Default.Register<RevealCountryCompletedMessage>(this, (message) => SetNextState());

            if (Countries.Count > 0)
            {
                Countries.First().IsSelected = true;
            }
            SetNextState();
        }

        private void OnBackCommand()
        {
            Messenger.Default.Send(new BackMessage());
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
        }

        public CountryResultsControlViewModel currentlyRevealedCountry;

        private void SetNextState()
        {
            this.state = this.state != ResultsState.TwelvePoints ? this.state + 1 : ResultsState.RevealCountry;

            if (this.state == ResultsState.RevealCountry)
            {
                this.currentlyRevealedCountry = GetSelectedCountry();
            }

            if (this.state == ResultsState.TwelvePoints)
            {
                // Dit klopt dus niet, want je kan inmiddels een ander land geselecteerd hebben. Zodra je naar REvealcountry gaat 
                // moet het onthulde land vastgelegd worden, en -die- moet hier verwijderd worden.
                                
                if (this.currentlyRevealedCountry != null)
                {
                    this.currentlyRevealedCountry.IsSelected = false;
                    this.currentlyRevealedCountry.IsInQueue = false;

                    if (Countries.Count(c => c.IsInQueue) > 1)
                    {
                        Countries.First(c => c.IsInQueue).IsSelected = true;
                    }
                }
            }

            Messenger.Default.Send(new ChangeStateMessage(this.state, dataService.GetCountry(Countries.FirstOrDefault(c => c.IsSelected).Id)));
            NextCommand.RaiseCanExecuteChanged();
        }

        private CountryResultsControlViewModel GetSelectedCountry()
        {
            return (from c in Countries
                    where c.IsSelected
                    select c).FirstOrDefault();
        }
    }
}
