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
        
        private void StartTotalPointsAnimation(int from, int to)
        {
            var storyboard = new Storyboard();
            var animation = new Int32Animation()
            {
                BeginTime = TimeSpan.Zero,
                From = from,
                To = to,
                Duration = TimeSpan.FromSeconds(0.7),
                //EasingFunction = new ExponentialEase()
                //{
                //    EasingMode = EasingMode.EaseOut,
                //    Exponent = 2
                //}
            };

            storyboard.Children.Add(animation);

            PropertyPath propertyPath = new PropertyPath(ScoreBoardItem.DisplayPointsProperty);
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, propertyPath);

            storyboard.Begin();
        }

        public int TotalPoints
        {
            get { return (int)GetValue(TotalPointsProperty); }
            set { SetValue(TotalPointsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalPoints.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalPointsProperty =
            DependencyProperty.Register("TotalPoints", typeof(int), typeof(ScoreBoardItem), new PropertyMetadata(0, OnTotalPointsUpdated));

        private static void OnTotalPointsUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scoreBoardItem = d as ScoreBoardItem;
            scoreBoardItem.StartTotalPointsAnimation((int)e.OldValue, (int)e.NewValue);
        }

        public int DisplayPoints
        {
            get { return (int)GetValue(DisplayPointsProperty); }
            set { SetValue(DisplayPointsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayPoints.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayPointsProperty =
            DependencyProperty.Register("DisplayPoints", typeof(int), typeof(ScoreBoardItem), new PropertyMetadata(0));
        
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
