using CSRedis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

/// <summary>
/// im 核心类实现的配置所需
/// </summary>
public class ImClientOptions
{
    /// <summary>
    /// CSRedis 对象，用于存储数据和发送消息
    /// </summary>
    public CSRedisClient Redis { get; set; }
    /// <summary>
    /// 负载的服务端
    /// </summary>
    public string[] Servers { get; set; }
    /// <summary>
    /// websocket请求的路径，默认值：/ws
    /// </summary>
    public string PathMatch { get; set; } = "/ws";
}

public class ImSendEventArgs : EventArgs
{
    /// <summary>
    /// 发送者的客户端id
    /// </summary>
    public string SenderClientId { get; }
    /// <summary>
    /// 接收者的客户端id
    /// </summary>
    public List<string> ReceiveClientId { get; } = new List<string>();
    /// <summary>
    /// imServer 服务器节点
    /// </summary>
    public string Server { get; }
    /// <summary>
    /// 消息
    /// </summary>
    public object Message { get; }
    /// <summary>
    /// 是否回执
    /// </summary>
    public bool Receipt { get; }

    internal ImSendEventArgs(string server, string senderClientId, object message, bool receipt = false)
    {
        this.Server = server;
        this.SenderClientId = senderClientId;
        this.Message = message;
        this.Receipt = receipt;
    }
}

/// <summary>
/// im 核心类实现
/// </summary>
public class ImClient
{
    protected CSRedisClient _redis;
    protected string[] _servers;
    protected string _redisPrefix;
    protected string _pathMatch;

    /// <summary>
    /// 推送消息的事件，可审查推向哪个Server节点
    /// </summary>
    public EventHandler<ImSendEventArgs> OnSend;

    /// <summary>
    /// 初始化 imclient
    /// </summary>
    /// <param name="options"></param>
    public ImClient(ImClientOptions options)
    {
        if (options.Redis == null) throw new ArgumentException("ImClientOptions.Redis 参数不能为空");
        if (options.Servers.Any() == false) throw new ArgumentException("ImClientOptions.Servers 参数不能为空");
        _redis = options.Redis;
        _servers = options.Servers;
        _redisPrefix = $"wsim{options.PathMatch.Replace('/', '_')}";
        _pathMatch = options.PathMatch ?? "/ws";
    }
    /// <summary>
    /// 计算字符串的hash
    /// </summary>
    /// <param name="read"></param>
    /// <returns></returns>
    public static long Hash(string read)
    {
        UInt64 hashedValue = 3074457345618258791ul;
        for (int i = 0; i < read.Length; i++)
        {
            hashedValue += read[i];
            hashedValue *= 3074457345618258799ul;
        }
        return Math.Abs((long)(hashedValue - 9223372036854775808));
    }
    /// <summary>
    /// 负载分区规则：取clientId后四位字符，转成10进制数字0-65535，求模
    /// </summary>
    /// <param name="clientId">客户端id</param>
    /// <returns></returns>
    protected string SelectServer(string clientId)
    {
        //var servers_idx = int.Parse(clientId.ToString("N").Substring(28), NumberStyles.HexNumber) % _servers.Length;
        var servers_idx = Hash(clientId) % _servers.Length;
        if (servers_idx >= _servers.Length) servers_idx = 0;
        return _servers[servers_idx];
    }

    /// <summary>
    /// ImServer 连接前的负载、授权，返回 ws 目标地址，使用该地址连接 websocket 服务端
    /// </summary>
    /// <param name="clientId">客户端id</param>
    /// <param name="clientMetaData">客户端相关信息，比如ip</param>
    /// <returns>websocket 地址：ws://xxxx/ws?token=xxx</returns>
    public string PrevConnectServer(string clientId, string clientMetaData)
    {
        var server = SelectServer(clientId);
        var token = $"{Guid.NewGuid()}{Guid.NewGuid()}{Guid.NewGuid()}{Guid.NewGuid()}".Replace("-", "");
        _redis.Set($"{_redisPrefix}Token{token}", JsonConvert.SerializeObject((clientId, clientMetaData)), 10);
        //return $"ws://{server}{_pathMatch}?token={token}";
        return $"{server}{_pathMatch}?token={token}";
    }

