using DocumentManagerApi.Data.Repositories;
using DocumentManagerApi.Models;
using DocumentManagerApi.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentRepository _repo;
        public DocumentsController(IDocumentRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
        {
            var docs = await _repo.GetAllAsync(page, pageSize, search);
            return Ok(docs);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doc = await _repo.GetByIdAsync(id);
            if (doc == null) return NotFound();
            return Ok(doc);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Document doc)
        {
            doc.CreatedAt = DateTime.UtcNow;
            var id = await _repo.CreateAsync(doc);
            doc.Id = id;
            return CreatedAtAction(nameof(GetById), new { id = id }, doc);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Document doc)
        {
            if (id != doc.Id) return BadRequest();
            var updated = await _repo.UpdateAsync(doc);
            if (updated == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (deleted == 0) return NotFound();
            return NoContent();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] DocumentUploadRequest request)
        {
            // Validate caseId exists? (optional)

            string? filePath = null;

            if (request.File != null)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(request.File.FileName);
                var finalPath = Path.Combine(folderPath, uniqueName);

                using (var stream = new FileStream(finalPath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

                filePath = finalPath;
            }

            var doc = new Document
            {
                CaseId = request.CaseId,
                Title = request.Title,
                Tags = request.Tags,
                Content = request.Content,
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow,
                FilePath = filePath
            };

            var id = await _repo.CreateAsync(doc);

            return Ok(new { DocumentId = id, FileSavedAt = filePath });
        }
    }
}
