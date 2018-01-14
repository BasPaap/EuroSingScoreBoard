using Bas.EuroSing.ScoreBoard.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                collection.CollectionChanged += scoreBoard.Collection_CollectionChanged;

                foreach (var item in collection)
                {
                    var scoreBoardItem = new ScoreBoardItem()
                    {
                        DataContext = item,
                        Opacity = 0
                    };
                    
                    scoreBoard.AddScoreBoardItem(scoreBoardItem);
                }
            }
            else
            {
                (e.OldValue as ObservableCollection<CountryResultsViewModel>).CollectionChanged -= (d as ScoreBoard).Collection_CollectionChanged;
            }
            
        }

        public Storyboard EntranceStoryboard { get; set; }

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
                EasingFunction = new ExponentialEase()
                {
                    EasingMode = EasingMode.EaseOut,
                    Exponent = 2.0
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

        private  void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        }
        
    }
}
