namespace WorkExperianceAPI.ModelViewer
{
    public class CompanyCreateDto
    {
        public string CompanyName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        // Company Image
        public IFormFile? CompanyImage { get; set; }

        // Social Media Names
        public List<string> SocialMediaNames { get; set; } = new();

        // URLs
        public List<string> SocialMediaUrls { get; set; } = new();

        // Status
        public List<bool> SocialMediaStatus { get; set; } = new();

        // Images
        public List<IFormFile> SocialMediaImages { get; set; } = new();
    }
}
