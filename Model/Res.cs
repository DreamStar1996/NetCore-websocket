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
     * 资源
     */
    [Serializable]
    [Table("Res")]
    public class Res : UserID
    {

        [Display(Name = "名称")]
        [Required(ErrorMessage = "名称必填")]
        public string Name { get; set; }
        /**
         * 资源值 ,如网址、方法名
         */
        public string Value { get; set; }      
        [Display(Name = "简介")]
        public string Intro { get; set; }
    }
}