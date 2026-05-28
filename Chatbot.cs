using System;
using System.Collections.Generic;
using CybersecurityChatbot.Utilities;

namespace CybersecurityChatbot
{
    /// <summary>
    /// ChatBot — central controller for the WPF GUI (Part 2).
    ///
    /// Message pipeline:
    ///   1. Name capture on first message
    ///   2. Add input to rolling history (MemoryStore)
    ///   3. Follow-up phrases ("tell me more")
    ///   4. Special commands (how are you / what can you do / bye)
    ///   5. Favourite-topic detection
    ///   6. Sentiment detection
    ///   7. Keyword matching with random response variant
    ///   8. Sentiment-only response (emotion with no keyword)
    ///   9. Random fallback
    /// </summary>
    public class ChatBot
    {
        // ── Dependencies ──────────────────────────────────────────────
        private readonly KeywordResponder _keywords;
        private readonly SentimentDetector _sentiment;
        private readonly MemoryStore _memory;
        private readonly Random _rng = new Random();

        // ── Conversation state ────────────────────────────────────────
        private bool _awaitingName = true;
        private string _lastTopic = string.Empty;

        // ── Varied fallback responses (random pool) ───────────────────
        private readonly List<string> _fallbacks = new List<string>
        {
            "I'm not sure I understood that. Try asking about passwords, phishing, malware, VPNs, or encryption!",
            "Hmm, that's outside my expertise. I specialise in cybersecurity — try safe browsing, data breaches, or 2FA.",
            "I didn't catch that. You can ask about privacy, ransomware, firewalls, or identity theft.",
            "Could you rephrase that? Try keywords like scam, spyware, backups, or zero-day.",
            "That one stumped me! Try asking about dark web safety, cyber hygiene, or social engineering."
        };

        // ── Phrases that trigger a follow-up on the last topic ────────
        private readonly List<string> _followUpPhrases = new List<string>
        {
            "tell me more", "more info", "explain more", "go on", "continue",
            "expand on that", "elaborate", "more details", "what else", "and?",
            "keep going", "dig deeper", "say more"
        };

        // ── Tips rotated after keyword responses ──────────────────────
        private readonly List<string> _tips = new List<string>
        {
            "💡 Type 'tell me more' to go deeper on this topic.",
            "💡 Ask 'what can you do' to see every topic I cover.",
            "💡 I remember our conversation — feel free to reference earlier topics!",
            "💡 Try asking about a related topic like ransomware or 2FA."
        };

        // ─────────────────────────────────────────────────────────────
        // CONSTRUCTOR
        // ─────────────────────────────────────────────────────────────
        public ChatBot()
        {
            _keywords = new KeywordResponder();
            _sentiment = new SentimentDetector();
            _memory = new MemoryStore();
        }

        // ─────────────────────────────────────────────────────────────
        // PUBLIC API
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the bot's opening message shown before the user types.
        /// </summary>
        public string GetGreeting()
            => "👋 Hello! I'm CyberGuard, your personal cybersecurity assistant.\n\n" +
               "Before we dive in — what's your name?";

        /// <summary>
        /// Returns the full conversation history list so MainWindow can
        /// display it in the History overlay panel.
        /// Satisfies the Memory and Recall rubric criterion.
        /// </summary>
        public List<string> GetHistory() => _memory.GetHistory();

        /// <summary>
        /// Processes one user message through the full pipeline and
        /// returns the bot's reply string.
        /// </summary>
        public string ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Please type a message — I'm here to help! 😊";

            string trimmed = input.Trim();

            // ── 1. Capture name on very first message ─────────────────
            if (_awaitingName)
            {
                _memory.UserName = trimmed;
                _memory.Store("name", trimmed);
                _awaitingName = false;
                return $"Great to meet you, {_memory.UserName}! 🛡️\n\n" +
                       "I can help you with cybersecurity topics including:\n" +
                       "passwords, phishing, malware, VPNs, encryption, ransomware,\n" +
                       "privacy, 2FA, scams, firewalls, backups, and much more.\n\n" +
                       "Type any topic or question to get started, or ask\n" +
                       "'what can you do' to see the full list.";
            }

            // ── 2. Add to rolling history ─────────────────────────────
            _memory.AddToHistory(trimmed);
            string lower = trimmed.ToLowerInvariant();

