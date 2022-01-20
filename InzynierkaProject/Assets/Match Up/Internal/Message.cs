using UnityEngine;

namespace MatchUp
{
    /// <summary>A Message is made up of a Command and an optional string payload</summary>
    public class Message
    {
        public Command command;
        public string payload;

        public Message(Command command, string payload)
        {
            this.command = command;
            this.payload = payload;
        }

        /// <summary>Parse a quoted bit of text, skipping escaped quotes.</summary>
        /// <remarks>
        /// Returns the quoted string with the outer quotes removed.
        /// Sets endPos equal to the index of the closing quote.
        /// </remarks>
        public static string ParseQuoted(string text, int startPos, ref int endPos)
        {
            startPos++; // For starting quote
            int lastQuotePos = startPos;
            while (true)
            {
                endPos = text.IndexOf('"', lastQuotePos);
                lastQuotePos = endPos + 1;
                if (endPos == -1)
                {
                    Debug.LogError(Matchmaker.TAG + "Failed parsing match data string. Found open quote with no closing quote: " + text);
                    return "";
                }
                if (text[endPos - 1] != '\\')
                {
                    // Found the non-escaped closing quote
                    break;
                }
            }
            string parsed = text.Substring(startPos, (endPos - startPos));
            parsed = parsed.Replace("\\\"", "\""); // Replace escaped quotes with quotes
            endPos = endPos + 1; // For ending quote
            return parsed;
        }

    }
}
