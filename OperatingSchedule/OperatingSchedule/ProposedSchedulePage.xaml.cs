using System.Windows.Controls;

namespace OperatingSchedule
{
    using Newtonsoft.Json;
    using OperatingSchedule.MVVM.Model;
    using System.Runtime.InteropServices;

    using System.Windows;

    public partial class ProposedSchedulePage : Page
    {
        // Import the function from Algorithm.dll
        [DllImport("C:\\Users\\Fillu\\Desktop\\Fairytale\\OperatingSchedule\\x64\\Debug\\Algorithm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ReceiveHospitalData(IntPtr jsonData);
        // Reference to Surgeon and Anesthesiologist pages
        private SurgeonsPage surgeonsPage;
        private AnesthesiologistsPage anesthesiologistsPage;

        public ProposedSchedulePage(SurgeonsPage surgeonsPage, AnesthesiologistsPage anesthesiologistsPage)
        {
            InitializeComponent();

            // Initialize the other pages (you may already have these set up)
            this.surgeonsPage = surgeonsPage;
            this.anesthesiologistsPage = anesthesiologistsPage;
        }

        // Function to generate the schedule by collecting data from both pages
        private void GenerateSchedule()
        {
            // Collect surgeon data from SurgeonsPage
            var surgeonsData = surgeonsPage.BuildSurgeonData();

            // Collect anesthesiologist data from AnesthesiologistsPage
            var anesthesiologistData = anesthesiologistsPage.BuildAnesthesiologistData();

            // Combine both sets of data into a single HospitalData structure
            var hospitalData = new HospitalData
            {
                Surgeons = surgeonsData.Surgeons,
                Anesthesiologists = anesthesiologistData.Anesthesiologists,
                NumberOfOrRooms = surgeonsData.NumberOfOrRooms  // Assuming OR rooms are defined in SurgeonsPage
            };

            // Pass hospitalData to the scheduling algorithm
            SendToCppAlgorithm(hospitalData);
        }

        // Function to send data to C++
        private void SendToCppAlgorithm(HospitalData hospitalData)
        {
            // Serialize the data to JSON
            string jsonData = JsonConvert.SerializeObject(hospitalData);
            MessageBox.Show(jsonData);

            // Marshal the string to an unmanaged pointer
            IntPtr jsonPtr = Marshal.StringToHGlobalAnsi(jsonData);

            // Pass the serialized data to the C++ function
            IntPtr resultPtr = ReceiveHospitalData(jsonPtr);

            // Convert the result back to a managed string
            string result = Marshal.PtrToStringAnsi(resultPtr);

            // Free the unmanaged string
            Marshal.FreeHGlobal(jsonPtr);

            // Show the result from C++
            MessageBox.Show(result);
        }

        // Event handler for the "Send to C++" button
        private void SendToCppButton_Click(object sender, RoutedEventArgs e)
        {
            // Call the GenerateSchedule method to collect data and send it to C++
            GenerateSchedule();
        }
    }

}
