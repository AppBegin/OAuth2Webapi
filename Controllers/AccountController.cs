using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace auth
{
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IApplicationUserRepository _userRepository;
        private readonly ISmsSender _smsSender;
        private readonly Regex regex = new Regex(@"^[1][3-8]\d{9}$");

        public AccountController(IApplicationUserRepository userRepository,ISmsSender smsSender)
        {
            _userRepository = userRepository;
            _smsSender = smsSender;
        }

        [Route("signup")]
        [HttpPost]
        public IActionResult Register([FromBody] ApplicationUser user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (! regex.IsMatch(user.phone))
            {
                return BadRequest();
            }
            if ( user.password.Length < 6)
            {
                return BadRequest();
            }
            if (_userRepository.CheckPhone(user.phone))
            {
                return BadRequest();
            }
            Random rd = new Random();
            int num = rd.Next(100000, 999999);
            user.code = num.ToString();
            if (_userRepository.Add(user)){
                _smsSender.SendSmsAsync(user.phone,user.code);
                return StatusCode(201);
            }
            else{
                return BadRequest();
            }
        }

        [Route("verify")]
        [HttpPost]
        public IActionResult Verify([FromBody] ApplicationUser user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (_userRepository.CheckVerifycode(user.phone,user.code)){
                return StatusCode(200);
            }
            else{
                return BadRequest();
            }
        }
        
        [Route("verifycode/{phone}")]
        [HttpGet]
        public IActionResult GetVerifycode(string phone)
        {
            if (! regex.IsMatch(phone)){
                return BadRequest();
            }
            var user = _userRepository.FindByPhone(phone);
            if (user != null){
                _smsSender.SendSmsAsync(user.phone,user.code);
                return StatusCode(200);
            }
            else{
                return BadRequest();
            }
        }

        [Route("resetpassword")]
        [HttpPost]
        public IActionResult ResetPassword([FromBody] ApplicationUser user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if ( user.password.Length < 6)
            {
                return BadRequest();
            }
            if (_userRepository.ResetPassword(user.phone,user.code,user.password)){
                return StatusCode(200);
            }
            else{
                return BadRequest();
            }
        }

        [Route("{phone}")]
        [HttpGet]
        public IActionResult GetUserInfo(string phone)
        {
            if (! regex.IsMatch(phone)){
                return BadRequest();
            }
            var user = _userRepository.FindByPhone(phone);
            if (user != null){
                user.code = null;
                user.password = null;
                return Ok(user);
            }
            else{
                return NotFound(user);
            }
        }
    }
}
