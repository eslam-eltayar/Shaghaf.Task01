using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shaghaf.APIs.DTOs;
using Shaghaf.APIs.Errors;
using Shaghaf.Core.Entities.Identity;
using Shaghaf.Core.Services;
using Shaghaf.Repository.Identity;
using Shaghaf.Repository.Identity.Helper;

namespace Shaghaf.APIs.Controllers
{
    public class AccountsController : ApiBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ISMSService _smsService;
        private readonly AppIdentityDbContext _context;

        public AccountsController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            ISMSService smsService,
            AppIdentityDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _smsService = smsService;
            _context = context;
        }


        // Register
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            var user = new AppUser()
            {
                PhoneNumber = model.PhoneNumber,
                UserName = model.UserName,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(new ApiResponse(400));

            var returnedUser = new UserDto()
            {
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Token = await _tokenService.CreateTokenAsync(user)
            };

            return Ok(returnedUser);

        }



        // Login
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == model.PhoneNumber);

            if (user is null)
                return BadRequest(new ApiResponse(400, "You dont have an account, SignUp Now!")); 

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401, "Invalid Login"));

            return Ok(new UserDto()
            {
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Token = await _tokenService.CreateTokenAsync(user)
            });

        }


        [HttpPost("ForgetPassword")]
        public async Task<ActionResult> ForgetPassword(ForgetPasswordDto model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == model.PhoneNumber);

            if (user is null)
                return BadRequest(new ApiResponse(400, "User not found"));

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await SaveResetTokenAsync(user.Id, token);

            user.PhoneNumber = $"+2{user.PhoneNumber}";

            _smsService.SendMessage(user.PhoneNumber, $"Your password reset token is: {token}");

            return Ok(new { Message = "Password reset token has been sent via SMS" });
        }


        // Reset Password
        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == model.PhoneNumber);

            if (user is null)
                return BadRequest(new ApiResponse(400, "User not found"));

            if (!await IsResetTokenValidAsync(user.Id, model.Token))
                return BadRequest(new ApiResponse(400, "Invalid or expired reset token"));


            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400, "Failed to reset password"));

            return Ok(new { Message = "Password has been reset successfully" });
        }

        private async Task SaveResetTokenAsync(string userId, string token)
        {
            var resetToken = new PasswordResetToken
            {
                UserId = userId,
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(15)
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();
        }

        private async Task<bool> IsResetTokenValidAsync(string userId, string token)
        {
            var resetToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Token == token && t.Expiration > DateTime.UtcNow);

            return resetToken != null;
        }

    }
}