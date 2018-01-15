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
                    FillBehavior = FillBehavior.Stop
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

        private void ScoreBoardItem_CurrentPointsUpdated(object sender, int e)
        {
            CurrentPointsUpdated?.Invoke(sender, e);
        }

        public Storyboard EntranceStoryboard { get; set; }

        private int numAnimationsCompleted = 0;
        private const double itemHeight = 40.0;
        private double nextYOffset = 0;
        private TimeSpan nextTimeSpan = TimeSpan.FromSeconds(1);
        private void AddScoreBoardItem(ScoreBoardItem item)
        {
            Binding b = new Binding("ActualWidth");
            b.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor,
                                                     typeof(ScoreBoard), 1);
            item.SetBinding(ScoreBoardItem.WidthProperty, b);

            var opacityAnimation = new DoubleAnimation()
            {
                BeginTime = nextTimeSpan,
                Duration = TimeSpan.FromSeconds(1.0),
                From = 0,
                To = 1
            };


            PropertyPath propertyPath = new PropertyPath(UIElement.OpacityProperty);
            Storyboard.SetTarget(opacityAnimation, item);
            Storyboard.SetTargetProperty(opacityAnimation, propertyPath);

            EntranceStoryboard.Children.Add(opacityAnimation);


            var translateYAnimation = new DoubleAnimation()
            {
                BeginTime = nextTimeSpan,
                Duration = TimeSpan.FromSeconds(0.7),
                From = nextYOffset + 180.0,
                To = nextYOffset,
                FillBehavior = FillBehavior.Stop,
                EasingFunction = new ExponentialEase()
                {
                    EasingMode = EasingMode.EaseOut,
                    Exponent = 2.0
                }
            };
            translateYAnimation.Completed += (sender, e) =>
            {
                HoldEndPosition(sender);
                numAnimationsCompleted++;

                if (numAnimationsCompleted == EntranceStoryboard.Children.Count / 2)
                {
                    EntranceAnimationCompleted?.Invoke(this, EventArgs.Empty);
                }
            };

            PropertyPath propertyPath2 = new PropertyPath(Canvas.TopProperty);
            Storyboard.SetTarget(translateYAnimation, item);
            Storyboard.SetTargetProperty(translateYAnimation, propertyPath2);

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
