using System;
using System.Text.RegularExpressions;
using Core.Common.Interfaces;

namespace Core.Common.SharedDataObjects
{
    public class User: IIdentifiable
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }
        public string Email { get; set; }

        private User(string email, int userId)
        {
            Email = email;
            UserId = userId;
        }

        public static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        public static User CreateNewUser(string email, int userId)
        {
            return !IsValidEmail(email) ? null : new User(email,userId);
        }
    }
}
