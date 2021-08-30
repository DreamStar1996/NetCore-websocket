using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Model
{
    public enum Gender
    {
        [Display(Name = "男")]
        man=1,
        [Display(Name = "女")]
        woman=2
    }
}
