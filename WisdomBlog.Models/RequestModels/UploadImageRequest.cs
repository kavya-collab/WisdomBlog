using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomBlog.Models.RequestModels
{
    public class UploadImageRequest
    {
        [Display(Name = "Image")]
        public IFormFile Image { get; set; }
    }
}
