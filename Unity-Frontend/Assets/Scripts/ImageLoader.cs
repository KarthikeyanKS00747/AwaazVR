using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ImageLoader : MonoBehaviour
{
    public IEnumerator LoadImages(string[] urls, ProgramManager manager)
    {
        manager.slideSprites = new Sprite[urls.Length];

        for (int i = 0; i < urls.Length; i++)
        {
            string url = urls[i];
            if (string.IsNullOrEmpty(url)) continue;

            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Texture2D tex = DownloadHandlerTexture.GetContent(www);
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    manager.slideSprites[i] = sprite;
                    Debug.Log($"✅ Loaded slide {i + 1}");
                }
                else
                {
                    Debug.LogError($"❌ Failed to load slide {i + 1}: {www.error}");
                }
            }
        }

        // Automatically show first slide after all sprites loaded
        PresentationManager pm = FindObjectOfType<PresentationManager>();
        if (pm != null)
        {
            pm.ShowSlide(0);
        }

        Debug.Log("🎉 All slides loaded!");
    }
}
