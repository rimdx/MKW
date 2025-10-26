// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.GUI.Images;

namespace MKW.GUI.Model
{
    public class DatabaseUserModel
    {
        private readonly UserInfo user;

        public DatabaseUserModel(UserInfo user)
        {
            this.user = user;
        }

        public UserId Id => user.Id;
        public bool IsAdmin => user.IsAdmin;
        public string Name
        {
            get
            {
                if (user.IsAdmin)
                {
                    return "Admin";
                }
                else
                {
                    return Formatters.FormatUserName(user.Metadata.UserId, user.Metadata.DisplayName);
                }
            }
        }

        public string LoginUserName
        {
            get
            {
                if (user.IsAdmin)
                {
                    return "Admin";
                }
                else
                {
                    return Formatters.FormatLoginUserName(user.Metadata.UserId, user.Metadata.DisplayName);
                }
            }
        }

        public string ShortName
        {
            get
            {
                if (user.IsAdmin)
                {
                    return "Admin";
                }
                else
                {
                    return Formatters.FormatShortName(user.Metadata.UserId, user.Metadata.DisplayName);
                }
            }
        }

        public ImageMoniker Icon => user.IsAdmin ? ImageMoniker.Admin : ImageMoniker.User;
    }
}
