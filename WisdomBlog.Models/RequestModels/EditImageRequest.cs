using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomBlog.Models.RequestModels
{
    public class EditImageRequest : UploadImageRequest
    {
        public int Id { get; set; }
        public string ExistingImage { get; set; }
    }
}
