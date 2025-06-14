using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public Transform canvas;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitBtn();
    }

    void InitBtn()
    {
        canvas.Find("Login/LoginBtn").GetComponent<Button>().onClick.AddListener(OnClickLogin);
        canvas.Find("Login/JoinBtn").GetComponent<Button>().onClick.AddListener(OnClickJoinPage);

        canvas.Find("Join/BackBtn").GetComponent<Button>().onClick.AddListener(OnClickLoginPage);
        canvas.Find("Join/JoinBtn").GetComponent<Button>().onClick.AddListener(OnClickJoin);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClickLogin()
    {
        string id = canvas.Find("Login/ID").GetComponent<InputField>().text;
        string password = canvas.Find("Login/Password").GetComponent<InputField>().text;

        Debug.Log("id : " + id + " pwd : " + password);

        // todo 로그인 연결 - 임시로 씬전환
        SceneManager.LoadScene("GameScene");
        //NetworkManager.Instance.SendLoginServer(CommonDefine.LOGIN_URL, id, password, LoginAction);
    }

    void OnClickJoinPage()
    {
        canvas.Find("Login").gameObject.SetActive(false);
        canvas.Find("Join").gameObject.SetActive(true);

        canvas.Find("Join/ID").GetComponent<InputField>().text = "";
        canvas.Find("Join/Password").GetComponent<InputField>().text = "";

    }

    void OnClickLoginPage()
    {
        canvas.Find("Login").gameObject.SetActive(true);
        canvas.Find("Join").gameObject.SetActive(false);

        canvas.Find("Login/ID").GetComponent<InputField>().text = "";
        canvas.Find("Login/Password").GetComponent<InputField>().text = "";

    }

    void OnClickJoin()
    {
        string id = canvas.Find("Join/ID").GetComponent<InputField>().text;
        string password = canvas.Find("Join/Password").GetComponent<InputField>().text;

        Debug.Log("id : " + id + " pwd : " + password);

        // todo 회원가입 연결 + 회원가입 이후 패킷 처리
        //NetworkManager.Instance.SendLoginServer(CommonDefine.REGISTER_URL, id, password, JoinAction);
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
            // todo 에러창 띄우기
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
            // todo 에러창 띄우기
        }
    }



}
