using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Util
{
    /// <summary>
    /// Excel操作工具类
    /// </summary>
    public class OfficeHelper
    {
        /// <summary>
        /// 将excel文件内容读取到DataTable数据表中
        /// </summary>
        /// <param name="fileName">文件完整路径名</param>
        /// <param name="sheetName">指定读取excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名：true=是，false=否</param>
        /// <returns>DataTable数据表</returns>
        public static DataTable ReadExcelToDataTable(string fileName, string sheetName = null, bool isFirstRowColumn = true)
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    return null;
                }
                //根据指定路径读取文件
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                return ReadExcelToDataTable(fs, sheetName, isFirstRowColumn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将文件流读取到DataTable数据表中
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="sheetName">指定读取excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名：true=是，false=否</param>
        /// <returns>DataTable数据表</returns>
        public static DataTable ReadExcelToDataTable(Stream fileStream, string sheetName = null, bool isFirstRowColumn = true)
        {
            //定义要返回的datatable对象
            DataTable data = new DataTable();
            //excel工作表
            ISheet sheet = null;
            //数据开始行(排除标题行)
            int startRow = 0;
            try
            {
                //根据文件流创建excel数据结构,NPOI的工厂类WorkbookFactory会自动识别excel版本，创建出不同的excel数据结构
                IWorkbook workbook = WorkbookFactory.Create(fileStream);
                //如果有指定工作表名称
                if (!string.IsNullOrEmpty(sheetName))
                {
                    sheet = workbook.GetSheet(sheetName);
                    //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    //如果没有指定的sheetName，则尝试获取第一个sheet
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    //一行最后一个cell的编号 即总的列数
                    int cellCount = firstRow.LastCellNum;
                    //如果第一行是标题列名
                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }
                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null || row.FirstCellNum < 0) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            //同理，没有数据的单元格都默认是null
                            ICell cell = row.GetCell(j);
                            if (cell != null)
                            {
                                if (cell.CellType == CellType.Numeric)
                                {
                                    //判断是否日期类型
                                    if (DateUtil.IsCellDateFormatted(cell))
                                    {
                                        dataRow[j] = row.GetCell(j).DateCellValue;
                                    }
                                    else
                                    {
                                        dataRow[j] = row.GetCell(j).ToString().Trim();
                                    }
                                }
                                else
                                {
                                    dataRow[j] = row.GetCell(j).ToString().Trim();
                                }
                            }
                        }
                        data.Rows.Add(dataRow);
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 导出为Excel
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        //3.导出DataTable即ExportExcel
        public static Stream DataTableToExcel(DataTable dataTable)
        {
            IWorkbook workbook = null;
            IRow row = null;
            ISheet sheet = null;
            ICell cell = null;
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                workbook = new HSSFWorkbook();
                sheet = workbook.CreateSheet("Sheet1");
                int rowCount = dataTable.Rows.Count;
                int columnCount = dataTable.Columns.Count;

                //设置列头  
                row = sheet.CreateRow(0);

                for (int i = 0; i < columnCount; i++)
                {
                    cell = row.CreateCell(i);
                    cell.SetCellValue(dataTable.Columns[i].ColumnName);
                }

                for (int i = 0; i < rowCount; i++)
                {
                    row = sheet.CreateRow(i + 1);
                    for (int j = 0; j < columnCount; j++)
                    {
                        cell = row.CreateCell(j);
                        cell.SetCellValue(dataTable.Rows[i][j].ToString());
                    }
                }
                //保存路径
                //strName = Server.MapPath("/excel/OnlineResume.xlsx");
                //using (MemoryStream fs = new MemoryStream())
                // {
                MemoryStream ms = new MemoryStream();
                //必须将流的当前位置置0，否则将引发异常
                //如果不设置为0，则流的当前位置在流的末端1629，然后读流就会从索引1629开始读取，实际上流的最大索引是1628，就会引发无效操作异常System.InvalidOperationException
                //System.InvalidOperationException: Response Content-Length mismatch: too few bytes written (0 of 1628)           
                workbook.Write(ms);
                ms.Seek(0, SeekOrigin.Begin); //seek begin  InvalidOperationException: Response Content-Length mismatch 解决方案：
                return ms;
                //  }
                //using (fs = System.IO.File.OpenWrite(@strName))
                //{
                //    workbook.Write(fs);
                //    result = true;
                //}
            }
            return new MemoryStream();
        }
    }

}
