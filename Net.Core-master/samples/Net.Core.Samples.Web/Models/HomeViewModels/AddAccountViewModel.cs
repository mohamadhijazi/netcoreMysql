using Net.Core.Common.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace net.core.Samples.Web.Models.HomeViewModels
{
    public class AddAccountViewModel
    {
        [Required]
        [Display(Name = "CustomerID")]
        public int CustomerID { get; set; }

        [Required]
        [Display(Name = "InitialCredit")]
        public float InitialCredit { get; set; }
    }
}
