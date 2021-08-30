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
     * 权限资源中间表，多对多用哪到
     * @see https://docs.microsoft.com/zh-cn/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key
     */
    [Serializable]
    [Table("AuthorityRes")]
    public class AuthorityRes : UserID
    {
        public int AuthorityId { get; set; }
        public virtual Authority Authority { get; set; }
        public int ResId { get; set; } 
        public virtual Res Res { get; set; }                
    }
}