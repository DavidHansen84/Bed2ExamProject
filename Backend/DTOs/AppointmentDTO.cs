
public class AppointmentDTO
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int CategoryId { get; set; }
    public string Category { get; set; }
    public int ClinicId { get; set; }
    public string Clinic { get; set; }
    public int DoctorId { get; set; }
    public string Doctor { get; set; }
    public int PatientId { get; set; }
    public string Patient { get; set; }
    public string PatientNote { get; set; }
    public string DoctorNote { get; set; }
}


