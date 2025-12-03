namespace DocumentManagerApi.Models
{
    public class Case
    {
        public int Id { get; set; }
        public string CaseNumber { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
