using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : Singleton<GameDataManager>
{
    [SerializeField] public string sessionId = "8335e46c-4900-45ca-ad25-63c41598269e";

    public string temp = "8335e46c-4900-45ca-ad25-63c41598269e";

    public List<PokemonShop> pokemonShopList = null;
    public List<Pokemon> myPokemonList = null;

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
    protected override void Awake()
    {
        base.Awake();  // ΩÃ±€≈Ê √ ±‚»≠

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
}