    /// <summary>
    /// 向指定的多个客户端id发送消息
    /// </summary>
    /// <param name="senderClientId">发送者的客户端id</param>
    /// <param name="receiveClientId">接收者的客户端id</param>
    /// <param name="message">消息</param>
    /// <param name="receipt">是否回执</param>
    public void SendMessage(string senderClientId, IEnumerable<string> receiveClientId, object message, bool receipt = false)
    {
        receiveClientId = receiveClientId.Distinct().ToArray();
        Dictionary<string, ImSendEventArgs> redata = new Dictionary<string, ImSendEventArgs>();

        foreach (var uid in receiveClientId)
        {
            string server = SelectServer(uid);
            if (redata.ContainsKey(server) == false) redata.Add(server, new ImSendEventArgs(server, senderClientId, message, receipt));
            redata[server].ReceiveClientId.Add(uid);
        }
        var messageJson = JsonConvert.SerializeObject(message);
        foreach (var sendArgs in redata.Values)
        {
            OnSend?.Invoke(this, sendArgs);
            _redis.Publish($"{_redisPrefix}Server{sendArgs.Server}",
                JsonConvert.SerializeObject((senderClientId, sendArgs.ReceiveClientId, messageJson, sendArgs.Receipt)));
        }
    }

    /// <summary>
    /// 获取所在线客户端id
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetClientListByOnline()
    {
        return _redis.HKeys($"{_redisPrefix}Online").Where(a => a != String.Empty);
    }

    /// <summary>
    /// 判断客户端是否在线
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    public bool HasOnline(string clientId)
    {
        return _redis.HGet<int>($"{_redisPrefix}Online", clientId.ToString()) > 0;
    }

    /// <summary>
    /// 事件订阅
    /// </summary>
    /// <param name="online">上线</param>
    /// <param name="offline">下线</param>
    public void EventBus(
        Action<(string clientId, string clientMetaData)> online,
        Action<(string clientId, string clientMetaData)> offline)
    {
        _redis.Subscribe(
            ($"evt_{_redisPrefix}Online", msg => online(JsonConvert.DeserializeObject<(string clientId, string clientMetaData)>(msg.Body))),
            ($"evt_{_redisPrefix}Offline", msg => offline(JsonConvert.DeserializeObject<(string clientId, string clientMetaData)>(msg.Body))));
    }
    /// <summary> 
    /// 事件订阅，加入离开频道
    /// </summary>
    /// <param name="joinchannel">加入频道</param>
    /// <param name="leavechannel">离开频道 </param>
    public void EventChan(
    Action<(string clientId, string chan)> joinchannel,
    Action<(string clientId, string chan)> leavechannel)
    {
        _redis.Subscribe(
            ($"evt_{_redisPrefix}JoinChan", msg => joinchannel(JsonConvert.DeserializeObject<(string clientId, string chan)>(msg.Body))),
            ($"evt_{_redisPrefix}LeaveChan", msg => leavechannel(JsonConvert.DeserializeObject<(string clientId, string chan)>(msg.Body))));
    }
    #region 群聊频道，每次上线都必须重新加入

