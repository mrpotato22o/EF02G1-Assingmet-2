namespace EF02G1_Assingmet__2.Entities
{
    public class Badge
    {
        public int Id { get; set; }
        public string BadgeNumber { get; set; } = null!;
        public DateOnly IssuedDate { get; set; }
        public BadgeTier Tier { get; set; }

        public int AttendeeId { get; set; }
        public Attendee Attendee { get; set; } = null!;
    }
}