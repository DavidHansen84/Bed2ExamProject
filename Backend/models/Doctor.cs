using System.ComponentModel.DataAnnotations;

namespace OnlineClinicBooking.Models
{
    public class Doctor
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }


        //Foreign Key
        public int? ClinicId { get; set; }
        //Navigation property
        public virtual Clinic? Clinic { get; set; }

        //Foreign Key
        public int? SpecialityId { get; set; }
        //Navigation property
        public virtual Speciality? Speciality { get; set; }

        // One Doctor has Many Appontments
        public virtual ICollection<Appointment>? Appointments { get; set; }
    }
}