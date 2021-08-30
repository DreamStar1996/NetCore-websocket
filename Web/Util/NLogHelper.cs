using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Util
{
    /// <summary>
    /// 日志帮助类。调用方法：
    /// NLogHelper.logger.Error("错误消息");
    /// </summary>
    public class NLogHelper : NLog.Logger
    {
        /// <summary>
        /// 获取当前调用类的日志对象
        /// </summary>
        public static readonly NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "nlog.config").GetCurrentClassLogger();
    }
}
