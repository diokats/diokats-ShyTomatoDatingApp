using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public required string Url { get; set; }
        public string? PublicId { get; set; }
        public bool IsMain { get; set; } = false;
        // public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int AppUserId { get; set; }
        public  AppUser AppUser { get; set; } = null!;

    }
}