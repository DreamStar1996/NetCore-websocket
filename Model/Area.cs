using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Model
{
    /// <summary>
    /// 区域地址
    /// </summary>
    [Serializable]
    [Table("Area")]
    public class Area : TreeID<Area>
    {
        public Area()
        {
            Childs = new HashSet<Area>();
        }
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "区域名称必填")]
        public String Name { get; set; }//名称             
        public int Level { get; set; }//层级      
        public bool Expanded { get; set; }//展开
        public bool Common { get; set; }//设为常用
       
    }
}
