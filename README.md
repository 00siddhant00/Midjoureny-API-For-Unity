# Midjoureny-API-For-Unity
 - Generating images in unity using midjoureny through [userapi.ai](https://userapi.ai)

## Functionality
 - This Unity script provides an interface to communicate with the Midjourney API for generating and upscaling AI images based on user input. Here's a summary of its functionality:
 
 1. **User Input & Button Interaction**: 
    - The script links a user input field (`TMP_InputField`) and a button (`Post`) in the Unity UI. 
    - When the button is clicked, it sends the text prompt entered by the user to the Midjourney API.
 
 2. **API Requests**:
    - **Send Post Request**: Posts the input prompt to the Midjourney API for image generation. It includes a webhook URL to track the progress of image generation.
    - **Check Image Status**: Repeatedly checks the status of the image creation until it's either done or an error occurs.
    - **Send Upscale Request**: Once the image is generated, the script randomly selects one of the four images and sends an upscale request to enhance the selected image.
 
 3. **Webhook and Progress Tracking**:
    - The webhook is used to receive updates on the image generation progress. The script continuously checks the status of the image until it's fully processed.
 
 4. **Image Download and Display**:
    - Once the image is upscaled and finalized, the script downloads the image and displays it on a Unity `RawImage` UI component.
 
 This script integrates API calls with Unity's `UnityWebRequest` system to interact with external APIs and updates the Unity UI dynamically based on the API responses.


## UI
 ![Main](main.png)

## Implementation
 - Make an account in [userapi.ai](https://userapi.ai)
 - Follow [THIS](https://medium.com/@divan.brexov/how-to-get-all-midjourney-functional-via-api-7ece4ab0660f) Blog to setup & connect to your MidJourney API.
 - Setup Webhook to expose endpoints, I prefer [Webhook.site](https://webhook.site/) (Easy) and assign webhook variable in below code.
 - Enter all the Kyes and URL from userapi.ai [Dashboard](https://dashboard.userapi.ai/discord-accounts) in the code provided below in this repo.
 - All the base code is provided in the [CODE](https://github.com/00siddhant00/Midjoureny-API-For-Unity/blob/main/Assets/Scripts/MidjourneyAPIs.cs)
   - Callbacks Used:
     - POST
     - GET 
 - Check out the doc for ferther implimentations - [Read Doc](https://butternut-saffron-e5e.notion.site/Midjourney-userapi-ai-doc-en-119680339b0a47e2ba6ae467eca58142#03f78e40cb094001b7b1c6f4aae0af4c).
 - Enjoy 😌!
