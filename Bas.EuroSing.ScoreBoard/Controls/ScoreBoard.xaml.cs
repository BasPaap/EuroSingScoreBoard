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
                    var scoreBoardItem = new ScoreBoardItem();
                    scoreBoardItem.DataContext = item;
                    //scoreBoardItem.SetBinding(ScoreBoardItem.CountryNameProperty, "Name");
                    scoreBoard.AddScoreBoardItem(scoreBoardItem);
                }
            }
            else
            {
                (e.OldValue as ObservableCollection<CountryResultsViewModel>).CollectionChanged -= (d as ScoreBoard).Collection_CollectionChanged;
            }
            
        }

        private const double itemHeight = 40.0;
        private double nextYOffset = 0;
        private void AddScoreBoardItem(ScoreBoardItem item)
        {
            Binding b = new Binding("ActualWidth");
            b.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor,
                                                     typeof(ScoreBoard), 1);
            item.SetBinding(ScoreBoardItem.WidthProperty, b);

            rootCanvas.Children.Add(item);
            Canvas.SetTop(item, nextYOffset);
            nextYOffset += itemHeight;
        }

        private  void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        }
    }
}
