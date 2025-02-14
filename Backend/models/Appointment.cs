using System.ComponentModel.DataAnnotations;

namespace OnlineClinicBooking.Models
{
    public class Appointment
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string? PatientNote { get; set; }
        public string? DoctorNote { get; set; }

        //Foreign Key
        [Required]
        public int CategoryId { get; set; }
        //Navigation property
        public virtual Category? Category { get; set; }

        //Foreign Key
        [Required]
        public int ClinicId { get; set; }
        //Navigation property
        public virtual Clinic? Clinic { get; set; }

        //Foreign Key
        [Required]
        public int DoctorId { get; set; }
        //Navigation property
        public virtual Doctor? Doctor { get; set; }

        //Foreign Key
        [Required]
        public int PatientId { get; set; }
        //Navigation property
        public virtual Patient? Patient { get; set; }
    }
}