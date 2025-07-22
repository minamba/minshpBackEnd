namespace MinshpWebApp.Api.Request
{
    public class UploadRequest
    {
        public int Id { get; set; }
        public IFormFile? File { get; set; }
        public string? Type { get; set; }

        public int? IdProduct { get; set; }

        public string? Description { get; set; }

        public string? TypeUpload { get; set; }

        public int? Position { get; set; }
    }
}
