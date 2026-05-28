using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot
{
    /// <summary>
    /// Sentiment enum representing the emotional tone detected in user input.
    /// </summary>
    //Improve sentiment detection triggers
    public enum Sentiment
    {
        Neutral,
        Worried,
        Curious,
        Frustrated,
        Happy,
        Confident
    }

    /// <summary>
    /// SentimentDetector — analyses user messages for emotional tone and
    /// returns a context-appropriate opening phrase to prepend to responses.
    ///
    /// Satisfies the Sentiment Detection rubric criterion by adapting
    /// bot responses dynamically based on the user's mood/tone.
    /// </summary>
    public class SentimentDetector
    {
        // ── Keyword triggers per sentiment ───────────────────────────────
        private readonly Dictionary<Sentiment, List<string>> _triggers = new()
        {
            [Sentiment.Worried] = new List<string>
            {
                "worried", "scared", "afraid", "anxious", "nervous", "unsafe",
                "terrified", "fear", "frightened", "concern", "concerned", "panic",
                "help me", "hacked", "breach", "compromised", "attacked", "stolen"
            },
            [Sentiment.Curious] = new List<string>
            {
                "curious", "wondering", "interested", "want to know", "how does",
                "what is", "explain", "tell me about", "how do", "what are",
                "how can", "why is", "what happens", "can you explain", "i want to learn"
            },
            [Sentiment.Frustrated] = new List<string>
            {
                "frustrated", "annoyed", "confused", "don't understand", "dont understand",
                "irritated", "angry", "fed up", "sick of", "this is hard", "difficult",
                "complicated", "why is this so", "i give up", "too complex"
            },
            [Sentiment.Happy] = new List<string>
            {
                "great", "thanks", "helpful", "awesome", "love it", "thank you",
                "amazing", "excellent", "perfect", "brilliant", "good job", "well done",
                "fantastic", "nice", "wonderful", "appreciate", "grateful"
            },
            [Sentiment.Confident] = new List<string>
            {
                "i know", "i understand", "i already", "i use", "i have", "i do",
                "i always", "i practice", "i'm aware", "i follow"
            }
        };

        // ── Response openers per sentiment ───────────────────────────────
        private readonly Dictionary<Sentiment, List<string>> _responses = new()
        {
            [Sentiment.Worried] = new List<string>
            {
                "I understand your concern — let me give you some reassuring and practical advice: ",
                "It's completely understandable to feel worried. Here's what you can do right now: ",
                "Cybersecurity threats can feel overwhelming, but you're in the right place. Here's the key information: "
            },
            [Sentiment.Curious] = new List<string>
            {
                "Great question! Here's what you need to know: ",
                "I love your curiosity about cybersecurity! Here's a thorough explanation: ",
                "Excellent — let me break this down clearly for you: "
            },
            [Sentiment.Frustrated] = new List<string>
            {
                "I hear you — this can be confusing at first. Let me explain it as simply as possible: ",
                "No worries, cybersecurity has a lot of jargon. Here's a plain-English explanation: ",
                "Let's slow down and make this clear. Here's a straightforward breakdown: "
            },
            [Sentiment.Happy] = new List<string>
            {
                "Glad to hear that! Here's some more useful information: ",
                "You're welcome! Let me share another helpful tip: ",
                "Great attitude! Here's what else you should know: "
            },
            [Sentiment.Confident] = new List<string>
            {
                "That's great that you're already on top of this! Here's some extra detail to deepen your knowledge: ",
                "Good to hear! Here's some advanced context to add to what you know: "
            },
            [Sentiment.Neutral] = new List<string> { "" }
        };

        private readonly Random _rng = new Random();

        // ─────────────────────────────────────────────────────────────────
        // PUBLIC API
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Analyses <paramref name="input"/> and returns the dominant sentiment.
        /// Returns <see cref="Sentiment.Neutral"/> when no trigger words match.
        /// </summary>
        public Sentiment Detect(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return Sentiment.Neutral;
            string lower = input.ToLowerInvariant();

            foreach (var kvp in _triggers)
            {
                foreach (var word in kvp.Value)
                {
                    if (lower.Contains(word))
                        return kvp.Key;
                }
            }
            return Sentiment.Neutral;
        }

        /// <summary>
        /// Returns a randomly selected response opener appropriate for the
        /// given sentiment. Returns empty string for Neutral sentiment.
        /// </summary>
        public string GetSentimentResponse(Sentiment sentiment)
        {
            if (_responses.TryGetValue(sentiment, out var options) && options.Count > 0)
                return options[_rng.Next(options.Count)];
            return string.Empty;
        }
    }
}
