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

public enum BATTLE_STATE
{
    NONE,
    WAIT,
    PLAYER1_TURN,
    PLAYER2_TURN,
    PLAYER3_TURN,
    PLAYER4_TURN,
    BOSS_TURN,
    VICTORY,
    DEFEAT,
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

public class Room
{
    public int idx;
    public string title;
    public int level;
    public int masterPokeIdx;
}

public class BattlePoke
{
    public int pokeIdx;
    public int curHp;
    public int maxHp;
}

public class MyPokemon
{
    public int idx;
    public int skill1_idx;
    public string skill1_name;
    public int skill1_attack;
    public int skill2_idx;
    public string skill2_name;
    public int skill2_attack;
    public int skill3_idx;
    public string skill3_name;
    public int skill3_attack;
}