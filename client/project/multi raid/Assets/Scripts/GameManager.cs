using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public Transform canvas;

    GameObject lobbyObj = null;

    protected override void Awake()
    {
        base.Awake();  // 싱글톤 초기화

        Debug.Log("GameManager init");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    void Init()
    {
        GameObject lobbyPrefab = Resources.Load<GameObject>("prefabs/GameLobby");
        lobbyObj = Instantiate(lobbyPrefab, canvas);

        lobbyObj.transform.Find("MakeRoomBtn").GetComponent<Button>().onClick.AddListener(OnClickMakeRoom);
        //lobbyObj.transform.Find("EnterRoomBtn").GetComponent<Button>().onClick.AddListener(OnClickLogin);
        lobbyObj.transform.Find("ShopBtn").GetComponent<Button>().onClick.AddListener(OnClickEnterShop);
        lobbyObj.transform.Find("InvenBtn").GetComponent<Button>().onClick.AddListener(OnClickInventory);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClickEnterShop()
    {
        // todo GameDataManager의 포켓몬 데이터 확인후 없으면 서버에서 포켓몬 데이터 받아오기
        if(GameDataManager.Instance.pokemonShopList == null)
        {
            //NetworkManager.Instance.SendServer(CommonDefine.MAKE_ROOM_URL, title, dropdownText);
            GameDataManager.Instance.pokemonShopList = new List<GameDataManager.PokemonShop>();
            for (int i = 0; i < 5; ++i)
            {
                GameDataManager.PokemonShop data = new GameDataManager.PokemonShop
                {
                    idx = i,
                    name = "이상해씨" + i.ToString(),
                    desc = "이상해씨가 이상해" + i.ToString(),
                };

                GameDataManager.Instance.pokemonShopList.Add(data);
            }
            
        }
        

        CreateShop();

    }

    void CreateShop()
    {
        GameObject ShopPrefab = Resources.Load<GameObject>("prefabs/ShopContainer");
        GameObject shopObj = Instantiate(ShopPrefab, canvas);

        shopObj.transform.Find("closeBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(shopObj));

        Sprite[] spriteAll = Resources.LoadAll<Sprite>("images/pokemon-front");
        for (int i = 0; i < GameDataManager.Instance.pokemonShopList.Count; i++)
        {
            var pokemon = GameDataManager.Instance.pokemonShopList[i];

            GameObject ShopItemPrefab = Resources.Load<GameObject>("prefabs/ShopItem");
            GameObject itemObj = Instantiate(ShopItemPrefab, shopObj.transform.Find("ShopItemScrollView/Viewport/Content"));

            itemObj.transform.Find("Icon/IconImage").GetComponent<Image>().sprite = spriteAll[pokemon.idx];

            itemObj.transform.Find("Title").GetComponent<TMP_Text>().text = pokemon.name;
            itemObj.transform.Find("Context").GetComponent<TMP_Text>().text = pokemon.desc;

            itemObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => BuyPokemon(pokemon.idx));
        }

    }

    void BuyPokemon(int idx)
    {
        Debug.Log("BuyPokemon : " + idx);
    }
    

    void OnClickMakeRoom()
    {
        GameObject MakeRoomPrefab = Resources.Load<GameObject>("prefabs/MakeRoom");
        GameObject obj = Instantiate(MakeRoomPrefab, canvas);

        var dropdown = obj.transform.Find("Level/Dropdown").GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();
        List<string> list = new List<string>();
        for(int i = 0; i < 20; ++i)
        {
            list.Add("level " + (i + 1));
        }
        dropdown.AddOptions(list);

        obj.transform.Find("MakeBtn").GetComponent<Button>().onClick.AddListener(() => MakeRoom(obj));
        obj.transform.Find("CancelBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));


    }

    void MakeRoom(GameObject obj)
    {
        string title = obj.transform.Find("Title/InputField").GetComponent<TMP_InputField>().text;
        var dropdown = obj.transform.Find("Level/Dropdown").GetComponent<TMP_Dropdown>();
        string dropdownText = dropdown.options[dropdown.value].text;
        Debug.Log("title : " + title + " / ropdown : " + dropdownText);

        NetworkManager.Instance.SendServer(CommonDefine.MAKE_ROOM_URL, title, dropdownText);

    }

    void OnClickInventory()
    {
        GameObject InvenPrefab = Resources.Load<GameObject>("prefabs/Inventory");

        GameObject inven = Instantiate(InvenPrefab, canvas);

        for (int i = 0; i < 3; i++)
        {
            GameObject InvenItemPrefab = Resources.Load<GameObject>("prefabs/InventoryItem");

            GameObject invenItem = Instantiate(InvenItemPrefab, inven.transform.Find("Scroll View/Viewport/Content"));
        }

    }

    void SetTransformTwoBtn(Transform trans, Action<bool> firstResult, Action<bool> secondResult)
    {
        trans.Find("firstBtn").GetComponent<Button>().onClick.AddListener(() => firstResult(true));
        trans.Find("secondBtn").GetComponent<Button>().onClick.AddListener(() => secondResult(true));
    }

    void DestroyObject(GameObject obj)
    {
        Destroy(obj);
    }

}
