using Bas.EuroSing.ScoreBoard.Model;
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
        private IDataService dataService;

        public ObservableCollection<CountryListItemViewModel> Countries { get; set; }

        public RelayCommand<DragEventArgs> DropCommand { get; set; }
        
        public SettingsViewModel(IDataService dataService)
        {
            this.dataService = dataService;

            DropCommand = new RelayCommand<DragEventArgs>(OnDropCommandAsync);


            //TODO: REMOVE -----------------
            System.Windows.Media.Imaging.BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.None;
            bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = File.OpenRead("Assets\\Netherlands.png");
            bitmapImage.EndInit();
            //TODO: AAAAAAARG
            Countries = new ObservableCollection<CountryListItemViewModel>(from c in dataService.GetAllCountries()
                                                                           orderby c.Name
                                                                           select new CountryListItemViewModel(c) { FlagImage = bitmapImage });
        }

        private async void OnDropCommandAsync(DragEventArgs e)
        {
            var filePaths = e.Data.GetData(DataFormats.FileDrop) as IEnumerable<string>;

            foreach (var filePath in filePaths)
            {
                var countryName = Path.GetFileNameWithoutExtension(filePath);
                var imageBytes = File.ReadAllBytes(filePath);

                var newCountry = new Country() { Name = countryName };
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
