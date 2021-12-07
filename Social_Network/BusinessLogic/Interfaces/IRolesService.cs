using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Interfaces
{
    public interface IRolesService
    {
        Task<IEnumerable<IdentityRole>> GetRoles();
        Task<IdentityResult> Create(string name);
        Task<IdentityResult> Delete(string id);
        Task<IList<string>> GetUserRoles(User user);
        Task EditUserRoles(User user, List<string> roles);
    }
}
