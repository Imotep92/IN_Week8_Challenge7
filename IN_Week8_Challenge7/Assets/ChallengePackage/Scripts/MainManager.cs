using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText, BestScoreText;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points, finalScore, bestScore;

    private bool m_GameOver = false;

    public string playerName, bestPlayerName;



    private void Awake()
    {
        // Always one copy of this in any scene (destroying copies)
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        // setting the current object as Instance
        Instance = this;

        // Ensure this is the only persistent object
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }
    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 0.5f, ForceMode.Impulse);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {

        finalScore = m_Points;

        if (finalScore > PlayerPrefs.GetInt("Best Score"))
        {
            PlayerPrefs.SetInt("Best Score", finalScore);
        }

        BestScoreText.text = "Best Score " + PlayerPrefs.GetInt("Best Score");

        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    ///////////////////////////////
    /// 
    //

    //save/load serializable infomation section

    [System.Serializable]
    class saveData
    {
        public string _playerName;
        public int _bestScore;
        public string _bestPlayerName;
    }

    //data._bestScore = bestScore;
    //bestScore = data._bestScore;

    public void SavePlayerName()
    {
        saveData data = new saveData();
        data._playerName = playerName;

        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", jsonData);
    }

    public void SaveBestPlayerInfo()
    {

    }

    public void LoadPlayerName()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            saveData data = JsonUtility.FromJson<saveData>(json);

            playerName = data._playerName;
        }
    }

    public void LoadBestPlayerInfo()
    {
        
    }

}
