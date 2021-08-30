# office帮助类
office帮助类Web.Util.OfficeHelper.cs。主要功能有导入与导出Excel

## 1、导入Excel
```
[Route("api/[controller]/[action]")]
[ApiController]
public class StudentController : MyBaseController
{
    public IClasssBll classsBll { get; set; }
    public IStudentBll bll { get; set; }
    private readonly IWebHostEnvironment webHostEnvironment;
    public StudentController(IWebHostEnvironment hostingEnvironment)
    {
        this.webHostEnvironment = hostingEnvironment;
    }              
    [HttpPost()]        
    public Result Import([FromForm] IFormCollection formCollection)
    {
        //var classsId = Convert.ToInt32(formCollection["classsId"]);
        FormFileCollection fileCollection = (FormFileCollection)formCollection.Files;
        foreach (IFormFile file in fileCollection)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !ExcelExt.Contains(ext))
            {
                 return Result.Error("上传失败,请选择xlsx类型文件");
            }
            if (file.Length > 0)
            {
                if (file.Length > ImgSizeLimit)
                {
                    var size = ImgSizeLimit / 1024;
                    return Result.Error($"上传失败,文件超过大小{size}KB");
                }                  
                DataTable dt = OfficeHelper.ReadExcelToDataTable(file.OpenReadStream());
                int count = 0;
                List<Student> ss = new List<Student>();
                foreach (DataRow row in dt.Rows)
                {
                    Student s = new Student();
                    s.Username = row["学号"].ToString();
                    s.Role = "student";
                    s.Pswd = "****"; 
                    s.Sn = row["学号"].ToString();
                    s.Gender = Gender.man.Parse<Gender>(row["性别"].ToString());                       
                    count++;
                    ss.Add(s);
                }
                MyDbContext db = this.classsBll.DbContext();
                //开始事务
                IDbContextTransaction transaction = this.classsBll.DbContext().Database.BeginTransaction();
                try
                {
                    this.bll.Add(ss);
                    //修改班级人数
                   var cls = this.classsBll.SelectOne(classsId);
                   cls.Count = count;                                              
                   this.classsBll.Update(cls);
                   //提交事务
                   transaction.Commit();
                } catch (Exception)
                {
                    transaction.Rollback(); //回滚事务
                }                    
                    return Result.Success("导入成功");
                }
                else
                {
                    return Result.Success("导入失败,空文件");
                }
            }
            return Result.Success("导入失败,请上传文件");
        }
    }
}
```
## 2、导出Excel
```
[HttpGet]
public ActionResult ExportToExcel()
{
    var st = OfficeHelper.DataTableToExcel(CreateTable());           
    return File(st, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "面试记录表.xlsx");
}
//创建DataTable 方法 
private DataTable CreateTable()
{

    string[] Headers = { "ID", "公司名称", "应聘岗位", "薪资", "职位发布时间", "报名面试者", "性别", "班级", "报名时间" };
    DataTable dataTable = new DataTable("ExportData");//*新建一张表
    foreach (string TemStr in Headers)
    {
        DataColumn strNameColumn = new DataColumn();
        strNameColumn.DataType = typeof(String);
        strNameColumn.ColumnName = TemStr;
        dataTable.Columns.Add(strNameColumn); //*建立五列数据
    }
    var db = bll.DbContext();
    var query = from p in db.Posts;
    var objs = query.ToList();
    for (int i = 0, len = objs.Count; i < len; i++)
    {
        var o = objs[i];               
		DataRow rowData = dataTable.NewRow();   //*建立行数据
        rowData["ID"] = (i + 1).ToString();
        rowData["报名面试者"] = o.GetType().GetProperty("StudentName")?.GetValue(o)?.ToString();
        rowData["性别"] = o.GetType().GetProperty("Gender")?.GetValue(o)?.ToString();
        rowData["班级"] = o.GetType().GetProperty("ClassName")?.GetValue(o)?.ToString();              
        dataTable.Rows.Add(rowData);
    }
    return dataTable;
}            
```