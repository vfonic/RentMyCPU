using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentMyCPU.Backend.Data;
using RentMyCPU.Backend.Data.Entities;
using RentMyCPU.Backend.Logic.Jwt;
using RentMyCPU.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RentMyCPU.Backend.Controllers
{

    [Route("api/[controller]/[action]")]
    public class TokenController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IJwtTokenProvider _jwtProvider;
        private readonly ApplicationDbContext _applicationDbContext;

        public TokenController(SignInManager<User> signInManager,
            UserManager<User> userManager,
            IUserStore<User> userStore,
            IJwtTokenProvider jwtProvider,
            ApplicationDbContext applicationDbContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _jwtProvider = jwtProvider;
            _applicationDbContext = applicationDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] TokenAuthViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    Guid deviceId = Guid.Empty;
                    var isValidRequest = Request.Headers.ContainsKey("x-device-id") && Guid.TryParse(Request.Headers["x-device-id"], out deviceId)
                        && Request.Headers.ContainsKey("x-device-os") && Request.Headers.ContainsKey("x-device-name");
                    if (isValidRequest)
                    {
                        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, model.Email) };
                        var accessToken = _jwtProvider.CreateAccessToken(new ClaimsIdentity(claims));
                        var userId = _applicationDbContext.Users.Where(x => x.NormalizedEmail == model.Email.ToUpper()).Select(x => x.Id).FirstOrDefault();
                        var worker = _applicationDbContext.Workers.FirstOrDefault(x => x.DeviceId == deviceId && x.UserId == userId);
                        if (worker == null)
                        {
                            var dbWorker = new Worker
                            {
                                DeviceId = deviceId,
                                Name = Request.Headers["x-device-name"],
                                OS = Request.Headers["x-device-os"],
                                UserId = userId
                            };
                            _applicationDbContext.Workers.Add(dbWorker);
                            await _applicationDbContext.SaveChangesAsync();
                        }
                        return Ok(new TokenResultViewModel
                        {
                            AccessToken = accessToken.Token,
                            ExpireInSeconds = (int)accessToken.Expire.TotalSeconds
                        });
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                return Unauthorized();
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(new User { UserName = model.UserName, Email = model.UserName }, model.Password);
                if (!result.Succeeded)
                {
                    return BadRequest();
                }
                return Ok();
            }
            return BadRequest(ModelState);
        }
    }
}