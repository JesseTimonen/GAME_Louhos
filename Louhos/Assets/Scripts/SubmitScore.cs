using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System;


public class SubmitScore : MonoBehaviour
{
    [SerializeField] private DayManager dayManager;
    [SerializeField] private GameObject SubmitContainer;
    [SerializeField] private Text usernameInput;
    [SerializeField] private Text submitFeedback;

    private string submitScoreURL = "https://jessetimonen.fi/GameOff2023/AddScore.php";
    private string secretKey = "JgilpAc4wto8eMNo1Du3FiF1XKWWGBXN";


    public void SubmitScores()
    {
        StartCoroutine(SubmitScoreCoroutine(usernameInput.text, dayManager.currentDay.ToString()));
    }


    private IEnumerator SubmitScoreCoroutine(string username, string score)
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string hash = CreateSHA256(username + score + timestamp + secretKey);

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("score", score);
        form.AddField("hash", hash);
        form.AddField("timestamp", timestamp.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(submitScoreURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success || string.IsNullOrEmpty(www.downloadHandler.text))
            {
                submitFeedback.text = "Apologies, an error occurred, preventing us from saving your score";
                submitFeedback.color = Color.red;
            }
            else
            {
                SubmitContainer.SetActive(false);
                submitFeedback.text = "Score submitted successfully";
                submitFeedback.color = Color.green;
            }
        }
    }


    string CreateSHA256(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }

}