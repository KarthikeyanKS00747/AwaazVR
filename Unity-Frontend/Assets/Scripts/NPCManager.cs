using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [Header("Session Control")]
    public bool isSessionActive = false;

    private string[] questions;
    private int currentIndex = 0;

    [Header("References")]
    public NPCController npcController;

    public void LoadQuestions(string[] qns) => questions = qns;

    public void StartSession()
    {
        if (questions == null || questions.Length == 0)
        {
            Debug.LogWarning("⚠️ No questions loaded.");
            return;
        }

        if (npcController == null)
        {
            npcController = FindObjectOfType<NPCController>();
            if (npcController == null)
            {
                Debug.LogError("❌ NPCController not assigned or found!");
                return;
            }
        }

        isSessionActive = true;
        currentIndex = 0;
        QADataManager.Instance?.ClearQAs();

        TriggerNextQuestion();
    }

    public void StopSession()
    {
        isSessionActive = false;
        Debug.Log("⏹ Session stopped. Q&A session ended.");
    }

    public void StartQuestions(string[] qns)
    {
        LoadQuestions(qns);
        StartSession();
    }

    public void TriggerNextQuestion()
    {
        if (!isSessionActive || currentIndex >= questions.Length)
        {
            StopSession();
            return;
        }

        string question = questions[currentIndex];
        currentIndex++;

        Debug.Log($"🤖 Triggering NPC question: {question}");
        npcController.TriggerQuestion(question);
    }
}
