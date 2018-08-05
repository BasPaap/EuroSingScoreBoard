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
using Bas.EuroSing.ScoreBoard.Extensions;

namespace Bas.EuroSing.ScoreBoard.ViewModels
{
    internal class CountryListItemViewModel : ViewModelBase
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

        private bool isInEditMode;

        public bool IsInEditMode
        {
            get { return isInEditMode; }
            set { Set(ref isInEditMode, value); }
        }

        private bool isComplete;

        public bool IsComplete
        {
            get { return isComplete; }
            set { Set(ref isComplete, value); }
        }
        
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        public RelayCommand StopEditCommand { get; set; }
        public RelayCommand<KeyEventArgs> KeyUpCommand { get; set; }

        public CountryListItemViewModel(Country country, IDataService dataService, int? numValidPoints = null)
        {
            this.dataService = dataService;

            DeleteCommand = new RelayCommand(OnDeleteCommand);
            EditCommand = new RelayCommand(() => { IsInEditMode = true; });

            StopEditCommand = new RelayCommand(async () => 
            {
                // Stop editing and change the country name if the StopEditCommand is executed.
                IsInEditMode = false;
                await this.dataService.ChangeCountryNameAsync(this.Id, this.Name);
            });

            KeyUpCommand = new RelayCommand<KeyEventArgs>((e) =>
            {
                // Stop editing if the enter key was pressed.
                if (e.Key == Key.Enter)
                {
                    StopEditCommand.Execute(null);
                }
            });

            Id = country.Id;
            Name = country.Name;
            FlagImage = country.FlagImage?.ToBitmapImage();                
            IsComplete = this.dataService.GetVotes(country.Id).Count(v => v.NumPoints > 0) == numValidPoints;            
        }

        // Send Delete message and tell the dataservice to delete the country.
        private async void OnDeleteCommand()
        {
            Messenger.Default.Send(new RemoveCountryMessage(this.Id));
            await this.dataService.DeleteCountryAsync(this.Id);
        }
    }
}
