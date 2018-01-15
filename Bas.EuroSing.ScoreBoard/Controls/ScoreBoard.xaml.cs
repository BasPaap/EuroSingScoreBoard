using Bas.EuroSing.ScoreBoard.Messages;
using Bas.EuroSing.ScoreBoard.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bas.EuroSing.ScoreBoard.Controls
{
    /// <summary>
    /// Interaction logic for ScoreBoard.xaml
    /// </summary>
    public partial class ScoreBoard : UserControl
    {
        public ScoreBoard()
        {
            InitializeComponent();

            EntranceStoryboard = new Storyboard();
            Messenger.Default.Register<ReorderCountriesMessage>(this, OnReorderCountriesMessage);
        }


        private void OnReorderCountriesMessage(ReorderCountriesMessage message)
        {
            var orderedItems = Items.OrderByDescending(i => i.TotalPoints).ThenBy(i => i.Name).ToList();

            var reorderStoryboard = new Storyboard();
            foreach (var scoreBoardItem in rootCanvas.Children.OfType<ScoreBoardItem>())
            {
                var viewModel = scoreBoardItem.DataContext as CountryResultsViewModel;
                var newIndex = orderedItems.IndexOf(orderedItems.Single(i => i.Id == viewModel.Id));

                var reorderAnimation = new DoubleAnimation(Canvas.GetTop(scoreBoardItem), newIndex * itemHeight, TimeSpan.FromSeconds(1.0))
                {
                    BeginTime = TimeSpan.FromSeconds(1.0),
                    FillBehavior = FillBehavior.Stop,
                    EasingFunction = new CubicEase()
                    {
                        EasingMode = EasingMode.EaseOut
                    }
                };
                reorderAnimation.Completed += (sender, e) => HoldEndPosition(sender);
                Storyboard.SetTarget(reorderAnimation, scoreBoardItem);
                Storyboard.SetTargetProperty(reorderAnimation, new PropertyPath(Canvas.TopProperty));
                reorderStoryboard.Children.Add(reorderAnimation);
            }
            reorderStoryboard.Begin();
        }

        internal ObservableCollection<CountryResultsViewModel> Items
        {
            get { return (ObservableCollection<CountryResultsViewModel>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<CountryResultsViewModel>), typeof(ScoreBoard), new PropertyMetadata(null, OnItemsSet));

        private static void OnItemsSet(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scoreBoard = d as ScoreBoard;
            if (e.NewValue != null)
            {
                var collection = (ObservableCollection<CountryResultsViewModel>)e.NewValue;

                foreach (var item in collection)
                {
                    var scoreBoardItem = new ScoreBoardItem()
                    {
                        DataContext = item,
                        Opacity = 0
                    };

                    var binding = new Binding("TotalPoints");
                    binding.Source = item;
                    scoreBoardItem.SetBinding(ScoreBoardItem.TotalPointsProperty, binding);

                    scoreBoardItem.CurrentPointsUpdated += scoreBoard.ScoreBoardItem_CurrentPointsUpdated;
                    scoreBoard.AddScoreBoardItem(scoreBoardItem);
                }
            }
        }

        public event EventHandler EntranceAnimationCompleted;
        public event EventHandler<int> CurrentPointsUpdated;

        public Storyboard EntranceStoryboard { get; set; }

        private void ScoreBoardItem_CurrentPointsUpdated(object sender, int e)
        {
            CurrentPointsUpdated?.Invoke(sender, e);
        }

        private int numAnimationsCompleted = 0;
        private const double itemHeight = 40.0;
        private double nextYOffset = 0;
        private TimeSpan nextTimeSpan = TimeSpan.FromSeconds(1);

        private void AddScoreBoardItem(ScoreBoardItem item)
        {
            Binding widthBinding = new Binding("ActualWidth");
            widthBinding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor,
                                                     typeof(ScoreBoard), 1);
            item.SetBinding(ScoreBoardItem.WidthProperty, widthBinding);

            var opacityAnimation = new DoubleAnimation()
            {
                BeginTime = nextTimeSpan,
                Duration = TimeSpan.FromSeconds(1.0),
                From = 0,
                To = 1
            };

            Storyboard.SetTarget(opacityAnimation, item);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));
            EntranceStoryboard.Children.Add(opacityAnimation);
            
            var translateYAnimation = new DoubleAnimation()
            {
                BeginTime = nextTimeSpan,
                Duration = TimeSpan.FromSeconds(0.7),
                From = nextYOffset + 180.0,
                To = nextYOffset,
                FillBehavior = FillBehavior.Stop, // Deze moet op stop staan omdat we anders na de animatie de Canvas.Top property niet meer kunnen setten
                EasingFunction = new ExponentialEase()
                {
                    EasingMode = EasingMode.EaseOut,
                    Exponent = 2.0
                }
            };

            // Aan het eind van de animatie wordt de toppositie weer gereset, omdat fillbehavior op stop moet staan.
            translateYAnimation.Completed += (sender, e) =>
            {
                HoldEndPosition(sender);

                // We houden bij hoeveel ScoreboardItem-animaties er al geweest zijn. Als ze allemaal geweest zijn vuren we een event af
                // zodat Scoreboard weet dat hij het signaal kan sturen dat de animatie voltooid is.
                numAnimationsCompleted++;

                if (numAnimationsCompleted == EntranceStoryboard.Children.Count / 2) // Het aantal animaties moet door twee gedeeld worden omdat we voor elk storyboarditem zowel een opacity als een translate-animatie hebben.
                {
                    EntranceAnimationCompleted?.Invoke(this, EventArgs.Empty);
                }
            };

            Storyboard.SetTarget(translateYAnimation, item);
            Storyboard.SetTargetProperty(translateYAnimation, new PropertyPath(Canvas.TopProperty));
            EntranceStoryboard.Children.Add(translateYAnimation);
            
            rootCanvas.Children.Add(item);
            Canvas.SetTop(item, nextYOffset);
            nextYOffset += itemHeight;
            nextTimeSpan = nextTimeSpan + TimeSpan.FromSeconds(0.1);
        }

        private void HoldEndPosition(object sender)
        {
            var animation = (sender as AnimationClock).Timeline as DoubleAnimation;
            var animationTarget = Storyboard.GetTarget(animation) as ScoreBoardItem;
            Canvas.SetTop(animationTarget, animation.To.Value);
        }
    }
}
