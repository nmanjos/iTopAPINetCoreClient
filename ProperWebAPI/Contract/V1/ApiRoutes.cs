using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProperWebAPI.Contract.V1
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string Base = Root + "/" +Version;
        public static class Posts
        {
            public const string GetAll = Base +"/posts";
            public const string Create = Base + "/posts";
            public const string Get = Base + "/posts/{PostId}";
            public const string Update = Base + "/posts/{PostId}";
            public const string Delete = Base + "/posts/{PostId}";
        }

        public static class Identity
        {
            public const string Login = Base + "/identity/login";

            public const string Register = Base + "/identity/register";
        }
    }
}
