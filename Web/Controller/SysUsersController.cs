﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using iread_identity_ms.DataAccess.Repo;
using iread_identity_ms.Web.Service;
using iread_identity_ms.Web.Dto;
using iread_identity_ms;
using iread_identity_ms.DataAccess.Data.Entity;
using AutoMapper;
using iread_identity_ms.Web.Util;
using Microsoft.AspNetCore.Identity;
using iread_identity_ms.DataAccess;

namespace M3allem.M3allem.Controller
{
    [Route("api/identity_ms/[controller]/")]
    [ApiController]
    public class SysUsersController : ControllerBase
    {
        private readonly AppUsersService _usersService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;


        public SysUsersController(
            IPublicRepository repository, 
            AppUsersService usersService, 
             IMapper mapper,
             UserManager<ApplicationUser> userManager)
        {
            _usersService = usersService;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: api/SysUsers/all
        [HttpGet("all")]
        [Authorize(Roles = Policies.Administrator)]
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            return _mapper.Map<IEnumerable<UserDto>>(await _usersService.GetAll());
        }
        
        
        // GET: api/SysUsers/get-by-email
        [HttpGet("get-by-email")]
        public async Task<IActionResult> GetUserByEmail([FromQuery(Name = "email")] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Startup.GetErrorsFromModelState(ModelState));
            }

            if (String.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("Email", "Email is required");
                return BadRequest(Startup.GetErrorsFromModelState(ModelState));
            }

            if (!(new EmailAddressAttribute().IsValid(email)))
            {
                ModelState.AddModelError("Email", "Email is not valid");
                return BadRequest(Startup.GetErrorsFromModelState(ModelState));
            }

            var user = await _usersService.GetByEmail(email);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserDto>(user));
        }


        // GET: api/SysUsers/5/get
        [HttpGet("{id}/get")]
        public async Task<IActionResult> GetUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Startup.GetErrorsFromModelState(ModelState));
            }

            var user = await _usersService.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserDto>(user));
        }

        
        // POST: api/SysUsers/add
        [HttpPost("add")]
        public async Task<IActionResult> PostUser([FromBody] ApplicationUser user)
        {

            if (user == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(Startup.GetErrorsFromModelState(ModelState));
            }

            await UserFieldValidationAsync(user);

            if (!ModelState.IsValid)
            {
                return BadRequest(Startup.GetErrorsFromModelState(ModelState));
            }

            IActionResult response = BadRequest();
            if (!_usersService.Insert(user))
            {
                return BadRequest();
            }

            if (user != null)
            {
    // request token
    //     var tokenClient = new TokenClient("http://localhost:5000/connect/token", "ro.client", "secret");
    //     var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "password", "api1");

    //     if (tokenResponse.IsError)
    //     {
    //         Console.WriteLine(tokenResponse.Error);
    //         return;
    //     }

    //     Console.WriteLine(tokenResponse.Json);
    //     Console.WriteLine("\n\n");

                response = Ok(user);
            }

            return response;
        }
        

        // DELETE: api/SysUsers/5
        [HttpDelete("{id}/delete")]
        [Authorize(Roles = Policies.Administrator)]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Startup.GetErrorsFromModelState(ModelState));
            }

            var user = await _usersService.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            if (!_usersService.Delete(user))
            {
                return BadRequest();
            }

            return NoContent();
        }


        private async Task UserFieldValidationAsync(ApplicationUser user)
        {
 
            var similarUser = await _usersService.GetByEmail(user.Email);
            if (similarUser != null)
            {
                ModelState.AddModelError("email", "Email already exist");
            }
        }
    }
}