using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using OpenAI;

public class Whisper : MonoBehaviour
{
    // Properties
    public enum WhisperModels
    {
        Whisper1
    }
    private string[] modelNames = new string[] {
        "whisper-1"
    };
    [SerializeField] private WhisperModels model = WhisperModels.Whisper1;

    public enum Languages
    {
        Auto,
        English,
        Chinese,
        Russian,
        Italian
    }
    private string[] languageCodes = new string[] {
        "",
        "en",
        "zh",
        "ru",
        "it"
    };
    [SerializeField] private Languages language = Languages.English;

    [SerializeField] private int maxSpeechTime = 10;  // s

    // API
    private OpenAIApi openai = new();
    // request
    [field: SerializeField]
    public bool IsRecording { get; private set; }
    private string[] microphones;
    private readonly string fileName = "whisper.wav";
    private AudioClip clip;
    // response
    private string transcript = "";

    // Unity event
    public UnityEvent<string> OnTranscriptReceived;

    void Start()
    {
        microphones = Microphone.devices;
    }

    void Update() {}

    void OnDestroy()
    {
        Microphone.End(null);
    }

    public void StartRecording()
    {
        IsRecording = true;
        clip = Microphone.Start(
            microphones[0], false, maxSpeechTime, 44100
        );
    }

    public async void EndRecording()
    {
        if (!IsRecording)
        {
            return;
        }
        IsRecording = false;
        Microphone.End(null);

        // Save to local audio file
        byte[] data = SaveWav.Save(fileName, clip);
        // Transciprt audio file
        var req = new CreateAudioTranscriptionsRequest
        {
            FileData = new FileData() {Data = data, Name = fileName},
            Model = modelNames[(int) model]
        };
        if (language != Languages.Auto)
        {
            req.Language = languageCodes[(int) language];
        }

        // Get the response
        var res = await openai.CreateAudioTranscription(req);
        transcript = res.Text;
        // trigger
        OnTranscriptReceived.Invoke(transcript);
    }
}
