using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RadegastNc
{
    public class LoginOptions
    {
        private string firstName;
        private string lastName;
        private string password;
        private bool isPasswordMD5 = false;
        private string author = string.Empty;
        private string userAgent = string.Empty;

        private StartLocationType startLocation = StartLocationType.Home;
        private string startLocationCustom = string.Empty;

        private LoginGrid grid = LoginGrid.MainGrid;
        private string gridCustomLoginUri = string.Empty;

        public LoginOptions()
        {

        }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                    return string.Empty;
                else
                    return firstName + " " + lastName;
            }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public bool IsPasswordMD5
        {
            get { return isPasswordMD5; }
            set { isPasswordMD5 = value; }
        }

        public StartLocationType StartLocation
        {
            get { return startLocation; }
            set { startLocation = value; }
        }

        public string StartLocationCustom
        {
            get { return startLocationCustom; }
            set { startLocationCustom = value; }
        }

        public string UserAgent
        {
            get { return userAgent; }
            set { userAgent = value; }
        }

        public string Author
        {
            get { return author; }
            set { author = value; }
        }

        public LoginGrid Grid
        {
            get { return grid; }
            set { grid = value; }
        }

        public string GridCustomLoginUri
        {
            get { return gridCustomLoginUri; }
            set { gridCustomLoginUri = value; }
        }
    }
}
