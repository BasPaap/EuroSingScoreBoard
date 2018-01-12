using Bas.EuroSing.ScoreBoard.Messages;
using Bas.EuroSing.ScoreBoard.Model;
using Bas.EuroSing.ScoreBoard.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Bas.EuroSing.ScoreBoard.ViewModels
{
    internal class CountryResultsViewModel : ViewModelBase
    {
        private IDataService dataService;
        private Country country;

        public int Id { get; set; }

        private string name;

        public string Name
        {
            get { return name; }
            set { Set(ref name, value); }
        }

        private BitmapImage flagImage;

        public BitmapImage FlagImage
        {
            get { return flagImage; }
            set { Set(ref flagImage, value); }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                Set(ref isSelected, value);
                Messenger.Default.Send(new CountryResultClickedMessage());
            }
        }

        private bool isInQueue;

        public bool IsInQueue
        {
            get { return isInQueue; }
            set { Set(ref isInQueue, value); }
        }

        public RelayCommand ClickCommand { get; set; }

        public CountryResultsViewModel(Country country, IDataService dataService)
        {
            this.dataService = dataService;
            this.country = country;

            Id = country.Id;
            Name = country.Name;
            IsInQueue = true;

            ClickCommand = new RelayCommand(() => IsSelected = true );

            if (country.FlagImage != null)
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CreateOptions = BitmapCreateOptions.None;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = new MemoryStream(country.FlagImage);
                bitmapImage.EndInit();

                FlagImage = bitmapImage;
            }

        }
    }
}
