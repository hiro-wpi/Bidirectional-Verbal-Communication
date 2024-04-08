using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OpenAI;

public class Chat : MonoBehaviour
{
    [SerializeField] private ChatGPT chatGPT;
    [SerializeField] private Whisper whisper;
    [SerializeField] private List<string> conversation = new();
    [SerializeField] string userMessage = "";
    [SerializeField] bool streamResponse = true;

    void Start() 
    {
        // Subscribe to the OnMessageReceived event
        chatGPT.OnMessageReceived.AddListener(OnMessageReceived);
        whisper.OnTranscriptReceived.AddListener(OnTranscriptReceived);
    }

    void Update()
    {
        // check space pressed
        if (Input.GetKeyDown(KeyCode.Return) && userMessage != "")
        {
            // send message
            conversation.Add(userMessage);
            if (streamResponse)
            {
                chatGPT.SendChatMessageAsync(userMessage);
            }
            else
            {
                chatGPT.SendChatMessage(userMessage);
            }

            userMessage = "";
        }
    }

    private void OnMessageReceived(string message)
    {
        // Add the message to the conversation
        // new message received
        if (conversation.Count == chatGPT.GetConversation().Count - 2)
        {
            conversation.Add(message);
        }
        // streaming
        else
        {
            conversation[conversation.Count-1] = message;
        }
    }

    private void OnTranscriptReceived(string transcript)
    {
        // Add the message to the conversation
        userMessage = transcript;
    }
}
