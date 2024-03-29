﻿using System;
using IROFramework.Core.Tools.AbstractDatabase;

namespace IROFramework.Core.Models
{
    public class UserModel : IBaseModel<Guid>
    {
        #region Base props.
        public Guid Id { get; set; }

        public string Nickname { get; set; }

        public string RealName { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; }
        #endregion

        #region Github props.
        public int Github_UserId { get; set; }

        public string Github_Login { get; set; }

        public string Github_AccessToken { get; set; }
        #endregion
    }
}