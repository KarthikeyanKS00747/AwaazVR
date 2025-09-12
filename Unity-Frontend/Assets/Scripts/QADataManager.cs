using System.Collections.Generic;
using UnityEngine;

public class QADataManager : MonoBehaviour
{
    public static QADataManager Instance;

    [System.Serializable]
    public class QAPair
    {
        public string question;
        public string answer;
    }

    public List<QAPair> QAPairs = new List<QAPair>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [System.Serializable]
    private class WhisperResult
    {
        public string text;
    }

    public void AddQA(string question, string whisperJsonResponse)
    {
        string cleanAnswer;

        try
        {
            WhisperResult whisper = JsonUtility.FromJson<WhisperResult>(whisperJsonResponse);
            cleanAnswer = whisper != null && !string.IsNullOrEmpty(whisper.text)
                ? whisper.text
                : "[No transcription text found]";
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Failed to parse Whisper JSON: " + e.Message);
            cleanAnswer = "[Parsing error]";
        }

        QAPairs.Add(new QAPair { question = question, answer = cleanAnswer });
    }

    public void ClearQAs()
    {
        QAPairs.Clear();
    }
}
