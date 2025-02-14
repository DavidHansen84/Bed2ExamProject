using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineClinicBooking.Models
{
    public class Category
    {
        public Category() { }
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        
       

       // One Category has Many Appointments
       [JsonIgnore]
        public virtual ICollection<Appointment>? Appointments { get; set; }

    }
}