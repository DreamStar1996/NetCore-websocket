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
     * 角色
     */
    [Serializable]
    [Table("Role")]
    public class Role : UserID
    {

        [Display(Name = "名称")]
        [Required(ErrorMessage = "名称必填")]
        public string Name { get; set; }
        /**
         * 角色标识，用于程序内部区别查找
        */
        [Display(Name = "角色码")]
        public string Code { get; set; }
        /**
         * 角色拥有的权限，一个角色可以拥有多个权限，一个权限可以属于多个角色，多对多关系
         */
        public virtual ICollection<RoleAuthority> Authorities { get; set; } = new HashSet<RoleAuthority>();
        [Display(Name = "简介")]
        public string Intro { get; set; }
    }
    /// <summary>
    /// 角色类型,或身份
    /// </summary>
    public enum RoleType
    {
        [Display(Name = "系统会员")]
        user = 1,
        [Display(Name = "教师")]
        teacher = 2,
        [Display(Name = "学生")]
        student = 3
    }

}