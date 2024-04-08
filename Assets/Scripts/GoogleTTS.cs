using System;
using TMPro;
using UnityEngine;

using GoogleTextToSpeech.Scripts;
using GoogleTextToSpeech.Scripts.Data;
using GoogleTextToSpeech.Scripts.Example;


public class GoogleTTS : MonoBehaviour
{
    [SerializeField] private VoiceScriptableObject voice;
    [SerializeField] private TextToSpeech textToSpeech;
    [SerializeField] private AudioSource audioSource;

    private Action<AudioClip> _audioClipReceived;
    private Action<BadRequestData> _errorReceived;

    void Start()
    {
        _errorReceived += ErrorReceived;
        _audioClipReceived += AudioClipReceived;
    }

    public void TextToSpeech(string message)
    {
        textToSpeech.GetSpeechAudioFromGoogle(
            message, voice, _audioClipReceived, _errorReceived
        );
        
    }

    private void ErrorReceived(BadRequestData badRequestData)
    {
        Debug.Log($"Error {badRequestData.error.code} : {badRequestData.error.message}");
    }

    private void AudioClipReceived(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
