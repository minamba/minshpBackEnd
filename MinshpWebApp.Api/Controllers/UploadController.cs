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
    [Route("Upload")]
    public class UploadController : Controller
    {
        private readonly IWebHostEnvironment _env;
        IImageViewModelBuilder _imageViewModelBuilder;
        IVideoViewModelBuilder _videoViewModelBuilder;

        const string images = "images";
        const string videos = "videos";
        const string add = "add";
        const string update = "update";

        public UploadController(IWebHostEnvironment env, IVideoViewModelBuilder videoViewModelBuilder, IImageViewModelBuilder imageViewModelBuilder)
        {
            _env = env;
            _videoViewModelBuilder = videoViewModelBuilder;
            _imageViewModelBuilder = imageViewModelBuilder;
        }

        [RequestSizeLimit(524288000)]
        [Consumes("multipart/form-data")]
        [HttpPost("/upload")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "ajout temoignage")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> UploadDocument([FromForm] UploadRequest request)
        {
            string folder = null;
            string requestType = null;
            string fName = null;
            string fileUrl = null;


            if (!Enum.TryParse<MediasType>(request.Type, true, out var mediaType))
                return BadRequest("Type invalide. Utilise IMAGE, VIDEO, SOUND.");


            folder = mediaType switch
            {
                MediasType.IMAGE => images,
                MediasType.VIDEO => videos,
                _ => images
            };


            if (request.TypeUpload == "ADD")
            {
                if (request.File == null)
                    return Ok("création sans l'ajout du fichier");

                if (request.File == null || request.File.Length == 0)
                    return BadRequest("Fichier non fourni."); 
            }

            if (!Enum.TryParse<UploadType>(request.TypeUpload, true, out var uploadType))
                return BadRequest("Type upload invalide. Utilise ADD, UPLOAD");

            requestType = uploadType switch
            {
                UploadType.UPLOAD => update,
                UploadType.ADD => add,
                _ => add
            };

            if (request.File != null)
            {
                fName = Path.GetFileName(request.File.FileName);
                fileUrl = $"{Request.Scheme}://{Request.Host}/{folder}/{fName}";
                
            }
      
               

            // URL d'accès si exposée



            if (folder == images)
            {

                if (requestType == add)
                    _imageViewModelBuilder.AddImagesAsync(new ImageRequest { Description = request.Description, Url = fileUrl, IdProduct = request.IdProduct });
                else
                    _imageViewModelBuilder.UpdateImagesAsync(new ImageRequest { Id = request.Id, Description = request.Description, Url = fileUrl, IdProduct = request.IdProduct });

            }
            else
            {
                if (requestType == add)
                    _videoViewModelBuilder.AddVideoAsync(new VideoRequest { Description = request.Description, Url = fileUrl, IdProduct = request.IdProduct });
                else
                    _videoViewModelBuilder.UpdateVideoAsync(new VideoRequest { Id = request.Id, Description = request.Description, Url = fileUrl, IdProduct = request.IdProduct });
            }



            if (request.File != null)
            {
                // Dossier de destination
                var uploadsFolder = Path.Combine(_env.WebRootPath, folder);

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Nom de fichier sécurisé
                var filePath = Path.Combine(uploadsFolder, fName);

                // Sauvegarde du fichier
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }
            }


            return Ok(new { url = fileUrl });
        }
    }
}
