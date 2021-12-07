 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace Social_Network.Hubs
{
    public class ChatHub: Hub
    {
        public async Task<string> GetConnectionId(string chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            return Context.ConnectionId;
        }
    }
}
