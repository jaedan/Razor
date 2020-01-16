using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant
{
    public static class Journal
    {
        private static string _content = "";

        public static void AddLine(string text)
        {
            _content += text;
        }

        public static void Clear()
        {
            _content = "";
        }

        public static bool Contains(string text)
        {
            return _content.Contains(text);
        }
    }
}
