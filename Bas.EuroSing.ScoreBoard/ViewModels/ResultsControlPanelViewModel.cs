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

        public RelayCommand NextCommand { get; set; }

        public ResultsControlPanelViewModel(IDataService dataService)
        {
            this.dataService = dataService;
            SettingsCommand = new RelayCommand(() => MessengerInstance.Send(new GenericMessage<Message>(Message.ShowSettings)));

            Countries = new ObservableCollection<CountryResultsViewModel>(from c in this.dataService.GetAllCountries()
                                                                          select new CountryResultsViewModel(c, this.dataService));
            NextCommand = new RelayCommand(OnNextCommand);            
        }

        private void OnNextCommand()
        {
            resultsView = new ResultsView();
            this.resultsView.Show();
        }
    }
}
