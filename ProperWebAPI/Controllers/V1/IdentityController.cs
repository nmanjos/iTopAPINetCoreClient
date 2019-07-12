using Microsoft.AspNetCore.Mvc;
using ProperWebAPI.Contract.V1;
using ProperWebAPI.Contract.V1.Requests;
using ProperWebAPI.Contract.V1.Response;
using ProperWebAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProperWebAPI.Controllers.V1
{
    public class IdentityController : Controller
    {
        private readonly IIdentityService identityService;

        public IdentityController(IIdentityService IdentityService)
        {
            identityService = IdentityService;
        }

        [HttpPost(template: ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors= ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
            });
                
            }
            var authResponse = await identityService.RegisterAsync(request.Email, request.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token
            });
        }

        [HttpPost(template: ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var authResponse = await identityService.LoginAsync(request.Email, request.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token
            });
        }

    }
}
