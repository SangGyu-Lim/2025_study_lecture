using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public Transform canvas;

    GameObject loginObj = null;
    GameObject registerObj = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

   
    void Init()
    {
        loginObj = null;
        registerObj = null;

        GameDataManager.Instance.ResetData();

        if (canvas == null)
            canvas = GameObject.Find("Canvas").transform;

        GameObject prefab = Resources.Load<GameObject>("prefabs/Login");
        loginObj = Instantiate(prefab, canvas);

        loginObj.transform.Find("LoginBtn").GetComponent<Button>().onClick.AddListener(OnClickLogin);
        loginObj.transform.Find("RegisterBtn").GetComponent<Button>().onClick.AddListener(OnClickRegisterPage);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClickLogin()
    {
        string id = loginObj.transform.Find("ID").GetComponent<TMP_InputField>().text;
        string password = loginObj.transform.Find("Password").GetComponent<TMP_InputField>().text;

        Debug.Log("id : " + id + " pwd : " + password);

        NetworkManager.Instance.SendLoginServer(CommonDefine.LOGIN_URL, id, password, CallbackLogin);
    }

    void OnClickRegisterPage()
    {
        if(registerObj == null)
        {
            GameObject prefab = Resources.Load<GameObject>("prefabs/Register");
            registerObj = Instantiate(prefab, canvas);

            registerObj.transform.Find("BackBtn").GetComponent<Button>().onClick.AddListener(OnClickLoginPage);
            registerObj.transform.Find("RegisterBtn").GetComponent<Button>().onClick.AddListener(OnClickRegister);
        }
        else
        {
            registerObj.SetActive(true);
        }

        registerObj.transform.Find("ID").GetComponent<TMP_InputField>().text = "";
        registerObj.transform.Find("Password").GetComponent<TMP_InputField>().text = "";

    }

    void OnClickLoginPage()
    {
        registerObj.SetActive(false);

        loginObj.transform.Find("ID").GetComponent<TMP_InputField>().text = "";
        loginObj.transform.Find("Password").GetComponent<TMP_InputField>().text = "";

    }

    void OnClickRegister()
    {
        string id = registerObj.transform.Find("ID").GetComponent<TMP_InputField>().text;
        string password = registerObj.transform.Find("Password").GetComponent<TMP_InputField>().text;

        Debug.Log("id : " + id + " pwd : " + password);

        NetworkManager.Instance.SendLoginServer(CommonDefine.REGISTER_URL, id, password, CallbackRegister);
    }

    void CallbackRegister(bool result)
    {
        if(result)
        {
            CreateMsgBoxOneBtn("È¸¿ø°¡ÀÔ ¼º°ø", OnClickLoginPage);
        }
        else
        {
            CreateMsgBoxOneBtn("È¸¿ø°¡ÀÔ ½ÇÆÐ");
        }
    }

    void CallbackLogin(bool result)
    {
        if (result)
        {
            CreateMsgBoxOneBtn("·Î±×ÀÎ ¼º°ø", GetMyPokemon);
            
        }
        else
        {
            CreateMsgBoxOneBtn("·Î±×ÀÎ ½ÇÆÐ");
        }
    }

    void GetMyPokemon()
    {
        NetworkManager.Instance.SendServerGet(CommonDefine.GET_MY_POKEMON_URL, null, CallbackMyPokemon);
    }

    void CallbackMyPokemon(bool result)
    {
        if (result)
        {
            GetMyWallet();

        }
        else
        {
            CreateMsgBoxOneBtn("³» Æ÷ÄÏ¸ó ·Îµå ½ÇÆÐ");
        }
    }

    void GetMyWallet()
    {
        NetworkManager.Instance.SendServerGet(CommonDefine.GET_MY_WALLET_URL, null, CallbackMyWallet);
    }

    void CallbackMyWallet(bool result)
    {
        if (!result)
        {
            Debug.Log("³» Áö°© ·Îµå ½ÇÆÐ");
        }

        GetAllPokemonData();
    }

    void GetAllPokemonData()
    {
        NetworkManager.Instance.SendServerGet(CommonDefine.GET_ALL_POKEMON_DATA_URL, null, CallbackAllPokemonData);
    }

    void CallbackAllPokemonData(bool result)
    {
        if (!result)
        {
            Debug.Log("ï¿½ï¿½ï¿½Ï¸ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½Îµï¿½ ï¿½ï¿½ï¿½ï¿½");
        }

        LoadScene(CommonDefine.GAME_SCENE);
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

    void LoadScene(string nextSceneName)
    {
        GameDataManager.Instance.nextScene = nextSceneName;
        SceneManager.LoadScene(CommonDefine.LOADING_SCENE);
    }
}
