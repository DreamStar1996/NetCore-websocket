using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Serializable]
    [Table("Images")]
    public class Image : UserID
    {
        //文件在数据库中的显现名称.为空则显示为名字
        public String Title { get; set; }
        //文件保存在磁盘的真实名称，目前不可更改
        public String Name { get; set; }
        //路径
        [Required(ErrorMessage = "路径必填")]
        public String Path { get; set; }
        //SIEZ为oracle的关键词 
        public float Size { get; set; }
    }
}
