using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Helpers;
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

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.userName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.password))
            {
                //Get role assigned to the user
                var role = await _userManager.GetRolesAsync(user);
                IdentityOptions _options = new IdentityOptions();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim(_options.ClaimsIdentity.RoleClaimType,role.FirstOrDefault()),
                        new Claim("username",user.UserName),
                        new Claim("fullName",user.firstName + " " + user.lastName),
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtLocalConstants.SecretKey)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var auth_token = tokenHandler.WriteToken(securityToken);
                return Ok(new { auth_token });
            }
            //else
            return BadRequest(new { message = "Username or password is incorrect." });



        }
        [HttpGet]
        [Authorize]
        //GET : /api/UserProfile
        public async Task<Object> GetUserProfile()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            return new
            {
                user.Email,
                user.UserName
            };
        }

        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        [Route("ForAdmin")]
        public string GetForAdmin()
        {
            return "Web method for Admin";
        }

        [HttpGet]
        [Authorize(Roles = Roles.User)]
        [Route("ForUser")]
        public string GetUser()
        {
            return "Web method for User";
        }

        [HttpGet]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        [Route("ForAdminOrUser")]
        public string GetForAdminOrCustome()
        {
            return "Web method for Admin or Customer";
        }
    }
}

