﻿using BANKFE.Models;
using BANKFE.Services;
using BANKFE.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANKFE.Controllers
{
    public class ChangePasswordController : Controller
    {
        private readonly HttpService _httpservices;
        private readonly IConfiguration _configuration;
        public ChangePasswordController(HttpService httpservice, IConfiguration configuration)
        {
            _httpservices = httpservice;
            _configuration = configuration;
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult Index()
        {
            string mode = HttpContext.Request.Query["mode"][0];
            string username = HttpContext.Request.Query["username"][0];

            if (mode == "change")
            {
                var sha1 = System.Security.Cryptography.SHA1.Create();
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(username));
            }

            ViewData["mode"] = mode;
            ViewData["username"] = username;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DoChangePassword([FromBody] ChangePasswordViewModel param)
        {
            ChangePassword changePassword = new ChangePassword(param.Token, param.UserName, "", param.NewPassword, param.Mode, param.Reff);
            var result = await _httpservices.PostData(_configuration["APIUrl"] + "/User/ChangePassword", changePassword);
            if ((int)result.StatusCode != 200)
            {
                return Unauthorized(result.Content.ReadAsStringAsync().Result);
            }
            return Ok(result.Content.ReadAsStringAsync().Result);
        }

        [HttpGet]
        public async Task<IActionResult> SendChangePassword([FromQuery] string email)
        {
            var result = await _httpservices.PostData(_configuration["APIUrl"] + "/User/ForgotPassword", new { Email = email });
            if ((int)result.StatusCode != 200)
            {
                return Unauthorized(await result.Content.ReadAsStringAsync());
            }
            return Ok(await result.Content.ReadAsStringAsync());
        }
    }
}
