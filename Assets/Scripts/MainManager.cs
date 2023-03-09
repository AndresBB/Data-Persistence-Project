using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;
    public static GameObject BestScoreText;

    private bool m_Started = false;
    public int m_Points;
    public static int scoreFinal;

    private bool m_GameOver = false;


    // Start is called before the first frame update
    void Start()
    {
        BestScoreText = GameObject.Find("BestScoreText");

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
        SaveData.LoadScore();
    }

    public static void SetBestScore(int score, string player)
    {
        Text texto = BestScoreText.GetComponent<Text>();
        texto.text = $"Best Score: {player} : {score}";
    }
    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = UnityEngine.Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                scoreFinal = m_Points;
                SaveData.SaveBestScore();
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
        m_GameOver = true;
        GameOverText.SetActive(true);
    }
    [System.Serializable]
    class SaveData
    {
        public string playerName;
        public int score;
        public static void SaveBestScore()
        {
            SaveData data = new SaveData();
            data.playerName = MainManagerMenu.Instance.playerName;
            data.score = MainManager.scoreFinal;

            string path = Application.persistentDataPath + "/savefile.json";
            if (File.Exists(path))
            {
                string datajson = File.ReadAllText(path);
                SaveData data2 = JsonUtility.FromJson<SaveData>(datajson);

                if (data.score > data2.score)
                {
                    datajson = JsonUtility.ToJson(data);

                    File.WriteAllText(Application.persistentDataPath + "/savefile.json", datajson);
                }
                return;
            }
            
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        }
        public static void LoadScore()
        {
            int score = 0 ;
            string player = "";
            string path = Application.persistentDataPath + "/savefile.json";
            if (File.Exists(path))
            {
                string datajson = File.ReadAllText(path);
                SaveData data = JsonUtility.FromJson<SaveData>(datajson);
                
                score = data.score;
                player = data.playerName;
            }

            MainManager.SetBestScore(score, player);
        }
    }

}
