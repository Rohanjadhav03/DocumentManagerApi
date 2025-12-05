namespace DocumentManagerApi.Models
{
    public class Document
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        public string Title { get; set; } = null!;
        public string? Tags { get; set; }
        public string? Content { get; set; } // or filepath if real app
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string? FilePath { get; set; }
    }
}
