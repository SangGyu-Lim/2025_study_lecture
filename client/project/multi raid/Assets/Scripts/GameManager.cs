using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Transform canvas;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitLogin();
    }

    void InitLogin()
    {
        canvas.Find("Login/LoginBtn").GetComponent<Button>().onClick.AddListener(OnClickLogin);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClickLogin()
    {
        string id = canvas.Find("Login/ID/Text").GetComponent<Text>().text;
        string password = canvas.Find("Login/Password/Text").GetComponent<Text>().text;

        Debug.Log("id : " + id + " pwd : " + password);

        SceneManager.LoadScene("GameScene");
        //NetworkManager.Instance.SendServer(API_TYPE.login, id, password);
    }



}
