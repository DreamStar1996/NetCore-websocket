using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Model
{
    /// <summary>
    /// 统一
    /// </summary>
   public abstract class UserID:ID,IUser
    {       
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }            
    }
}
