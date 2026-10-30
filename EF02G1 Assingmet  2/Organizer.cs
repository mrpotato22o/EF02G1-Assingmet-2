using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF02G1_Assingmet__2.Entities
{
    [Table("EventOrganizers")]
    public class Organizer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(150)]
        public string? CompanyName { get; set; }   // optional

        public bool IsVerified { get; set; }

        public OrganizerProfile? Profile { get; set; }                         // 1 : 1
        public ICollection<Event> Events { get; set; } = new List<Event>();    // 1 : N
    }
}