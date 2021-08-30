using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Common
{
    /// <summary>
    /// 结果数据
    /// </summary>
    public class Result
    {
        public bool Status { get; set; }
        public int  Code { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
        public static Result Instance(bool status, string msg)
        {
            return new Result() { Status = status,Code=500, Msg = msg };
        }
        public static Result Error(string msg)
        {
            return new Result() { Status = false, Code = 500, Msg = msg };
        }
        public static Result Success(string msg= "succeed")
        {
            return new Result() { Status = true, Code = 200, Msg = msg };
        }
        public  Result SetData(object obj)
        {
            this.Data = obj;
            return this;
        }
        public Result SetCode(int Code)
        {
            this.Code = Code;
            return this;
        }
    }
}