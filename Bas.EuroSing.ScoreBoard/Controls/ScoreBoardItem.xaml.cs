using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bas.EuroSing.ScoreBoard.Controls
{
    /// <summary>
    /// Interaction logic for ScoreBoardItem.xaml
    /// </summary>
    public partial class ScoreBoardItem : UserControl
    {
        public ScoreBoardItem()
        {
            InitializeComponent();
        }

        private void CurrentPoints_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            var textBlock = e.Source as TextBlock;
            if (textBlock.Text != "0")
            {
                (Resources["ShowCurrentPointsStoryboard"] as Storyboard).Begin();

                if (int.TryParse(textBlock.Text, out int numPoints))
                {
                    CurrentPointsUpdated?.Invoke(this, numPoints);
                }
            }
        }

        public event EventHandler<int> CurrentPointsUpdated;
    }
}
