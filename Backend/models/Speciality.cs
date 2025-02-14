using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineClinicBooking.Models
{
    public class Speciality
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        
       

        // One Speciality has Many Doctors
        [JsonIgnore]
        public virtual ICollection<Doctor>? Doctors { get; set; }

       
    }
}