            // ── 3. Follow-up handling ─────────────────────────────────
            foreach (var phrase in _followUpPhrases)
            {
                if (lower.Contains(phrase))
                {
                    if (!string.IsNullOrEmpty(_lastTopic))
                    {
                        string followUp = _keywords.GetResponse(_lastTopic);
                        return $"{_memory.GetPersonalisedOpener()}" +
                               $"Here's more about {_lastTopic}:\n\n{followUp}";
                    }
                    return $"Sure! What topic would you like me to expand on, {_memory.UserName}?";
                }
            }

            // ── 4. Special commands ───────────────────────────────────

            if (lower.Contains("how are you"))
                return $"I'm running at full capacity, {_memory.UserName}! 💻 " +
                       "What cybersecurity topic can I help with today?";

            // Exact "help" only — avoids swallowing "help me, I'm worried"
            if (lower == "help" || lower.Contains("what can you do") ||
                lower.Contains("your purpose") || lower.Contains("what topics"))
            {
                return $"I'm your cybersecurity awareness guide, {_memory.UserName}! 🛡️\n\n" +
                       "Here are the topics I can help with:\n\n• " +
                       string.Join("\n• ", _keywords.GetAllKeywords()) +
                       "\n\nJust type any keyword and I'll give you expert advice!";
            }

            if (lower.Contains("bye") || lower.Contains("goodbye") || lower.Contains("exit"))
                return $"Stay safe online, {_memory.UserName}! 🔒\n\n" +
                       "Remember: strong passwords + 2FA + cautious browsing = your best defence.\n\n" +
                       (string.IsNullOrWhiteSpace(_lastTopic)
                           ? "Goodbye!"
                           : $"Keep exploring {_lastTopic} — knowledge is your best shield. Goodbye!");

            // ── 5. Detect and store favourite topic ───────────────────
            if (lower.Contains("i am interested in") || lower.Contains("i'm interested in") ||
                lower.Contains("i like"))
            {
                foreach (var kw in _keywords.GetAllKeywords())
                {
                    if (lower.Contains(kw.ToLowerInvariant()))
                    {
                        _memory.FavouriteTopic = kw;
                        _memory.Store("favouriteTopic", kw);
                        break;
                    }
                }
            }

            // ── 6. Sentiment detection ────────────────────────────────
            var detectedSentiment = _sentiment.Detect(lower);
            string sentimentOpener = _sentiment.GetSentimentResponse(detectedSentiment);

            // ── 7. Keyword matching → random response variant ─────────
            string keywordResponse = _keywords.GetResponse(lower);
            string matchedKeyword = _keywords.GetMatchedKeyword(lower);

            if (!string.IsNullOrEmpty(keywordResponse))
            {
                _lastTopic = matchedKeyword;
                _memory.TrackTopic(matchedKeyword);

                string personalOpener = _memory.GetPersonalisedOpener();
                string recallNote = _memory.GetRecallStatement(matchedKeyword);
                string tip = _tips[_rng.Next(_tips.Count)];

                return $"{sentimentOpener}{personalOpener}{keywordResponse}{recallNote}\n\n{tip}";
            }

            // ── 8. Sentiment-only response ────────────────────────────
            // Handles pure emotion messages like "I am terrified" or
            // "I'm frustrated" where no cyber keyword was found.
            if (detectedSentiment != Sentiment.Neutral)
            {
                return detectedSentiment switch
                {
                    Sentiment.Worried =>
                        $"{sentimentOpener}{_memory.UserName}, it sounds like you're worried. " +
                        "Tell me what's going on and I'll help. 💙\n\n" +
                        "You can ask about phishing, malware, scams, account security, or any cyber topic.",

                    Sentiment.Frustrated =>
                        $"{sentimentOpener}{_memory.UserName}, let's slow right down.\n\n" +
                        "Tell me which part is confusing — passwords? encryption? phishing? — " +
                        "and I'll explain it in plain English.",

                    Sentiment.Happy =>
                        $"Love the positive energy, {_memory.UserName}! 😄\n\n" +
                        "What cybersecurity topic would you like to explore next?",

                    Sentiment.Curious =>
                        $"Curiosity is the best starting point, {_memory.UserName}! 🔍\n\n" +
                        "What would you like to learn about?\n" +
                        "Try: passwords, phishing, VPNs, 2FA, malware, or encryption.",

                    Sentiment.Confident =>
                        $"Great mindset, {_memory.UserName}! 💪\n\n" +
                        "Want to go deeper? Ask about zero-day vulnerabilities, " +
                        "social engineering, or dark web safety.",

                    _ => _fallbacks[_rng.Next(_fallbacks.Count)]
                };
            }

            // ── 9. Fallback ───────────────────────────────────────────
            return _fallbacks[_rng.Next(_fallbacks.Count)];
        }
    }
}