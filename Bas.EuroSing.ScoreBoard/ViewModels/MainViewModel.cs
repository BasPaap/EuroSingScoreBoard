using Bas.EuroSing.ScoreBoard.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.EuroSing.ScoreBoard.ViewModels
{
    public sealed class MainViewModel : ViewModelBase
    {
        enum View
        {
            None = 0,
            Vote,
            Settings,
            ResultsControlPanel
        }

        public MainViewModel()
        {
            MessengerInstance.Register<GenericMessage<Message>>(this, OnGenericMessageReceived);
            ShowView(View.Vote);
        }

        private void OnGenericMessageReceived(GenericMessage<Message> message)
        {
            switch (message.Content)
            {
                case Message.ShowSettings:
                    ShowView(View.Settings);
                    break;
                case Message.ShowVoteForm:
                    ShowView(View.Vote);
                    break;
                case Message.ShowResultsControlPanel:
                    ShowView(View.ResultsControlPanel);
                    break;
                case Message.None:
                default:
                    break;
            }
        }

        private void ShowView(View view)
        {
            IsSettingsViewVisible = false;
            IsVoteViewVisible = false;
            IsResultsControlPanelViewVisible = false;

            switch (view)
            {
                case View.Vote:
                    IsVoteViewVisible = true;
                    break;
                case View.Settings:
                    IsSettingsViewVisible = true;
                    break;
                case View.ResultsControlPanel:
                    IsResultsControlPanelViewVisible = true;
                    break;
                case View.None:
                default:
                    break;
            }
        }

        private bool isResultsControlPanelViewVisible;

        public bool IsResultsControlPanelViewVisible
        {
            get { return isResultsControlPanelViewVisible; }
            set { Set(ref isResultsControlPanelViewVisible, value); }
        }

        private bool isVoteViewVisible;

        public bool IsVoteViewVisible
        {
            get { return isVoteViewVisible; }
            set { Set(ref isVoteViewVisible, value); }
        }

        private bool isSettingsViewVisible;

        public bool IsSettingsViewVisible
        {
            get { return isSettingsViewVisible; }
            set { Set(ref isSettingsViewVisible, value); }
        }

    }
}
