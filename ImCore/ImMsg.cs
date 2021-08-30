using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 客户端遵从消息协议
/// </summary>
public class Sender
{
    public string ClientId { get; set; }
    public string Username { get; set; }
    public string Pic { get; set; }
}
public class ImMsg
{
    public string Cmd { get; set; }
    public object Data { get; set; }
    public Sender Sender { get; set; }
    public string Channel { get; set; }
    public static ImMsg Instance(string cmd, object data, string channel, Sender sender)
    {
        return new ImMsg() { Cmd = cmd, Data = data, Sender = sender, Channel = channel };
    }
    /// <summary>
    /// 心跳检查
    /// </summary>
    /// <returns></returns>
    public static ImMsg Ping()
    {
        return Instance("ping", null, null, null);
    }
    /// <summary>
    /// 加入欢迎
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="sender"></param>
    /// <returns></returns>
    public static ImMsg Hello(string channel, object data, Sender sender)
    {
        return Instance("hello", data, channel, sender);
    }
    /// <summary>
    /// 离开再见
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="sender"></param>
    /// <returns></returns>
    public static ImMsg Bye(string channel, object data, Sender sender)
    {
        return Instance("bye", data, channel, sender);
    }
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="data"></param>
    /// <param name="channel"></param>
    /// <param name="sender"></param>
    /// <returns></returns>
    public static ImMsg Msg(string channel, object data,  Sender sender)
    {
        return Instance("msg", data, channel, sender);
    }
    /// <summary>
    /// 上线
    /// </summary>
    /// <param name="data"></param>
    /// <param name="sender"></param>
    /// <returns></returns>
    public static ImMsg Online(object data, Sender sender)
    {
        return Instance("online", data, null, sender);
    }
    /// <summary>
    /// 下线
    /// </summary>
    /// <param name="data"></param>
    /// <param name="sender"></param>
    /// <returns></returns>
    public static ImMsg Offline(object data, Sender sender)
    {
        return Instance("offline", data, null, sender);
    }
}

