using Bas.EuroSing.ScoreBoard.Messages;
using Bas.EuroSing.ScoreBoard.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Bas.EuroSing.ScoreBoard.ViewModels
{
    internal class ResultsViewModel : ViewModelBase
    {
        private IDataService dataService;
        public ObservableCollection<CountryResultsViewModel> Countries { get; set; }

        private string currentCountryName;

        public string CurrentCountryName
        {
            get { return currentCountryName; }
            set { Set(ref currentCountryName, value); }
        }

        private int currentCountryNumber;

        public int CurrentCountryNumber
        {
            get { return currentCountryNumber; }
            set { Set(ref currentCountryNumber, value); }
        }

        private BitmapImage currentCountryFlagImage;

        public BitmapImage CurrentCountryFlagImage
        {
            get { return currentCountryFlagImage; }
            set { Set(ref currentCountryFlagImage, value); }
        }

        
        public ResultsViewModel(IDataService dataService)
        {
            this.dataService = dataService;

            Countries = new ObservableCollection<CountryResultsViewModel>(from c in dataService.GetAllCountries()
                                                                           orderby c.Name
                                                                           select new CountryResultsViewModel(c));

            Messenger.Default.Register<ChangeStateMessage>(this, (message) =>
            {
                if (message.State == ResultsState.FirstGroupOfPoints)
                {
                    CurrentCountryNumber++;
                }

                CurrentCountryName = message.CurrentCountry.Name;
                
                if (message.CurrentCountry.FlagImage != null)
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CreateOptions = BitmapCreateOptions.None;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = new MemoryStream(message.CurrentCountry.FlagImage);
                    bitmapImage.EndInit();

                    CurrentCountryFlagImage = bitmapImage;
                }
            });
        }
    }
}
