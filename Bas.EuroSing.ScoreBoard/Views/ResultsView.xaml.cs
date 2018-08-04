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

            // Handle any ChangeState messages coming from the main window.            
            Messenger.Default.Register<ChangeStateMessage>(this, (message) =>
            {
                var fromRevealCountryToRevealPointsStoryboard = Resources["FromRevealCountryToRevealPointsStoryboard"] as Storyboard;

                switch (message.State)
                {
                    case ResultsState.SplashScreen:
                        VisualStateManager.GoToElementState(grid, SplashScreen.Name, true);
                        break;


                    case ResultsState.RevealCountry:
                        ResetAnimationsForNewCountry(fromRevealCountryToRevealPointsStoryboard);
                        VisualStateManager.GoToElementState(grid, RevealCountry.Name, true);
                        break;

                    // When the first group of given points (everything up to eight) is to be shown, 
                    // Transition the main grid to the RevealPoints state and start the fromRevealCountryToRevealPointsStoryboard.
                    case ResultsState.FirstGroupOfPoints:
                        VisualStateManager.GoToElementState(grid, RevealPoints.Name, false);

                        fromRevealCountryToRevealPointsStoryboard.BeginTime = TimeSpan.Zero;
                        fromRevealCountryToRevealPointsStoryboard.Begin();

                        break;

                    case ResultsState.RevealWinner:
                        VisualStateManager.GoToElementState(grid, RevealWinner.Name, true);
                        break;

                    // No need for state transitions etc. for these actions, the ScoreBoard will handle any animations etc.
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
                var fromRevealCountryToRevealPointsStoryboard = Resources["FromRevealCountryToRevealPointsStoryboard"] as Storyboard;
                fromRevealCountryToRevealPointsStoryboard.Children.Add(animation);
            }
        }

        private void ResetAnimationsForNewCountry(Storyboard fromRevealCountryToRevealPointsStoryboard)
        {
            foreach (var animation in scoreBoard.EntranceStoryboard.Children)
            {
                fromRevealCountryToRevealPointsStoryboard.Children.Remove(animation);
            }

            scoreBoard.ResetScoreBoardItemAnimations();

            foreach (var animation in scoreBoard.EntranceStoryboard.Children)
            {
                fromRevealCountryToRevealPointsStoryboard.Children.Add(animation);
            }
        }

        // Workaround required to seamlessly loop an autoplaying video.
        private void backgroundVideo_Loaded(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Play();            
        }

        // Workaround required to seamlessly loop an autoplaying video.
        private void backgroundVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.FromSeconds(0);
        }

        // Enable the ability to move the resultsview by dragging it around with the mouse.
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }


        // Toggle fullscreen on doubleclick
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ToggleFullScreen();
        }

        private void ToggleFullScreen()
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

        // Signal to the viewmodel that the entrance animation has completed.
        private void scoreBoard_EntranceAnimationCompleted(object sender, EventArgs e)
        {
            (DataContext as ResultsViewModel).EntranceAnimationCompletedCommand.Execute(null);
        }

        // When the scoreboard signals us that the current points for a country has been updated,
        // the animation for the bottom "point tracking" row of dots should run for the dot that holds 
        // the currently given amount of points.
        private void scoreBoard_CurrentPointsUpdated(object sender, int pointAmount)
        {
            var storyboard = Resources["CurrentPointsUsedStoryboard"] as Storyboard;

            var controlToAnimate = GetControlToAnimate(pointAmount);
            RunPointsGivenAnimation(storyboard, controlToAnimate);
        }

        private static void RunPointsGivenAnimation(Storyboard storyboard, Grid controlToAnimate)
        {
            Storyboard.SetTarget(storyboard, controlToAnimate);
            Storyboard.SetTargetProperty(storyboard, new PropertyPath("Opacity"));
            storyboard.Begin();
        }

        private Grid GetControlToAnimate(int pointAmount)
        {
            Grid grid;
            switch (pointAmount)
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

            return grid;
        }


        // Let subscribers know that the transition from splash screen to first country reveal has been completed.
        private void FromSplashScreenToRevealCountryStoryboard_Completed(object sender, EventArgs e)
        {
            Messenger.Default.Send(new RevealCountryCompletedMessage());
        }

        private void FromRevealCountryToRevealPointsStoryboard_Completed(object sender, EventArgs e)
        {
            Debug.WriteLine($"FromRevCToRevP ({(sender as ClockGroup).Children.Count} children) completed.");
        }
    }
}
