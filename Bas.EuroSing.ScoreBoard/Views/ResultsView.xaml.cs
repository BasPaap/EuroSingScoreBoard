using Bas.EuroSing.ScoreBoard.Messages;
using Bas.EuroSing.ScoreBoard.ViewModels;
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
                    case ResultsState.RevealCountry:
                        VisualStateManager.GoToElementState(grid, RevealCountry.Name, true);
                        break;
                    case ResultsState.FirstGroupOfPoints:
                        VisualStateManager.GoToElementState(grid, RevealPoints.Name, true);
                        break;
                    case ResultsState.ScoreOverview:
                    case ResultsState.EightPoints:
                    case ResultsState.TenPoints:
                    case ResultsState.TwelvePoints:
                    case ResultsState.None:
                    default:
                        break;
                }
            });

            foreach (var animation in scoreBoard.EntranceStoryboard.Children)
            {
                FromRevealCountryToRevealPointsStoryboard.Children.Add(animation);
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
        
        private void scoreBoard_EntranceAnimationCompleted(object sender, EventArgs e)
        {

            foreach (var animation in scoreBoard.EntranceStoryboard.Children)
            {
                FromRevealCountryToRevealPointsStoryboard.Children.Remove(animation);
            }

            (DataContext as ResultsViewModel).EntranceAnimationCompletedCommand.Execute(null);
        }

        private void scoreBoard_CurrentPointsUpdated(object sender, int e)
        {
            var storyboard = Resources["CurrentPointsUsedStoryboard"] as Storyboard;

            Grid grid;
            switch (e)
            {
                case 1:
                    grid = grid1;
                    break;
                case 2:
                    grid = grid2;
                    break;
                case 3:
                    grid = grid3;
                    break;
                case 4:
                    grid = grid4;
                    break;
                case 5:
                    grid = grid5;
                    break;
                case 6:
                    grid = grid6;
                    break;
                case 7:
                    grid = grid7;
                    break;
                case 8:
                    grid = grid8;
                    break;
                case 10:
                    grid = grid9;
                    break;
                case 12:
                    grid = grid10;
                    break;
                default:
                    grid = null;
                    break;
            }

            Storyboard.SetTarget(storyboard, grid);
            Storyboard.SetTargetProperty(storyboard, new PropertyPath("Opacity"));
            storyboard.Begin();
        }
        

        private void FromSplashScreenToRevealCountryStoryboard_Completed(object sender, EventArgs e)
        {
            Messenger.Default.Send(new RevealCountryCompletedMessage());
        }
    }
}
