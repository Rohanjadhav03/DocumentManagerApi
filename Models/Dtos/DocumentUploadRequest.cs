namespace DocumentManagerApi.Models.Dtos
{
    public class DocumentUploadRequest
    {
        public int CaseId { get; set; }
        public string Title { get; set; }
        public string? Tags { get; set; }
        public string? Content { get; set; }

        public IFormFile? File { get; set; }
    }
}
