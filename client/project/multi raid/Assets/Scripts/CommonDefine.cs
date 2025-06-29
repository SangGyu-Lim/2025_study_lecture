using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class CommonDefine
{
    public const string WEB_POST_BASE_URL = "http://127.0.0.1:3000/";
    public const string REGISTER_URL = "users/register";
    public const string LOGIN_URL = "users/login";
    public const string MAKE_ROOM_URL = "rooms";
    public const string MAKE_ROOM_LIST_URL = "MAKE_ROOM_URL";
    


    public const string WEB_SOCKET_URL = "ws://localhost:3000/rooms";

}

public class LoginPostData
{
    public string username;
    public string password;
}

public class LoginData
{
    public string sessionId;
    public string username;
}

public class PostData
{
    public string sessionId;
}

public class PostData2
{
    public string roomId;
}

public class PokemonShop
{
    public int idx;
    public string name;
    public string desc;
    public int price;
}

public class Pokemon
{
    public int idx;
    public string name;
    public string desc;
}