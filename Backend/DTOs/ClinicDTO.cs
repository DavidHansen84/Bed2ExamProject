public class ClinicDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<ClinicDoctorDTO> Doctors { get; set; }
    public List<ClinicAppointmentDTO> Appointments { get; set; }
}

