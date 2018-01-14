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
    internal class ResultsViewModel : ViewModelBase
    {
        private IDataService dataService;
        public ObservableCollection<CountryResultsViewModel> Countries { get; set; }

        public ResultsViewModel(IDataService dataService)
        {
            this.dataService = dataService;

            Countries = new ObservableCollection<CountryResultsViewModel>(from c in dataService.GetAllCountries()
                                                                           orderby c.Name
                                                                           select new CountryResultsViewModel(c));
        }
    }
}
