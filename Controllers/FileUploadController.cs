using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StudyGroup.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly long _maxFileSize = 10 * 1024 * 1024; // 10MB
        private readonly string[] _allowedImageTypes = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private readonly string[] _allowedVideoTypes = { ".mp4", ".webm", ".ogg", ".mov", ".avi" };
        private readonly string[] _allowedFileTypes = { ".pdf", ".doc", ".docx", ".txt", ".zip", ".pptx", ".xlsx" };

        public FileUploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }

        /// <summary>
        /// POST: api/fileupload/image
        /// Upload an image file for messaging
        /// </summary>
        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            if (file == null || file.Length == 0)
                return BadRequest(new { Error = "No file uploaded" });

            if (file.Length > _maxFileSize)
                return BadRequest(new { Error = $"File size exceeds limit of {_maxFileSize / (1024 * 1024)}MB" });

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedImageTypes.Contains(extension))
                return BadRequest(new { Error = "Invalid image file type. Allowed: jpg, jpeg, png, gif, webp" });

            try
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "images");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileUrl = $"/uploads/images/{uniqueFileName}";

                return Ok(new
                {
                    Success = true,
                    Message = "?? Image uploaded successfully",
                    FileName = file.FileName,
                    FileUrl = fileUrl,
                    FileSize = file.Length,
                    FileType = "Image",
                    UploadedBy = currentUserId.Value
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Failed to upload image", Details = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/fileupload/video
        /// Upload a video file for messaging
        /// </summary>
        [HttpPost("video")]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            if (file == null || file.Length == 0)
                return BadRequest(new { Error = "No file uploaded" });

            if (file.Length > _maxFileSize * 5) // 50MB for videos
                return BadRequest(new { Error = $"Video file size exceeds limit of {(_maxFileSize * 5) / (1024 * 1024)}MB" });

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedVideoTypes.Contains(extension))
                return BadRequest(new { Error = "Invalid video file type. Allowed: mp4, webm, ogg, mov, avi" });

            try
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "videos");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileUrl = $"/uploads/videos/{uniqueFileName}";

                return Ok(new
                {
                    Success = true,
                    Message = "?? Video uploaded successfully",
                    FileName = file.FileName,
                    FileUrl = fileUrl,
                    FileSize = file.Length,
                    FileType = "Video",
                    UploadedBy = currentUserId.Value
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Failed to upload video", Details = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/fileupload/file
        /// Upload a general file for messaging
        /// </summary>
        [HttpPost("file")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            if (file == null || file.Length == 0)
                return BadRequest(new { Error = "No file uploaded" });

            if (file.Length > _maxFileSize)
                return BadRequest(new { Error = $"File size exceeds limit of {_maxFileSize / (1024 * 1024)}MB" });

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedFileTypes.Contains(extension))
                return BadRequest(new { Error = "Invalid file type. Allowed: pdf, doc, docx, txt, zip, pptx, xlsx" });

            try
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "files");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileUrl = $"/uploads/files/{uniqueFileName}";

                return Ok(new
                {
                    Success = true,
                    Message = "?? File uploaded successfully",
                    FileName = file.FileName,
                    FileUrl = fileUrl,
                    FileSize = file.Length,
                    FileType = "File",
                    UploadedBy = currentUserId.Value
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Failed to upload file", Details = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/fileupload/limits
        /// Get file upload limits and allowed types
        /// </summary>
        [HttpGet("limits")]
        public IActionResult GetUploadLimits()
        {
            return Ok(new
            {
                MaxFileSizes = new
                {
                    Images = $"{_maxFileSize / (1024 * 1024)}MB",
                    Videos = $"{(_maxFileSize * 5) / (1024 * 1024)}MB",
                    Files = $"{_maxFileSize / (1024 * 1024)}MB"
                },
                AllowedTypes = new
                {
                    Images = _allowedImageTypes,
                    Videos = _allowedVideoTypes,
                    Files = _allowedFileTypes
                },
                Message = "?? File upload specifications"
            });
        }
    }
}