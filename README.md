# CyberGuard — Cybersecurity Awareness Chatbot

A WPF desktop chatbot that educates users on cybersecurity best practices.

## Features
- Polished dark-theme GUI with ASCII art banner and voice greeting
- 23 cybersecurity topics with multiple random responses per topic
- Sentiment detection — adapts responses to worried, curious, frustrated, happy, or confident users
- Memory and recall — remembers your name, favourite topic, and conversation history
- Conversation history panel (📋 VIEW HISTORY button)
- Smooth animated chat bubbles with timestamps

## Setup Instructions
1. Clone or download this repository
2. Open `CybersecurityChatbot.sln` in Visual Studio 2022
3. Ensure .NET 8.0 SDK is installed
4. Build the solution (`Ctrl+Shift+B`)
5. Run the project (`F5`)

## Usage Examples
- Type `password` to get password security advice
- Type `I am worried about phishing` — the bot detects your concern and responds empathetically
- Type `tell me more` to get a different response on the same topic
- Click any sidebar button for instant topic shortcuts
- Click `📋 VIEW HISTORY` to see your conversation history
- Type `what can you do` to see all 23 supported topics

## Project Structure
```
CybersecurityChatbot/
├── ChatBot.cs              # Main controller — full message pipeline
├── KeywordResponder.cs     # 23 topics × 4 random responses each
├── SentimentDetector.cs    # Detects 5 emotional states
├── MemoryStore.cs          # Stores name, history, topic visits
├── AudioPlayer.cs          # Async voice greeting (greeting.wav)
├── MainWindow.xaml         # WPF UI layout
├── MainWindow.xaml.cs      # UI logic and chat bubbles
└── Utilities/
    └── ResponseHandler.cs  # Part 1 console compatibility
```

## YouTube Presentation
[Add your YouTube link here]

## GitHub Repository
https://github.com/Asipenathi2222/CyberGuard-Chatbot.git
