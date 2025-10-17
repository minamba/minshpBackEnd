using System.Text;
using System.Text.RegularExpressions;

namespace MinshpWebApp.Api.Utils
{
    public static class FileUploadHelper
    {
        /// <summary>
        /// Slugifie une chaîne pour utilisation en nom de dossier/fichier (sans accents, minuscules, tirets).
        /// </summary>
        public static string Slugify(string? input)
        {
            input ??= string.Empty;

            // Normalisation (supprime accents)
            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            var noAccents = sb.ToString().Normalize(NormalizationForm.FormC);

            // Remplacements de base
            noAccents = noAccents.Trim().ToLowerInvariant();
            noAccents = noAccents.Replace("’", "'"); // apostrophes typographiques

            // Remplace tout ce qui n'est pas lettre, chiffre ou - par un séparateur espace
            noAccents = Regex.Replace(noAccents, @"[^a-z0-9]+", "-");

            // Compacte les tirets
            noAccents = Regex.Replace(noAccents, @"-+", "-").Trim('-');

            return string.IsNullOrWhiteSpace(noAccents) ? "n-a" : noAccents;
        }

        /// <summary>
        /// Rend un nom de fichier "safe" mais garde l'extension.
        /// </summary>
        public static string MakeSafeFileName(string originalFileName)
        {
            var originalName = Path.GetFileName(originalFileName ?? "file");
            var stem = Path.GetFileNameWithoutExtension(originalName);
            var ext = Path.GetExtension(originalName);

            // slugify du stem uniquement, pour garder l'extension
            var safeStem = Slugify(stem);
            // Extensions : nettoie au cas où
            var safeExt = Regex.Replace(ext ?? "", @"[^a-zA-Z0-9\.\-]", "");

            if (string.IsNullOrWhiteSpace(safeExt))
                safeExt = ".bin";

            return $"{safeStem}{safeExt.ToLowerInvariant()}";
        }

        /// <summary>
        /// Assure l'existence du dossier cible : wwwroot/{baseFolder}/{brand-model}
        /// Renvoie (absoluteFolderPath, relativeFolderPath)
        /// </summary>
        public static (string absoluteFolder, string relativeFolder) EnsureBrandModelFolder(
            IWebHostEnvironment env,
            string baseFolder, // "images" ou "videos"
            string brand,
            string model)
        {
            var brandSlug = Slugify(brand);
            var modelSlug = Slugify(model);
            var folderRel = Path.Combine(baseFolder, $"{brandSlug}-{modelSlug}").Replace("\\", "/");

            var folderAbs = Path.Combine(env.WebRootPath, folderRel);

            if (!Directory.Exists(folderAbs))
                Directory.CreateDirectory(folderAbs);

            return (folderAbs, folderRel);
        }

        /// <summary>
        /// Évite les collisions : si le fichier existe, suffixe un timestamp.
        /// </summary>
        public static (string fileName, string fullPath) GetNonCollidingPath(string folderAbs, string fileName)
        {
            var fullPath = Path.Combine(folderAbs, fileName);

            if (System.IO.File.Exists(fullPath))
            {
                var stem = Path.GetFileNameWithoutExtension(fileName);
                var ext = Path.GetExtension(fileName);
                var unique = $"{stem}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}";
                fullPath = Path.Combine(folderAbs, unique);
                return (unique, fullPath);
            }

            return (fileName, fullPath);
        }
    }
}
