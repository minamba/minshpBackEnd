using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Enums;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.Utils; // <<— Helper (Slugify, EnsureBrandModelFolder, etc.)
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
        private readonly IProductViewModelBuilder _productViewModelBuilder;

        private const string Images = "images";
        private const string Videos = "videos";
        private const string Add = "add";
        private const string Update = "update";

        public UploadController(
            IWebHostEnvironment env,
            IVideoViewModelBuilder videoViewModelBuilder,
            IImageViewModelBuilder imageViewModelBuilder,
            IProductViewModelBuilder productViewModelBuilder)
        {
            _env = env;
            _videoViewModelBuilder = videoViewModelBuilder;
            _imageViewModelBuilder = imageViewModelBuilder;
            _productViewModelBuilder = productViewModelBuilder;
        }

        [RequestSizeLimit(524_288_000)]
        [Consumes("multipart/form-data")]
        [HttpPost("/upload")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "ajout d'une image/vidéo")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> UploadDocument([FromForm] UploadRequest request)
        {
            // ---- 1) Validation des enums
            if (!Enum.TryParse<MediasType>(request.Type, true, out var mediaType))
                return BadRequest("Type invalide. Utilise IMAGE, VIDEO, SOUND.");

            if (!Enum.TryParse<UploadType>(request.TypeUpload, true, out var uploadType))
                return BadRequest("Type upload invalide. Utilise ADD, UPDATE.");

            // ---- 2) Dossier racine cible
            var baseFolder = mediaType switch
            {
                MediasType.IMAGE => Images,
                MediasType.VIDEO => Videos,
                _ => Images
            };

            var requestType = uploadType switch
            {
                UploadType.UPLOAD => Update, // garde ta logique existante
                UploadType.ADD => Add,
                _ => Add
            };

            // ---- 3) Si ADD, on attend un fichier (sinon c'est juste une création "métadonnée")
            if (uploadType == UploadType.ADD)
            {
                if (request.File == null)
                    return Ok("création sans l'ajout du fichier");

                if (request.File.Length == 0)
                    return BadRequest("Fichier non fourni.");
            }

            string? relativePath = null;  // <<— à SAUVEGARDER en base
            string? absoluteUrl = null;   // <<— retournée au front

            // ---- 4) Upload physique si un fichier est posté (ADD ou UPDATE avec fichier)
            if (request.File != null)
            {
                // 4.a) Nom de fichier safe
                var safeName = FileUploadHelper.MakeSafeFileName(request.File.FileName);

                // 4.b) Dossier final : brand-model si brand & model fournis, sinon dossier racine (images/videos)
                string targetAbsFolder;
                string targetRelFolder;


                var product = (await _productViewModelBuilder.GetProductsAsync()).FirstOrDefault(p => p.Id == request.IdProduct);

                if (!string.IsNullOrWhiteSpace(product.Brand) && !string.IsNullOrWhiteSpace(product.Model))
                {
                    (targetAbsFolder, targetRelFolder) =
                        FileUploadHelper.EnsureBrandModelFolder(_env, baseFolder, product.Brand!, product.Model!);
                }
                else
                {
                    targetRelFolder = baseFolder;
                    targetAbsFolder = Path.Combine(_env.WebRootPath, targetRelFolder);
                    if (!Directory.Exists(targetAbsFolder))
                        Directory.CreateDirectory(targetAbsFolder);
                }

                // 4.c) Évite les collisions
                var (finalFileName, fullPath) = FileUploadHelper.GetNonCollidingPath(targetAbsFolder, safeName);

                // 4.d) Sauvegarde
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

                // 4.e) Chemins de sortie
                relativePath = $"{targetRelFolder}/{finalFileName}".Replace("\\", "/");
                absoluteUrl = $"{Request.Scheme}://{Request.Host}/{relativePath}";
            }

            // ---- 5) Enregistrement en base avec le CHEMIN RELATIF
            if (baseFolder == Images)
            {
                if (requestType == Add)
                {
                    await _imageViewModelBuilder.AddImagesAsync(new ImageRequest
                    {
                        Description = request.Description,
                        Url = relativePath, // peut être null si "création sans upload"
                        IdProduct = request.IdProduct,
                        Title = request.Title,
                        Position = request.Position,
                        Display = request.Display
                    });
                }
                else // update
                {
                    await _imageViewModelBuilder.UpdateImagesAsync(new ImageRequest
                    {
                        Id = request.Id,
                        Description = request.Description,
                        Url = relativePath, // laisse null si pas de nouveau fichier
                        IdProduct = request.IdProduct,
                        Title = request.Title,
                        Position = request.Position,
                        Display = request.Display
                    });
                }
            }
            else // videos
            {
                if (requestType == Add)
                {
                    await _videoViewModelBuilder.AddVideoAsync(new VideoRequest
                    {
                        Description = request.Description,
                        Url = relativePath,
                        IdProduct = request.IdProduct,
                        Title = request.Title,
                        Position = request.Position,
                        Display = request.Display
                    });
                }
                else // update
                {
                    await _videoViewModelBuilder.UpdateVideoAsync(new VideoRequest
                    {
                        Id = request.Id,
                        Description = request.Description,
                        Url = relativePath, // null si pas de nouveau fichier
                        IdProduct = request.IdProduct,
                        Title = request.Title,
                        Position = request.Position,
                        Display = request.Display
                    });
                }
            }

            // ---- 6) Réponse front
            return Ok(new
            {
                path = relativePath, // à stocker côté front si besoin
                url = absoluteUrl    // pratique pour prévisualiser
            });
        }
    }
}
