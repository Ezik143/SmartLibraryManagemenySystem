using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartLib.Data;
using SmartLib.Interfaces;
using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;
using SmartLib.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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

        private readonly TokenValidationParameters _tokenValidationParameters;
        public AuthRepository(
                              ApplicationDbContext context,
                              RoleManager<IdentityRole> roleManager,
                              UserManager<ApplicationUser> userManager,
                              IConfiguration configuration,
                              IMapper mapper,
                              TokenValidationParameters tokenValidationParameters
                             )
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<ApplicationUserResponse> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var isMatch = await _userManager.CheckPasswordAsync(user, password);
            if (!isMatch)
            {
                throw new Exception("Invalid password");
            }

            var tokenValue = await GenerateJwtTokenAsync(user, string.Empty);

            var response = _mapper.Map<ApplicationUserResponse>(user);
            return response;
        }

        public async Task<AuthResponse> RefreshTokenAsync(string email, string refreshToken)
        {
            try
            {
                var result = await VeryfyAndGenerateTokenAsync(new CreateRefreshToken
                {
                    Token = email,
                    RefreshToken = refreshToken
                });

                if (result == null)
                {
                    throw new Exception("invalid token");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error refreshing token: " + ex.Message);
            }
        }

        private async Task<ApplicationUser> CreateUserAsync(
            string name,
            string email,
            string password,
            UserRole role,
            Department department)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is required.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email is required.", nameof(email));
            }

            var normalizedEmail = email.Trim();
            var trimmedName = name.Trim();

            var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User already exist");
            }

            var user = new ApplicationUser
            {
                Name = trimmedName,
                UserName = trimmedName,
                Email = normalizedEmail,
                Role = role,
                IsAdmin = false,
                Department = department,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Error occurred while creating user: {errorMessage}");
            }

            return user;
        }

        public async Task<StudentResponseDto> RegisterStudentAsync(CreateStudentDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Student data cannot be null.");
            }

            var user = await CreateUserAsync(
                request.Name,
                request.Email,
                request.Password,
                UserRole.Student,
                request.Department);

            var entity = _mapper.Map<Student>(request);
            entity.UserId = user.Id;
            entity.User = user;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.Students.AddAsync(entity);
            await _context.SaveChangesAsync();
            var studentResponse = _mapper.Map<StudentResponseDto>(entity);
            return studentResponse;
        }

        public async Task<TeacherResponseDto> RegisterTeacherAsync(CreateTeacherDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Teacher data cannot be null.");
            }

            var user = await CreateUserAsync(
                request.Name,
                request.Email,
                request.Password,
                UserRole.Teacher,
                request.Department);

            var entity = _mapper.Map<Teacher>(request);
            entity.UserId = user.Id;
            entity.User = user;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.Teachers.AddAsync(entity);
            await _context.SaveChangesAsync();
            var teacherResponse = _mapper.Map<TeacherResponseDto>(entity);
            return teacherResponse;
        }

        private async Task<AuthResponse> GenerateJwtTokenAsync(ApplicationUser user, string existingRefreshToken)
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

            var refreshToken = new RefreshToken();

            if (string.IsNullOrEmpty(existingRefreshToken))
            {
                refreshToken = new RefreshToken
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
            }
            var response = new AuthResponse
            {
                Token = jwtToken,
                RefreshToken = (string.IsNullOrEmpty(existingRefreshToken)) ? refreshToken.Token : existingRefreshToken,
                ExpireAt = token.ValidTo
            };

            return response;
        }
        private async Task<AuthResponse?> VeryfyAndGenerateTokenAsync(CreateRefreshToken payload)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                //1 - check JWT token format
                var tokenInVerification = jwtTokenHandler.ValidateToken(payload.Token, _tokenValidationParameters, out var validatedToken);
                //2 - Encryption Algorithm
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                    {
                        return null;
                    }
                }
                //3 - Validate expiry date 
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value ?? "0");

                var expiryDate = unixTimeStampToDateTimeInUTC(utcExpiryDate);
                if (expiryDate > DateTime.UtcNow)
                {
                    throw new Exception("Token has not expired yet");
                }

                //4 - Refresh Token exist in database
                var dbRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == payload.RefreshToken);
                if (dbRefreshToken == null)
                {
                    throw new Exception("Refresh token does not exist");
                }
                else
                {
                    //5- validate id
                    var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

                    if (dbRefreshToken.JwtId != jti)
                    {
                        throw new Exception("Token mismatch");
                    }
                    //check 6 - is token expired or revoked
                    if (dbRefreshToken.DateExpire < DateTime.UtcNow)
                    {
                        throw new Exception("Refresh token has expired");
                    }
                    //check 7 - is token revoked
                    if (dbRefreshToken.IsRevoked)
                    {
                        throw new Exception("Refresh token has been revoked");
                    }

                    //Generate new token (with existing refresh token)
                    var dbUser = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
                    if (dbUser == null)
                    {
                        throw new Exception("User not found for refresh token");
                    }

                    return await GenerateJwtTokenAsync(dbUser, payload.RefreshToken);
                }
            }
            catch (SecurityTokenExpiredException)
            {
                var dbRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == payload.RefreshToken);
                if (dbRefreshToken == null)
                {
                    throw new Exception("Refresh token does not exist");
                }
                //Generate new token (with existing refresh token)
                var dbUser = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
                if (dbUser == null)
                {
                    throw new Exception("User not found for refresh token");
                }

                return await GenerateJwtTokenAsync(dbUser, payload.RefreshToken);
            }
        }


        private DateTime unixTimeStampToDateTimeInUTC(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dateTimeVal;
        }
    }
}
