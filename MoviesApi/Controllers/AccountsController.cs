using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoviesApi.DTO;
using MoviesApi.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AccountsController
            (
                UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager,
                IConfiguration configuration,
                ApplicationDbContext context,
                IMapper mapper
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserInfo model)
        {
            var user = new IdentityUser { UserName = model.EmailAddress, Email = model.EmailAddress };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return await BuildToken(model);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.EmailAddress, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuildToken(model);
            }
            else
            {
                return BadRequest("Invalid Login attempt");
            }
        }

        [HttpPost("RenewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        // Method to renew a users token if they are already authorized
        public async Task<ActionResult<UserToken>> Renew()
        {
            var userInfo = new UserInfo
            {
                EmailAddress = HttpContext.User.Identity.Name
            };

            return await BuildToken(userInfo);
        }

        private async Task<UserToken> BuildToken(UserInfo userInfo)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userInfo.EmailAddress),
                new Claim(ClaimTypes.Email, userInfo.EmailAddress),
                new Claim("myKey", "Some value")
            };

            var identityUser = await _userManager.FindByEmailAsync(userInfo.EmailAddress); // Gets the user from the database
            var claimsDB = await _userManager.GetClaimsAsync(identityUser); // Gets all the users roles and claims from the database

            claims.AddRange(claimsDB); // Adds the user roles to the claim token

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:key"])); // Generates a new secure key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Encrypts the key?

            var expiration = DateTime.UtcNow.AddMinutes(30); // Sets how long the key will be vaid for

            JwtSecurityToken token = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: claims,
                    expires: expiration,
                    signingCredentials: creds); // Generates a new JWT Token based upon the previous settings

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            }; // Returns the new token that the user can use
        }

        [HttpGet("Users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        // Returns a full list of users from the database
        public async Task<ActionResult<List<UserDTO>>> Get([FromQuery] PaginationDTO pagination)
        {
            var queryable = _context.Users.AsQueryable();
            queryable = queryable.OrderBy(x => x.Email);
            await HttpContext.InsertPaginationParametersInResponse(queryable, pagination.RecordsPerPage);

            var users = await queryable.Paginate(pagination).ToListAsync();

            return _mapper.Map<List<UserDTO>>(users);
        }

        [HttpGet("Roles")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        // Returns a list of all the roles in the database
        public async Task<ActionResult<List<string>>> GetRoles()
        {
            return await _context.Roles.Select(x => x.Name).ToListAsync();
        }

        [HttpPost("AssignRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        // Adds a new role to a user
        public async Task<ActionResult> AssignRole(EditRoleDTO editRole)
        {
            var user = await _userManager.FindByIdAsync(editRole.UserID);

            if (user == null)
            {
                return NotFound();
            }

            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, editRole.RoleName));

            return NoContent();
        }

        [HttpPost("RemoveRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        // Removes a role from a user
        public async Task<ActionResult> RemoveRole(EditRoleDTO editRole)
        {
            var user = await _userManager.FindByIdAsync(editRole.UserID);

            if (user == null)
            {
                return NotFound();
            }

            await _userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, editRole.RoleName));

            return NoContent();
        }
    }
}