using Bas.EuroSing.ScoreBoard.Messages;
using Bas.EuroSing.ScoreBoard.Model;
using Bas.EuroSing.ScoreBoard.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
        private IDataService dataService;

        public ObservableCollection<CountryListItemViewModel> Countries { get; set; }

        public RelayCommand<DragEventArgs> DropCommand { get; set; }
        public RelayCommand DeleteAllVotesCommand { get; set; }
        public RelayCommand BackCommand { get; set; }

        public SettingsViewModel(IDataService dataService)
        {
            this.dataService = dataService;

            DropCommand = new RelayCommand<DragEventArgs>(OnDropCommandAsync);
            DeleteAllVotesCommand = new RelayCommand(OnDeleteAllVotesCommandAsync);
            BackCommand = new RelayCommand(OnBackCommand);

            Countries = new ObservableCollection<CountryListItemViewModel>(from c in dataService.GetAllCountries()
                                                                           orderby c.Name
                                                                           select new CountryListItemViewModel(c));
        }

        private void OnBackCommand()
        {
            Messenger.Default.Send(new BackMessage());
        }

        private async void OnDeleteAllVotesCommandAsync()
        {
            if (MessageBox.Show("Are you sure you want to delete all votes?", "EuroSing 2018", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                await this.dataService.DeleteAllVotesAsync();
            }
        }

        private async void OnDropCommandAsync(DragEventArgs e)
        {
            var filePaths = e.Data.GetData(DataFormats.FileDrop) as IEnumerable<string>;

            foreach (var filePath in filePaths)
            {
                var countryName = Path.GetFileNameWithoutExtension(filePath);
                var imageBytes = File.ReadAllBytes(filePath);

                var newCountry = new Country() { Name = countryName, FlagImage = imageBytes };
                await this.dataService.AddCountryAsync(newCountry);
                
                InsertCountryOrdered(new CountryListItemViewModel(newCountry));
            }
        }

        private void InsertCountryOrdered(CountryListItemViewModel newCountry)
        {
            if (Countries.Count > 0)
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
            else
            {
                Countries.Add(newCountry);
            }
        }
    }
}
