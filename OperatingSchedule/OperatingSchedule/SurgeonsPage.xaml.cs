using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace OperatingSchedule
{
    using OperatingSchedule.MVVM.Model;
    public partial class SurgeonsPage : Page
    {
        private ProposedSchedulePage proposedSchedulePage;

        //non-relative address, fix this later
        [DllImport("C:\\Users\\Fillu\\Desktop\\Fairytale\\OperatingSchedule\\x64\\Debug\\Algorithm.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ReceiveSurgeonData(IntPtr jsonData);
        public SurgeonsPage(ProposedSchedulePage proposedPage)
        {
            InitializeComponent();
            proposedSchedulePage = proposedPage;
        }
        //Click on a textbox
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text == "Enter Surgeon Name" || textBox.Text == "Enter Surgery Name" || textBox.Text == "Enter Duration (min)")
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
                if (textBox.Name.Contains("Surgeon"))
                    textBox.Text = "Enter Surgeon Name";
                else if (textBox.Name.Contains("Surgery"))
                    textBox.Text = "Enter Surgery Name";
                else
                    textBox.Text = "Re-enter data"; //this doesn't work, need to set this based on textbox type

                textBox.Foreground = Brushes.Gray; // Optional: set color to gray for placeholder text
            }
        }

        // Add new surgeon dynamically
        private void AddSurgeonButton_Click(object sender, RoutedEventArgs e)
        {
            var surgeonPanel = new StackPanel { Margin = new Thickness(5) };

            // Surgeon name TextBox
            var surgeonNameTextBox = new TextBox
            {
                Width = 250,
                Margin = new Thickness(5),
                Text = "Enter Surgeon Name",
                Style = (Style)FindResource("SurgeryModernTextbox")
            };
            surgeonNameTextBox.GotFocus += TextBox_GotFocus;
            surgeonNameTextBox.LostFocus += TextBox_LostFocus;
            surgeonPanel.Children.Add(surgeonNameTextBox);

            var surgeon = new Surgeon { SurgeonName = surgeonNameTextBox.Text };

            // Surgery list for this surgeon
            var surgeryList = new ItemsControl();
            surgeonPanel.Children.Add(surgeryList);

            // Add new surgery for surgeon
            var addSurgeryButton = new Button
            {
                Content = "Add Surgery",
                Width = 100,
                Margin = new Thickness(5),
                Style = (Style)FindResource("AddSurgeryButtonTheme")
            };

            addSurgeryButton.Click += (s, args) =>
            {
                var surgeryPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };

                // Surgery name TextBox
                var surgeryNameTextBox = new TextBox 
                { 
                    Width = 250, 
                    Margin = new Thickness(5), 
                    Text = "Enter Surgery Name", 
                    Style = (Style)FindResource("SurgeryModernTextbox")
                };
                surgeryNameTextBox.GotFocus += TextBox_GotFocus;
                surgeryNameTextBox.LostFocus += TextBox_LostFocus;

                //Estimated duration of surgery
                var durationLabel = new Label
                {
                    Content = "Duration (min)",
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5),
                    Foreground = Brushes.White
                };

                // Do it this way, doesn't allow non-integers
                var durationUpDown = new IntegerUpDown
                {
                    Width = 120,
                    Margin = new Thickness(5),
                    Minimum = 1,
                    Maximum = 1000,
                    Value = 60,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                //Only 5 options, no reason to give user more freedom than that
                var dayOfWeekComboBox = new ComboBox
                {
                    Width = 150,
                    Margin = new Thickness(5),
                    ItemsSource = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" },
                    SelectedIndex = 0, // Default to Monday
                    HorizontalAlignment = HorizontalAlignment.Left
                };
               
                //Probably shouldn't be a combobox to allow flexibility if schedule changes, but waterbury hospital
                //hasn't changed in my lifetime
                var startTimeComboBox = new ComboBox
                {
                    Width = 120,
                    Margin = new Thickness(5),
                    ItemsSource = new List<string>
                {
                    "08:00 AM", "08:30 AM", "09:00 AM", "09:30 AM", "10:00 AM",
                    "10:30 AM", "11:00 AM", "11:30 AM", "12:00 PM", "12:30 PM",
                    "01:00 PM", "01:30 PM", "02:00 PM", "02:30 PM", "03:00 PM",
                    "03:30 PM", "04:00 PM"
                },
                    SelectedIndex = 0, // Default to 8:00 AM
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                

                surgeryPanel.Children.Add(surgeryNameTextBox);
                surgeryPanel.Children.Add(durationLabel);
                surgeryPanel.Children.Add(durationUpDown);
                surgeryPanel.Children.Add(dayOfWeekComboBox);
                surgeryPanel.Children.Add(startTimeComboBox);

                //Undo mistakes
                var removeSurgeryButton = new Button
                {
                    Content = "Remove Surgery",
                    Width = 120,
                    Margin = new Thickness(5),
                    Style = (Style)FindResource("AddSurgeryButtonTheme")
                };
                
                removeSurgeryButton.Click += (removeSender, removeArgs) =>
                {
                    surgeryList.Items.Remove(surgeryPanel); 
                };

                surgeryPanel.Children.Add(removeSurgeryButton);

                surgeryList.Items.Add(surgeryPanel);

                if (surgeryNameTextBox.Text != "Enter Surgery Name")
                {
                    var surgery = new Surgery
                    {
                        SurgeryName = surgeryNameTextBox.Text,
                        Duration = durationUpDown.Value ?? 0
                    };
                    surgeon.Surgeries.Add(surgery);
                }
            };

            surgeonPanel.Children.Add(addSurgeryButton);

            //Undo mistakes
            var removeSurgeonButton = new Button
            {
                Content = "Remove Surgeon",
                Width = 120,
                Margin = new Thickness(5),
                Style = (Style)FindResource("AddSurgeonButtonTheme") 
            };
            // Event handler to remove the surgeon
            removeSurgeonButton.Click += (removeSender, removeArgs) =>
            {
                SurgeonStackPanel.Children.Remove(surgeonPanel);
            };

            surgeonPanel.Children.Add(removeSurgeonButton);

            SurgeonStackPanel.Children.Add(surgeonPanel);

        }
        //Create the hospitaldata data structure with all information input
        public HospitalData BuildSurgeonData()
        {
            var hospitalData = new HospitalData();

            hospitalData.NumberOfOrRooms = orRoomUpDown.Value ?? 6; // Use 6 as a default if no value is set, waterbury hospital has 6

            foreach (var surgeonPanel in SurgeonStackPanel.Children.OfType<StackPanel>())
            {
                var surgeonNameTextBox = surgeonPanel.Children.OfType<TextBox>().FirstOrDefault();
                //Don't send empty data
                if (surgeonNameTextBox == null || surgeonNameTextBox.Text == "Enter Surgeon Name" || surgeonNameTextBox.Text == "Re-enter data") continue;

                var surgeon = new Surgeon { SurgeonName = surgeonNameTextBox.Text };

                var surgeryList = surgeonPanel.Children.OfType<ItemsControl>().FirstOrDefault();
                if (surgeryList != null)
                {
                    //collect data for each surgeon to variables, check that it's complete
                    foreach (var surgeryPanel in surgeryList.Items.OfType<StackPanel>())
                    {
                        var surgeryNameTextBox = surgeryPanel.Children.OfType<TextBox>().FirstOrDefault();
                        var durationUpDown = surgeryPanel.Children.OfType<IntegerUpDown>().FirstOrDefault();
                        var dayOfWeekComboBox = surgeryPanel.Children.OfType<ComboBox>().FirstOrDefault(c => c.ItemsSource.Cast<string>().Contains("Monday"));
                        var startTimeComboBox = surgeryPanel.Children.OfType<ComboBox>().FirstOrDefault(c => c.ItemsSource.Cast<string>().Contains("08:00 AM"));

                        if (surgeryNameTextBox != null && durationUpDown != null && surgeryNameTextBox.Text != "Enter Surgery Name"
                            && dayOfWeekComboBox != null && startTimeComboBox != null)
                        {
                            //add to surgery data struct
                            var surgery = new Surgery
                            {
                                SurgeryName = surgeryNameTextBox.Text,
                                Duration = durationUpDown.Value ?? 0,
                                DayOfWeek = dayOfWeekComboBox.SelectedItem.ToString(),
                                StartTime = startTimeComboBox.SelectedItem.ToString()
                            };
                            //add surgeries to each surgeon
                            surgeon.Surgeries.Add(surgery);
                        }
                    }
                }
                //add surgeons to the hospital data
                hospitalData.Surgeons.Add(surgeon); // Add each surgeon to the overall list
            }

            return hospitalData;
        }
    }
}
