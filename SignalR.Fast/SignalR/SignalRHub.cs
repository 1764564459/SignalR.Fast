using Microsoft.AspNetCore.SignalR;
using SignalR.Fast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.Fast.SignalR
{
    public class SignalRHub:Hub
    {
        IHubContext<SignalRHub> _context;

        /// <summary>
        /// 所有用户
        /// </summary>
        private static readonly List<SignalrModel> _clients = new List<SignalrModel>();
        public SignalRHub(IHubContext<SignalRHub> context)
        {
            _context = context;
        }
        public async Task SendMessage( string message)
        {
            
            await Clients.All.SendAsync("ReceiveMessage", Context.ConnectionId, message);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task Login(string name)
        {
            var Id = Context.ConnectionId;
            var _client = _clients.Find(p => p.Id == Id);
            if (_client==null)
                _clients.Add(new SignalrModel() {Id = Id, Name = name, Status = 1});
            else
            {
                if (_client.Status == 0)
                    _client.Status = 1;
            }
            await Clients.All.SendAsync("Login", "Login Successful ！！！");
        }

        /// <summary>
        /// 退出登录【刷新状态】
        /// </summary>
        /// <returns></returns>
        public async Task Logout()
        {
            var _client = _clients.Find(p => p.Id == Context.ConnectionId);
            _client.Status = 0;
            await Clients.All.SendAsync("Login", _clients);
        }

        /// <summary>
        /// 获取当前登录用户
        /// </summary>
        /// <returns></returns>
        public async Task GetClients()
        {
            await Clients.Client(Context.ConnectionId).SendAsync("GetClients",
                _clients.Where(p => p.Id != Context.ConnectionId).OrderBy(p => p.Id).ThenByDescending(p => p.Status));
        }

        /// <summary>
        /// 向某个用户发送消息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Send(string user, string message)
        {
            await Clients.Client(user).SendAsync("Send", message);
        }


        /// <summary>
        /// 向指定群组发送信息
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="message">信息内容</param>  
        /// <returns></returns>
        public async Task SendMessageToGroupAsync(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync(message);
        }

        /// <summary>
        /// 加入指定组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// 退出指定组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// 向指定成员发送信息
        /// </summary>
        /// <param name="user">成员名</param>
        /// <param name="message">信息内容</param>
        /// <returns></returns>
        public async Task SendPrivateMessage(string user, string message)
        {
            await Clients.User(user).SendAsync("SendAll",message);
        }

        
       

        /// <summary>
        /// 当链接断开时运行
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(System.Exception ex)
        {
            //TODO..
            return base.OnDisconnectedAsync(ex);
        }


        public async Task SendAll(string user, string message)
        {
            await Clients.All.SendAsync("SendAll",user, message);
        }

        //定于一个通讯管道，用来管理我们和客户端的连接
        //1、客户端调用 GetLatestCount，就像订阅
        public async Task GetLatestCount(string random)
        {
            //2、服务端主动向客户端发送数据，名字千万不能错
            await Clients.All.SendAsync("");

            //3、客户端再通过 ReceiveUpdate ，来接收

        }

        public override async Task OnConnectedAsync()
        {
            var _id = Context.ConnectionId;

            //找到_id并发送数据
           // await Clients.Client(_id).SendAsync("Method",new { });

            //除了这个_id
            //await Clients.AllExcept(_id).SendAsync("");

            //添加分组【id、组名】
            await Groups.AddToGroupAsync(_id, "Group");

            //向改组发送数据
            await Clients.Group("Group").SendAsync("SendAll",_id,"Hello !!");
        }
    }
}
