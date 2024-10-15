namespace OperatingSchedule.MVVM.Model
{
    public class Surgery
    {
        public string SurgeryName { get; set; }
        public int Duration { get; set; }
        public string DayOfWeek { get; set; }
        public string StartTime { get; set; }
    }

    public class Surgeon
    {
        public string SurgeonName { get; set; }
        public List<Surgery> Surgeries { get; set; } = new List<Surgery>();
    }

    public class Availability
    {
        public string DayOfWeek { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

    }

    public class Anesthesiologist
    {
        public string AnesthesiologistName { get; set; }
        public List<Availability> AvailableTimes { get; set; } = new List<Availability>();
    }

    public class HospitalData
    {
        public List<Surgeon> Surgeons { get; set; } = new List<Surgeon>();
        public List<Anesthesiologist> Anesthesiologists { get; set; } = new List<Anesthesiologist>();
        public int NumberOfOrRooms { get; set; }
    }
}
