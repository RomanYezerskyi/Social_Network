using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Entities;
using Social_Network.Models;

namespace Social_Network.ViewModels
{
    public class PageOfChatPageViewModel
    {
        public Chat Chat { get; set; }
        public PaginatedListModel PageMessagesViewModel { get; set; }
    }
}

