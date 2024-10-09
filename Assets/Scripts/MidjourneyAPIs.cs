using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class MidjourneyAPIs : MonoBehaviour
{
    public TMP_InputField inputPrompt; // Input field for user prompt
    public Button Post; // Button to trigger the request
    public RawImage displayImage; // Image to display the generated result

    void Start()
    {
        inputPrompt = GameObject.Find("InputPrompt").GetComponent<TMP_InputField>(); // Find input field in the scene
        Post.GetComponent<Button>().onClick.AddListener(PostData); // Add listener to Post button
    }

    // This function triggers the data posting process
    void PostData() => StartRequest(inputPrompt.text);

    // URL for Midjourney API
    private string apiUrl = "https://api.userapi.ai/midjourney/v2/imagine";
    // URL for Upscaling the generated image
    private string upscaleUrl = "https://api.userapi.ai/midjourney/v2/upscale";
    // Webhook URL to receive progress updates
    private string webhookUrl = "https://webhook.site/(Enter Your Webhook URL Here)"; 

    // Starts the request process with the entered prompt
    public void StartRequest(string prompt)
    {
        StartCoroutine(SendPostRequest(prompt));
    }

    // Send the initial post request to the API
    IEnumerator SendPostRequest(string prompt)
    {
        string json = "{\"prompt\": \"" + prompt + "\"," +
                      "\"webhook_url\": \"" + webhookUrl + "\"," +
                      "\"webhook_type\": \"progress\"," +
                      "\"account_hash\": \"(Enter Your Account Hash Here)\"," + // Add your account hash here
                      "\"is_disable_prefilter\": false}";

        // Set up a POST request
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("api-key", "(Enter Your API Key Here)"); // Add your API key here
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Request completed: " + request.downloadHandler.text);
                var response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
                // Start checking status for the image processing
                StartCoroutine(CheckImageStatus(response.hash));
            }
            else
            {
                Debug.LogError("Error: " + request.error + "\nResponse: " + request.downloadHandler.text);
            }
        }
    }

    // Check the status of the image generation
    IEnumerator CheckImageStatus(string imageHash)
    {
        string statusUrl = $"https://api.userapi.ai/midjourney/v2/status?hash={imageHash}"; // Status URL

        while (true)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(statusUrl))
            {
                request.SetRequestHeader("api-key", "(Enter Your API Key Here)"); // Add your API key here
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var statusResponse = JsonUtility.FromJson<StatusResponse>(request.downloadHandler.text);

                    if (statusResponse.status == "done")
                    {
                        Debug.Log("Image processing done.");
                        // Now upscale the generated image
                        StartCoroutine(SendUpscaleRequest(imageHash, webhookUrl));
                        yield break; // Exit the loop
                    }
                    else if (statusResponse.status == "error")
                    {
                        Debug.LogError("Error checking status: " + statusResponse.status_reason);
                        yield break; // Exit the loop in case of error
                    }
                    else
                    {
                        Debug.Log("Current status: " + statusResponse.status + "\nProgress:" + statusResponse.progress);
                    }
                }
                else
                {
                    Debug.LogError("Error checking status: " + request.error);
                    yield break; // Exit the loop
                }
            }

            // Wait for a bit before checking the status again
            yield return new WaitForSeconds(1.5f);
        }
    }

    // Request to upscale the image
    IEnumerator SendUpscaleRequest(string imageHash, string webhookUrl)
    {
        Debug.Log("Upscaling image with hash: " + imageHash); // Print the image hash

        // Randomly select an upscale option
        //This is for randomly selecting 1 image between 4 but if u can to specify an img then just change the coice no. from 1 to 4

        int choice = Random.Range(1, 5);
        string json = "{\"hash\": \"" + imageHash + "\"," +
                      "\"choice\": " + choice + "," +
                      "\"webhook_url\": \"" + webhookUrl + "\"," +
                      "\"webhook_type\": \"result\"}";

        // Send the upscale request
        using (UnityWebRequest request = new UnityWebRequest(upscaleUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("api-key", "(Enter Your API Key Here)"); // Add your API key here
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Upscale request completed.");
                var uresponse = JsonUtility.FromJson<UpscaleResponse>(request.downloadHandler.text);
                // Start checking the status for the upscaled image
                StartCoroutine(CheckUpscaledImageStatus(uresponse.hash));
            }
            else
            {
                Debug.LogError("Upscale request failed: " + request.error);
            }
        }
    }

    // Check the status of the upscaled image
    IEnumerator CheckUpscaledImageStatus(string uimageHash)
    {
        string statusUrl = $"https://api.userapi.ai/midjourney/v2/status?hash={uimageHash}"; // Status URL

        while (true)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(statusUrl))
            {
                request.SetRequestHeader("api-key", "(Enter Your API Key Here)"); // Add your API key here
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var statusResponse = JsonUtility.FromJson<StatusResponse>(request.downloadHandler.text);

                    if (statusResponse.status == "done")
                    {
                        Debug.Log("Upscaling done.");
                        var upscale = JsonUtility.FromJson<UpscaleResponse>(request.downloadHandler.text);
                        StartCoroutine(DownloadImage(upscale.result.url)); // Download the final upscaled image
                        yield break; // Exit the loop
                    }
                    else if (statusResponse.status == "error")
                    {
                        Debug.LogError("Error checking status: " + statusResponse.status_reason);
                        yield break; // Exit the loop in case of error
                    }
                    else
                    {
                        Debug.Log("Current status: " + statusResponse.status + "\nProgress:" + statusResponse.progress);
                    }
                }
                else
                {
                    Debug.LogError("Error checking status: " + request.error);
                    yield break; // Exit the loop
                }
            }

            // Wait for a bit before checking the status again
            yield return new WaitForSeconds(1.5f);
        }
    }

    // Download the final image and display it
    IEnumerator DownloadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            displayImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture; // Display the image on UI
    }

    [System.Serializable]
    public class Response
    {
        public string account_hash;
        public string hash;
        public string webhook_url;
        public string webhook_type;
        public string prompt;
        public int progress;
        public string status;
        public Result result;

        [System.Serializable]
        public class Result
        {
            public string url;
            public string proxy_url;
            public string filename;
            public string content_type;
            public int width;
            public int height;
            public int size;
        }
    }

    [System.Serializable]
    public class StatusResponse
    {
        public string status;
        public string status_reason; // Provides reason for an error status
        public string progress;
        public Result result;

        [System.Serializable]
        public class Result
        {
            public string url;
            public string proxy_url;
            public string filename;
            public string content_type;
            public int width;
            public int height;
            public int size;
        }
    }

    [System.Serializable]
    public class UpscaleResponse
    {
        public string account_hash;
        public string hash;
        public string webhook_url;
        public string webhook_type;
        public string callback_id;
        public string choice;
        public string type;
        public string status;
        public Result result;
        public List<NextAction> next_actions;
        public string status_reason;
        public string created_at;

        [System.Serializable]
        public class Result
        {
            public string url;
            public string proxy_url;
            public string filename;
            public string content_type;
            public int width;
            public int height;
            public int size;
        }

        [System.Serializable]
        public class NextAction
        {
            public string type;
            public string label;
            public string value;
        }
    }
}
