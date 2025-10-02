using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public FilesController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No se ha subido ningún archivo." });
        }

        if (!file.ContentType.StartsWith("image/"))
        {
            return BadRequest(new { message = "El archivo debe ser una imagen." });
        }

        try
        {
            var extension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var folderPath = Path.Combine(_environment.WebRootPath, "images");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"/images/{uniqueFileName}";

            return Ok(new { imageUrl = imageUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error interno al subir el archivo: {ex.Message}" });
        }
    }
}