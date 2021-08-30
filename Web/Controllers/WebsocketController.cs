using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Common.Util;
namespace Web.Controllers
{
    /// <summary>
    /// 即时聊天控制器websocket
    /// </summary>
    [Route("api/ws/")]
    [ApiController]
    public class WebSocketController : MyBaseController<Object>
    {

        // public string Ip => this.Request.Headers["X-Real-IP"].FirstOrDefault() ?? this.Request.HttpContext.Connection.RemoteIpAddress.ToString();

        /// <summary>
        /// 获取websocket地址。即时聊天之前必须调用本方法获取地址
        /// </summary>
        /// <param name="websocketId">本地标识，若无则不传，接口会返回新的，请保存本地localStoregy重复使用</param>
        /// <returns></returns>
        [HttpPost("pre-connect")]
        public Result PreConnect([FromForm]string? websocketId)
        {
            if (websocketId == null)
            {
                if (MyUser == null || MyUser.Id <= 0)
                {
                    websocketId = "anony-" + UtilString.GetRandomNum(12);
                }
                else
                {
                    string type = "user";
                    if ("teacher" == MyUser.Role)
                    {
                        type = "teacher";
                    }
                    else if ("student" == MyUser.Role)
                    {
                        type = "student";
                    }
                    websocketId = type + "-" + MyUser.Id;
                }
            }
            var wsserver = ImHelper.PrevConnectServer(websocketId, JsonConvert.SerializeObject(MyUser));
            return Result.Success("succeed").SetData(new
            {
                server = wsserver,
                websocketId = websocketId
            });
        }

        /// <summary>
        /// 群聊，获取群列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("get-channels")]
        [Authorize]
        public Result GetChannels()
        {
            return Result.Success("succeed").SetData(new
            {
                channels = ImHelper.GetChanList().Select(a => new { a.chan, a.online })
            });
        }
        /// <summary>
        /// 获取群聊频道的在线人数
        /// </summary>
        /// <param name="chan">群聊频道名</param>
        /// <returns>在线人数</returns>
        [HttpPost("get-channelonline-count")]
        public Result GetChanOnline(string chan)
        {
            return Result.Success("succeed").SetData(ImHelper.GetChanOnline(chan));
        }

        /// <summary>
        /// 群聊，绑定消息频道。加入频道
        /// </summary>
        /// <param name="websocketId">本地标识，若无则不传，接口会返回，请保存本地重复使用</param>
        /// <param name="channel">消息频道</param>
        /// <returns></returns>
        [HttpPost("join-channel")]
        public Result JoinChannel([FromForm]string websocketId, [FromForm]  string channel)
        {
            ImHelper.JoinChan(websocketId, channel);
            return Result.Success("succeed");
        }
        /// <summary>
        /// 退出频道
        /// </summary>
        /// <param name="websocketId"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        [HttpPost("leave-channel")]
        public Result LeaveChan([FromForm]string websocketId, [FromForm]  string channel)
        {
            ImHelper.LeaveChan(websocketId, channel);
            return Result.Success("succeed");
        }

        /// <summary>
        /// 群聊，发送频道消息，绑定频道的所有人将收到消息
        /// </summary>
        /// <param name="channel">消息频道</param>
        /// <param name="msg">发送内容</param>
        /// <returns></returns>
        [HttpPost("send-channelmsg")]
        public Result SendChannelmsg([FromForm]string websocketId, [FromForm] string channel, [FromForm] string msg)
        {
            ImHelper.SendChanMessage(websocketId, channel, msg);
            return Result.Success("succeed");
        }
        /// <summary>
        /// 单聊
        /// </summary>
        /// <param name="senderWebsocketId">发送者</param>
        /// <param name="receiveWebsocketId">接收者</param>
        /// <param name="msg">发送内容</param>
        /// <param name="isReceipt">是否需要回执</param>
        /// <returns></returns>
        [HttpPost("send-msg")]
        public Result Sendmsg([FromForm] string senderWebsocketId, [FromForm] string receiveWebsocketId, [FromForm] string msg, [FromForm]bool isReceipt = false)
        {
            //var loginUser = 发送者;
            //var recieveUser = User.Get(receiveWebsocketId);

            //if (loginUser.好友 != recieveUser) throw new Exception("不是好友");

            ImHelper.SendMessage(senderWebsocketId, new[] { receiveWebsocketId }, msg, isReceipt);

            //loginUser.保存记录(msg);
            //recieveUser.保存记录(msg);

            return Result.Success("succeed");
        }
    }
}
