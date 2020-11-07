using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public GameObject gameOver;
    private GameObject gameOverInstance;
    public InputField usernameField;
    public GameObject gameFinished;
    private GameObject gameFinishedInstance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying objects!");
            Destroy(this);
        }
    }

    public void OnConnectedToServer()
    {
        startMenu.SetActive(false);
        usernameField.interactable = false;
        Client.instance.ConnectToServer();
    }

    public void OnDie()
    {
        Debug.Log("Show game over screen");
        if(gameOverInstance == null)
        {
            gameOverInstance = Instantiate(gameOver, transform);
            gameOverInstance.transform.parent = transform;
        }
    }

    public void OnRespawn()
    {
        if (gameOverInstance != null)
        {
            Destroy(gameOverInstance);
        }
        if(gameFinishedInstance != null)
        {
            Destroy(gameFinishedInstance);
        }
    }

    public void OnGameFinished(string[] finishedPlayerNames, string[] diedPlayerNames)
    {
        Debug.Log("OnGameFinished");
        if(gameFinishedInstance == null)
        {
            Destroy(gameOverInstance);
            gameFinishedInstance = Instantiate(gameFinished, transform);
            gameFinishedInstance.GetComponentInChildren<PlayerList>()
                .RenderPlayerList(finishedPlayerNames.ToList(), diedPlayerNames.ToList());
            gameFinishedInstance.transform.parent = transform;
        }
    }
}
