using Bas.EuroSing.ScoreBoard.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.EuroSing.ScoreBoard.ViewModels
{
    internal class ResultsControlPanelViewModel : ViewModelBase
    {
        public RelayCommand SettingsCommand { get; set; }

        public ResultsControlPanelViewModel()
        {
            SettingsCommand = new RelayCommand(() => MessengerInstance.Send(new GenericMessage<Message>(Message.ShowSettings)));
        }
    }
}
