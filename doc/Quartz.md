# 定时器Quartz
定时器帮助类Web.Util.QuartzHelper.cs。有两种定时器用法：作为服务使用定时器、非服务使用定时器

## 1、作为服务使用定时器，即在后台不断执行定时任务
### 在Web/Controllers/Job目录添加定时器服务类。（也可在其他目录）
```
/// <summary>
///定时器服务类 DisallowConcurrentExecution, 该属性可防止Quartz.NET尝试同时运行同一作业
/// </summary>
[DisallowConcurrentExecution]
public class TimerDemo1Job : IJob, IJobProvider
{
    private List<JobSchedule> jobs = new List<JobSchedule>();  
	//第2步 系统调用此方法获取任务列表
    public List<JobSchedule> Jobs()
    {
        return jobs;
    }
	//第1步 系统调用此方法初始化任务列表
    public void Init()
    {  
		var  obj={From_time=定时时间,……}
        //开始时间
        var start_time = obj.From_time - new TimeSpan(0, 20, 0);
        var start_h = start_time.Hours;
        var start_m = start_time.Minutes;
        var cron = $"0 {start_m} {start_h} * * ?";
        //结束时间
        //var end_time = obj.From_time + new TimeSpan(0, 10, 0);          
		//向执行任务传递数据
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("expire", 30 * 60);
		data.Add("taskName", "测试任务1");
        //NLogHelper.logger.Info($"发布定时任务:开始{start_h}:{start_m},cors表达式:{cron}");
        Console.WriteLine($"发布定时任务:开始{start_h}:{start_m},cors表达式:{cron}");
		//添加到任务列表
        this.jobs.Add(new JobSchedule(name: "TimerDemo1Job", jobType: typeof(TimerDemo1Job), cronExpression: cron, data: data));                        
    }
    //可以住人其他业务对象
    //public IStudentBll bll { get; set; }
	//public IClassScheduleBll classScheduleBll { get; set; }
    public IClaimsAccessor MyUser { get; set; }
	//第3步 系统调用此方法执行定时任务（回调方法）
    public Task Execute(IJobExecutionContext context)
    {
        return Task.Run(() =>
        {      
			//通过context.JobDetail.JobDataMap获取初始任务传递过来的数据data
            var expire = context.JobDetail.JobDataMap.GetInt("expire");
			var taskName = context.JobDetail.JobDataMap.GetString("taskName");
            //var classSchedules = classScheduleBll.SelectAll(o => o.TimeTableSn == timetable_sn && o.Date == DateTime.Now.Date);
            Console.WriteLine($"执行启动定时任务:" +taskName);                  
        });
    }
}
```

## 2、作为非服务使用定时器，即只执行一次定时
```
//开始时间
var start_time = obj.From_time - new TimeSpan(0, offset, 0);
var start_h = start_time.Hours;
var start_m = start_time.Minutes;
var cors = $"1 {start_m} {start_h} * * ?";
//向执行任务传递数据
Dictionary<string, object> data = new Dictionary<string, object>();
data.Add("expire", during * 60);
data.Add("taskName", "测试任务2");
//NLogHelper.logger.Info($"发布定时签到:开始{start_h}:{start_m},cors:{cors}");
Console.WriteLine($"发布定时任务:开始{start_h}:{start_m},cors:{cors}");
//添加在某个时间点执行的任务
QuartzHelper.AddAt(typeof(StartTimerClockJob), new JobKey($"TimerClock{obj.Sn}", "TimerClock"), cors, data);


//定时任务类
[DisallowConcurrentExecution]
public class StartTimerClockJob : IJob
{           
    public Task Execute(IJobExecutionContext context)
    {
        return Task.Run(() =>
        {               
            var expire = context.JobDetail.JobDataMap.GetInt("expire");
			var taskName = context.JobDetail.JobDataMap.GetString("taskName");
            Console.WriteLine($"执行启动定时任务:" +taskName);      
        });
    }
}
```