# Unity Bi-directional Verbal Communication with AI

This is a Unity project that achieve bi-directional verbal communication with AI agents. Specifically, we use OpenAI whisper to convert human voice to speech, use OpenAI ChatGPT to produce response and Google text-to-speech cloud service to synthesize voice.

## Dependency

Dependencies are already installed in the project. 

We use OpenAI API provided by [OpenAI-Unity](https://github.com/srcnalt/OpenAI-Unity) and Google Text-to-speech API provided by [Text-to-Speech-using-Google-Cloud](https://github.com/anomalisfree/Unity-Text-to-Speech-using-Google-Cloud).

To use these two functions, you will need to provide your own API key in two places:

- For OpenAI API, follow the instructions in [OpenAI-Unity](https://github.com/srcnalt/OpenAI-Unity) to set it up.
- For Google API, you need to put it in the **GameObject** in the scene that has **TextToSpeech.cs** attached. **IMPORTANT:** Your API key is a secret. Do not push the **scene** with your API to GitHub.

## Run

This repository is tested in Unity 2020+, but other Unity version should work as well.



Open the **Sample** scene

- Set up API keys for OpenAI and Google (optional)
- Set up the agent behavior by changing the **Initial Prompt** of **Chat GPT** component under **ChatGPT** game object. Set up other parameters as you need.
- Set up the voice synthesis behavior under **TTS Test** game object.



Play the **Sample** scene

- Press "spacebar" to speak or type in the dialog what you want to say
- Send your dialog and wait for the response
- Continue conversation