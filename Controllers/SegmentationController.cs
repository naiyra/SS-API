using Microsoft.AspNetCore.Mvc;
using SS_API.Models;
using SS_API.Pipeline;
using SS_API.Utils;

namespace SS_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SegmentationController : ControllerBase
    {
        private readonly SmartSegPipeline _pipeline;

        public SegmentationController()
        {
            _pipeline = new SmartSegPipeline();
        }

        [HttpPost("run")]
        public async Task<IActionResult> RunSegmentation([FromForm] UploadFileModel upload)
        {
            if (upload.File == null || upload.File.Length == 0)
                return BadRequest(new { success = false, message = "No file uploaded.", data = (object)null });

            try
            {
                using var stream = upload.File.OpenReadStream();
                var df = CsvLoader.LoadCsv(stream);

                
                if (upload.LimitRows)
                {
                    
                    df = df.Take(1000);
                }

                var segments = _pipeline.Run(df);

                return Ok(new
                {
                    success = true,
                    message = "Segmentation completed successfully.",
                    data = segments
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message, data = (object)null });
            }
        }
    }
}
