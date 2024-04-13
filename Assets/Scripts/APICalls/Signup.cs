namespace APICalls
{
    using System.Collections;
    using UnityEngine;
    using System.Text.RegularExpressions; // for password validation
    using System;
    using Model;
    using UnityEngine.Networking;
    using UnityEngine.UI; // for DateTime parsing

    public class Signup : MonoBehaviour
    {
        public InputField usernameField;
        public InputField passwordField;
        public InputField emailField;
        public InputField dobField;

        public Text signupMessage; // Text object to display signup message

        private User _currentUser; // Stores the received User object

        public User GetCurrentUser()
        {
            return _currentUser;
        }

        // Function to validate password format
        private static bool IsValidPassword(string password)
        {
            const string passwordPattern = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-])(?=.{8,}$)";
            return Regex.IsMatch(password, passwordPattern);
        }

        // Function to try parsing date and handle exceptions
        private bool ValidateDobFormat(string dobString)
        {
            // Check if the string length is 8 characters (YYYYMMDD)
            if (dobString.Length != 8)
            {
                signupMessage.text = "Invalid DoB format. Please use YYYYMMDD.";
                return false;
            }

            try
            {
                // Convert the string to integer for basic validation
                int year = int.Parse(dobString.Substring(0, 4));
                int month = int.Parse(dobString.Substring(4, 2));
                int day = int.Parse(dobString.Substring(6, 2));

                // Validate month (1-12)
                if (month < 1 || month > 12)
                {
                    signupMessage.text = "Invalid month (must be between 1 and 12).";
                    return false;
                }

                // Validate day based on month (considering leap years is complex)
                int maxDay = DateTime.DaysInMonth(year, month);
                if (day < 1 || day > maxDay)
                {
                    signupMessage.text = "Invalid day for the chosen month.";
                    return false;
                }

                // If all validations pass, format the date as YYYY-MM-DD
                dobString = string.Format("{0:0000}-{1:00}-{2:00}", year, month, day);
                return true;
            }
            catch (FormatException)
            {
                signupMessage.text = "Invalid DoB format. Please use YYYYMMDD.";
                return false;
            }
        }

        public void OnSignupButtonClick()
        {
            string username = usernameField.text;
            string password = passwordField.text;
            string email = emailField.text;
            string dobString = dobField.text;

            // Validate password format
            if (!IsValidPassword(password))
            {
                signupMessage.text = "Password is invalid. Please refer to password requirements.";
                return; // Early exit if password is invalid
            }

            // Validate DoB format
            if (!ValidateDobFormat(dobString))
            {
                return; // Early exit if DoB format is invalid
            }

            StartCoroutine(SignupRequest(username, password, email, dobString));
        }

        IEnumerator SignupRequest(string username, string password, string email, string dob) {
            // Create SignupObject (assuming SignupRequest class exists)
            SignupObject signupObject = new SignupObject(username, password, email, dob);
            string jsonData = JsonUtility.ToJson(signupObject);

            var url = "http://localhost:8080/api/auth/signup"; // Replace with your backend URL
            UnityWebRequest request = new UnityWebRequest(url, "POST");

            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("Error during signup request: " + request.error);
                signupMessage.text = "Signup failed! Check the console for details.";
            }
            else
            {
                string response = request.downloadHandler.text;
                // Assuming JSON response with a Result object containing a User object
                Result<User> signupResult = JsonUtility.FromJson<Result<User>>(response);

                if (signupResult.IsSuccess)
                {
                    signupMessage.text = "Signup successful! Please login.";
                    // Parse the received User object
                    User user = JsonUtility.FromJson<User>(signupResult.Data.ToString());

                    // Store the User object for future requests
                    _currentUser = user;
                }
                else
                {
                    signupMessage.text = "Signup failed! " + signupResult.Message;
                }
            }
        }
    }
}
