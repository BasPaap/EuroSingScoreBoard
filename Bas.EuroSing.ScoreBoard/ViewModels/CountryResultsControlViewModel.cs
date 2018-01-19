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
    internal class CountryResultsControlViewModel : ViewModelBase
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

        public ObservableCollection<CountryListItemViewModel> CountriesToGiveEightPointsTo { get; set; }
        public ObservableCollection<CountryListItemViewModel> CountriesToGiveTenPointsTo { get; set; }
        public ObservableCollection<CountryListItemViewModel> CountriesToGiveTwelvePointsTo { get; set; }

        private CountryListItemViewModel eightPointsVote;

        public CountryListItemViewModel EightPointsVote
        {
            get { return eightPointsVote; }
            set
            {
                Set(ref eightPointsVote, value);
                Messenger.Default.Send(new UpdateCountriesToGivePointsToMessage(), this.Id);
            }
        }

        private CountryListItemViewModel tenPointsVote;

        public CountryListItemViewModel TenPointsVote
        {
            get { return tenPointsVote; }
            set
            {
                Set(ref tenPointsVote, value);
                Messenger.Default.Send(new UpdateCountriesToGivePointsToMessage(), this.Id);
            }
        }

        private CountryListItemViewModel twelvePointsVote;

        public CountryListItemViewModel TwelvePointsVote
        {
            get { return twelvePointsVote; }
            set
            {
                Set(ref twelvePointsVote, value);
                Messenger.Default.Send(new UpdateCountriesToGivePointsToMessage(), this.Id);
            }
        }

        public CountryResultsControlViewModel(Country country, IDataService dataService)
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

            var votes = dataService.GetIssuedVotes(country.Id);
            var eightPoints = votes.FirstOrDefault(v => v.NumPoints == 8);
            var tenPoints = votes.FirstOrDefault(v => v.NumPoints == 10);
            var twelvePoints = votes.FirstOrDefault(v => v.NumPoints == 12);

            EightPointsVote = eightPoints != null ? new CountryListItemViewModel(eightPoints.ToCountry, dataService) : null;
            TenPointsVote = tenPoints != null ? new CountryListItemViewModel(tenPoints.ToCountry, dataService) : null;
            TwelvePointsVote = twelvePoints != null ? new CountryListItemViewModel(twelvePoints.ToCountry, dataService) : null;

            Messenger.Default.Register<UpdateCountriesToGivePointsToMessage>(this, Id, OnUpdateCountriesToGivePointsTo);
            OnUpdateCountriesToGivePointsTo(null);
        }

        private void OnUpdateCountriesToGivePointsTo(UpdateCountriesToGivePointsToMessage message)
        {
            CountriesToGiveEightPointsTo = new ObservableCollection<CountryListItemViewModel>(GetCountriesToGivePointsTo(8));

            CountriesToGiveTenPointsTo = new ObservableCollection<CountryListItemViewModel>(GetCountriesToGivePointsTo(10));

            CountriesToGiveTwelvePointsTo = new ObservableCollection<CountryListItemViewModel>(GetCountriesToGivePointsTo(12));
        }

        private List<CountryListItemViewModel> GetCountriesToGivePointsTo(int numPoints)
        {
            Collection<Country> countries = dataService.GetCountriesToGiveVotesTo(Id, numPoints );

            var countryViewmodels = from c in countries
                                    where c.Id != this.Id
                                    select new CountryListItemViewModel(c, dataService);

            return new List<CountryListItemViewModel>(countryViewmodels);
        }
    }
}
