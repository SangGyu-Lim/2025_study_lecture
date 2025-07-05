using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public Transform canvas;

    GameObject lobbyObj = null;
    GameObject battleObj = null;

    BATTLE_STATE state = BATTLE_STATE.NONE;
    int myBattleTurn = -1;

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
        GameObject prefab = Resources.Load<GameObject>("prefabs/GameLobby");
        lobbyObj = Instantiate(prefab, canvas);

        lobbyObj.transform.Find("MakeRoomBtn").GetComponent<Button>().onClick.AddListener(OnClickMakeRoom);
        lobbyObj.transform.Find("RoomListBtn").GetComponent<Button>().onClick.AddListener(OnClickRoomList);
        lobbyObj.transform.Find("ShopBtn").GetComponent<Button>().onClick.AddListener(OnClickEnterShop);
        lobbyObj.transform.Find("InvenBtn").GetComponent<Button>().onClick.AddListener(OnClickEnterInventory);
    }

    // Update is called once per frame
    void Update()
    {
        BattleState();
    }

    void EnterBattle()
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/Battle");
        battleObj = Instantiate(prefab, canvas);

        // 보스 세팅
        Sprite[] spriteFrontAll = Resources.LoadAll<Sprite>("images/pokemon-front");
        battleObj.transform.Find("Boss/Image").GetComponent<Image>().sprite = spriteFrontAll[158];

        Slider bossHpSlider = battleObj.transform.Find("Boss/HpBar").GetComponent<Slider>();
        bossHpSlider.maxValue = 150;
        bossHpSlider.value = 150;
        battleObj.transform.Find("Boss/HpBar/HpText").GetComponent<TMP_Text>().text = "150 / 150";

        // 유저 세팅
        List<BattlePoke> userList = new List<BattlePoke>();
        for (int i = 0; i < 4; ++i)
        {
            BattlePoke data = new BattlePoke
            {
                pokeIdx = i + 10,
                curHp = (i + 1) * 10,
                maxHp = (i + 1) * 10,
            };

            userList.Add(data);
        }

        Sprite[] spriteBackAll = Resources.LoadAll<Sprite>("images/pokemon-back");
        for (int i = 0; i < userList.Count; i++)
        {
            var user = userList[i];
            string player = "4Player/Player" + (i + 1).ToString();

            battleObj.transform.Find(player + "/Image").GetComponent<Image>().sprite = spriteBackAll[user.pokeIdx];

            Slider hpSlider = battleObj.transform.Find(player + "/HpBar").GetComponent<Slider>();
            hpSlider.maxValue = user.maxHp;
            hpSlider.value = user.curHp;

            battleObj.transform.Find(player + "/HpBar/HpText").GetComponent<TMP_Text>().text = user.curHp.ToString() + " / " + user.maxHp;
        }

    }

    void BattleState()
    {
        // todo 배틀의 각각 상태 처리
        switch (state)
        {
            case BATTLE_STATE.NONE:
                {
                    // 전투 상태 아님.
                }
                break;
            case BATTLE_STATE.WAIT:
                {
                    // 다른 사람들 턴.
                    battleObj.transform.Find("State/state").gameObject.SetActive(true);
                    battleObj.transform.Find("State/Skill").gameObject.SetActive(false);
                }
                break;
            case BATTLE_STATE.PLAYER1_TURN:
                {
                    SetBattleTurn(1);
                }
                break;
            case BATTLE_STATE.PLAYER2_TURN:
                {
                    SetBattleTurn(2);
                }
                break;
            case BATTLE_STATE.PLAYER3_TURN:
                {
                    SetBattleTurn(3);
                }
                break;
            case BATTLE_STATE.PLAYER4_TURN:
                {
                    SetBattleTurn(4);
                }
                break;
            case BATTLE_STATE.BOSS_TURN:
                {
                    battleObj.transform.Find("State/state").GetComponent<TMP_Text>().text = "보스의 순서입니다.";
                    state = BATTLE_STATE.WAIT;
                }
                break;
            case BATTLE_STATE.VICTORY:
                {
                    myBattleTurn = -1;
                }
                break;
            case BATTLE_STATE.DEFEAT:
                {
                    myBattleTurn = -1;
                }
                break;
        }
    }

    void SetBattleTurn(int turn)
    {
        if (myBattleTurn == turn)
        {
            battleObj.transform.Find("State/Skill").gameObject.SetActive(true);

            var myPokemon = GameDataManager.Instance.myCurPokemon;

            for(int i = 0; i < myPokemon.pokemon.skills.Count; ++i)
            {
                string idx = (i + 1).ToString();
                var skill = myPokemon.pokemon.skills[i];

                battleObj.transform.Find("State/Skill/skill" + idx + "Btn").GetComponent<Button>().onClick.AddListener(() => UseSkill(skill.id));
                battleObj.transform.Find("State/Skill/skill" + idx + "Btn/Text").GetComponent<TMP_Text>().text = skill.name;

            }

        }
        else
        {
            battleObj.transform.Find("State/state").GetComponent<TMP_Text>().text = "Player " + turn.ToString() + "의 순서입니다.";
            state = BATTLE_STATE.WAIT;
        }
    }

    async void UseSkill(int skillIdx)
    {
        await NetworkManager.Instance.SendMessageToRoom(skillIdx.ToString());
    }

    async void ConnectSocket()
    {
        // todo 웹소켓 연결
        await NetworkManager.Instance.ConnectSocket();
    }

    void OnClickRoomList()
    {
        // todo 서버에서 룸리스트 받아오기
        ConnectSocket();
        NetworkManager.Instance.SendServerGet(CommonDefine.ROOM_LIST_URL, null, CallbackRoomList);

    }

    void CallbackRoomList(bool result)
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/RoomList");
        GameObject obj = Instantiate(prefab, canvas);

        obj.transform.Find("closeBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));
        obj.transform.Find("closeBtn").GetComponent<Button>().onClick.AddListener(() => { GameDataManager.Instance.roomList = null; });

        Sprite[] spriteFrontAll = Resources.LoadAll<Sprite>("images/pokemon-front");
        for (int i = 0; i < GameDataManager.Instance.roomList.Length; i++)
        {
            var room = GameDataManager.Instance.roomList[i];

            GameObject itemPrefab = Resources.Load<GameObject>("prefabs/RoomListItem");
            GameObject itemObj = Instantiate(itemPrefab, obj.transform.Find("ScrollView/Viewport/Content"));

            itemObj.transform.Find("Icon/IconImage").GetComponent<Image>().sprite = spriteFrontAll[room.members[0].id];

            //itemObj.transform.Find("Title").GetComponent<TMP_Text>().text = room.title;
            //itemObj.transform.Find("Level").GetComponent<TMP_Text>().text = "level " + room.level.ToString();

            itemObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => JoinRoom(room.roomId));
        }

    }

    void JoinRoom(string idx)
    {
        // todo 포켓몬 구입후 데이터 갱신
        Debug.Log("JoinRoom : " + idx);
        NetworkManager.Instance.JoinRoom(idx);
    }

    void OnClickEnterShop()
    {
        if(GameDataManager.Instance.pokemonShopList == null)
        {
            NetworkManager.Instance.SendServerGet(CommonDefine.SHOP_LIST_URL, null, CallbackShopList);
        }
        else
        {
            CreateShop();
        }

    }

    void CallbackShopList(bool result)
    {
        if (result)
        {
            CreateShop();
        }
        else
        {
            CreateMsgBoxOneBtn("상점 로드 실패");
        }
    }

    void CreateShop()
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/Shop");
        GameObject obj = Instantiate(prefab, canvas);

        obj.transform.Find("closeBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));

        Sprite[] spriteFrontAll = Resources.LoadAll<Sprite>("images/pokemon-front");
        for (int i = 0; i < GameDataManager.Instance.pokemonShopList.Length; i++)
        {
            var shopItem = GameDataManager.Instance.pokemonShopList[i];

            GameObject itemPrefab = Resources.Load<GameObject>("prefabs/ShopItem");
            GameObject itemObj = Instantiate(itemPrefab, obj.transform.Find("ScrollView/Viewport/Content"));

            itemObj.transform.Find("Icon/IconImage").GetComponent<Image>().sprite = spriteFrontAll[shopItem.pokemon.id];

            itemObj.transform.Find("Title").GetComponent<TMP_Text>().text = shopItem.pokemon.name;
            itemObj.transform.Find("Context").GetComponent<TMP_Text>().text = "hp : " + shopItem.pokemon.hp.ToString() + " / 가격 : " + shopItem.price.ToString();

            itemObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => PurchasePokemon(shopItem.shop_id));
        }

    }

    void PurchasePokemon(int idx)
    {
        Debug.Log("PurchasePokemon : " + idx);
        PurchasePostData data = new PurchasePostData
        {
            itemId = idx,
        };

        // todo 포켓몬 구입후 데이터 갱신 + 패킷 오류
        NetworkManager.Instance.SendServerPost(CommonDefine.SHOP_PURCHASE_URL, data, CallbackPurchasePokemon);
    }

    void CallbackPurchasePokemon(bool result)
    {
        if (result)
        {
            CreateShop();
        }
        else
        {
            CreateMsgBoxOneBtn("상점 구매 실패");
        }
    }


    void OnClickMakeRoom()
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/MakeRoom");
        GameObject obj = Instantiate(prefab, canvas);

        var dropdown = obj.transform.Find("Level/Dropdown").GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();
        List<string> list = new List<string>();
        for(int i = 0; i < 20; ++i)
        {
            list.Add("level " + (i + 1));
        }
        dropdown.AddOptions(list);

        obj.transform.Find("MakeBtn").GetComponent<Button>().onClick.AddListener(() => MakeRoom(obj));
        obj.transform.Find("MakeBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));
        obj.transform.Find("CancelBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));


    }

    void MakeRoom(GameObject obj)
    {
        string title = obj.transform.Find("Title/InputField").GetComponent<TMP_InputField>().text;
        var dropdown = obj.transform.Find("Level/Dropdown").GetComponent<TMP_Dropdown>();
        string dropdownText = dropdown.options[dropdown.value].text;
        Debug.Log("title : " + title + " / ropdown : " + dropdownText);

        MakeRoomPostData data = new MakeRoomPostData
        {
            roomName = title,
            roomLevel = dropdownText
        };

        ConnectSocket();
        NetworkManager.Instance.SendServerPost(CommonDefine.MAKE_ROOM_URL, data, CallbackMakeRoom);

    }

    void CallbackMakeRoom(bool result)
    {
        if (result)
        {
            // todo 로그인 완료 안내창
            CreateMsgBoxOneBtn("방생성 완료");
        }
        else
        {
            CreateMsgBoxOneBtn("방생성 실패");
        }
    }

    void OnClickEnterInventory()
    {
        // todo GameDataManager의 내 포켓몬 데이터 확인후 없으면 서버에서 포켓몬 데이터 받아오기
        if (GameDataManager.Instance.myPokemonList == null)
        {
            NetworkManager.Instance.SendServerGet(CommonDefine.GET_MY_POKEMON_URL, null, CallbackMyPokemon);
        }
        else
        {
            CreateInventory();
        }
    }


    void CallbackMyPokemon(bool result)
    {
        if (result)
        {
            CreateInventory();

        }
        else
        {
            CreateMsgBoxOneBtn("내 포켓몬 로드 실패");
        }
    }

    void CreateInventory()
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/Inventory");
        GameObject obj = Instantiate(prefab, canvas);

        obj.transform.Find("closeBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));

        Sprite[] spriteFrontAll = Resources.LoadAll<Sprite>("images/pokemon-front");
        for (int i = 0; i < GameDataManager.Instance.myPokemonList.Length; i++)
        {
            var pokemon = GameDataManager.Instance.myPokemonList[i];

            GameObject itemPrefab = Resources.Load<GameObject>("prefabs/InventoryItem");
            GameObject itemObj = Instantiate(itemPrefab, obj.transform.Find("ScrollView/Viewport/Content"));

            itemObj.transform.Find("Icon/IconImage").GetComponent<Image>().sprite = spriteFrontAll[pokemon.pokemon.id];

            itemObj.transform.Find("Title").GetComponent<TMP_Text>().text = pokemon.pokemon.name;
            itemObj.transform.Find("Context").GetComponent<TMP_Text>().text = "hp : " + pokemon.pokemon.hp.ToString();

            itemObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => UsePokemon(pokemon.id));
        }

    }

    void UsePokemon(int idx)
    {
        // todo 내 포켓몬 설정후 데이터 갱신
        Debug.Log("UsePokemon : " + idx);
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

    void CreateMsgBoxOneBtn(string desc)
    {
        GameObject msgBoxPrefabOneBtn = Resources.Load<GameObject>("prefabs/MessageBox_1Button");
        GameObject obj = Instantiate(msgBoxPrefabOneBtn, canvas);

        obj.transform.Find("desc").GetComponent<TMP_Text>().text = desc;
        obj.transform.Find("CheckBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));
    }

    void CreateMsgBoxTwoBtn(string desc, Action<bool> yesResult, Action<bool> noResult)
    {
        GameObject msgBoxPrefabOneBtn = Resources.Load<GameObject>("prefabs/MessageBox_2Button");
        GameObject obj = Instantiate(msgBoxPrefabOneBtn, canvas);

        obj.transform.Find("desc").GetComponent<TMP_Text>().text = desc;
        obj.transform.Find("YesBtn").GetComponent<Button>().onClick.AddListener(() => yesResult(obj));
        obj.transform.Find("NoBtn").GetComponent<Button>().onClick.AddListener(() => noResult(obj));
    }

}
