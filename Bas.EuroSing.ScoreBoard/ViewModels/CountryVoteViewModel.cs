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
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Bas.EuroSing.ScoreBoard.ViewModels
{
    internal class CountryVoteViewModel : ViewModelBase
    {
        private IDataService dataService;
        private Vote vote;
        
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
                bool isFirstTimeNumPointsIsSet = false;
                if (numPoints == null)
                {
                    isFirstTimeNumPointsIsSet = true;
                }

                if (string.IsNullOrWhiteSpace(value) ||
                    (int.TryParse(value, out int pointValue) && (isFirstTimeNumPointsIsSet || availablePoints.Contains(pointValue))))
                {
                    Set(ref numPoints, value);
                }
                else
                {
                    Set(ref numPoints, string.Empty);
                }

                if (!isFirstTimeNumPointsIsSet)
                {
                    Messenger.Default.Send(new VoteCastMessage());

                    var parsedPoints = int.TryParse(numPoints, out int points) ? points : 0;
                    if (parsedPoints == 0)
                    {
                        var fromCountry = vote.FromCountry;
                        var toCountry = vote.ToCountry;

                        this.dataService.DeleteVote(vote);

                        vote = new Vote()
                        {
                            FromCountry = fromCountry,
                            FromCountryId = fromCountry.Id,
                            ToCountry = toCountry,
                            ToCountryId = toCountry.Id
                        };
                    }
                    else
                    {
                        vote.NumPoints = parsedPoints;
                        this.dataService.SaveVote(vote);
                    }                    
                }
            }
        }

        private IEnumerable<int> availablePoints;

        public CountryVoteViewModel(Vote vote, IDataService dataService, IEnumerable<int> availablePoints)
        {
            this.dataService = dataService;
            this.vote = vote;
            this.availablePoints = availablePoints;

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

            Messenger.Default.Register<VotesToCastUpdatedMessage>(this, (message) =>
            {
                this.availablePoints = message.VotesToCast;
            });
        }
    }
}
