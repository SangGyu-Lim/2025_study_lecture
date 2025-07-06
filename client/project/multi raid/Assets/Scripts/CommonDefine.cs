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

    public const string BLOCKCHAIN_GRANT_URL = "blockchain/grant";

    public const string SHOP_LIST_URL = "shop/items";
    public const string SHOP_PURCHASE_URL = "shop/purchase";

    public const string MAKE_ROOM_URL = "rooms/createRoom";
    public const string ROOM_LIST_URL = "rooms/getRooms";

    

    public const string WEB_SOCKET_URL = "ws://localhost:3000/rooms";

    public const string LOADING_SCENE = "LoadingScene";
    public const string GAME_SCENE = "GameScene";
    public const string LOGIN_SCENE = "SampleScene";
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

#region POST_DATA
public class LoginPostData
{
    public string username;
    public string password;
}

public class PurchasePostData
{
    public int itemId;
}

public class MakeRoomPostData
{
    public string roomName;
    public string roomLevel;
}

public class LinkWalletPostData
{
    public string privateKey;
}

public class GrantPostData
{
    public string amount;
}

#endregion

public class LoginData
{
    public string sessionId;
    public int id;
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

[System.Serializable]
public class Room
{
    public string roomId;
    public List<RoomMember> members;
}

[System.Serializable]
public class RoomMember
{
    public int id;
    public string username;
}
