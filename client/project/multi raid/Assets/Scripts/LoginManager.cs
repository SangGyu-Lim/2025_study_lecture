using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public Transform canvas;

    GameObject loginObj;
    GameObject joinObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

   
    void Init()
    {
        GameObject loginPrefab = Resources.Load<GameObject>("prefabs/Login");
        loginObj = Instantiate(loginPrefab, canvas);

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

        // todo 로그인 연결 - 임시로 씬전환
        //SceneManager.LoadScene("GameScene");
        NetworkManager.Instance.SendLoginServer(CommonDefine.LOGIN_URL, id, password, LoginAction);
    }

    void OnClickJoinPage()
    {
        if(joinObj == null)
        {
            GameObject joinPrefab = Resources.Load<GameObject>("prefabs/Join");
            joinObj = Instantiate(joinPrefab, canvas);

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
        string temp = GameDataManager.Instance.sessionId;
        GameDataManager.Instance.sessionId = "temp";
        string temp2 = GameDataManager.Instance.temp;

        // todo 회원가입 연결 + 회원가입 이후 패킷 처리
        NetworkManager.Instance.SendLoginServer(CommonDefine.REGISTER_URL, id, password, JoinAction);
    }

    void JoinAction(bool result)
    {
        if(result)
        {
            // todo 회원가입 완료창
            OnClickLoginPage();
        }
        else
        {
            CreateMsgBoxOneBtn("helhel");
        }
    }

    void LoginAction(bool result)
    {
        if (result)
        {
            // todo 로그인 완료 안내창
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            CreateMsgBoxOneBtn("helhel2");
        }
    }

    async void TestScoket()
    {
        //NetworkManager.Instance.SendServer(CommonDefine.MAKE_ROOM_URL, "", "");

        await NetworkManager.Instance.ConnectSocket();
    }

    void CreateMsgBoxOneBtn(string desc)
    {
        GameObject msgBoxPrefabOneBtn = Resources.Load<GameObject>("prefabs/MessageBox_1Button");
        GameObject obj = Instantiate(msgBoxPrefabOneBtn, canvas);

        obj.transform.Find("desc").GetComponent<TMP_Text>().text = desc;
        obj.transform.Find("CheckBtn").GetComponent<Button>().onClick.AddListener(() => DestroyObject(obj));
    }

    void DestroyObject(GameObject obj)
    {
        Destroy(obj);
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
