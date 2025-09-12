using UnityEngine;

public class AudioRecorder : MonoBehaviour
{
    private AudioClip recordedClip;
    private string microphoneDevice;

    public void StartRecording()
    {
        if (Microphone.devices.Length > 0)
        {
            microphoneDevice = Microphone.devices[0]; // use first mic
            recordedClip = Microphone.Start(microphoneDevice, false, 10, 44100);
            Debug.Log("Recording started...");
        }
        else
        {
            Debug.LogError("No microphone found!");
        }
    }

    public AudioClip StopRecording()
    {
        if (microphoneDevice != null)
        {
            Microphone.End(microphoneDevice);
            Debug.Log("Recording stopped.");
            return recordedClip;
        }
        return null;
    }
}
