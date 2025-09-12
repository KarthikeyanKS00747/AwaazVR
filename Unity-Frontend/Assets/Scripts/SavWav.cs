using System;
using System.IO;
using UnityEngine;

public static class SavWav
{
    public static byte[] GetWavBytes(AudioClip clip)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            int frequency = clip.frequency;
            int channels = clip.channels;
            float[] samples = new float[clip.samples * channels];
            clip.GetData(samples, 0);

            byte[] wav = ConvertToWav(samples, frequency, channels);
            stream.Write(wav, 0, wav.Length);
            return stream.ToArray();
        }
    }

    private static byte[] ConvertToWav(float[] samples, int sampleRate, int channels)
    {
        int byteCount = samples.Length * 2;
        using (MemoryStream stream = new MemoryStream(44 + byteCount))
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            // Header
            writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
            writer.Write(36 + byteCount);
            writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));
            writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)channels);
            writer.Write(sampleRate);
            writer.Write(sampleRate * channels * 2);
            writer.Write((short)(channels * 2));
            writer.Write((short)16);
            writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
            writer.Write(byteCount);

            // Data
            foreach (float sample in samples)
            {
                short intData = (short)(sample * short.MaxValue);
                writer.Write(intData);
            }
            return stream.ToArray();
        }
    }
}
