using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomBlog.Models.RequestModels
{
    public class ItemRequest : EditImageRequest
    {
        public ItemRequest()
        {
            this.IsActive = true;
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
