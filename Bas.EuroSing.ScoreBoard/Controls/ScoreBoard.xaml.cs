﻿using Bas.EuroSing.ScoreBoard.Messages;
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
            Messenger.Default.Register<ReorderCountriesMessage>(this, (message) => ReorderCountries());
        }


        private void ReorderCountries(bool skipAnimation = false)
        {
            var orderedItems = Items.OrderByDescending(i => i.TotalPoints).ThenBy(i => i.Name).ToList(); // Reorder all items so that we can look up their new indexes.
            var duration = skipAnimation ? TimeSpan.Zero : TimeSpan.FromSeconds(1.0);

            var reorderStoryboard = new Storyboard();
            foreach (var scoreBoardItem in rootCanvas.Children.OfType<ScoreBoardItem>())
            {
                var countryResultsViewModel = scoreBoardItem.DataContext as CountryResultsViewModel;
                var newIndex = orderedItems.IndexOf(orderedItems.Single(i => i.Id == countryResultsViewModel.Id)); // Get the new index for this scoreBoardItem, from which we can calculate the new top position.
                var newTopPosition = newIndex * itemHeight;

                // Animate the item to its new position.
                var reorderAnimation = new DoubleAnimation(Canvas.GetTop(scoreBoardItem), newIndex * itemHeight, duration)
                {
                    BeginTime = duration,
                    FillBehavior = FillBehavior.Stop,
                    EasingFunction = new CubicEase()
                    {
                        EasingMode = EasingMode.EaseOut
                    }
                };
                reorderAnimation.Completed += (sender, e) => { Debug.Write("Hold on reorder"); HoldEndPosition(sender); }; 
                Storyboard.SetTarget(reorderAnimation, scoreBoardItem);
                Storyboard.SetTargetProperty(reorderAnimation, new PropertyPath(Canvas.TopProperty));
                reorderStoryboard.Children.Add(reorderAnimation);

                Debug.WriteLine($"Reordering {countryResultsViewModel.Name}({countryResultsViewModel.TotalPoints} points, position {Canvas.GetTop(scoreBoardItem)}) from {reorderAnimation.From} to {reorderAnimation.To}");
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
            // If the Items property is set, create new ScoreBoardItem controls for each item, bind the appropriate values, and subscribe to any events.
            var scoreBoard = d as ScoreBoard;
            if (e.NewValue != null)
            {
                var collection = (ObservableCollection<CountryResultsViewModel>)e.NewValue;

                foreach (var countryResultsViewModel in collection)
                {
                    var scoreBoardItem = new ScoreBoardItem()
                    {
                        DataContext = countryResultsViewModel,
                        Opacity = 0
                    };

                    var binding = new Binding("TotalPoints");
                    binding.Source = countryResultsViewModel;
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
        private const double itemHeight = 50.0;
        private double nextYOffset = 0;
        private TimeSpan nextTimeSpan = TimeSpan.FromSeconds(1);

        private void AddScoreBoardItem(ScoreBoardItem item)
        {
            Binding widthBinding = new Binding("ActualWidth");
            widthBinding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor,
                                                     typeof(ScoreBoard), 1);
            item.SetBinding(ScoreBoardItem.WidthProperty, widthBinding);

            rootCanvas.Children.Add(item);
            Canvas.SetTop(item, nextYOffset);
            Debug.WriteLine($"AddItem: Canvas.SetTop({(item.DataContext as CountryResultsViewModel).Name}, {nextYOffset});");
            nextYOffset += itemHeight;
            nextTimeSpan = nextTimeSpan + TimeSpan.FromSeconds(0.1);
        }

        public void ResetScoreBoardItemAnimations()
        {
            EntranceStoryboard.Children.Clear();

            var yOffset = 0.0;
            var timeSpan = TimeSpan.FromSeconds(1.0);

            foreach (var item in rootCanvas.Children.OfType<ScoreBoardItem>().OrderBy(s => Canvas.GetTop(s))) 
            {                
                SetAnimationForScoreBoardItem(item, yOffset, timeSpan);

                yOffset += itemHeight;
                timeSpan += TimeSpan.FromSeconds(0.1);
            }
        }

        private void SetAnimationForScoreBoardItem(ScoreBoardItem item, double yOffset, TimeSpan timeSpan)
        {
            var initialOpacityAnimation = new DoubleAnimation()
            {
                BeginTime = TimeSpan.Zero,
                Duration = TimeSpan.Zero,
                To = 0
            };

            Storyboard.SetTarget(initialOpacityAnimation, item);
            Storyboard.SetTargetProperty(initialOpacityAnimation, new PropertyPath(UIElement.OpacityProperty));
            EntranceStoryboard.Children.Add(initialOpacityAnimation);


            var opacityAnimation = new DoubleAnimation()
            {
                BeginTime = timeSpan,
                Duration = TimeSpan.FromSeconds(1.0),
                From = 0,
                To = 1
            };

            Storyboard.SetTarget(opacityAnimation, item);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));
            EntranceStoryboard.Children.Add(opacityAnimation);

            var translateYAnimation = new DoubleAnimation()
            {
                BeginTime = timeSpan,
                Duration = TimeSpan.FromSeconds(0.7),
                From = yOffset + 180.0,
                To = yOffset,
                FillBehavior = FillBehavior.Stop, // This animation has to be on Stop because otherwise we can't set  Canvas.Top anymore after the animation.
                EasingFunction = new ExponentialEase()
                {
                    EasingMode = EasingMode.EaseOut,
                    Exponent = 2.0
                }
            };

            // Because FillBehavior has to be on stop, we have to manually reset the top position at the end of the animation.
            Debug.WriteLine("-------- Setting completed event");
            translateYAnimation.Completed += (sender, e) =>
            {
                Debug.Write("Hold on enter");
                HoldEndPosition(sender);

                // Keep track of how many ScoreboardItem-animations we have had. When we've seen them all, fire an event so that 
                // any subscribers know the entire animation is completed. 
                numAnimationsCompleted++;

                const int numPropertiesSetInAnimation = 3;
                if (numAnimationsCompleted == EntranceStoryboard.Children.Count / numPropertiesSetInAnimation) // Divide the number of animations by the number of properties we set in each animation to know the "real" number of animations.
                {
                    EntranceAnimationCompleted?.Invoke(this, EventArgs.Empty);
                    numAnimationsCompleted = 0;
                }
            };

            Storyboard.SetTarget(translateYAnimation, item);
            Storyboard.SetTargetProperty(translateYAnimation, new PropertyPath(Canvas.TopProperty));
            EntranceStoryboard.Children.Add(translateYAnimation);

            Debug.WriteLine($"ENTERING {(item.DataContext as CountryResultsViewModel).Name}({((item.DataContext as CountryResultsViewModel)).TotalPoints} points, position {Canvas.GetTop(item)}) from {translateYAnimation.From} to {translateYAnimation.To}");
        }

        // Makes sure the control's property values don't reset after the animation is over.
        private void HoldEndPosition(object sender)
        {
            var animation = (sender as AnimationClock).Timeline as DoubleAnimation;
            var animationTarget = Storyboard.GetTarget(animation) as ScoreBoardItem;
            Canvas.SetTop(animationTarget, animation.To.Value);
            Debug.WriteLine($"HoldEnd: Canvas.SetTop({(animationTarget.DataContext as CountryResultsViewModel).Name}, {animation.To.Value});");
        }
    }
}
