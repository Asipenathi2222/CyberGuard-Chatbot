using System;
using System.Collections.Generic;

// ResponseHandler is kept for Part 1 console compatibility.
// The WPF Part 2 GUI uses ChatBot.cs / KeywordResponder.cs instead.
//Add extra phishing response
namespace CybersecurityChatbot.Utilities
{
    public class ResponseHandler
    {
        private Dictionary<string, string> responses;

        public ResponseHandler()
        {
            InitializeResponses();
        }

        public string GetResponse(string userInput, string name)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return "Please provide a valid input.";

            string normalized = userInput.Trim().ToLowerInvariant();

            if (responses.TryGetValue(normalized, out var dictResponse))
                return dictResponse.Replace("{name}", name);

            string bestMatch = null;
            foreach (var key in responses.Keys)
            {
                if (normalized.Contains(key.ToLowerInvariant()))
                {
                    if (bestMatch == null || key.Length > bestMatch.Length)
                        bestMatch = key;
                }
            }

            if (bestMatch != null)
                return responses[bestMatch].Replace("{name}", name);

            if (userInput.Contains("hello", StringComparison.OrdinalIgnoreCase))
                return $"Hello, {name}! How can I assist you today?";
            if (userInput.Contains("help", StringComparison.OrdinalIgnoreCase))
                return "I am here to help you. What do you need assistance with?";

            return "I'm sorry, I didn't understand that. Can you please rephrase?";
        }

        private void InitializeResponses()
        {
            responses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // ===== GREETINGS =====
                { "how are you",  "I'm doing great, thank you for asking! Ready to help with cybersecurity questions." },
                { "hi",           "Hello! Welcome to the Cybersecurity Awareness Bot. How can I assist you today?" },
                { "hello",        "Hi there! I'm here to help you stay safe online. What would you like to know?" },
                { "hey",          "Hello! I'm your Cybersecurity Awareness Bot. Ask me anything about staying safe online!" },

                // ===== PURPOSE =====
                { "what's your purpose",    "My purpose is to educate and guide you on cybersecurity best practices." },
                { "what is your purpose",   "I'm here to help you understand and practise good cybersecurity hygiene." },
                { "what can you help with", "I can help with password protection, phishing awareness, safe browsing, malware, VPNs, and more." },
                { "your purpose",           "My purpose is to educate and guide you on cybersecurity best practices." },

                // ===== PASSWORDS =====
                { "password",          "Use at least 12 characters with uppercase, lowercase, numbers, and symbols. Never reuse passwords across sites." },
                { "password safety",   "Create strong unique passwords (12+ chars), use a password manager, enable 2FA, never share passwords." },
                { "strong password",   "A strong password: 12+ characters, mixed case, numbers, special chars. Example: Tr0pic@lSunset#2024" },
                { "create password",   "Use 12+ chars with mixed case, numbers and special characters. Never use your name or birthdate." },
                { "secure password",   "12+ characters, mixed case, numbers, special characters, unique per site. Use a password manager." },

                // ===== PHISHING =====
                { "phishing",              "Phishing tricks you into revealing data. Be cautious of suspicious emails, unknown links, and urgent language." },
                { "identify phishing",     "Red flags: strange sender, urgent tone, requests for personal info, poor grammar, suspicious links." },
                { "phishing attacks",      "Never click links from unknown senders. Verify email addresses and check for spelling errors." },
                { "phishing email",        "Phishing emails pretend to be from trusted sources. Verify by contacting the company directly." },
                { "suspicious email",      "Don't click links or download attachments. Check the sender address and contact the org directly." },
                { "email security",        "Don't click unexpected links, verify senders, be wary of attachments, use 2FA." },

                // ===== SAFE BROWSING =====
                { "safe browsing",   "Check for HTTPS, be cautious on public WiFi, keep browser updated, use antivirus." },
                { "https",           "HTTPS encrypts your connection. Look for the padlock icon before entering sensitive information." },
                { "public wifi",     "Public WiFi is risky! Use a VPN, avoid sensitive accounts, disable auto-connect." },
                { "browse safely",   "Use HTTPS, keep browser updated, avoid public WiFi for sensitive data, use antivirus." },

                // ===== 2FA =====
                { "two-factor authentication", "2FA adds a second security layer — even with your password stolen, attackers can't log in." },
                { "2fa",            "2FA requires two forms of verification. Use an authenticator app rather than SMS codes." },
                { "authentication", "Use strong passwords, enable 2FA, and never share your login credentials." },

                // ===== ANTIVIRUS & MALWARE =====
                { "antivirus",         "Keep antivirus updated, run regular scans. Windows Defender is a solid baseline." },
                { "virus",             "A virus damages your system. Use updated antivirus and avoid suspicious downloads." },
                { "malware",           "Malware steals data or damages systems. Use antivirus, avoid untrusted downloads, keep systems patched." },
                { "malicious software","Malware includes viruses, spyware, and ransomware. Use antivirus and update regularly." },

                // ===== PRIVACY & DATA =====
                { "personal information", "Share only what's necessary, check privacy settings, use secure passwords, beware of phishing." },
                { "privacy",              "Use strong passwords, enable 2FA, check social media privacy settings, verify websites." },
                { "data protection",      "Use encryption, strong passwords, 2FA, regular backups, and antivirus." },
                { "social media safety",  "Use strong passwords, enable 2FA, check privacy settings, avoid oversharing." },

                // ===== GENERAL =====
                { "cybersecurity", "Cybersecurity protects your data and devices. Key: strong passwords, 2FA, safe browsing, antivirus." }
            };
        }
    }
}


