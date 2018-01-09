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

namespace Bas.EuroSing.ScoreBoard.ViewModels
{
    internal class VoteViewModel : ViewModelBase
    {
        private IDataService dataService;

        public RelayCommand SettingsCommand { get; set; }


        public ObservableCollection<CountryListItemViewModel> Countries { get; set; }


        public VoteViewModel(IDataService dataService)
        {
            this.dataService = dataService;

            SettingsCommand = new RelayCommand(() => MessengerInstance.Send(new GenericMessage<Message>(Message.ShowSettings)));

            Countries = new ObservableCollection<CountryListItemViewModel>(from c in dataService.GetAllCountries()
                                                                           orderby c.Name
                                                                           select new CountryListItemViewModel(c, this.dataService));
        }
    }
}
