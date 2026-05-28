using System;

// User model shared by both Part 1 (ChatbotManager) and Part 2 (MemoryStore/ChatBot).
namespace CybersecurityChatbot.Models
{
    public class User
    {
        public string Name { get; set; } = string.Empty;
        public string CreatedAt { get; set; }

        public User()
        {
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}