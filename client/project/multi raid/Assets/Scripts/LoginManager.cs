using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public Transform canvas;

    GameObject loginObj = null;
    GameObject joinObj = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

   
    void Init()
    {
        loginObj = null;
        joinObj = null;

        GameDataManager.Instance.ResetData();

        canvas = GameObject.Find("Canvas").transform;

        GameObject prefab = Resources.Load<GameObject>("prefabs/Login");
        loginObj = Instantiate(prefab, canvas);

        loginObj.transform.Find("LoginBtn").GetComponent<Button>().onClick.AddListener(OnClickLogin);
        loginObj.transform.Find("JoinBtn").GetComponent<Button>().onClick.AddListener(OnClickJoinPage);

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

    void OnClickJoinPage()
    {
        if(joinObj == null)
        {
            GameObject prefab = Resources.Load<GameObject>("prefabs/Join");
            joinObj = Instantiate(prefab, canvas);

            joinObj.transform.Find("BackBtn").GetComponent<Button>().onClick.AddListener(OnClickLoginPage);
            joinObj.transform.Find("JoinBtn").GetComponent<Button>().onClick.AddListener(OnClickJoin);
        }
        else
        {
            joinObj.SetActive(true);
        }

        joinObj.transform.Find("ID").GetComponent<TMP_InputField>().text = "";
        joinObj.transform.Find("Password").GetComponent<TMP_InputField>().text = "";

    }

    void OnClickLoginPage()
    {
        joinObj.SetActive(false);

        loginObj.transform.Find("ID").GetComponent<TMP_InputField>().text = "";
        loginObj.transform.Find("Password").GetComponent<TMP_InputField>().text = "";

    }

    void OnClickJoin()
    {
        string id = joinObj.transform.Find("ID").GetComponent<TMP_InputField>().text;
        string password = joinObj.transform.Find("Password").GetComponent<TMP_InputField>().text;

        Debug.Log("id : " + id + " pwd : " + password);

        NetworkManager.Instance.SendLoginServer(CommonDefine.REGISTER_URL, id, password, CallbackJoin);
    }

    void CallbackJoin(bool result)
    {
        if(result)
        {
            CreateMsgBoxOneBtn("회원가입 성공", OnClickLoginPage);
        }
        else
        {
            CreateMsgBoxOneBtn("회원가입 실패");
        }
    }

    void CallbackLogin(bool result)
    {
        if (result)
        {
            CreateMsgBoxOneBtn("로그인 성공", GetMyPokemon);
            
        }
        else
        {
            CreateMsgBoxOneBtn("로그인 실패");
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
            CreateMsgBoxOneBtn("내 포켓몬 로드 실패");
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
            Debug.Log("내 지갑 로드 실패");
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
