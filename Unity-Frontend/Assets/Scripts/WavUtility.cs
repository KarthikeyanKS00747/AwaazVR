using System;
using System.IO;
using UnityEngine;

public static class WavUtility
{
    // Convert an AudioClip to a WAV byte array
    public static byte[] FromAudioClip(AudioClip clip)
    {
        using (var stream = new MemoryStream())
        using (var writer = new BinaryWriter(stream))
        {
            int sampleCount = clip.samples * clip.channels;
            float[] samples = new float[sampleCount];
            clip.GetData(samples, 0);

            int byteCount = sampleCount * 2; // 16-bit
            int headerSize = 44;

            // RIFF header
            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(headerSize + byteCount - 8);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

            // fmt subchunk
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)clip.channels);
            writer.Write(clip.frequency);
            writer.Write(clip.frequency * clip.channels * 2);
            writer.Write((short)(clip.channels * 2));
            writer.Write((short)16);

            // data subchunk
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(byteCount);

            // write samples
            foreach (float s in samples)
            {
                short val = (short)(Mathf.Clamp(s, -1f, 1f) * short.MaxValue);
                writer.Write(val);
            }

            return stream.ToArray();
        }
    }
}
