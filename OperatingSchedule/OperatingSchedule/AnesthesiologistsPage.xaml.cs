using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OperatingSchedule
{
    using OperatingSchedule.MVVM.Model;
    public partial class AnesthesiologistsPage : Page
    {
        private ProposedSchedulePage proposedSchedulePage;
        public AnesthesiologistsPage(ProposedSchedulePage ProposedPage)
        {
            InitializeComponent();
            proposedSchedulePage = ProposedPage;
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text == "Enter Anesthesiologist Name")
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.Black;
            }
        }
        //click off a textbox
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox.Name.Contains("Anesthesiologist"))
                    textBox.Text = "Enter Anesthesiologist Name";
                else
                    textBox.Text = "Re-enter data"; //this doesn't work, need to set this based on textbox type

                textBox.Foreground = Brushes.Gray; // Optional: set color to gray for placeholder text
            }
        }

        private void AddAnesthesiologistButton_Click(object sender, RoutedEventArgs e)
        {
            var anesthesiologistPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };

            // Anesthesiologist name TextBox
            var anesthesiologistNameTextBox = new TextBox
            {
                Width = 250,
                Margin = new Thickness(5),
                Text = "Enter Anesthesiologist Name",
                Style = (Style)FindResource("SurgeryModernTextbox")
            };
            anesthesiologistNameTextBox.GotFocus += TextBox_GotFocus;
            anesthesiologistNameTextBox.LostFocus += TextBox_LostFocus;

            // Availability start time ComboBox
            var startTimeComboBox = new ComboBox
            {
                Width = 150,
                Margin = new Thickness(5),
                ItemsSource = new List<string>
                    {
                        "08:00 AM", "08:30 AM", "09:00 AM", "09:30 AM", "10:00 AM",
                        "10:30 AM", "11:00 AM", "11:30 AM", "12:00 PM", "12:30 PM",
                        "01:00 PM", "01:30 PM", "02:00 PM", "02:30 PM", "03:00 PM",
                        "03:30 PM"
                    },
                SelectedIndex = 0, // Default to 8:00 AM
                HorizontalAlignment = HorizontalAlignment.Left
            };

            // Availability end time ComboBox
            var endTimeComboBox = new ComboBox
            {
                Width = 150,
                Margin = new Thickness(5),
                ItemsSource = new List<string>
                    {
                        "08:30 AM", "09:00 AM", "09:30 AM", "10:00 AM",
                        "10:30 AM", "11:00 AM", "11:30 AM", "12:00 PM", "12:30 PM",
                        "01:00 PM", "01:30 PM", "02:00 PM", "02:30 PM", "03:00 PM",
                        "03:30 PM", "04:00 PM"
                    },
                SelectedIndex = 8, // Default to 12:00 PM
                HorizontalAlignment = HorizontalAlignment.Left
            };

            var dayOfWeekComboBox = new ComboBox
            {
                Width = 150,
                Margin = new Thickness(5),
                ItemsSource = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" },
                SelectedIndex = 0, // Default to Monday
                HorizontalAlignment = HorizontalAlignment.Left
            };


            anesthesiologistPanel.Children.Add(anesthesiologistNameTextBox);
            anesthesiologistPanel.Children.Add(startTimeComboBox);
            anesthesiologistPanel.Children.Add(endTimeComboBox);
            anesthesiologistPanel.Children.Add(dayOfWeekComboBox);

            // Undo mistakes
            var removeAnesthesiologistButton = new Button
            {
                Content = "Remove Anesthesiologist",
                Width = 200,
                Margin = new Thickness(5),
                Style = (Style)FindResource("AddSurgeonButtonTheme")
            };
            // Event handler to remove the anesthesiologist
            removeAnesthesiologistButton.Click += (removeSender, removeArgs) =>
            {
                AnesthesiologistStackPanel.Children.Remove(anesthesiologistPanel);
            };

            anesthesiologistPanel.Children.Add(removeAnesthesiologistButton);

            AnesthesiologistStackPanel.Children.Add(anesthesiologistPanel);


        }
        // Collect anesthesiologist data into HospitalData object
        public HospitalData BuildAnesthesiologistData()
        {
            var hospitalData = new HospitalData();

            foreach (var anesthesiologistPanel in AnesthesiologistStackPanel.Children.OfType<StackPanel>())
            {
                var anesthesiologistNameTextBox = anesthesiologistPanel.Children.OfType<TextBox>().FirstOrDefault();
                // Don't send empty data
                if (anesthesiologistNameTextBox == null || anesthesiologistNameTextBox.Text == "Enter Anesthesiologist Name") continue;

                var anesthesiologist = new Anesthesiologist { AnesthesiologistName = anesthesiologistNameTextBox.Text };

                var startTimeComboBox = anesthesiologistPanel.Children.OfType<ComboBox>().FirstOrDefault(c => c.ItemsSource.Cast<string>().Contains("08:00 AM"));
                var endTimeComboBox = anesthesiologistPanel.Children.OfType<ComboBox>().FirstOrDefault(c => c.ItemsSource.Cast<string>().Contains("04:00 PM"));
                var dayOfWeekComboBox = anesthesiologistPanel.Children.OfType<ComboBox>().FirstOrDefault(c => c.ItemsSource.Cast<string>().Contains("Monday"));


                if (startTimeComboBox != null && endTimeComboBox != null)
                {
                    var availability = new Availability
                    {
                        DayOfWeek = dayOfWeekComboBox.SelectedItem.ToString(), // You can modify this if you add a day of the week field
                        StartTime = startTimeComboBox.SelectedItem.ToString(),
                        EndTime = endTimeComboBox.SelectedItem.ToString()
                    };

                    anesthesiologist.AvailableTimes.Add(availability);
                }

                hospitalData.Anesthesiologists.Add(anesthesiologist); // Add each anesthesiologist to the list
            }

            return hospitalData;
        }
    }
}