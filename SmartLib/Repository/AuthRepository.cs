using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SmartLib.Data;
using SmartLib.Interfaces;
using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;
using SmartLib.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartLib.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public AuthRepository(
                              ApplicationDbContext context,
                              RoleManager<IdentityRole> roleManager,
                              UserManager<ApplicationUser> userManager,
                              IConfiguration configuration,
                              IMapper mapper
                             )
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
        }

        public Task<ApplicationUserResponse> LoginAsync(string email, string password)
        {
            var user = _userManager.FindByEmailAsync(email).Result;
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var isMatch = _userManager.CheckPasswordAsync(user, password).Result;
            if (!isMatch)
            {
                throw new Exception("Invalid password");
            }

            var tokenValue =  GeneratejwtToken(user);

            var response = _mapper.Map<ApplicationUserResponse>(user);
            return Task.FromResult(response);
        }

        public async Task<StudentResponseDto> RegisterStudentAsync(CreateStudentDto request)
        {
            var UserExist = await _userManager.FindByEmailAsync(request.Email);
            if (UserExist != null)
            {
                throw new Exception("User already exist");
            }

            ApplicationUser user = new ApplicationUser()
            {
                UserName = request.Email,
                Email = request.Email,
                Department = request.Department,
                CreatedAt = DateTime.UtcNow,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if(!result.Succeeded)
            {
                throw new Exception("Error occurred while creating user");
            }

            Student createStudent = new Student()
            {
                UserId = user.Id,
                Section = request.Section,
                YearLevel = request.YearLevel,
                CreatedAt = DateTime.UtcNow,
                IsActive = request.IsActive
            };

            var student = await _context.Students.AddAsync(createStudent);
            await _context.SaveChangesAsync();
            var studentResponse = _mapper.Map<StudentResponseDto>(student.Entity);
            return studentResponse;
        }

        private async Task<AuthResponse> GeneratejwtToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty), 
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var jwtSecret = _configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT:Secret is missing in configuration.");
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                IsRevoked = false,
                UserId = user.Id,
                DateAdded = DateTime.UtcNow,
                DateExpire = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            var response = new AuthResponse
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                ExpireAt = token.ValidTo
            };

            return response;
        }
    }
}
