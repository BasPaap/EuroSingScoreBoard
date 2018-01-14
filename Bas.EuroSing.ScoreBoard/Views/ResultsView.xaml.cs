using Bas.EuroSing.ScoreBoard.Messages;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace Bas.EuroSing.ScoreBoard.Views
{
    /// <summary>
    /// Interaction logic for ResultsView.xaml
    /// </summary>
    public partial class ResultsView : Window
    {
        public ResultsView()
        {
            InitializeComponent();
            
            Messenger.Default.Register<ChangeStateMessage>(this, (message) =>
            {
                switch (message.State)
                {
                    case ResultsState.SplashScreen:
                        VisualStateManager.GoToElementState(grid, SplashScreen.Name, true);
                        break;
                    case ResultsState.ScoreOverview:
                        VisualStateManager.GoToElementState(grid, ScoreOverview.Name, true);
                        break;
                    case ResultsState.FirstGroupOfPoints:
                        VisualStateManager.GoToElementState(grid, FirstGroupOfPoints.Name, true);
                        break;
                    case ResultsState.EightPoints:
                        VisualStateManager.GoToElementState(grid, EightPoints.Name, true);
                        break;
                    case ResultsState.TenPoints:
                        VisualStateManager.GoToElementState(grid, TenPoints.Name, true);
                        break;
                    case ResultsState.TwelvePoints:
                        VisualStateManager.GoToElementState(grid, TwelvePoints.Name, true);
                        break;
                    case ResultsState.None:
                    default:
                        break;
                }
            });

            foreach (var animation in scoreBoard.EntranceStoryboard.Children)
            {
                ToFirstGroupOfPointsStoryboard.Children.Add(animation);
            }
        }
        
        

        private void backgroundVideo_Loaded(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Play();            
        }

        private void backgroundVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.FromSeconds(0);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
    }
}
