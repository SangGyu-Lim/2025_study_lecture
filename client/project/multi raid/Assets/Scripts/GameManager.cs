using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public Transform canvas;

    GameObject lobbyObj;

    protected override void Awake()
    {
        base.Awake();  // ΩÃ±€≈Ê √ ±‚»≠

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
        //lobbyObj.transform.Find("ShopBtn").GetComponent<Button>().onClick.AddListener(OnClickLogin);
        lobbyObj.transform.Find("InvenBtn").GetComponent<Button>().onClick.AddListener(OnClickInventory);
    }

    // Update is called once per frame
    void Update()
    {
        
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

        //GameObject MakeRoomPrefab = Resources.Load<GameObject>("prefabs/MakeRoom");
        //GameObject obj = Instantiate(MakeRoomPrefab, canvas);

        //obj.transform.Find("MakeBtn").GetComponent<Button>().onClick.AddListener(() => firstResult(true));
        //obj.transform.Find("CancelBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));


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
