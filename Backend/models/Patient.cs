using System.ComponentModel.DataAnnotations;

namespace OnlineClinicBooking.Models
{
    public class Patient
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public DateTime Birthdate { get; set; }


        // One Patient has Many Appointments
        public virtual ICollection<Appointment>? Appointments { get; set; }


    }
}