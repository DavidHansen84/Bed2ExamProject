public class PatientDTO
{
    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public DateTime Birthdate { get; set; }
    public List<PatientAppointmentDTO> Appointments { get; set; }

}

