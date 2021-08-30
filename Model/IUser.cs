using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Model
{
   public interface IUser
    {
        public int? UserId  { get; set; }
    }
}
