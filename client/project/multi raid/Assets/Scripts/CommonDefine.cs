using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class CommonDefine
{
    public const string WEB_POST_BASE_URL = "http://127.0.0.1:3000/";
    public const string REGISTER_URL = "users/register";
    public const string LOGIN_URL = "users/login";
    public const string MAKE_ROOM_URL = "rooms";
    

    public const string WEB_SOCKET_URL = "ws://localhost:3000/rooms";

}

public enum API_TYPE
{
    None = 0,
    join = 1,
    login = 2,
}