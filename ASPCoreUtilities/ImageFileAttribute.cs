using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Utilities.Extensions;

namespace ASPCoreUtilities
{
    public class ImageFileAttribute : ValidationAttribute
    {
        readonly int _maxFileSize;
        public ImageFileAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                extension.IsOneOf(".jpg", ".png");
                if (file.Length > _maxFileSize || !extension.IsOneOf(".jpg", ".png"))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Maximum allowed file size is {_maxFileSize/(1024*1024):F2} MB.";
        }
    }
}
