using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text;

public class PresentationManager : MonoBehaviour
{
    [Header("References")]
    public ProgramManager programManager;
    public NPCManager npcManager;
    public Image slideImage;
    public TMP_Text reportCodeText; // 👈 TMP field to display the code

    [Header("Backend API")]
    public string reportApiUrl = "https://awaazbackend.onrender.com/api/generate-report";

    private int currentSlide = 0;

    public void ShowSlide(int index)
    {
        if (programManager == null || programManager.slideSprites == null || programManager.slideSprites.Length == 0)
            return;

        if (index >= 0 && index < programManager.slideSprites.Length)
        {
            currentSlide = index;
            if (slideImage != null)
                slideImage.sprite = programManager.slideSprites[currentSlide];
        }
    }

    public void NextSlide() => ShowSlide(currentSlide + 1);

    public void PreviousSlide() => ShowSlide(currentSlide - 1);

    public void StopPresentation()
    {
        if (npcManager != null)
            npcManager.StopSession();

        StartCoroutine(SendQAsToBackend());
    }

    private IEnumerator SendQAsToBackend()
    {
        List<QAPairBackend> qaPairs = new List<QAPairBackend>();
        if (QADataManager.Instance != null)
        {
            foreach (var pair in QADataManager.Instance.QAPairs)
            {
                qaPairs.Add(new QAPairBackend
                {
                    question = pair.question,
                    userAnswer = pair.answer
                });
            }
        }

        ReportRequestBackend requestBody = new ReportRequestBackend
        {
            title = "Technical Interview Report - AWS Solutions Architect",
            qaPairs = qaPairs.ToArray()
        };

        string json = JsonUtility.ToJson(requestBody);
        byte[] body = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest www = new UnityWebRequest(reportApiUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(body);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Report sent successfully: " + www.downloadHandler.text);

                // Deserialize only the needed part
                ReportResponseBackend response = JsonUtility.FromJson<ReportResponseBackend>(www.downloadHandler.text);

                if (reportCodeText != null && response != null)
                {
                    reportCodeText.text = $"Report Code: {response.generatedAt}";
                }

                QADataManager.Instance?.ClearQAs();
            }
            else
            {
                Debug.LogError("❌ Failed to send report: " + www.error);
                Debug.LogError("Response: " + www.downloadHandler.text);
            }
        }
    }
}

[System.Serializable]
public class QAPairBackend
{
    public string question;
    public string userAnswer;
}

[System.Serializable]
public class ReportRequestBackend
{
    public string title;
    public QAPairBackend[] qaPairs;
}

[System.Serializable]
public class ReportResponseBackend
{
    public string generatedAt; // 👈 only parse this field
}
