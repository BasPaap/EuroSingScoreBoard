using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.EuroSing.ScoreBoard.ViewModels
{
    internal class VoteViewModel : ViewModelBase
    {
        private bool isVisible;
        public bool IsVisible
        {
            get { return isVisible; }
            set { Set(ref isVisible, value); }
        }
    }
}
