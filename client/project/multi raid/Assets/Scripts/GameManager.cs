using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Transform canvas;

    GameObject lobbyObj = null;
    GameObject battleObj = null;

    BATTLE_STATE state = BATTLE_STATE.NONE;
    int myBattleTurn = -1;

    void Awake()
    {
        Debug.Log("GameManager init");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
        EasterEggInit();
    }

    void Init()
    {
        lobbyObj = null;
        battleObj = null;

        state = BATTLE_STATE.NONE;
        myBattleTurn = -1;

        canvas = GameObject.Find("Canvas").transform;

        GameObject prefab = Resources.Load<GameObject>("prefabs/GameLobby");
        lobbyObj = Instantiate(prefab, canvas);

        lobbyObj.transform.Find("MakeRoomBtn").GetComponent<Button>().onClick.AddListener(OnClickMakeRoom);
        lobbyObj.transform.Find("RoomListBtn").GetComponent<Button>().onClick.AddListener(OnClickRoomList);
        lobbyObj.transform.Find("ShopBtn").GetComponent<Button>().onClick.AddListener(OnClickEnterShop);
        lobbyObj.transform.Find("InvenBtn").GetComponent<Button>().onClick.AddListener(OnClickEnterInventory);
        lobbyObj.transform.Find("Wallet/LinkWalletBtn").GetComponent<Button>().onClick.AddListener(OnClickLinkWalletPage);
        lobbyObj.transform.Find("Wallet/UpdateWalletBtn").GetComponent<Button>().onClick.AddListener(OnClickUpdateWallet);
        lobbyObj.transform.Find("LogOutBtn").GetComponent<Button>().onClick.AddListener(OnClickLogOut);

        UpdateWallet(true);
    }

    // Update is called once per frame
    void Update()
    {
        BattleState();
        EasterEgg();
    }

    void OnClickLogOut()
    {
        LoadScene(CommonDefine.LOGIN_SCENE);
    }

    void LoadScene(string nextSceneName)
    {
        GameDataManager.Instance.nextScene = nextSceneName;
        SceneManager.LoadScene(CommonDefine.LOADING_SCENE);
    }

    void OnClickUpdateWallet()
    {
        NetworkManager.Instance.SendServerGet(CommonDefine.GET_MY_WALLET_URL, null, UpdateWallet);
    }

    void UpdateWallet(bool result)
    {
        if (!result)
        {
            Debug.Log("내 지갑 로드 실패");
        }

        if(GameDataManager.Instance.walletBalance < 0)
        {
            lobbyObj.transform.Find("Wallet/balance").GetComponent<TMP_Text>().text = "지갑 연동 안됨.";
        }
        else
        {
            lobbyObj.transform.Find("Wallet/balance").GetComponent<TMP_Text>().text = "잔액 : " + GameDataManager.Instance.walletBalance.ToString("F2");
        }

    }

    void OnClickLinkWalletPage()
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/LinkWallet");
        GameObject obj = Instantiate(prefab, canvas);

        obj.transform.Find("CloseBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));
        obj.transform.Find("LinkBtn").GetComponent<Button>().onClick.AddListener(() => OnClickLinkWallet(obj));
        obj.transform.Find("LinkBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));
    }

    void OnClickLinkWallet(GameObject obj)
    {
        string privateKey = obj.transform.Find("PrivateKey").GetComponent<TMP_InputField>().text;

        LinkWalletPostData data = new LinkWalletPostData
        {
            privateKey = privateKey,
        };

        NetworkManager.Instance.SendServerPost(CommonDefine.LINK_WALLET_URL, data, CallbackLinkWallet);
    }


    void CallbackLinkWallet(bool result)
    {
        if (result)
        {
            CreateMsgBoxOneBtn("지갑 연동 성공", OnClickUpdateWallet);

        }
        else
        {
            CreateMsgBoxOneBtn("지갑 연동 실패");
        }
    }

    void EnterBattle()
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/Battle");
        battleObj = Instantiate(prefab, canvas);

        // 보스 세팅
        Sprite[] spriteFrontAll = Resources.LoadAll<Sprite>("images/pokemon-front");
        battleObj.transform.Find("Boss/Image").GetComponent<Image>().sprite = spriteFrontAll[158];

        Slider bossHpBar = battleObj.transform.Find("Boss/HpBar").GetComponent<Slider>();
        bossHpBar.maxValue = 1.0f;
        bossHpBar.value = 1.0f;

        Slider bossManaBar = battleObj.transform.Find("Boss/ManaBar").GetComponent<Slider>();
        bossManaBar.maxValue = 1.0f;
        bossManaBar.value = 1.0f;

        battleObj.transform.Find("Boss/HpBar/Text").GetComponent<TMP_Text>().text = "150 / 150";
        battleObj.transform.Find("Boss/ManaBar/Text").GetComponent<TMP_Text>().text = "100 / 100";

        // 유저 세팅
        List<BattlePoke> userList = new List<BattlePoke>();
        for (int i = 0; i < 4; ++i)
        {
            BattlePoke data = new BattlePoke
            {
                pokeIdx = i + 10,
                curHp = (i + 1) * 10,
                maxHp = (i + 1) * 10,
                curMana = (i + 1) * 10,
                maxMana = (i + 1) * 10,
            };

            userList.Add(data);
        }

        Sprite[] spriteBackAll = Resources.LoadAll<Sprite>("images/pokemon-back");
        for (int i = 0; i < userList.Count; i++)
        {
            var user = userList[i];
            string player = "4Player/Player" + (i + 1).ToString();

            battleObj.transform.Find(player + "/Image").GetComponent<Image>().sprite = spriteBackAll[user.pokeIdx];

            Slider hpBar = battleObj.transform.Find(player + "/HpBar").GetComponent<Slider>();
            hpBar.maxValue = 1.0f;
            hpBar.value = 1.0f;

            Slider manaBar = battleObj.transform.Find(player + "/ManaBar").GetComponent<Slider>();
            manaBar.maxValue = 1.0f;
            manaBar.value = 1.0f;

            battleObj.transform.Find(player + "/HpBar/Text").GetComponent<TMP_Text>().text = user.curHp.ToString() + " / " + user.maxHp;
            battleObj.transform.Find(player + "/ManaBar/Text").GetComponent<TMP_Text>().text = user.curMana.ToString() + " / " + user.maxMana;
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
        await NetworkManager.Instance.SendMessageToRoom("roomid", skillIdx.ToString());
    }


    public void OnClickAttackTest()
    {
        EnterBattle();

        StartCoroutine(AttackTest());
    }

    public IEnumerator AttackTest()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(TackleCoroutine(battleObj.transform.Find("Boss/Image").position, 0.2f, 0.1f, battleObj.transform.Find("Boss/Image"), battleObj.transform.Find("4Player/Player1/Image")));

        yield return new WaitForSeconds(2f);
        StartCoroutine(TackleCoroutine(battleObj.transform.Find("Boss/Image").position, 0.2f, 0.1f, battleObj.transform.Find("Boss/Image"), battleObj.transform.Find("4Player/Player1/Image")));
        
        yield return new WaitForSeconds(2f);
        StartCoroutine(TackleCoroutine(battleObj.transform.Find("Boss/Image").position, 0.2f, 0.1f, battleObj.transform.Find("Boss/Image"), battleObj.transform.Find("4Player/Player1/Image")));
        
        yield return new WaitForSeconds(2f);
        StartCoroutine(TackleCoroutine(battleObj.transform.Find("Boss/Image").position, 0.2f, 0.1f, battleObj.transform.Find("Boss/Image"), battleObj.transform.Find("4Player/Player1/Image")));
        
        yield return new WaitForSeconds(2f);
        StartCoroutine(TackleCoroutine(battleObj.transform.Find("Boss/Image").position, 0.2f, 0.1f, battleObj.transform.Find("Boss/Image"), battleObj.transform.Find("4Player/Player1/Image")));


    }

    private IEnumerator TackleCoroutine(Vector3 originalPosition, float moveDuration, float waitAfterHit, Transform attacker, Transform target)
    {
        // 1. 목표 방향 계산
        Vector3 targetPosition = target.position;

        // 2. 약간 덜 도착하도록 조정 (중앙까지 가면 겹치므로)
        Vector3 approachPosition = Vector3.Lerp(originalPosition, targetPosition, 0.8f);

        // 3. 돌진
        yield return MoveToPosition(attacker, approachPosition, moveDuration);

        // 4. 맞은 효과 (흔들리기 등) — 선택
        // 예: 타겟 살짝 흔들기
        StartCoroutine(HitEffect(target));

        // 5. 대기
        yield return new WaitForSeconds(waitAfterHit);

        // 6. 제자리로 복귀
        yield return MoveToPosition(attacker, originalPosition, moveDuration);
    }

    private IEnumerator MoveToPosition(Transform obj, Vector3 destination, float duration)
    {
        float elapsed = 0f;
        Vector3 start = obj.position;

        while (elapsed < duration)
        {
            obj.position = Vector3.Lerp(start, destination, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.position = destination;
    }

    private IEnumerator HitEffect(Transform target)
    {
        Vector3 original = target.position;
        float shakeAmount = 1.5f;
        float duration = 0.1f;

        for (int i = 0; i < 3; i++)
        {
            target.position = original + (Vector3.right * shakeAmount);
            yield return new WaitForSeconds(duration);
            target.position = original - (Vector3.right * shakeAmount);
            yield return new WaitForSeconds(duration);
        }

        target.position = original;
    }

    public void OnClickBarTest()
    {
        EnterBattle();

        StartCoroutine(BarTest());
    }

    public IEnumerator BarTest()
    {
        Slider hpBar = battleObj.transform.Find("Boss/HpBar").GetComponent<Slider>();
        Slider manaBar = battleObj.transform.Find("Boss/ManaBar").GetComponent<Slider>();

        hpBar.value = 1.0f;
        hpBar.maxValue = 1.0f;

        manaBar.value = 1.0f;
        manaBar.maxValue = 1.0f;

        yield return new WaitForSeconds(5f);

        StartCoroutine(ReduceBattleBar(hpBar, 90, 90, 30));
        StartCoroutine(ReduceBattleBar(manaBar, 50, 50, 10));

        yield return new WaitForSeconds(5f);

        StartCoroutine(ReduceBattleBar(hpBar, 90, 60, 20));
        StartCoroutine(ReduceBattleBar(manaBar, 50, 40, 15));

        yield return new WaitForSeconds(5f);

        StartCoroutine(ReduceBattleBar(hpBar, 90, 40, 40));
        StartCoroutine(ReduceBattleBar(manaBar, 50, 25, 15));

    }

    private IEnumerator ReduceBattleBar(Slider bar, int maxValue, int curValue, int reduceValue)
    {
        float time = 0f;
        float startValue = bar.value;
        int chageValue = curValue - reduceValue;
        if(chageValue < 0)
            chageValue = 0;
        float endValue = (float)chageValue / (float)maxValue;

        while (time < CommonDefine.BATTLE_BAR_DURATION)
        {
            time += Time.deltaTime;
            float t = time / CommonDefine.BATTLE_BAR_DURATION;
            float cur = Mathf.Lerp(startValue, endValue, t);
            bar.value = cur;
            bar.transform.Find("Text").GetComponent<TMP_Text>().text = (cur * maxValue).ToString("F0") + " / " + maxValue.ToString();
            yield return null;
        }

        bar.value = endValue;
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

            itemObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => SelectPokemon_JoinRoom(room.roomId));
        }

    }

    void SelectPokemon_JoinRoom(string roomId)
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/Inventory");
        GameObject obj = Instantiate(prefab, canvas);

        obj.transform.Find("closeBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));

        obj.transform.Find("Title").GetComponent<TMP_Text>().text = "포켓몬 선택";

        Sprite[] spriteFrontAll = Resources.LoadAll<Sprite>("images/pokemon-front");
        for (int i = 0; i < GameDataManager.Instance.myPokemonList.Length; i++)
        {
            var pokemon = GameDataManager.Instance.myPokemonList[i];

            GameObject itemPrefab = Resources.Load<GameObject>("prefabs/InventoryItem");
            GameObject itemObj = Instantiate(itemPrefab, obj.transform.Find("ScrollView/Viewport/Content"));

            itemObj.transform.Find("Icon/IconImage").GetComponent<Image>().sprite = spriteFrontAll[pokemon.pokemon.id];

            itemObj.transform.Find("Title").GetComponent<TMP_Text>().text = pokemon.pokemon.name;
            itemObj.transform.Find("Context").GetComponent<TMP_Text>().text = "hp : " + pokemon.pokemon.hp.ToString();

            itemObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => JoinRoom(roomId, pokemon));
            itemObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));
        }

    }


    void JoinRoom(string roomId, MyPokemon pokemon)
    {
        // todo 포켓몬 구입후 데이터 갱신
        Debug.Log("JoinRoom : " + roomId);
        NetworkManager.Instance.JoinRoom(roomId, pokemon);
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

            bool isHave = false;
            if(GameDataManager.Instance.myPokemonIds != null && GameDataManager.Instance.myPokemonIds.Contains(shopItem.pokemon.id))
            {
                isHave = true;
            }

            GameObject itemPrefab = Resources.Load<GameObject>("prefabs/ShopItem");
            GameObject itemObj = Instantiate(itemPrefab, obj.transform.Find("ScrollView/Viewport/Content"));

            itemObj.transform.Find("Icon/IconImage").GetComponent<Image>().sprite = spriteFrontAll[shopItem.pokemon.id];

            itemObj.transform.Find("Title").GetComponent<TMP_Text>().text = shopItem.pokemon.name;
            itemObj.transform.Find("Context").GetComponent<TMP_Text>().text = "hp : " + shopItem.pokemon.hp.ToString() + " / 가격 : " + shopItem.price.ToString();

            if (isHave)
            {
                itemObj.transform.Find("Button/buyText").GetComponent<TMP_Text>().text = "보유";
            }
            else
            {
                itemObj.transform.Find("Button/buyText").GetComponent<TMP_Text>().text = "구매";
                itemObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => PurchasePokemon(shopItem.shop_id));
            }

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

        
        obj.transform.Find("CancelBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));
        obj.transform.Find("Select/SelectBtn").GetComponent<Button>().onClick.AddListener(() => SelectPokemon_MakeRoom(obj));

        obj.transform.Find("Select/Context").GetComponent<TMP_Text>().text = "포켓몬을\n선택해주세요.";

    }

    void SelectPokemon_MakeRoom(GameObject makeRoomObj)
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/Inventory");
        GameObject obj = Instantiate(prefab, canvas);

        obj.transform.Find("closeBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));

        obj.transform.Find("Title").GetComponent<TMP_Text>().text = "포켓몬 선택";

        Sprite[] spriteFrontAll = Resources.LoadAll<Sprite>("images/pokemon-front");
        for (int i = 0; i < GameDataManager.Instance.myPokemonList.Length; i++)
        {
            var pokemon = GameDataManager.Instance.myPokemonList[i];

            GameObject itemPrefab = Resources.Load<GameObject>("prefabs/InventoryItem");
            GameObject itemObj = Instantiate(itemPrefab, obj.transform.Find("ScrollView/Viewport/Content"));

            itemObj.transform.Find("Icon/IconImage").GetComponent<Image>().sprite = spriteFrontAll[pokemon.pokemon.id];

            itemObj.transform.Find("Title").GetComponent<TMP_Text>().text = pokemon.pokemon.name;
            itemObj.transform.Find("Context").GetComponent<TMP_Text>().text = "hp : " + pokemon.pokemon.hp.ToString();

            itemObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => UsePokemon_MakeRoom(pokemon, makeRoomObj));
            itemObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));
        }

    }

    void UsePokemon_MakeRoom(MyPokemon pokemon, GameObject makeRoomObj)
    {
        // todo 내 포켓몬 설정후 데이터 갱신
        makeRoomObj.transform.Find("Select/Icon/IconImage").GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("images/pokemon-front")[pokemon.pokemon.id];
        makeRoomObj.transform.Find("Select/Context").GetComponent<TMP_Text>().text = pokemon.pokemon.name + "\nhp : " + pokemon.pokemon.hp.ToString();

        makeRoomObj.transform.Find("MakeBtn").GetComponent<Button>().onClick.RemoveAllListeners();
        makeRoomObj.transform.Find("MakeBtn").GetComponent<Button>().onClick.AddListener(() => MakeRoom(makeRoomObj, pokemon.id));
        makeRoomObj.transform.Find("MakeBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(makeRoomObj));
    }

    void MakeRoom(GameObject obj, int pokemonId)
    {
        string title = obj.transform.Find("Title/InputField").GetComponent<TMP_InputField>().text;
        var dropdown = obj.transform.Find("Level/Dropdown").GetComponent<TMP_Dropdown>();
        string dropdownText = dropdown.options[dropdown.value].text;
        Debug.Log("title : " + title + " / ropdown : " + dropdownText);

        MakeRoomPostData data = new MakeRoomPostData
        {
            roomName = title,
            roomLevel = dropdownText,
            pokemonId = pokemonId
        };

        ConnectSocket();
        NetworkManager.Instance.SendServerPost(CommonDefine.MAKE_ROOM_URL, data, CallbackMakeRoom);

    }

    void CallbackMakeRoom(bool result)
    {
        if (result)
        {
            // todo 로그인 완료 안내창
            EnterRoom();
        }
        else
        {
            CreateMsgBoxOneBtn("방생성 실패");
        }
    }

    void EnterRoom()
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/Room");
        GameObject obj = Instantiate(prefab, canvas);

        for(int i = 1; i <= 4; ++i)
            obj.transform.Find("User/" + i.ToString()).gameObject.SetActive(false);

        Sprite[] spriteFrontAll = Resources.LoadAll<Sprite>("images/pokemon-front");
        for (int i = 0; i < GameDataManager.Instance.myRoomInfo.members.Count; ++i)
        {
            string idx = (i + 1).ToString();
            var member = GameDataManager.Instance.myRoomInfo.members[i];

            obj.transform.Find("User/" + idx).gameObject.SetActive(true);
            obj.transform.Find("User/" + idx + "/Name").GetComponent<TMP_Text>().text = member.username;

            obj.transform.Find("User/" + idx + "/Icon/IconImage").GetComponent<Image>().sprite = spriteFrontAll[member.id];
        }

        obj.transform.Find("closeBtn").GetComponent<Button>().onClick.AddListener(() => NetworkManager.Instance.LeaveRoom(GameDataManager.Instance.myRoomInfo.roomId));
        obj.transform.Find("closeBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));


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

        obj.transform.Find("Title").GetComponent<TMP_Text>().text = "인벤토리";

        Sprite[] spriteFrontAll = Resources.LoadAll<Sprite>("images/pokemon-front");
        for (int i = 0; i < GameDataManager.Instance.myPokemonList.Length; i++)
        {
            var pokemon = GameDataManager.Instance.myPokemonList[i];

            GameObject itemPrefab = Resources.Load<GameObject>("prefabs/InventoryItem");
            GameObject itemObj = Instantiate(itemPrefab, obj.transform.Find("ScrollView/Viewport/Content"));

            itemObj.transform.Find("Icon/IconImage").GetComponent<Image>().sprite = spriteFrontAll[pokemon.pokemon.id];

            itemObj.transform.Find("Title").GetComponent<TMP_Text>().text = pokemon.pokemon.name;
            itemObj.transform.Find("Context").GetComponent<TMP_Text>().text = "hp : " + pokemon.pokemon.hp.ToString();

            itemObj.transform.Find("Button").gameObject.SetActive(false);
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

    void CreateMsgBoxOneBtn(string desc, Action checkResult = null)
    {
        GameObject msgBoxPrefabOneBtn = Resources.Load<GameObject>("prefabs/MessageBox_1Button");
        GameObject obj = Instantiate(msgBoxPrefabOneBtn, canvas);

        obj.transform.Find("desc").GetComponent<TMP_Text>().text = desc;
        obj.transform.Find("CheckBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));

        if (checkResult != null)
        {
            obj.transform.Find("CheckBtn").GetComponent<Button>().onClick.AddListener(() => checkResult());
        }
    }

    void CreateMsgBoxTwoBtn(string desc, Action<bool> yesResult, Action<bool> noResult)
    {
        GameObject msgBoxPrefabOneBtn = Resources.Load<GameObject>("prefabs/MessageBox_2Button");
        GameObject obj = Instantiate(msgBoxPrefabOneBtn, canvas);

        obj.transform.Find("desc").GetComponent<TMP_Text>().text = desc;
        obj.transform.Find("YesBtn").GetComponent<Button>().onClick.AddListener(() => yesResult(obj));
        obj.transform.Find("NoBtn").GetComponent<Button>().onClick.AddListener(() => noResult(obj));
    }







    #region EasterEgg

    int upArrowCount = 0;
    void EasterEggInit()
    {
        upArrowCount = 0;

        lobbyObj.transform.Find("GrantBtn").gameObject.SetActive(false);
        lobbyObj.transform.Find("DeductBtn").gameObject.SetActive(false);

        lobbyObj.transform.Find("GrantBtn").GetComponent<Button>().onClick.AddListener(OnClickGrant);
        lobbyObj.transform.Find("DeductBtn").GetComponent<Button>().onClick.AddListener(OnClickDeduct);
    }

    void OnClickDeduct()
    {
        WalletGetSetPostData data = new WalletGetSetPostData
        {
            amount = "1000",
        };

        NetworkManager.Instance.SendServerPost(CommonDefine.BLOCKCHAIN_DEDUCT_URL, data, CallbackDeduct);
    }

    void CallbackDeduct(bool result)
    {
        if (result)
        {
            CreateMsgBoxOneBtn("CallbackDeduct 성공");
            OnClickUpdateWallet();
        }
        else
        {
            CreateMsgBoxOneBtn("CallbackDeduct 실패");
        }
    }

    void OnClickGrant()
    {
        WalletGetSetPostData data = new WalletGetSetPostData
        {
            amount = "1000",
        };

        NetworkManager.Instance.SendServerPost(CommonDefine.BLOCKCHAIN_GRANT_URL, data, CallbackGrant);
    }

    void CallbackGrant(bool result)
    {
        if (result)
        {
            CreateMsgBoxOneBtn("CallbackGrant 성공");
            OnClickUpdateWallet();
        }
        else
        {
            CreateMsgBoxOneBtn("CallbackGrant 실패");
        }
    }

    void EasterEgg()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            upArrowCount++;
            if(upArrowCount >= 3)
            {
                lobbyObj.transform.Find("GrantBtn").gameObject.SetActive(true);
                lobbyObj.transform.Find("DeductBtn").gameObject.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            upArrowCount = 0;

            lobbyObj.transform.Find("GrantBtn").gameObject.SetActive(false);
            lobbyObj.transform.Find("DeductBtn").gameObject.SetActive(false);
        }
      
    }

    #endregion
}
