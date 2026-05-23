using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Singleton
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    //Properties
    private float gameTime;
    public float GetgameTime { get { return gameTime; } }

    private float incrementTimer=0f;

    private int dinosAlive;
    public void SetDinosAlive(int da) {  dinosAlive = da; }
    public int GetDinosAlive() { return dinosAlive; }

    private int dinosKilled = 0;
    public void SetDinosKilled(int dk) { dinosKilled = dk; }
    public int GetDinosKilled() { return dinosKilled; }

    private float loseThreshold=500f;

    private void Start() 
    {
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Update()
    {
        UpdateGameTime();
        CheckLoseCondition();
    }

    private void UpdateGameTime() 
    {
        incrementTimer += Time.deltaTime;
        if (incrementTimer >= 1f) 
        {
            gameTime += 1;
            incrementTimer = 0f;
        }

    }

    private void CheckLoseCondition() 
    {
        if (dinosAlive > loseThreshold) 
        {
            EnterLoseState();
        }
    }
   
    
    private void EnterLoseState() 
    {
        Debug.Log("you LOSE");
    }
}
