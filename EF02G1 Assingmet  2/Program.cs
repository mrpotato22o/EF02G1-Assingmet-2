using EF02G1_Assingmet__2.Data;
using EF02G1_Assingmet__2.Entities;
using Microsoft.EntityFrameworkCore;

namespace EF02G1_Assingmet__2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new EventHubContext();

            // 1) Create the database
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            Console.WriteLine("Database created.\n");

            // 2) small data
            var organizer = new Organizer
            {
                Name = "Sara Ahmed",
                CompanyName = "Nile Events",
                IsVerified = true,
                Profile = new OrganizerProfile
                {
                    Biography = "We organize tech and cultural events.",
                    WebsiteUrl = "https://nileevents.example.com"
                }
            };

            organizer.Events.Add(new Event
            {
                Title = "Cairo Tech Summit",
                Description = "A two-day conference about software and AI.",
                StartDate = new DateTime(2026, 12, 10, 9, 0, 0),
                EndDate = new DateTime(2026, 12, 11, 17, 0, 0),
                MaxAttendees = 500
            });

            organizer.Events.Add(new Event
            {
                Title = "Open Mic Night",
                Description = "Casual evening for local performers.",
                StartDate = new DateTime(2026, 12, 20, 19, 0, 0),
                MaxAttendees = 80     
            });

            var attendee = new Attendee
            {
                FullName = "Omar Hassan",
                Email = "omar@example.com",
                Street = "12 Nile Street",
                City = "Cairo",
                Country = "Egypt",
                PostalCode = "11511",
                Badge = new Badge
                {
                    BadgeNumber = "BDG-0001",
                    IssuedDate = DateOnly.FromDateTime(DateTime.Today),
                    Tier = BadgeTier.VIP
                }
            };

            context.Organizers.Add(organizer);
            context.Attendees.Add(attendee);
            context.SaveChanges();

            // 3)  schema from SQL Server
            Console.WriteLine("=== Tables & columns in SQL Server ===");
            var conn = context.Database.GetDbConnection();
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE,
                                           ISNULL(CAST(CHARACTER_MAXIMUM_LENGTH AS varchar(10)), '-'), IS_NULLABLE
                                    FROM INFORMATION_SCHEMA.COLUMNS
                                    WHERE TABLE_NAME IN ('EventOrganizers','OrganizerProfiles','Events','Attendees','Badges')
                                    ORDER BY TABLE_NAME, ORDINAL_POSITION";
                using var reader = cmd.ExecuteReader();
                string current = "";
                while (reader.Read())
                {
                    var table = reader.GetString(0);
                    if (table != current) { Console.WriteLine($"\n[{table}]"); current = table; }
                    Console.WriteLine($"  {reader.GetString(1),-14} {reader.GetString(2),-10} len: {reader.GetString(3),-5} nullable: {reader.GetString(4)}");
                }
            }
            conn.Close();

            // 4) Show relationships work
            Console.WriteLine("\n=== Organizer -> Profile -> Events ===");
            var org = context.Organizers
                             .Include(o => o.Profile)
                             .Include(o => o.Events)
                             .First();
            Console.WriteLine($"{org.Name} ({org.CompanyName}) | Verified: {org.IsVerified}");
            Console.WriteLine($"Profile: {org.Profile!.Biography} | {org.Profile.WebsiteUrl}");
            foreach (var e in org.Events)
                Console.WriteLine($"  Event: {e.Title} | {e.StartDate:d} -> {(e.EndDate?.ToString("d") ?? "no end date")} | Max: {e.MaxAttendees}");

            Console.WriteLine("\n=== Attendee -> Badge ===");
            var att = context.Attendees.Include(a => a.Badge).First();
            Console.WriteLine($"{att.FullName} | {att.Email}");
            Console.WriteLine($"Address: {att.FullAddress}");
            Console.WriteLine($"Badge: {att.Badge!.BadgeNumber} | {att.Badge.Tier} | Issued: {att.Badge.IssuedDate}");
        }
    }
}