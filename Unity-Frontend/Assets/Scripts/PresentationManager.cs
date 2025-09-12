using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text;
using TMPro;

public class PresentationManager : MonoBehaviour
{
    [Header("References")]
    public ProgramManager programManager;
    public NPCManager npcManager;
    public Image slideImage;

    [Header("UI")]
    public TextMeshProUGUI reportText; // 👈 Assign in Inspector

    [Header("Backend API")]
    public string reportApiUrl = "https://awaazbackend.onrender.com/api/generate-report";

    private int currentSlide = 0;

    public void ShowSlide(int index)
    {
        if (programManager == null || programManager.slideSprites == null || programManager.slideSprites.Length == 0)
        {
            Debug.LogWarning("⚠️ No slides available to show.");
            return;
        }

        if (index >= 0 && index < programManager.slideSprites.Length)
        {
            currentSlide = index;
            if (slideImage != null)
                slideImage.sprite = programManager.slideSprites[currentSlide];

            Debug.Log($"📑 Showing slide {currentSlide + 1}/{programManager.slideSprites.Length}");
        }
    }

    public void NextSlide() => ShowSlide((currentSlide + 1) % programManager.slideSprites.Length);
    public void PreviousSlide() => ShowSlide((currentSlide - 1 + programManager.slideSprites.Length) % programManager.slideSprites.Length);

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
                string response = www.downloadHandler.text;
                Debug.Log("✅ Report sent successfully: " + response);

                // 👉 Display report in TextMeshPro
                if (reportText != null)
                    reportText.text = $"<b>Generated Report:</b>\n\n{response}";

                QADataManager.Instance?.ClearQAs();
            }
            else
            {
                Debug.LogError("❌ Failed to send report: " + www.error);
                Debug.LogError("Response: " + www.downloadHandler.text);

                if (reportText != null)
                    reportText.text = $"<color=red>❌ Failed to generate report.</color>\n{www.downloadHandler.text}";
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
