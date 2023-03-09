using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class MainManagerMenu : MonoBehaviour
{
    public static MainManagerMenu Instance;

    public InputField inputName;
    public string playerName;
    public static GameObject BestScoreText;

    private void Awake()
    {
        BestScoreText = GameObject.Find("TxtScore");

        SaveData.LoadScore();
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        

    }
    public static void SetBestScore(int score, string player)
    {
        Text texto = BestScoreText.GetComponent<Text>();
        texto.text = $"Best Score: {player} : {score}";
    }
    public void NameSelected()
    {
        playerName = inputName.text;
    }
    public void StarGame()
    {
        SceneManager.LoadScene(1);
    }
    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
    [System.Serializable]
    class SaveData
    {
        public string playerName;
        public int score;
        
        public static void LoadScore()
        {
            int score = 0;
            string player = "";
            string path = Application.persistentDataPath + "/savefile.json";
            if (File.Exists(path))
            {
                string datajson = File.ReadAllText(path);
                SaveData data = JsonUtility.FromJson<SaveData>(datajson);

                score = data.score;
                player = data.playerName;
            }

            MainManagerMenu.SetBestScore(score, player);
        }
    }

}
