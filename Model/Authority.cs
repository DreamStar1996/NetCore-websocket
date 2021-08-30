using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    /**
     * 权限
     */
    [Serializable]
    [Table("Authority")]
    public class Authority : UserID
    {    
        /**
         * 
         * 权限名称,用于界面显示
         */
        [Display(Name = "名称")]
        [Required(ErrorMessage = "名称必填")]
        public string Name { get; set; }
        /**
         * 权限标识，用于程序内部区别查找
        */
        [Display(Name = "权限码")]
        public string Code { get; set; }
        //权限对应的资源（一个权限有多重资源，一个资源可以被多个权限拥有）  一个资源对应一个权限？或者一个资源可以对应多个权限？哪种更优？  
        public virtual ICollection<AuthorityRes> Reses { get; set; } = new HashSet<AuthorityRes>();
        //private AuthorityGroup group;      
        [Display(Name = "简介")]
        public string Intro { get; set; }
    }
}