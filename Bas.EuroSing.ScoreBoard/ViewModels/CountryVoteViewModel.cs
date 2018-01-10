using Bas.EuroSing.ScoreBoard.Messages;
using Bas.EuroSing.ScoreBoard.Model;
using Bas.EuroSing.ScoreBoard.Services;
using GalaSoft.MvvmLight;
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
    internal class CountryVoteViewModel : ViewModelBase
    {
        private IDataService dataService;

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

        private string numPoints;

        public string NumPoints
        {
            get { return numPoints; }
            set
            {
                Set(ref numPoints, value);

                int pointsValue;
                if (string.IsNullOrWhiteSpace(value) || int.TryParse(value, out pointsValue))
                {
                    Messenger.Default.Send(new VoteCastMessage());
                }
            }
        }

        public CountryVoteViewModel(Vote vote, IDataService dataService)
        {
            this.dataService = dataService;
            
            Id = vote.Id;
            Name = vote.ToCountry.Name;
            NumPoints = vote.NumPoints == 0 ? string.Empty : vote.NumPoints.ToString();

            if (vote.ToCountry.FlagImage != null)
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CreateOptions = BitmapCreateOptions.None;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = new MemoryStream(vote.ToCountry.FlagImage);
                bitmapImage.EndInit();

                FlagImage = bitmapImage;
            }
        }
    }
}
