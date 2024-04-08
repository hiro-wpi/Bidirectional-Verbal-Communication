using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using OpenAI;

public class ChatUI : MonoBehaviour
{
    // OpenAI
    [SerializeField] private ChatGPT chatGPT;
    [SerializeField] private Whisper whisper;
    // Google cloud
    [SerializeField] private GoogleTTS googleTTS;

    // UI
    [SerializeField] private InputField inputField;
    [SerializeField] private Button button;
    [SerializeField] private ScrollRect scroll;

    [SerializeField] private RectTransform sent;
    [SerializeField] private RectTransform received;

    private float height;

    void Start()
    {
        // Subscribe to the OnMessageReceived event
        chatGPT.OnMessageReceived.AddListener(OnMessageReceived);
        whisper.OnTranscriptReceived.AddListener(OnTranscriptReceived);

        // UI
        button.onClick.AddListener(SendReply);
    }

    void Update()
    {
        // When space is being pressed, record
        // when space is released, stop recording
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            whisper.StartRecording();
        }
        else if (Input.GetKeyUp(KeyCode.RightShift))
        {
            whisper.EndRecording();
        }

        // check enter pressed for sending
        if (Input.GetKeyDown(KeyCode.Return) && inputField.text != "")
        {
            SendReply();
        }
    }

    private void SendReply()
    {
        // Send message
        chatGPT.SendChatMessage(inputField.text);

        // UI
        AppendMessage(
            new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            }
        );
        button.enabled = false;
        inputField.text = "";
        // inputField.enabled = false;
    }

    private void OnMessageReceived(string message)
    {
        // Got message
        AppendMessage(
            new ChatMessage()
            {
                Role = "assistant",
                Content = message
            }
        );

        // Text to speech
        googleTTS.TextToSpeech(message);

        // UI
        button.enabled = true;
        // inputField.enabled = true;
    }

    private void OnTranscriptReceived(string transcript)
    {
        // Add the message to the conversation
        inputField.text += transcript;
    }

    private void AppendMessage(ChatMessage message)
    {
        scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

        var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
        item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
        item.anchoredPosition = new Vector2(0, -height);
        LayoutRebuilder.ForceRebuildLayoutImmediate(item);
        height += item.sizeDelta.y;
        scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        scroll.verticalNormalizedPosition = 0;
    }
}
