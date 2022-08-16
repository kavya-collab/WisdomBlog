using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomBlog.Models.ResponseModels
{
    public class ValidateTokenResponse
    {
        public int? Id { get; set; }
        public bool? IsAdmin { get; set; }
    }
}
