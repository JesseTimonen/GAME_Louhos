using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;


[Serializable]
public class ScoreEntry
{
    public string username;
    public int score;
}


public class Leaderboard : MonoBehaviour
{
    [SerializeField] private Text scoreBoardText;
    private string getScoresURL = "https://jessetimonen.fi/GameOff2023/GetScores.php";


    void Start()
    {
        StartCoroutine(GetScores());
    }


    IEnumerator GetScores()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(getScoresURL))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                DisplayErrorMessage();
            }
            else
            {
                if (!string.IsNullOrEmpty(www.downloadHandler.text))
                {
                    try
                    {
                        ProcessScores(www.downloadHandler.text);
                    }
                    catch
                    {
                        DisplayErrorMessage();
                    }
                }
            }
        }
    }


    void ProcessScores(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            DisplayErrorMessage();
            return;
        }

        try
        {
            ScoreEntry[] scores = JsonHelper.FromJson<ScoreEntry>(json);

            scoreBoardText.text = "";
            int scoreCount = scores.Length;
            for (int i = 0; i < 10; i++)
            {
                if (i < scoreCount)
                {
                    scoreBoardText.text += (i + 1) + ". " + scores[i].username + ": " + scores[i].score + " days\n";
                }
                else
                {
                    scoreBoardText.text += (i + 1) + ".\n";
                }
            }
        }
        catch
        {
            DisplayErrorMessage();
        }
    }


    void DisplayErrorMessage()
    {
        scoreBoardText.text = "Apologies, an error occurred, preventing us from displaying the scores";
        scoreBoardText.color = Color.red;
    }
}


// Helper class to handle JSON array
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>("{\"Items\":" + json + "}");
        return wrapper.Items;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}