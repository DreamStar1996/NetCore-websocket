using Bll;
using Common.Util;
using Microsoft.Extensions.Logging;
using Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Extension;
using Web.Security;
using Web.Util;
using Web.Autofac;
using Web.Redis;

namespace Web.Controllers.Job
{
    /// <summary>
    /// 定时示例
    ///DisallowConcurrentExecution, 该属性可防止Quartz.NET尝试同时运行同一作业
    /// </summary>
    //[DisallowConcurrentExecution]
    //public class StartTimerClockJob : IJob, IJobProvider
    //{
    //    private List<JobSchedule> jobs = new List<JobSchedule>();
    //    public ITimeTableBll timeTableBll { get; set; }
    //    public List<JobSchedule> Jobs()
    //    {
    //        return jobs;
    //    }
    //    public void Init()
    //    {
    //        var objs = timeTableBll.SelectAll();
    //        foreach (var obj in objs)
    //        {
    //            //开始时间
    //            var start_time = obj.From_time - new TimeSpan(0, 20, 0);
    //            var start_h = start_time.Hours;
    //            var start_m = start_time.Minutes;
    //            var cron = $"0 {start_m} {start_h} * * ?";
    //            //结束时间
    //            //var end_time = obj.From_time + new TimeSpan(0, 10, 0);
    //            //添加定时任务 在签到结束后将数据持久化到数据库
    //            Dictionary<string, object> data = new Dictionary<string, object>();
    //            data.Add("expire", 30 * 60);
    //            data.Add("timetable_sn", obj.Sn);
    //            NLogHelper.logger.Info($"发布定时签到:开始{start_h}:{start_m},课表sn：{obj.Sn},cors表达式:{cron}");
    //            Console.WriteLine($"发布定时签到:开始{start_h}:{start_m},课表sn：{obj.Sn},cors表达式:{cron}");
    //            this.jobs.Add(new JobSchedule(name: "StartTimerClockJob" + obj.Sn, jobType: typeof(StartTimerClockJob), cronExpression: cron, data: data));
    //        }
    //        MyRedisHelper rd = MyRedisHelper.Instance();
    //        rd.StringSet("timer_clock_status", true);//定时签到状态          
    //    }
    //    public IClassScheduleBll classScheduleBll { get; set; }
    //    public IStudentBll bll { get; set; }
    //    public IClaimsAccessor MyUser { get; set; }
    //    public Task Execute(IJobExecutionContext context)
    //    {
    //        return Task.Run(() =>
    //        {
    //            var timetable_sn = context.JobDetail.JobDataMap.GetString("timetable_sn");
    //            var expire = context.JobDetail.JobDataMap.GetInt("expire");
    //            var classSchedules = classScheduleBll.SelectAll(o => o.TimeTableSn == timetable_sn && o.Date == DateTime.Now.Date);
    //            NLogHelper.logger.Info($"启动定时:" + DateTime.Now.ToLongTimeString() + "，课表数：" + classSchedules.Count());
    //            Console.WriteLine($"启动定时:" + DateTime.Now.ToLongTimeString() + "，课表数：" + classSchedules.Count());
    //            foreach (var cs in classSchedules)
    //            {
    //                Console.WriteLine($"班级：{cs.ClasssId}  课程{cs.CourseId} 教师{cs.TeacherId}");
    //                if (cs.ClasssId != null && cs.ClasssId != 0 && cs.CourseId != null && cs.CourseId != 0 && cs.TeacherId != null && cs.TeacherId != 0)
    //                {
    //                    ClockController.PublishClock(bll, typeof(SaveClockInfoJob), Clock.ClockType.clockin, cs.ClasssId.ToString(), cs.TeacherId.ToString(), cs.CourseId.ToString(), UtilString.GetRandomNum(8), expire, 120);
    //                }
    //                else
    //                {
    //                    NLogHelper.logger.Info($"发布签到任务失败，课表{cs.TimeTableSn}对应班级编号或课程编号或教师编号不存在");
    //                    Console.WriteLine($"发布签到任务失败，课表{cs.TimeTableSn}对应班级编号或课程编号或教师编号不存在");
    //                }
    //            }

    //        });
    //    }
    //}
}
