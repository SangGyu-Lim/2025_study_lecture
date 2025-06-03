using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Transform canvas;

    private Button loginBtn;
    private Button loginBtn2;

    private Text loginText;
    private Text loginText2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loginBtn = GameObject.Find("LoginBtn").GetComponent<Button>();
        loginBtn.onClick.AddListener(OnClickLoginBtn);

        loginText = GameObject.Find("LoginBtn/Text").GetComponent<Text>();
        loginText.text = "loginText1";


        loginBtn2 = canvas.Find("LoginBtn2").GetComponent<Button>();
        loginBtn2.onClick.AddListener(OnClickLoginBtn2);

        loginText2 = canvas.Find("LoginBtn2/Text").GetComponent<Text>();
        loginText2.text = "loginText2";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClickLoginBtn()
    {
        Debug.Log("OnClickLoginBtn");
    }

    void OnClickLoginBtn2()
    {
        Debug.Log("OnClickLoginBtn2");
    }

    public void OnClickLoginBtn3()
    {
        Debug.Log("OnClickLoginBtn3");
    }

}
