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
   public abstract class ID
    {
        [Key]
        public int Id { get; set; }      
        public DateTime Added_time { get; set; } = DateTime.Now;//添加时间
        /// <summary>
        /// true，为系统、行政，即学校或系统，false为个人。为true表示是系统发布的信息资源
        /// </summary>
        public bool Sys { get; set; } = true;
        /// <summary>
        /// 公开，是否公开显示到前台界面
        /// </summary>
        public bool Open { get; set; } = true;
        /// <summary>
        /// 显示到前台界面，审核是否通过。有些资源需要申请为公开，然后审核通过后才能显示到前台界面
        /// </summary>
        public bool Passed { get; set; } = true;
        /// <summary>
        /// 排序因子,默认系统不会根据Order排序，客户端前台可以通过查询参数指定排列方式
        /// </summary>
        public int Order { get; set; }
    }
}
