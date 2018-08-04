using Bas.EuroSing.ScoreBoard.Messages;
using GalaSoft.MvvmLight.Messaging;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bas.EuroSing.ScoreBoard.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private ResultsView resultsView = new ResultsView();

        public MainView()
        {
            InitializeComponent();

            Messenger.Default.Register<GenericMessage<Message>>(this, (message) =>
            {
                if (message.Content == Message.ShowResultsControlPanel)
                {
                    this.resultsView.Show();
                }
            });
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
        
        // If the main window (this one) is closed, make sure the results window is closed as well.
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.resultsView.Close();
        }
    }
}
