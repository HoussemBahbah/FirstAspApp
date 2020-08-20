using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        public UserProfileController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        
        [HttpPost]
        [Route("Register")]
        public async Task<Object> Register(RegisterViewModel model)
        {
            var user = new User()
            {
                firstName = model.firstName,
                lastName = model.lastName,
                Email = model.email,
                PhoneNumber = model.phoneNumber,
                UserName = model.userName
            };
            var userCreationResult = await _userManager.CreateAsync(user, model.password);
            if (userCreationResult.Succeeded)
            {
                var roleCreationResult = await _userManager.AddToRoleAsync(user, model.Role);
                if (roleCreationResult.Succeeded) return Ok();
                else return BadRequest();
            }
            return BadRequest();
        }
    }
}
