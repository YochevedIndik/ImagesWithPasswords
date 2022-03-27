using ImagesWithPasswords.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImagesWithPasswords.Web.Models
{
    public class ViewImageVm
    {
        public Images Image { get; set; }
        public bool CorrectPassword { get; set; }
        public string Message { get; set; }
    }
}
