using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private static MenuManager _instance;
    public static MenuManager Instance { get { return _instance; } }

    [SerializeField] GameObject menu;
    [SerializeField] GameObject gamePrefab;

    GameObject currentGame;

    private void Start()
    {
        
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Update()
    {
        
    }

    public void StartGame(GameManager.DifficultyEnum difficulty)
    {
        menu.SetActive(false);
        if (currentGame)
        {
            Destroy(currentGame);
        }
        currentGame = Instantiate(gamePrefab);
        currentGame.SetActive(true);
        GameManager.Instance.Difficulty = difficulty;
    }
}
