using Bas.EuroSing.ScoreBoard.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bas.EuroSing.ScoreBoard.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        public ObservableCollection<CountryListItemViewModel> Countries { get; set; }

        public RelayCommand<DragEventArgs> DropCommand { get; set; }
        
        public SettingsViewModel(IDataService dataService)
        {
            DropCommand = new RelayCommand<DragEventArgs>(OnDropCommand);

            Countries = new ObservableCollection<CountryListItemViewModel>(from c in dataService.GetAllCountries()
                                                                           orderby c.Name
                                                                           select new CountryListItemViewModel() { Id = c.Id, Name = c.Name });
        }

        private void OnDropCommand(DragEventArgs e)
        {
            var filePaths = e.Data.GetData(DataFormats.FileDrop) as IEnumerable<string>;

            foreach (var filePath in filePaths)
            {
                var countryName = Path.GetFileNameWithoutExtension(filePath);
                InsertCountryOrdered(new CountryListItemViewModel() { Name = countryName });
            }
        }

        private void InsertCountryOrdered(CountryListItemViewModel newCountry)
        {
            foreach (var country in Countries)
            {
                if (string.Compare(country.Name, newCountry.Name) == 1)
                {
                    Countries.Insert(Countries.IndexOf(country), newCountry);
                    break;
                }
            }
        }
    }
}
