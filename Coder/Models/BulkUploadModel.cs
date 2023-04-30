namespace Coder.Models
{
    public class BulkUploadModel
    {
        public string? FileName { get; set; }
        public IEnumerable<ApplicationUser>? Users { get; set; }
        public ResponseModel? Response { get; set; }
    }
}
