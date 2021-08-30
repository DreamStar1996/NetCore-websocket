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
     * 角色权限中间表，多对多用哪到
     * @see https://docs.microsoft.com/zh-cn/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key
     */
    [Serializable]
    [Table("RoleAuthority")]
    public class RoleAuthority : UserID
    {     
        public int RoleId { get; set; } 
        public virtual Role Role { get; set; }
        public int AuthorityId { get; set; }
        public virtual Authority Authority { get; set; }            
    }
}