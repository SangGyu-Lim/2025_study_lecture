using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("GameManager Awake");
    }

    void OnEnable()
    {
        Debug.Log("GameManager OnEnable");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("GameManager Start");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("GameManager Update");
    }

    void LateUpdate()
    {
        Debug.Log("GameManager LateUpdate");
    }

    void OnApplicationQuit()
    {
        Debug.Log("GameManager OnApplicationQuit");
    }

    void OnDisable()
    {
        Debug.Log("GameManager OnDisable");
    }

    void OnDestroy()
    {
        Debug.Log("GameManager OnDestroy  ");
    }
}
