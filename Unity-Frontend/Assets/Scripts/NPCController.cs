using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class NPCController : MonoBehaviour
{
    [Header("UI")]
    public GameObject canvas;
    public TextMeshProUGUI questionText;
    public Button mainButton;

    [Header("Question Data")]
    public string assignedQuestion;
    private AudioClip recordedClip;

    private enum State { Idle, WaitingForAsk, QuestionAsked, Recording }
    private State currentState = State.Idle;

    private string elevenLabsApiKey => SecretsManager.Instance.keys.elevenLabsApiKey;

    private void Start()
    {
        if (canvas != null) canvas.SetActive(false);

        if (mainButton != null)
            mainButton.onClick.AddListener(OnMainButtonClicked);
        else
            Debug.LogError("❌ NPCController: MainButton not assigned in Inspector.");
    }

    /// <summary>
    /// Called by NPCManager to assign a question.
    /// </summary>
    public void TriggerQuestion(string question)
    {
        assignedQuestion = question;

        if (canvas != null) canvas.SetActive(true);

        currentState = State.WaitingForAsk;

        if (questionText != null)
            questionText.text = "Press 'Ask Question' to hear your question";

        if (mainButton != null)
            mainButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ask Question";
    }

    private void OnMainButtonClicked()
    {
        switch (currentState)
        {
            case State.WaitingForAsk:
                // Show the assigned question
                if (questionText != null)
                    questionText.text = assignedQuestion;

                mainButton.GetComponentInChildren<TextMeshProUGUI>().text = "Record Answer";
                currentState = State.QuestionAsked;
                break;

            case State.QuestionAsked:
                // Start recording
                if (Microphone.devices.Length == 0)
                {
                    Debug.LogError("❌ No microphone found!");
                    return;
                }

                recordedClip = Microphone.Start(null, false, 10, 44100);

                if (recordedClip == null)
                {
                    Debug.LogError("❌ Failed to start microphone recording.");
                    return;
                }

                if (questionText != null)
                    questionText.text = "🎤 Recording... Press 'Stop' when done.";

                mainButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
                currentState = State.Recording;
                break;

            case State.Recording:
                // Stop recording
                Microphone.End(null);

                if (recordedClip == null)
                {
                    Debug.LogError("❌ recordedClip is NULL, nothing to send.");
                    return;
                }

                // Convert to WAV bytes
                byte[] wavData = SavWav.GetWavBytes(recordedClip);

                if (wavData == null || wavData.Length == 0)
                {
                    Debug.LogError("❌ Failed to convert AudioClip to WAV data.");
                    return;
                }

                // Send to ElevenLabs STT
                StartCoroutine(SendToElevenLabs(wavData, assignedQuestion));

                // Reset UI
                currentState = State.Idle;
                if (canvas != null) canvas.SetActive(false);
                break;
        }
    }

    IEnumerator SendToElevenLabs(byte[] audioData, string question)
    {
        if (string.IsNullOrEmpty(elevenLabsApiKey) || elevenLabsApiKey != "33923d92a30eb3fd19dcf06f19384efeaedbe612529e8153953aa2d48bb308e7")
        {
            Debug.LogError("❌ ElevenLabs API Key is not set!");
            yield break;
        }

        if (audioData == null || audioData.Length == 0)
        {
            Debug.LogError("❌ Audio data is empty, cannot send.");
            yield break;
        }

        string url = "https://api.elevenlabs.io/v1/speech-to-text";

        // --- Build multipart form ---
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        // Audio file
        formData.Add(new MultipartFormFileSection("file", audioData, "audio.wav", "audio/wav"));

        // Model ID (required)
        formData.Add(new MultipartFormDataSection("model_id", "scribe_v1"));

        UnityWebRequest www = UnityWebRequest.Post(url, formData);
        www.SetRequestHeader("xi-api-key", elevenLabsApiKey);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string transcription = www.downloadHandler.text;
            Debug.Log("✅ Transcription received: " + transcription);

            if (QADataManager.Instance != null)
            {
                QADataManager.Instance.AddQA(question, transcription);
            }
            else
            {
                Debug.LogError("❌ QADataManager.Instance is NULL. Add QADataManager to the scene.");
            }
        }
        else
        {
            Debug.LogError("❌ STT Error: " + www.error + "\nResponse: " + www.downloadHandler.text);
        }
    }
}
