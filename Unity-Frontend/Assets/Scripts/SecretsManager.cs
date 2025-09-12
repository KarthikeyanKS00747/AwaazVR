using UnityEngine;
using System.IO;

[System.Serializable]
public class SecretKeys
{
    public string elevenLabsApiKey;
}

public class SecretsManager : MonoBehaviour
{
    public static SecretsManager Instance;
    public SecretKeys keys;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSecrets();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSecrets()
    {
        string path = Path.Combine(Application.dataPath, "Config/keys.secret.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            keys = JsonUtility.FromJson<SecretKeys>(json);
            Debug.Log("✅ Secrets loaded");
        }
        else
        {
            Debug.LogError("❌ keys.secret.json not found!");
        }
    }
}
