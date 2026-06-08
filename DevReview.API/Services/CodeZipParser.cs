using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using DevReview.Application.DTOs;

namespace DevReview.API.Services
{
    public static class CodeZipParser
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".cs", ".ts", ".tsx", ".js", ".jsx", ".py", ".java", ".go", ".rs",
            ".cpp", ".h", ".json", ".md", ".html", ".css", ".scss", ".vue", ".sql", ".rb", ".php"
        };

        private const int MaxFileChars = 500_000;

        public static IList<CodeFileDto> Parse(Stream zipStream)
        {
            var result = new List<CodeFileDto>();
            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: true);

            var order = 0;
            foreach (var entry in archive.Entries)
            {
                if (string.IsNullOrEmpty(entry.Name))
                {
                    continue;
                }

                var extension = Path.GetExtension(entry.Name);
                if (!AllowedExtensions.Contains(extension))
                {
                    continue;
                }

                using var reader = new StreamReader(entry.Open());
                var content = reader.ReadToEnd();
                if (content.Length > MaxFileChars)
                {
                    continue;
                }

                result.Add(new CodeFileDto
                {
                    FileName = entry.FullName.Replace('\\', '/'),
                    Content = content,
                    OrderIndex = order++
                });
            }

            if (result.Count == 0)
            {
                throw new InvalidOperationException("No supported source files were found in the ZIP archive.");
            }

            return result;
        }
    }
}
