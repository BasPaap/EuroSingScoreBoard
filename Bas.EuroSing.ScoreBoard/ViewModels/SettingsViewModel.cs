using Bas.EuroSing.ScoreBoard.Services;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.EuroSing.ScoreBoard.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        public ObservableCollection<CountryListItemViewModel> Countries { get; set; }

        public SettingsViewModel(IDataService dataService)
        {
            Countries = new ObservableCollection<CountryListItemViewModel>(from c in dataService.GetAllCountries()
                                                                           select new CountryListItemViewModel() { Name = c.Name });
        }
    }
}
