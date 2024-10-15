using System.Windows;
using System.Windows.Controls;

namespace OperatingSchedule
{
    public partial class MainWindow : Window
    {
        private SurgeonsPage surgeonsPage;
        private AnesthesiologistsPage anesthesiologistPage;
        private ProposedSchedulePage proposedSchedulePage;
        public MainWindow()
        {
            InitializeComponent();
            surgeonsPage = new SurgeonsPage(proposedSchedulePage);
            anesthesiologistPage = new AnesthesiologistsPage(proposedSchedulePage);
            proposedSchedulePage = new ProposedSchedulePage(surgeonsPage, anesthesiologistPage);

        }

        private void SurgeonsButton_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(surgeonsPage);
        }
        
        private void AnesthesiologistButton_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(anesthesiologistPage);
        }

        private void ScheduleButton_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(proposedSchedulePage);
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}