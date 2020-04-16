using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.views;
using UserService.Models;

namespace UserService.Services
{
    public interface IUserService
    {
        Task Fill();

        Task<List<User>> Get();




        Task<User> Insert(string viewName, string viewEmail, string viewPassword);

        Task<User> Login(string viewEmail, string viewPassword);
    }
}