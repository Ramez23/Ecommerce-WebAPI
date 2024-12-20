﻿namespace Ecommerce.DTO
{
    public class UserDto
    { 
        public class RegisterUserDto
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Role { get; set; } 
        }

        public class LoginUserDto
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
