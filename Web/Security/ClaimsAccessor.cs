using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Security
{
    public class ClaimsAccessor : IClaimsAccessor
    {
        protected IPrincipalAccessor PrincipalAccessor { get; set; }

        public ClaimsAccessor(IPrincipalAccessor principalAccessor)
        {
            PrincipalAccessor = principalAccessor;
        }
        /// <summary>
        /// 登录用户ID
        /// </summary>
        public int Id
        {
            get
            {
                var userId = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value;
                if (userId != null)
                {
                    int id = 0;
                    int.TryParse(userId, out id);
                    return id;
                }
                return 0;
            }
        }
        public string Name
        {
            get
            {
                var roleIds = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                if (string.IsNullOrWhiteSpace(roleIds))
                {
                    return string.Empty;
                }

                return roleIds;
            }
        }
        /// <summary>
        /// 用户角色
        /// </summary>
        public string Role
        {
            get
            {
                var roleIds = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (string.IsNullOrWhiteSpace(roleIds))
                {
                    return string.Empty;
                }

                return roleIds;
            }
        }
        public string Project
        {
            get
            {
                var roleIds = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == "Project")?.Value;
                if (string.IsNullOrWhiteSpace(roleIds))
                {
                    return string.Empty;
                }

                return roleIds;
            }
        }
        public string Ip
        {
            get
            {
                var ip = PrincipalAccessor.HttpContext?.Request?.Headers["X-Real-IP"].FirstOrDefault() ?? PrincipalAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                if (string.IsNullOrWhiteSpace(ip))
                {
                    return string.Empty;
                }

                return ip;
            }
        }
        public int? ClasssId
        {
            get
            {
                var v = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == "ClasssId")?.Value;
                if (!string.IsNullOrEmpty(v))
                {
                    int _v = 0;
                    int.TryParse(v, out _v);
                    return _v;
                }
                return 0;
            }
        }
        public int? SchoolId
        {
            get
            {
                var v = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == "SchoolId")?.Value;
                if (!string.IsNullOrEmpty(v))
                {
                    int _v = 0;
                    int.TryParse(v, out _v);
                    return _v;
                }
                return 0;
            }
        }
        public string Type
        {
            get
            {
                var v = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == "Type")?.Value;
                if (string.IsNullOrWhiteSpace(v))
                {
                    return string.Empty;
                }
                return v;
            }
        }
    }

    public interface IClaimsAccessor
    {
        /// <summary>
        /// 登录用户ID
        /// </summary>
        int Id { get; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 用户角色
        /// </summary>
        string Role { get; }
        /// <summary>
        /// 其他身份
        /// </summary>
        string Project { get; }
        string Ip { get; }
        public int? ClasssId { get;  }
        /// <summary>
        /// 学校id
        /// </summary>
        public int? SchoolId { get;  }
        /// <summary>
        /// 身份类型
        /// </summary>
        public string Type { get; }
    }
}
