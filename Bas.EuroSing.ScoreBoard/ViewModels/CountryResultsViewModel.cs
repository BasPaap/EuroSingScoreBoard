using Bas.EuroSing.ScoreBoard.Model;
using GalaSoft.MvvmLight;
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
        private int totalPoints;

        public int TotalPoints
        {
            get { return totalPoints; }
            set { Set(ref totalPoints, value); }
        }

        private int currentPoints;

        public int CurrentPoints
        {
            get { return currentPoints; }
            set { Set(ref currentPoints, value); }
        }

        private BitmapImage flagImage;

        public BitmapImage FlagImage
        {
            get { return flagImage; }
            set { Set(ref flagImage, value); }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { Set(ref name, value); }
        }

        private int id;

        public int Id
        {
            get { return id; }
            set { Set(ref id, value); }
        }

        public CountryResultsViewModel(Country country)
        {
            Id = country.Id;
            Name = country.Name;

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
