using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Entities;
using Social_Network.Models;

namespace Social_Network.ViewModels
{
    public class PageUsersViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public PaginatedListModel PageViewModel { get; set; }
    }
}
