using System.IO;

namespace APICalls
{
    using System.Collections;
    using System.Collections.Generic;
    using Model;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.SceneManagement;
    
    public class TagSwapStats : MonoBehaviour
    {
        public TMP_Text GamesPlayed;
        public TMP_Text Wins;
        public TMP_Text Loses;
        public TMP_Text ErrorText;


        public string userName = "Shobhit";


        public void OnTagSwapButtonClick()
        {
            string username = userName;
            StartCoroutine(TagSwapRequest(username));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        IEnumerator TagSwapRequest(string username)
        {
            string jsonData = JsonUtility.ToJson(username);

            var url = "http://localhost:8080/api/tagSwap/tagSwapStats"; // Backend URL
            UnityWebRequest request = new UnityWebRequest(url, "POST");

            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string response = request.downloadHandler.text;

                Debug.Log(response);

                // Assuming JSON response with a Result object (check backend implementation)
                Result<TagSwapResponse> tagSwapResult = JsonUtility.FromJson<Result<TagSwapResponse>>(response);
                TagSwapResponse tagSwap = tagSwapResult.Data;

                if (tagSwapResult.success && tagSwapResult.Data is not null)
                {
                    GamesPlayed.text = tagSwap.gamesPlayed.ToString();
                    GamesPlayed.color = Color.green;

                    Wins.text = tagSwap.wins.ToString();
                    Wins.color = Color.green;

                    Loses.text = tagSwap.loses.ToString();
                    Loses.color = Color.green;

                    // Handle successful login (e.g., transition to another scene)
                    SceneManager.LoadScene("MainMenu_Scene");
                }
                else
                {
                    ErrorText.text = tagSwapResult.message;
                    ErrorText.color = Color.red;
                }
            }
        }
    }
}