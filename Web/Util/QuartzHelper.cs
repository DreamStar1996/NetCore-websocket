using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Autofac;

namespace Web.Util
{
    /// <summary>
    /// 定时器
    /// 参考：https://blog.csdn.net/xiaolu1014/article/details/103880704
    /// https://blog.csdn.net/weixin_44518486/article/details/101424176
    /// https://blog.csdn.net/yangshangwei/article/details/78539433
    /// https://blog.csdn.net/richard_666/article/details/89295460
    /// https://www.cnblogs.com/wudequn/p/8506938.html
    /// 定时器帮助类
    /// </summary>
    public class QuartzHelper
    {
        private static ISchedulerFactory _schedulerFactory;
        // private static IScheduler _scheduler;
        private static IOCJobFactory _IOCJobFactory;

        /// <summary>
        /// 添加作业任务任务
        /// </summary>
        /// <param name="type">作业类</param>
        /// <param name="jobKey">键，作业名、组</param>
        /// <param name="trigger">触发器</param>
        /// <param name="data">传递到job的数据</param>

        public static async Task Add(Type type, JobKey jobKey, ITrigger trigger = null, IDictionary<string, object> data = null)
        {
            Init();
            //通过工场类获得调度器
            var _scheduler = await _schedulerFactory.GetScheduler();
            _scheduler.JobFactory = _IOCJobFactory;
            //开启调度器
            await _scheduler.Start();
            //创建触发器(也叫时间策略)
            if (trigger == null)
            {
                trigger = TriggerBuilder.Create()
                    .WithIdentity(jobKey.Name, jobKey.Group)
                    .WithDescription("default")
                    .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionFireNow().WithRepeatCount(-1))
                    .Build();
            }
            //创建作业实例
            //Jobs即我们需要执行的作业
            var job = JobBuilder.Create(type).UsingJobData(new JobDataMap(data))
                .WithIdentity(jobKey)
                .Build();
            //将触发器和作业任务绑定到调度器中
            await _scheduler.ScheduleJob(job, trigger);
        }
        /// <summary>
        /// 添加作业任务任务,多少秒之后触发
        /// </summary>
        /// <param name="type"></param>
        /// <param name="jobKey"></param>
        /// <param name="second">秒</param>
        /// <param name="data">传递到job的数据</param>
        /// <returns></returns>
        public static async Task AddAfter(Type type, JobKey jobKey, long second, IDictionary<string, object> data = null)
        {
            var trigger = TriggerBuilder.Create()
                                        .WithSimpleSchedule()
                                        .StartAt(DateTimeOffset.Now.AddSeconds(second))
                                        .Build();
            await Add(type, jobKey, trigger, data);
        }
        /// <summary>
        /// 添加作业任务任务
        /// </summary>
        /// <param name="type"></param>
        /// <param name="jobKey"></param>
        /// <param name="cron"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task AddAt(Type type, JobKey jobKey, string cron, IDictionary<string, object> data = null)
        {
            //"0/5 * 8-22 ? * 1-5"
            ITrigger trigger = TriggerBuilder.Create().
                WithIdentity(jobKey.Name, jobKey.Group).WithCronSchedule(cron).Build();
            await Add(type, jobKey, trigger, data);
        }
        /// <summary>
        /// 恢复任务
        /// </summary>
        /// <param name="jobKey">键</param>
        public static async Task Resume(JobKey jobKey)
        {
            Init();
            var _scheduler = await _schedulerFactory.GetScheduler();
            //LogUtil.Debug($"恢复任务{jobKey.Group},{jobKey.Name}");
            await _scheduler.ResumeJob(jobKey);
        }
        /// <summary>
        /// 停止任务
        /// </summary>
        /// <param name="jobKey">键</param>
        public static async Task Stop(JobKey jobKey)
        {
            Init();
            var _scheduler = await _schedulerFactory.GetScheduler();
            //LogUtil.Debug($"暂停任务{jobKey.Group},{jobKey.Name}");
            await _scheduler.PauseJob(jobKey);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private static void Init()
        {
            if (_schedulerFactory == null)
            {
                _schedulerFactory = ConfigHelper.GetService<ISchedulerFactory>();
            }
            if (_IOCJobFactory == null)
            {
                _IOCJobFactory = ConfigHelper.GetService<IOCJobFactory>();
            }
        }
    }
    /// <summary>
    /// IJob的“中间” 实现，这里我们命名为QuartzJobRunner，该实现位于IJobFactory和要运行的IJob之间
    /// </summary>
    public class QuartzJobRunner : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        public QuartzJobRunner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var jobType = context.JobDetail.JobType;
                var job = scope.ServiceProvider.GetRequiredService(jobType) as IJob;
                await job.Execute(context);
            }
        }
    }
    public class IOCJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public IOCJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            //return _serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;
            return _serviceProvider.GetRequiredService<QuartzJobRunner>();
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }

    /// <summary>
    /// Job调度中间对象
    /// </summary>
    [NotAutofac]
    public class JobSchedule
    {
        public JobSchedule(string name, Type jobType, string cronExpression, IDictionary<string, object> data)
        {
            this.Name = name;
            this.JobType = jobType ?? throw new ArgumentNullException(nameof(jobType));
            this.Cron = cronExpression ?? throw new ArgumentNullException(nameof(cronExpression));
            this.Data = data;
        }
        /// <summary>
        /// 任务名称要唯一
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Job类型
        /// </summary>
        public Type JobType { get; private set; }
        /// <summary>
        /// Cron表达式
        /// </summary>
        public string Cron { get; private set; }
        /// <summary>
        /// 向job传递的数据
        /// </summary>
        public IDictionary<string, object> Data { get; private set; }
        /// <summary>
        /// Job状态
        /// </summary>
        public JobStatus JobStatu { get; set; } = JobStatus.Init;
    }

    /// <summary>
    /// Job运行状态
    /// </summary>
    public enum JobStatus : byte
    {
        [Description("初始化")]
        Init = 0,
        [Description("运行中")]
        Running = 1,
        [Description("调度中")]
        Scheduling = 2,
        [Description("已停止")]
        Stopped = 3,

    }

    /// <summary>
    /// https://www.cnblogs.com/yilezhu/p/12644208.html
    /// 
    /// https://www.cnblogs.com/dayang12525/p/13083026.html
    /// 
    /// https://www.cnblogs.com/yilezhu/p/12757411.html
    /// </summary>
    [NotAutofac]
    public class QuartzHostedService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly List<JobSchedule> _jobSchedules = new List<JobSchedule>();

        public QuartzHostedService(ISchedulerFactory schedulerFactory, IJobFactory jobFactory, IServiceProvider serviceProvider)
        {
            _schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
            _jobFactory = jobFactory ?? throw new ArgumentNullException(nameof(jobFactory));
            // _jobSchedules = jobSchedules ?? throw new ArgumentNullException(nameof(jobSchedules));
            //Console.WriteLine("serviceProvider：" + serviceProvider);
            var jobProviders = serviceProvider.GetServices<IJobProvider>();
            //Console.WriteLine("任务个数：" + jobSchedules.Count());
            foreach (var jobProvider in jobProviders)
            {
                jobProvider.Init();//初始化
                this._jobSchedules = jobProvider.Jobs();
                //foreach (var job in jobProvider.Jobs())
                //{
                //    //Console.WriteLine("任务：" + job.JobType + " Cron表达式：" + job.Cron + "  数据:" + job.Data);
                //    //_jobSchedules.Add(new JobSchedule(jobType: job.JobType, cronExpression: job.Cron, job.Data));
                //    _jobSchedules.Add(job);
                //}             
            }
        }
        public IScheduler Scheduler { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;
            await Scheduler.Clear();//清除原来的 防止报已经存在错误
            foreach (var jobSchedule in _jobSchedules)
            {
                var job = CreateJob(jobSchedule);
                var trigger = CreateTrigger(jobSchedule);
                await Scheduler.ScheduleJob(job, trigger, cancellationToken);
                jobSchedule.JobStatu = JobStatus.Scheduling;
            }
            await Scheduler.Start(cancellationToken);
            foreach (var jobSchedule in _jobSchedules)
            {
                jobSchedule.JobStatu = JobStatus.Running;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Scheduler?.Shutdown(cancellationToken);
            foreach (var jobSchedule in _jobSchedules)
            {

                jobSchedule.JobStatu = JobStatus.Stopped;
            }
        }

        private static IJobDetail CreateJob(JobSchedule schedule)
        {
            var jobType = schedule.JobType;
            return JobBuilder
                .Create(jobType).UsingJobData(new JobDataMap(schedule.Data == null ? new Dictionary<string, object>() : schedule.Data))
                .WithIdentity(schedule.Name)
                .WithDescription(schedule.Name)
                .Build();
        }

        private static ITrigger CreateTrigger(JobSchedule schedule)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity($"{schedule.Name}.trigger")
                .WithCronSchedule(schedule.Cron)
                .WithDescription(schedule.Cron)
                .Build();
        }
    }
  /// <summary>
  /// job提供者
  /// </summary>
    public interface IJobProvider
    {
        //Type jobType, string cronExpression, IDictionary<string, object> Data
        /// <summary>
        /// job类型
        /// </summary>
        /// <returns></returns>
        //Type JobType();
        /// <summary>
        /// 表达式
        /// </summary>
        /// <returns></returns>
        //string Cron();
        /// <summary>
        /// 传递给job的数据
        /// </summary>
        /// <returns></returns>
        /// IDictionary<string, object> Data();

        void Init();
        List<JobSchedule> Jobs();
    }
}

