namespace APICalls
{
    using System.Collections;
    using System.Text;
    using Model;
    using UnityEngine;
// for JSON encoding
    using UnityEngine.Networking;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using static System.Text.Encoding; // for UnityWebRequest
    using TMPro;
    public class Login : MonoBehaviour
    {
        public TMP_InputField usernameField;
        public TMP_InputField passwordField;
        public Text loginMessage; // Text object to display login message

        private UserInfo _currentUser; // Stores the received user object (optional) of type UserInfo

        public UserInfo GetCurrentUser()
        {
            return _currentUser;
        }

        public void OnLoginButtonClick()
        {
            string username = usernameField.text;
            string password = passwordField.text;
            StartCoroutine(LoginRequest(username, password));
        }

        IEnumerator LoginRequest(string username, string password)
        {
            // Create LoginRequest object
            var loginObject = new LoginObject(username, password);
            string jsonData = JsonUtility.ToJson(loginObject);

            var url = "http://localhost:8080/api/auth/login"; // Backend URL
            UnityWebRequest request = new UnityWebRequest(url, "POST");

            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("Error during login request: error " + request.error);
                loginMessage.text = "Login failed! Check the console for details.";
            }
            else
            {
                string response = request.downloadHandler.text;

                // Assuming JSON response with a Result object (check your backend implementation)
                Result<UserInfo> loginResult = JsonUtility.FromJson<Result<UserInfo>>(response);

                if (loginResult.IsSuccess)
                {
                    loginMessage.text = loginResult.Message;

                    // Assuming UserInfoResponse is included in the data field (check backend)
                    UserInfo userInfo = loginResult.Data;

                    // Optionally store user info for future use
                    _currentUser = userInfo; // Now currentUser is of type UserInfo

                    // Handle successful login (e.g., transition to another scene)
                    SceneManager.LoadScene("PlayMenu");
                }
                else
                {
                    loginMessage.text = "Login failed! " + loginResult.Message;
                }
            }
        }
    }
}
