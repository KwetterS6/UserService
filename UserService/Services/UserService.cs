using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using MessageBroker;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserService.Helpers;
using UserService.Models;
using UserService.Repositories;
using UserService.views;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IHasher _hasher;
        private readonly IJWTTokenGenerator _tokenGenerator;
        private readonly IMessageQueuePublisher _messageQueuePublisher;
        private readonly MessageQueueSettings _messageQueueSettings;
        

        public UserService(IUserRepository repository, IHasher hasher, IOptions<AppSettings> appSettings, IJWTTokenGenerator tokenGenerator, IMessageQueuePublisher messageQueuePublisher, IOptions<MessageQueueSettings> messageQueueSettings)
        {
            _repository = repository;
            _hasher = hasher;
            _tokenGenerator = tokenGenerator;
            _messageQueuePublisher = messageQueuePublisher;
            _messageQueueSettings = messageQueueSettings.Value;
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

            var messageQSendable = new RegisterMessageView()
            {
                Email = viewEmail
            };

            await _messageQueuePublisher.PublishMessageAsync(_messageQueueSettings.Exchange, "EmailService", "RegisterUser", messageQSendable);
            
            return await _repository.Create(user);
        }







        public async Task<User> Login(string email, string password)
        {
            var user = await _repository.Get(email);
            if (user == null) throw new ArgumentException("A user with this email address does not exist.");

            if (!await _hasher.VerifyHash(password, user.Salt, user.Password))
            {
                throw new ArgumentException("the password is incorrect.");
            }

            return _tokenGenerator.Authenticate(user);
        }
    }
}