using UnityEngine;
using System.Collections;

public class ReadOculusMicrophone : MonoBehaviour
{
    public AudioSource audioSourceToPlay;
    private AudioClip microphoneClip;
    private string selectedMicrophone;

    void Start()
    {
        // Check if any microphones are connected
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphones detected!");
            return;
        }

        // Select the first microphone (usually the Quest 3 microphone)
        selectedMicrophone = Microphone.devices[0];
        Debug.Log($"Selected Microphone: {selectedMicrophone}");

        // Ensure an AudioSource is assigned
        if (audioSourceToPlay == null)
        {
            Debug.LogError("Please assign an AudioSource in the Inspector!");
            return;
        }

        // Start capturing microphone input
        StartMicrophoneCapture();
    }

    void StartMicrophoneCapture()
    {
        // Parameters: 
        // - deviceName: The name of the microphone
        // - loop: Whether to loop the recording
        // - recordingTime: How long to record (10 seconds in this example)
        // - frequency: Sample rate (44100 is standard)
        microphoneClip = Microphone.Start(selectedMicrophone, true, 10, 44100);

        // Wait until the microphone starts recording
        StartCoroutine(WaitForMicrophoneStart());
    }

    IEnumerator WaitForMicrophoneStart()
    {
        // Wait until the microphone position is valid
        while (!(Microphone.GetPosition(selectedMicrophone) > 0))
        {
            yield return null;
        }

        // Assign the microphone clip to the AudioSource
        audioSourceToPlay.clip = microphoneClip;

        // Play the audio immediately
        audioSourceToPlay.Play();
    }

    void StopMicrophoneCapture()
    {
        if (selectedMicrophone != null)
        {
            Microphone.End(selectedMicrophone);
        }
    }

    void OnDisable()
    {
        // Always clean up the microphone when the script is disabled
        if (selectedMicrophone != null)
        {
            Microphone.End(selectedMicrophone);
        }
    }

    // Optional method to manually trigger recording
    public void StartRecording()
    {
        StartMicrophoneCapture();
    }

    // Optional method to get current microphone input volume
    public float GetMicrophoneVolume()
    {
        if (microphoneClip == null) return 0f;

        float[] samples = new float[microphoneClip.samples];
        microphoneClip.GetData(samples, 0);

        float totalVolume = 0f;
        for (int i = 0; i < samples.Length; i++)
        {
            totalVolume += Mathf.Abs(samples[i]);
        }
        return totalVolume / samples.Length;
    }
}
