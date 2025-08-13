using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : Singleton<GameDataManager>
{
    public LoginData loginData = null;

    public PokemonShop[] pokemonShopList = null;
    public MyPokemon[] myPokemonList = null;
    public HashSet<int> myPokemonIds = null;
   
    public Room[] roomList = null;
    public Room myRoomInfo = null;

    public Battle curBattle = null;
    public Dictionary<int, BattleAddInfo> curBattleAddInfo = null;

    public double walletBalance = -1;

    public string nextScene = "";

    protected override void Awake()
    {
        base.Awake();  // �̱��� �ʱ�ȭ

        Debug.Log("GameDataManager init");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetData()
    {
        loginData = null;

        pokemonShopList = null;
        myPokemonList = null;
        myPokemonIds = null;

        roomList = null;
        myRoomInfo = null;

        curBattle = null;
        curBattleAddInfo = null;

        walletBalance = -1;
    }

    public void ResetBattleData()
    {
        roomList = null;
        myRoomInfo = null;

        curBattle = null;
        curBattleAddInfo = null;
    }
}
