using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Enums;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Domain.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace MinshpWebApp.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("Upload")]
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IImageViewModelBuilder _imageViewModelBuilder;
        private readonly IVideoViewModelBuilder _videoViewModelBuilder;

        const string images = "images";
        const string videos = "videos";
        const string add = "add";
        const string update = "update";

        public UploadController(
            IWebHostEnvironment env,
            IVideoViewModelBuilder videoViewModelBuilder,
            IImageViewModelBuilder imageViewModelBuilder)
        {
            _env = env;
            _videoViewModelBuilder = videoViewModelBuilder;
            _imageViewModelBuilder = imageViewModelBuilder;
        }

        [RequestSizeLimit(524_288_000)]
        [Consumes("multipart/form-data")]
        [HttpPost("/upload")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "ajout d'une image/vidéo")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> UploadDocument([FromForm] UploadRequest request)
        {
            // Validation des enums
            if (!Enum.TryParse<MediasType>(request.Type, true, out var mediaType))
                return BadRequest("Type invalide. Utilise IMAGE, VIDEO, SOUND.");

            if (!Enum.TryParse<UploadType>(request.TypeUpload, true, out var uploadType))
                return BadRequest("Type upload invalide. Utilise ADD, UPDATE.");

            // Dossiers cibles
            var folder = mediaType switch
            {
                MediasType.IMAGE => images,
                MediasType.VIDEO => videos,
                _ => images
            };

            var requestType = uploadType switch
            {
                UploadType.UPLOAD => update,
                UploadType.ADD => add,
                _ => add
            };

            // Si ADD, on attend un fichier (sinon c'est juste une création "métadonnée")
            if (uploadType == UploadType.ADD)
            {
                if (request.File == null)
                    return Ok("création sans l'ajout du fichier");

                if (request.File.Length == 0)
                    return BadRequest("Fichier non fourni.");
            }

            string fileName = null;
            string relativePath = null;  // <<— ce qui sera SAUVEGARDÉ EN BASE
            string absoluteUrl = null;   // <<— ce qui sera RETOURNÉ AU FRONT (optionnel)

            if (request.File != null)
            {
                // Nom de fichier sécurisé (garde l'extension)
                var originalName = Path.GetFileName(request.File.FileName);
                var safeName = string.Concat(
                    Path.GetFileNameWithoutExtension(originalName)
                        .Replace(" ", "_")
                        .Replace("..", "_")
                        .Replace("/", "_")
                        .Replace("\\", "_"),
                    Path.GetExtension(originalName)
                );

                // (Option) éviter collisions : ajoute un timestamp si existe déjà
                var uploadsFolder = Path.Combine(_env.WebRootPath, folder);
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                fileName = safeName;
                var fullPath = Path.Combine(uploadsFolder, fileName);
                if (System.IO.File.Exists(fullPath))
                {
                    var stem = Path.GetFileNameWithoutExtension(safeName);
                    var ext = Path.GetExtension(safeName);
                    fileName = $"{stem}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}";
                    fullPath = Path.Combine(uploadsFolder, fileName);
                }

                // Sauvegarde physique
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

                // ✅ Chemin RELATIF à stocker en base
                relativePath = $"{folder}/{fileName}";

                // ✅ URL ABSOLUE (pour la réponse seulement)
                absoluteUrl = $"{Request.Scheme}://{Request.Host}/{relativePath}";
            }

            // Enregistrement en base : on envoie le CHEMIN RELATIF
            if (folder == images)
            {
                if (requestType == add)
                {
                    await _imageViewModelBuilder.AddImagesAsync(new ImageRequest
                    {
                        Description = request.Description,
                        Url = relativePath, // <<— RELATIF
                        IdProduct = request.IdProduct,
                        Title = request.Title,
                        Position = request.Position,
                        Display = request.Display
                    });
                }
                else
                {
                    await _imageViewModelBuilder.UpdateImagesAsync(new ImageRequest
                    {
                        Id = request.Id,
                        Description = request.Description,
                        Url = relativePath, // <<— RELATIF (ou laisse null si tu ne remplaces pas le fichier)
                        IdProduct = request.IdProduct,
                        Title = request.Title,
                        Position = request.Position,
                        Display = request.Display
                    });
                }
            }
            else // videos
            {
                if (requestType == add)
                {
                    await _videoViewModelBuilder.AddVideoAsync(new VideoRequest
                    {
                        Description = request.Description,
                        Url = relativePath, // <<— RELATIF
                        IdProduct = request.IdProduct,
                        Title = request.Title,
                        Position = request.Position,
                        Display = request.Display
                    });
                }
                else
                {
                    await _videoViewModelBuilder.UpdateVideoAsync(new VideoRequest
                    {
                        Id = request.Id,
                        Description = request.Description,
                        Url = relativePath, // <<— RELATIF
                        IdProduct = request.IdProduct,
                        Title = request.Title,
                        Position = request.Position,
                        Display = request.Display
                    });
                }
            }

            // Réponse : on peut renvoyer les deux pour le front
            return Ok(new
            {
                path = relativePath,   // à stocker/utiliser côté front si tu préfères
                url = absoluteUrl      // pratique pour affichage immédiat
            });
        }
    }
}
