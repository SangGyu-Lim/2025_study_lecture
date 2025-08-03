using System.Collections.Generic;


public static class CommonDefine
{
    public const string WEB_BASE_URL = "http://127.0.0.1:3000/";

    public const string REGISTER_URL = "users/register";
    public const string LOGIN_URL = "users/login";
    public const string GET_MY_POKEMON_URL = "users/pokemons";
    public const string LINK_WALLET_URL = "users/wallet/link";

    public const string GET_MY_WALLET_URL = "blockchain/balance";
    public const string BLOCKCHAIN_GRANT_URL = "blockchain/grant";
    public const string BLOCKCHAIN_DEDUCT_URL = "blockchain/deduct";

    public const string SHOP_LIST_URL = "shop/items";
    public const string SHOP_PURCHASE_URL = "shop/purchase";

    public const string ROOM_LIST_URL = "rooms";

    

    public const string WEB_SOCKET_URL = "ws://localhost:3000/rooms";

    public const string LOADING_SCENE = "LoadingScene";
    public const string GAME_SCENE = "GameScene";
    public const string LOGIN_SCENE = "SampleScene";

    public const string SOCKET_CREATE_ROOM = "createRoom";
    public const string SOCKET_ROOM_UPDATE = "roomUpdate";
    public const string SOCKET_JOIN_ROOM = "joinRoom";
    public const string SOCKET_LEAVE_ROOM = "leaveRoom";
    public const string SOCKET_START_RAID = "startRaid";
    public const string SOCKET_RAID_ACTION = "action";

    public const float BATTLE_BAR_DURATION = 3f;

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
    public string id;
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
    public int pokemonId;
}

public class LinkWalletPostData
{
    public string privateKey;
}

public class WalletGetSetPostData
{
    public string amount;
}

#endregion

public class LoginData
{
    public string sessionId;
    public string id;
}

public class WalletData
{
    public string balance;
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
    public int poketmonId;
    public string name;
    public int hp;
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
    public int pokemon_id;
    public int skill_id;
    public string name;
    public string type;
    public string target;
    public int damage;
    public int pp;
}

public class BattlePoke
{
    public int pokeIdx;
    public int curHp;
    public int maxHp;
    public int curMana;
    public int maxMana;
}

public class ServerPacket
{
    public string packetType;
    public string packetValue;
}

[System.Serializable]
public class Room
{
    public string roomId;
    public int leaderId;
    public int bossPokemonId;
    public List<RoomMember> members;
    public string eventType;
}

[System.Serializable]
public class RoomMember
{
    public int userSeq;
    public int pokemonId;
    public int order;
}
