using Microsoft.AspNetCore.Identity;
using SmartLib.Data;
using SmartLib.Interfaces;

namespace SmartLib.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationDbContext> _userManager;
        private readonly IConfiguration _configuration;
        public AuthRepository(
                              ApplicationDbContext context,
                              RoleManager<IdentityRole> roleManager,
                              UserManager<ApplicationDbContext> userManager,
                              IConfiguration configuration
                             )
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }
    }
}
