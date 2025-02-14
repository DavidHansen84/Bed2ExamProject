using System.ComponentModel.DataAnnotations;

namespace OnlineClinicBooking.Models
{
    public class Clinic
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        
       

       // One Clinic has Many Doctors
        public virtual ICollection<Doctor>? Doctors { get; set; }

        // One Clinic has Many Appontments
        public virtual ICollection<Appointment>? Appointments { get; set; }

       
    }
}