    /// <summary>
    /// 加入群聊频道，每次上线都必须重新加入
    /// </summary>
    /// <param name="clientId">客户端id</param>
    /// <param name="chan">群聊频道名</param>
    public void JoinChan(string clientId, string chan)
    {
        _redis.StartPipe(a => a
            .HSet($"{_redisPrefix}Chan{chan}", clientId.ToString(), 0)
            .HSet($"{_redisPrefix}Client{clientId}", chan, 0)
            .HIncrBy($"{_redisPrefix}ListChan", chan, 1));
        //加入频道事件
        _redis.Publish($"evt_{_redisPrefix}JoinChan", JsonConvert.SerializeObject((clientId, chan)));
    }
    /// <summary>
    /// 离开群聊频道
    /// </summary>
    /// <param name="clientId">客户端id</param>
    /// <param name="chans">群聊频道名</param>
    public void LeaveChan(string clientId, params string[] chans)
    {
        if (chans?.Any() != true) return;
        using (var pipe = _redis.StartPipe())
        {
            foreach (var chan in chans)
            {
                pipe
                    .HDel($"{_redisPrefix}Chan{chan}", clientId.ToString())
                    .HDel($"{_redisPrefix}Client{clientId}", chan)
                    .Eval($"if redis.call('HINCRBY', KEYS[1], '{chan}', '-1') <= 0 then redis.call('HDEL', KEYS[1], '{chan}') end return 1",
                        $"{_redisPrefix}ListChan");
                //Console.WriteLine("离开");
                //离开频道事件
                _redis.Publish($"evt_{_redisPrefix}LeaveChan", JsonConvert.SerializeObject((clientId, chan)));
            }
        }
    }
    /// <summary>
    /// 获取群聊频道所有客户端id
    /// </summary>
    /// <param name="chan">群聊频道名</param>
    /// <returns></returns>
    public string[] GetChanClientList(string chan)
    {
       // Console.WriteLine("在线惹事"+ _redis.HKeys($"{_redisPrefix}Chan{chan}").ToArray().Length);
        //return _redis.HKeys($"{_redisPrefix}Chan{chan}").Select(a => a).ToArray();
        return _redis.HKeys($"{_redisPrefix}Chan{chan}").ToArray();
    }
    /// <summary>
    /// 清理群聊频道的离线客户端（测试）
    /// </summary>
    /// <param name="chan">群聊频道名</param>
    public void ClearChanClient(string chan)
    {
        var websocketIds = _redis.HKeys($"{_redisPrefix}Chan{chan}");
        var offline = new List<string>();
        var span = websocketIds.AsSpan();
        var start = span.Length;
        while (start > 0)
        {
            start = start - 10;
            var length = 10;
            if (start < 0)
            {
                length = start + 10;
                start = 0;
            }
            var slice = span.Slice(start, length);
            var hvals = _redis.HMGet($"{_redisPrefix}Online", slice.ToArray().Select(b => b.ToString()).ToArray());
            for (var a = length - 1; a >= 0; a--)
            {
                if (string.IsNullOrEmpty(hvals[a]))
                {
                    offline.Add(span[start + a]);
                    span[start + a] = null;
                }
            }
        }
        //删除离线订阅
        if (offline.Any()) _redis.HDel($"{_redisPrefix}Chan{chan}", offline.ToArray());
    }

    /// <summary>
    /// 获取所有群聊频道和在线人数
    /// </summary>
    /// <returns>频道名和在线人数</returns>
    public IEnumerable<(string chan, long online)> GetChanList()
    {
        var ret = _redis.HGetAll<long>($"{_redisPrefix}ListChan");
        return ret.Select(a => (a.Key, a.Value));
    }
    /// <summary>
    /// 获取用户参与的所有群聊频道
    /// </summary>
    /// <param name="clientId">客户端id</param>
    /// <returns></returns>
    public string[] GetChanListByClientId(string clientId)
    {
        return _redis.HKeys($"{_redisPrefix}Client{clientId}");
    }
    /// <summary>
    /// 获取群聊频道的在线人数
    /// </summary>
    /// <param name="chan">群聊频道名</param>
    /// <returns>在线人数</returns>
    public long GetChanOnline(string chan)
    {
        //Console.WriteLine();
        //return _redis.HGet<long>($"{_redisPrefix}ListChan", chan);
        return Convert.ToInt64(_redis.HGet($"{_redisPrefix}ListChan", chan));//不行       
    }

    /// <summary>
    /// 发送群聊消息，所有在线的用户将收到消息
    /// </summary>
    /// <param name="senderClientId">发送者的客户端id</param>
    /// <param name="chan">群聊频道名</param>
    /// <param name="message">消息</param>
	public void SendChanMessage(string senderClientId, string chan, object message)
    {
        var websocketIds = _redis.HKeys($"{_redisPrefix}Chan{chan}");
        SendMessage(senderClientId, websocketIds.Where(a => !string.IsNullOrEmpty(a)).Select(a => a).ToArray(), message);
    }

    #endregion
}
