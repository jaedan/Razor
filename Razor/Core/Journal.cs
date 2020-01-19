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
        private static List<JournalEntry> _entries = new List<JournalEntry>();

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
                _entries.RemoveAt(_entries.Count - 1);
        }

        public static void AddLine(JournalEntry entry)
        {
            _entries.Insert(0, entry);
        }

        public static void Clear()
        {
            _entries.Clear();
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
