using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Model
{
    [Serializable]
    public abstract class Person : ID
    {
        [Required(ErrorMessage = "用户名必填")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "用户名长度3到20位")]
        public string Username { get; set; }
        [JsonIgnore]
        [Required(ErrorMessage = "密码必填")]
        [StringLength(40, MinimumLength = 6, ErrorMessage = "密码长度6到20位")]
        public string Pswd { get; set; }
        public string Realname { get; set; }//真实姓名
        /**
         * 角色，可以多个角色。多个之间用,分割角色可以继承
         */
        [Required(ErrorMessage = "角色必填")]
        public String Role { get; set; }
        public String Tel { get; set; }//电话
        public String Email { get; set; }//电子邮箱
        public DateTime Birthday { get; set; }//出生日期
        public String QQ { get; set; }
        public String Address { get; set; }//联系地址
        public Gender Gender { get; set; }//性别
        public String Photo { get; set; }//头像
        public String Nation { get; set; }//民族
        public String IDCard { get; set; }//身份证号码
        public String Zip { get; set; }//邮政编码
        public int? AreaId { get; set; }
        [ForeignKey("AreaId")]
        public virtual Area Area { get; set; }//所在地区
        //[Column(TypeName = "decimal(18, 2)")]
        public double? Available_balance { get; set; } //可用预存款
        //[Column(TypeName = "decimal(18, 2)")]
        public double? Freeze_blance;//冻结预存款
        public int Integral { get; set; }//积分
        public int Gold { get; set; }//金币
        public int Friend_num { get; set; }//好友数
        public int Attention_num { get; set; }//关注数
        public int Attentioned_num { get; set; }//被关注数
        public int Credit { get; set; }//会员信用\
        public DateTime Last_login_date { get; set; }//最后登录时间
        public DateTime Login_date { get; set; }//登陆时间
        public String Last_login_ip { get; set; }//最后登录IP
        public String Login_ip { get; set; }//登录IP
        public int Login_count { get; set; }//登录次数
        public String IMEI { get; set; }//国际移动设备识别码
        /// <summary>
        /// 紧急联系电话
        /// </summary>
        public String Emergency_number { get; set; }
        /// <summary>
        /// 资格证书
        /// </summary>
        public String Credentials { get; set; }
    }
}
