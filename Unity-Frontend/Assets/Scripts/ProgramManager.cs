using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using SimpleJSON;

public class ProgramManager : MonoBehaviour
{
    [Header("Input")]
    public string presentationCode;

    [Header("UI References")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI codeText;
    public GameObject hashcodeCanvas;

    [Header("Slide UI Canvases")]
    public GameObject speechCanvas;
    public TextMeshProUGUI speechText;
    public GameObject slideControlCanvas;

    [Header("Fetched Presentation Data")]
    public string title;
    public int slideCount;
    public bool hasImages;
    public string createdAt;

    [Header("Slide Data")]
    public string[] slideTexts;
    public string[] slideImageUrls;
    public string[] speechContents;
    public string[] questions;

    [Header("Slides as Sprites (Loaded)")]
    public Sprite[] slideSprites;

    private string baseUrl = "https://awaazbackend.onrender.com/api/presentation/";

    public IEnumerator LoadPresentation(string code)
    {
        string url = baseUrl + code;
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Request Error: " + www.error);
                if (statusText != null) statusText.text = "Failed";
                if (codeText != null) codeText.text = "";
                yield break;
            }

            var json = JSON.Parse(www.downloadHandler.text);
            if (json == null || !json["success"].AsBool)
            {
                Debug.LogError("Invalid response: " + www.downloadHandler.text);
                if (statusText != null) statusText.text = "Failed";
                if (codeText != null) codeText.text = "";
                yield break;
            }

            var pres = json["presentation"];
            title = pres["title"];
            slideCount = pres["slideCount"].AsInt;
            hasImages = pres["hasImages"].AsBool;
            createdAt = pres["createdAt"];

            // --- Slide URLs ---
            var slideUrls = pres["slideUrls"];
            if (slideUrls != null && slideUrls.Count > 0)
            {
                slideImageUrls = new string[slideUrls.Count];
                foreach (var kvp in slideUrls)
                {
                    if (int.TryParse(kvp.Key, out int index))
                    {
                        index -= 1;
                        if (index >= 0 && index < slideImageUrls.Length)
                            slideImageUrls[index] = kvp.Value.Value;
                    }
                }
            }

            // --- Slide Texts ---
            var texts = pres["slideTexts"];
            if (texts != null && texts.Count > 0)
            {
                slideTexts = new string[texts.Count];
                foreach (var kvp in texts)
                {
                    if (int.TryParse(kvp.Key, out int index))
                    {
                        index -= 1;
                        if (index >= 0 && index < slideTexts.Length)
                            slideTexts[index] = kvp.Value.Value;
                    }
                }
            }

            // --- Questions ---
            var qns = pres["questions"];
            if (qns != null && qns.Count > 0)
            {
                questions = new string[slideCount];
                foreach (var kvp in qns)
                {
                    if (int.TryParse(kvp.Key, out int slideIndex))
                    {
                        slideIndex -= 1;
                        if (slideIndex >= 0 && slideIndex < questions.Length)
                        {
                            JSONArray arr = kvp.Value.AsArray;
                            if (arr != null && arr.Count > 0)
                                questions[slideIndex] = arr[0].Value;
                        }
                    }
                }
            }

            // --- Speech Content ---
            var speech = pres["speechContent"];
            if (speech != null && speech.Count > 0)
            {
                speechContents = new string[speech.Count];
                for (int i = 0; i < speech.Count; i++)
                    speechContents[i] = speech[i].Value;

                if (speechText != null)
                {
                    string combined = "";
                    for (int i = 0; i < speechContents.Length; i++)
                        combined += $"Slide {i + 1}: {speechContents[i]}\n\n";
                    speechText.text = combined;
                }
            }

            // UI updates
            if (statusText != null) statusText.text = "Passed";
            if (codeText != null) codeText.text = "";
            if (hashcodeCanvas != null) hashcodeCanvas.SetActive(false);
            if (speechCanvas != null) speechCanvas.SetActive(true);
            if (slideControlCanvas != null) slideControlCanvas.SetActive(true);

            // --- Load Images ---
            ImageLoader imageLoader = FindObjectOfType<ImageLoader>();
            if (imageLoader != null && slideImageUrls != null && slideImageUrls.Length > 0)
            {
                yield return StartCoroutine(imageLoader.LoadImages(slideImageUrls, this));
            }
            else
            {
                Debug.LogWarning("⚠️ No ImageLoader found or no slide URLs.");
            }

            // --- Trigger NPC questions safely ---
            if (questions != null && questions.Length > 0)
            {
                NPCManager npcManager = FindObjectOfType<NPCManager>();
                if (npcManager != null)
                    npcManager.StartQuestions(questions);
            }

            // --- Show first slide ---
            PresentationManager pm = FindObjectOfType<PresentationManager>();
            if (pm != null)
                pm.ShowSlide(0);
        }
    }
}
