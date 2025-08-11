using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SS_API.Models
{
    public class UploadFileModel
    {
        [Required]
        public IFormFile File { get; set; }

        public bool LimitRows { get; set; } = false;
    }
}
