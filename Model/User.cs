using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Serializable]
    [Table("User")]
    public class User : Person
    {
        public String Wx_openid { get; set; }
        public String Wx_unionid { get; set; }       
    }
}
