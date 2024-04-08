using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine.Events;

using OpenAI;

public class ChatGPT : MonoBehaviour
{
    // Properties
    public enum ChatModels
    {
        GPT3_Turbo,
        GPT4
    }
    private string[] modelNames = new string[] {
        "gpt-3.5-turbo", 
        "gpt-4"
    };
    [SerializeField] private ChatModels model = ChatModels.GPT3_Turbo;

    [SerializeField] private float temperature = 0.5f;
    [SerializeField] private int maxReplyTokens = 200;

    // API
    private OpenAIApi openai = new();
    // request
    private CreateChatCompletionRequest request = new();
    private CancellationTokenSource token = new CancellationTokenSource();
    // conversation
    private List<ChatMessage> messages = new();
    public List<ChatMessage> GetConversation() { return messages; }

    // Unity event
    public UnityEvent<string> OnMessageReceived;

    // Initial prompt
    [SerializeField] private string initialPrompt = 
        "You should response as a patient living in a hospital." + 
        "Don't break your character. Don't tell anyone that you are an AI.";

    void Start() 
    {
        ResetChatGPT(initialPrompt);
    }

    void OnDestroy()
    {
        token.Cancel();
    }

    public void ResetChatGPT(string initialPrompt)
    {
        // Initialize the conversation
        var newMessage = new ChatMessage()
        {
            Role = "system",
            Content = initialPrompt
        };
        messages = new();
        messages.Add(newMessage);
    }

    public async void SendChatMessage(string text)
    {
        // Convert string text into ChatMessage with role "user"
        var newMessage = new ChatMessage()
        {
            Role = "user",
            Content = text
        };
        messages.Add(newMessage);

        // Complete the instruction
        // Send message and wait for response
        var completionResponse = await openai.CreateChatCompletion(
            new CreateChatCompletionRequest()
                {
                    Model = modelNames[(int) model],
                    Messages = messages,
                    Temperature = temperature,
                    MaxTokens = maxReplyTokens,
                }
        );
        
        // Add the response to the conversation
        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
        {
            var message = completionResponse.Choices[0].Message;
            message.Content = message.Content.Trim();
            
            messages.Add(message);

            // trigger 
            OnMessageReceived.Invoke(message.Content);
        }
        else
        {
            Debug.LogWarning("No text was generated from this prompt.");
        }
    }

    public void SendChatMessageAsync(string text)
    {
        // Convert string text into ChatMessage with role "user"
        var newMessage = new ChatMessage()
        {
            Role = "user",
            Content = text
        };
        messages.Add(newMessage);
        
        // Complete the instruction
        // Send message and use callback to handle response
        openai.CreateChatCompletionAsync(
            new CreateChatCompletionRequest()
            {
                Model = modelNames[(int) model],
                Messages = messages,
                Temperature = temperature,
                MaxTokens = maxReplyTokens,
                Stream = true
            }, HandleResponseAsync, null, token
        );
    }

    private void HandleResponseAsync(
        List<CreateChatCompletionResponse> responses
    )
    {
        string text = string.Join(
            "", responses.Select(r => r.Choices[0].Delta.Content)
        );

        // check if this is the first returned message
        if (messages[messages.Count - 1].Role != "assistant")
        {
            messages.Add(
                new ChatMessage()
                {
                    Role = "assistant",
                    Content = text
                }
            );
        }
        // else, update the last message with the new one
        messages[messages.Count - 1] = new ChatMessage()
        {
            Role = "assistant",
            Content = text
        };

        // trigger
        OnMessageReceived.Invoke(text);
    }
}
