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

        private Dictionary<int, IEnumerable<Vote>> votesByIssuingCountry;

        private int? currentCountryId;

        public RelayCommand EntranceAnimationCompletedCommand { get; set; }

        public ResultsViewModel(IDataService dataService)
        {
            this.dataService = dataService;

            Countries = new ObservableCollection<CountryResultsViewModel>(from c in dataService.GetAllCountries()
                                                                           orderby c.Name
                                                                           select new CountryResultsViewModel(c));

            EntranceAnimationCompletedCommand = new RelayCommand(OnEntranceAnimationCompleted);
            Messenger.Default.Register<ChangeStateMessage>(this, OnChangeStateMessage);
            Messenger.Default.Register<GenericMessage<Message>>(this, (message) =>
            {
                if (message.Content == Message.ShowResultsControlPanel)
                {
                    votesByIssuingCountry = this.dataService.GetAllVotes();
                }
            });
        }

        private void OnEntranceAnimationCompleted()
        {
            for (int i = 0; i < 7; i++)
            {
                var vote = votesByIssuingCountry[currentCountryId.Value].OrderBy(v => v.NumPoints).ToArray()[i];
                var country = Countries.Single(c => c.Id == vote.ToCountryId);

                country.CurrentPoints = vote.NumPoints;
                country.TotalPoints += vote.NumPoints;
            }
        }

        private void OnChangeStateMessage(ChangeStateMessage message)
        {
            if (message.State == ResultsState.FirstGroupOfPoints)
            {
                CurrentCountryNumber++;
                currentCountryId = message.CurrentCountry.Id;
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
        }
    }
}
