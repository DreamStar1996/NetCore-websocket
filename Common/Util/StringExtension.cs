using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections;

namespace Common.Util
{
    /// <summary>
    /// 字符串拓展类
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 过滤掉HTML标签，除了指定标签,如果cxcept 为空，则默认保留p标签
        /// </summary>
        /// <param name="html"></param>
        /// <param name="cxcept">保留的标签，未指定默认为p，多个写法"p|a|br"</param>
        /// <returns></returns>
        public static string FilterHtmlExcept(this string html, string cxcept)
        {
            string acceptable = string.IsNullOrEmpty(cxcept) ? "p" : cxcept;
            string stringPattern = @"</?(?(?=" + acceptable + @")notag|[a-zA-Z0-9]+)(?:\s[a-zA-Z0-9\-]+=?(?:(["",']?).*?\1?)?)*\s*/?>";
            html = Regex.Replace(html, stringPattern, "");
            html = Regex.Replace(html, @"[\t\n]", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"[\r]", "", RegexOptions.IgnoreCase);
            //html = Regex.Replace(html, @"[\t\n\r\s]","",RegexOptions.IgnoreCase);
            return html;
        }
        /// <summary>
        /// 按文本内容长度截取HTML字符串(支持截取带HTML代码样式的字符串)
        /// </summary>
        /// <param name="html">将要截取的字符串参数</param>
        /// <param name="len">截取的字节长度</param>
        /// <param name="endString">字符串末尾补上的字符串</param>
        /// <returns>返回截取后的字符串</returns>
        public static string HTMLSubstring(this string html, int len, string endString)
        {
            if (string.IsNullOrEmpty(html) || html.Length <= len) return html;
            MatchCollection mcentiry, mchtmlTag;
            ArrayList inputHTMLTag = new ArrayList();
            string r = "", tmpValue;
            int rWordCount = 0, wordNum = 0, i = 0;
            Regex rxSingle = new Regex("^<(br|hr|img|input|param|meta|link)", RegexOptions.Compiled | RegexOptions.IgnoreCase)//是否单标签正则
                , rxEndTag = new Regex("</[^>]+>", RegexOptions.Compiled)//是否结束标签正则
                , rxTagName = new Regex("<([a-z]+)[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase)//获取标签名正则
                , rxHtmlTag = new Regex("<[^>]+>", RegexOptions.Compiled)//html标签正则
                , rxEntity = new Regex("&[a-z]{1,9};", RegexOptions.Compiled | RegexOptions.IgnoreCase)//实体正则
                , rxEntityReverse = new Regex("§", RegexOptions.Compiled)//反向替换实体正则
                ;
            html = html.Replace("§", "§");//替换字符§为他的实体“§”，以便进行下一步替换
            mcentiry = rxEntity.Matches(html);//收集实体对象到匹配数组中
            html = rxEntity.Replace(html, "§");//替换实体为特殊字符§，这样好控制一个实体占用一个字符
            mchtmlTag = rxHtmlTag.Matches(html);//收集html标签到匹配数组中
            html = rxHtmlTag.Replace(html, "__HTMLTag__");//替换为特殊标签
            string[] arrWord = html.Split(new string[] { "__HTMLTag__" }, StringSplitOptions.None);//通过特殊标签进行拆分
            wordNum = arrWord.Length;
            //获取指定内容长度及HTML标签
            for (; i < wordNum; i++)
            {
                if (rWordCount + arrWord[i].Length >= len) r += arrWord[i].Substring(0, len - rWordCount) + endString;
                else r += arrWord[i];
                rWordCount += arrWord[i].Length;//计算已经获取到的字符长度
                if (rWordCount >= len) break;
                //搜集已经添加的非单标签，以便封闭HTML标签对
                if (i < wordNum - 1)
                {
                    tmpValue = mchtmlTag[i].Value;
                    if (!rxSingle.IsMatch(tmpValue))
                    { //不是单标签
                        if (rxEndTag.IsMatch(tmpValue) && inputHTMLTag.Count > 0) inputHTMLTag.RemoveAt(inputHTMLTag.Count - 1);
                        else inputHTMLTag.Add(tmpValue);
                    }
                    r += tmpValue;
                }

            }
            r = r + "...";
            //替换回实体
            for (i = 0; i < mcentiry.Count; i++) r = rxEntityReverse.Replace(r, mcentiry[i].Value, 1);
            //封闭标签
            for (i = inputHTMLTag.Count - 1; i >= 0; i--) r += "</" + rxTagName.Match(inputHTMLTag[i].ToString()).Groups[1].Value + ">";
            return r;
        }

    }
}
