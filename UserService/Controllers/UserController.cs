﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserService.Services;
using UserService.views;

namespace UserService.Controllers
{
    [Route( "[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.Get());
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] UserRegisterView view)
        {
            try
            {
                var user = await _service.Insert(view.Name, view.Email, view.Password);
                return Ok(new {user.Name, user.Email});
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpPost("fill")]

        public async Task<IActionResult> Fill()
        {
            await _service.Fill();
            return Ok();
        }
        
        
        
    }
}