using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Security.Policy;
using System.Threading.Tasks;
using UserService.Helpers;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IHasher _hasher;

        public UserService(IUserRepository repository, IHasher hasher)
        {
            _repository = repository;
            _hasher = hasher;
        }

        public async Task Fill()
        {
            var list = await _repository.Get();
            if (list.Count != 0) return;

            await _repository.Create(new User()
            {
                Name = "Test 1",
                Email = "test@user.email"
            });
            
            await _repository.Create(new User()
            {
                Name = "Test 2",
                Email = "test2@user.email"
            });
            
            await _repository.Create(new User()
            {
                Name = "Test 3",
                Email = "test3@user.email"
            });
            
        }

        public async Task<List<User>> Get()
        {
            return await _repository.Get();
        }

        public async Task<User> Insert(string viewName, string viewEmail, string viewPassword)
        {
            var tempemail = await _repository.Get(viewEmail);
            if (tempemail != null) throw new ArgumentException("This email address is already in use, please use another email address.");
            
            var salt = _hasher.CreateSalt();
            var password = await _hasher.HashPassword(viewPassword, salt);
            var user = new User()
            {
                Name = viewName,
                Email = viewEmail,
                Salt = salt,
                Password = password
            };

            return await _repository.Create(user);
        }
    }
}