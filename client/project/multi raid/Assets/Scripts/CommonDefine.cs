using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class CommonDefine
{
    public const string WEB_POST_BASE_URL = "http://127.0.0.1:80/";
    public const string REGISTER_URL = "users/register";
    public const string LOGIN_URL = "users/login";

    public const string WEB_SOCKET_URL = "http://127.0.0.1:80/";

}

public enum API_TYPE
{
    None = 0,
    join = 1,
    login = 2,
}