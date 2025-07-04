using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class CommonDefine
{
    public const string WEB_POST_BASE_URL = "http://127.0.0.1:3000/";

    public const string REGISTER_URL = "users/register";
    public const string LOGIN_URL = "users/login";
    public const string GET_MY_POKEMON_URL = "users/poketmons";
    public const string GET_MY_WALLET_URL = "users/wallet/link";

    public const string SHOP_LIST_URL = "shop/items";
    public const string SHOP_PURCHASE_URL = "shop/purchase";

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
    public int id;
}

public class PostData2
{
    public string roomId;
}


[System.Serializable]
public class PokemonShop
{
    public int shop_id;
    public int price;
    public int stock;
    public Pokemon pokemon;
}

[System.Serializable]
public class MyPokemon
{
    public int id;
    public Pokemon pokemon;
    public List<PokemonSkill> skills;
}

[System.Serializable]
public class Pokemon
{
    public int id;
    public string name;
    public int hp;
    public List<PokemonSkill> skills;
}

[System.Serializable]
public class PokemonSkill
{
    public int id;
    public int pokemon_id;
    public string name;
    public int attack;
    public int cost;
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

public class ServerPacket
{
    public string packetType;
    public string packetValue;
}

public class PostWalletData
{
    public string privateKey;
}

public class PurchasePostData
{
    public int itemId;
}