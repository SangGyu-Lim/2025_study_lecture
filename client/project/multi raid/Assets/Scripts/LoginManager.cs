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

        loginObj.transform.Find("LoginBtn").GetComponent<Button>().onClick.AddListener(OnClickSkillTest);
        loginObj.transform.Find("RegisterBtn").GetComponent<Button>().onClick.AddListener(OnClickRegisterPage);
    }

    void OnClickSkillTest()
    {
        GameObject prefab = Resources.Load<GameObject>("prefabs/Skills/bell1");
        skillObj = Instantiate(prefab, canvas);

        animator = skillObj.transform.GetComponent<Animator>();
        // Animator에서 사용하는 첫 번째 AnimationClip 가져오기
        //AnimationClip clip = anim.runtimeAnimatorController.animationClips[0];

        //// Loop 끄기
        //clip.wrapMode = WrapMode.Once;

        //// 재생
        //anim.Play(clip.name, 0, 0f);

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log(info.normalizedTime);
    }

    void OnClickSkillAgainTest()
    {
        AnimationClip clip = animator.runtimeAnimatorController.animationClips[0];
        Debug.Log(animator.runtimeAnimatorController);

        animator.Play(clip.name, 0, 0f);

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log(info.normalizedTime);
    }

    GameObject skillObj = null;
    public Animator animator = null;

    // Update is called once per frame
    void Update()
    {
        if (animator != null)
        {
            Debug.Log("애니메이션 시작");

            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.normalizedTime >= 1.0f)
            {
                Debug.Log("애니메이션 종료!");
                DestroyObject(skillObj);
            }
        }
        else
        {
            Debug.Log("animator null");
        }
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
            Debug.Log("포켓몬 데이터 로드 실패");
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
