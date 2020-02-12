using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant
{
    public class JournalEntry
    {
        public string Value { get; }

        public MessageType Type { get; }

        public JournalEntry(string value, MessageType type)
        {
            Value = value;
            Type = type;
        }
    }
    public static class Journal
    {
        private static LinkedList<JournalEntry> _entries = new LinkedList<JournalEntry>();

        public static void AddLine(string text)
        {
            AddLine(text, MessageType.Regular);
        }

        public static void AddLine(string text, MessageType type)
        {
            string newText = text;

            switch (type)
            {
                case MessageType.System:
                    {
                        newText = $"System: {text}";
                        break;
                    }
                case MessageType.Label:
                    {
                        newText = $"You see: {text}";
                        break;
                    }
                default:
                    break;
            }
            
            AddLine(new JournalEntry(newText, type));

            if (_entries.Count > 50)
                _entries.RemoveLast();
        }

        public static void AddLine(JournalEntry entry)
        {
            _entries.AddFirst(entry);
        }

        public static void Clear()
        {
            _entries.Clear();
        }

        public static bool ContainsSafe(string text)
        {
            return _entries.Any(
                (JournalEntry entry) => 
                    entry.Value.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1 && 
                    (entry.Type == MessageType.System || 
                    entry.Type == MessageType.Label));
        }

        public static bool Contains(string text)
        {
            return _entries.Any((JournalEntry entry) => entry.Value.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1);
        }

        public static bool Contains(string text, MessageType type)
        {
            return _entries.Any((JournalEntry entry) => entry.Value.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1 && entry.Type == type);
        }

        public static string Text()
        {
            return _entries.Aggregate("", (string text, JournalEntry entry) => entry.Value + "\r\n" + text);
        }
    }
}
