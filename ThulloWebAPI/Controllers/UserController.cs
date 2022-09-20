using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using ThulloWebAPI.Data;
using ThulloWebAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ThulloWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public UserController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserRegisterRequest>> Register(UserRegisterRequest request)
        {
            if (_context.Users.Any(user => user.Email == request.Email))
            {
                return BadRequest(new DefaultResponse("User already exists."));
            }

            if (_context.Users.Any(user => user.UniqueName == request.UniqueName))
            {
                return BadRequest(new DefaultResponse("Users uniqueName already exists."));
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            User user = new()
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Name = request.Name,
                UniqueName = request.UniqueName,
                Status = UserStatus.NotConfirmEmail,
            };


            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            var token = new Token
            {
                TokenType = TokenType.EmailConfim,
                TokenValue = CreateRandomToken(),
                TokenStatus = TokenStatus.Active,
                UserId = user.Id,
            };

            _context.Tokens.Add(token);

            await _context.SaveChangesAsync();

            return Ok(new DefaultResponse("User successfully created!"));
        }

        [HttpGet("confirmEmail/{tokenValue}")]
        public async Task<ActionResult<DefaultResponse>> ConfirmEmail(string tokenValue)
        {
            var tokenList = _context.Tokens
                .Where(t => t.TokenValue == tokenValue && t.TokenType == TokenType.EmailConfim && t.TokenStatus == TokenStatus.Active).ToList();

            

            if(tokenList.Count == 0)
            {
                return NotFound(new DefaultResponse("Not found token or it is inactive."));
            }

            var token = tokenList[0];

            var user = await _context.Users.FindAsync(token.UserId);

            if(user == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new DefaultResponse("Internal Server Error"));
            }

            if(user.Status != UserStatus.NotConfirmEmail)
            {
                return BadRequest(new DefaultResponse("User must not confirm the email"));
            }
            token.TokenStatus = TokenStatus.Inactive;
            user.Status = UserStatus.Active;

            await _context.SaveChangesAsync();

            return Ok(new DefaultResponse("Email was confirm"));
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponse>> Login(UserLoginRequest request)
        {
            var userList = _context.Users.Where(u => u.Email == request.Email).ToArray();

            if(userList.Length == 0)
            {
                return NotFound(new DefaultResponse("User does not exists."));
            }

            var user = userList[0];

            if(user.Status == UserStatus.NotConfirmEmail)
            {
                return BadRequest(new DefaultResponse("User not verified the email."));
            }

            if(user.Status == UserStatus.Inactive)
            {
                return Unauthorized(new DefaultResponse("User is not active."));
            }

            if(!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest(new DefaultResponse("Email or Password is incorrect."));
            }

            var newAccessToken = CreateToken(user);

            var userLoginResponse = new UserLoginResponse
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                UniqueName = user.UniqueName,
                Phone = user.Phone,
                Avatar = user.Avatar,
                Status = user.Status,
                AccessToken = newAccessToken,
            };

            var token = new Token
            {
                TokenStatus = TokenStatus.Active,
                TokenType = TokenType.AccessToken,
                TokenValue = newAccessToken,
                UserId = user.Id,
                Expires = DateTime.Now.AddDays(1),
            };

            _context.Tokens.Add(token);

            await _context.SaveChangesAsync();

            return Ok(userLoginResponse);

        }
        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:TokenSecret").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
