using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MatchUp
{
    /// <summary>Extra data for a match. Holds one string, float, double, int, or long.</summary>
    /// <remarks>
    /// You can add as many MatchData entries as you want for match.
    /// MatchData can be set by the host when creating a match or any time afterwards.
    /// MatchData is received on clients when they GetMatchList (unless you pass in false for includeMatchData) and when they JoinMatch().
    /// MatchData can not be requested except by listing matches or joining a match.
    /// You can use MatchFilter to filter the results of GetMatchList() based on a Match's MatchData.
    /// For example, the server might create a match with MatchData eloScore = 100
    /// <verbatim>
    /// Clients could then use MatchFilter to search for matches with eloScore < 150 and > 50
    /// </verbatim>
    /// </remarks>
    public class MatchData
    {
        /// <summary>The type of the value stored by this MatchData</summary>
        public enum ValueType
        {
            STRING, INT, FLOAT
        }
        
        /// <summary>The type of the value stored by this MatchData</summary>
        public ValueType valueType;

        /// <summary>The value as a string</summary>
        /// <remarks>All values are actually stored as strings regardless of their type.</remarks>
        string value = "";

        /// <summary>Get the value as a string</summary>
        public string stringValue {
            get {
                return value;
            }
        }

        /// <summary>Get the value as an int </summary>
        public int intValue {
            get {
                if (valueType != ValueType.INT) throw new Exception("Attempting to get intValue from non-int NATMatchData");
                return (int)long.Parse(value);
            }
        }

        /// <summary>Get the value as an unsigned int </summary>
        public uint uintValue
        {
            get
            {
                if (valueType != ValueType.INT) throw new Exception("Attempting to get unsigned intValue from non-int NATMatchData");
                return (uint)long.Parse(value);
            }
        }

        /// <summary>Get the value as a long</summary>
        public long longValue {
            get {
                if (valueType != ValueType.INT) throw new Exception("Attempting to get longValue from non-int NATMatchData");
                return long.Parse(value);
            }
        }

        /// <summary>Get the value as an unsigned long</summary>
        public ulong ulongValue
        {
            get
            {
                if (valueType != ValueType.INT) throw new Exception("Attempting to get unsigned longValue from non-int NATMatchData");
                return (ulong)long.Parse(value);
            }
        }

        /// <summary>Get the value as a float</summary>
        public float floatValue {
            get {
                if (valueType != ValueType.FLOAT) throw new Exception("Attempting to get floatValue from non-float NATMatchData");
                return (float)double.Parse(value);
            }
        }

        /// <summary>Get the value as a double</summary>
        public double doubleValue {
            get {
                if (valueType != ValueType.FLOAT) throw new Exception("Attempting to get doubleValue from non-float NATMatchData");
                return double.Parse(value);
            }
        }

        /// <summary>
        /// Construct MatchData from part of a response from the matchmaking server.
        /// </summary>
        /// <remarks>
        /// Starts from startPos and parses the comma separated value type, key, and value.
        /// </remarks>
        /// <param name="text">The response from the matchmaking server that contains some MatchData to be parsed</param>
        /// <param name="startPos">The index in the string to start parsing</param>
        /// <param name="endPos">Where parsing stopped, continue parsing from here if there are more MatchData entries</param>
        /// <param name="key">The key associated with this MatchData. Generally used as the key in a dictionary when storing this MatchData</param>
        public MatchData(string text, int startPos, ref int endPos, out string key)
        {
            // Value type
            endPos = text.IndexOf(",", startPos);
            string typeString = text.Substring(startPos, (endPos - startPos));
            int typeID = int.Parse(typeString);
            ValueType type = (ValueType)typeID;
            startPos = endPos + 1;

            // Key
            endPos = text.IndexOf(",", startPos);
            key = text.Substring(startPos, (endPos - startPos));
            startPos = endPos + 1;

            // Value
            string valueText;
            if (type == ValueType.STRING)
            {
                valueText = Message.ParseQuoted(text, startPos, ref endPos);
                if (endPos == -1)
                {
                    throw new Exception("Error parsing match data. Misaligned quotes.");
                }
            }
            else
            {
                endPos = text.IndexOf("|", startPos);
                if (endPos == -1) endPos = text.Length;
                valueText = text.Substring(startPos, (endPos - startPos));
            }

            init(valueText, type);
        }

        /// <summary>
        /// Create a new instance of matchData with a specific ValueType.
        /// </summary>
        /// <param name="value">The value as a string</param>
        /// <param name="valueType">The ValueType to store the value as</param>
        public MatchData(string value, ValueType valueType)
        {
            init(value, valueType);
        }

        /// <summary>
        /// Create a new instance of matchData that stores a double
        /// </summary>
        /// <param name="value">The value</param>
        public MatchData(double value)
        {
            init(value.ToString(), ValueType.FLOAT);
        }

        /// <summary>
        /// Create a new instance of matchData that stores a float
        /// </summary>
        /// <param name="value">The value</param>
        public MatchData(float value)
        {
            init(value.ToString(), ValueType.FLOAT);
        }

        /// <summary>
        /// Create a new instance of matchData that stores an int
        /// </summary>
        /// <param name="value">The value</param>
        public MatchData(int value)
        {
            init(value.ToString(), ValueType.INT);
        }

        /// <summary>
        /// Create a new instance of matchData that stores an unsigned int
        /// </summary>
        /// <param name="value">The value</param>
        public MatchData(uint value)
        {
            init(((int)(value)).ToString(), ValueType.INT);
        }

        /// <summary>
        /// Create a new instance of matchData that stores a long
        /// </summary>
        /// <param name="value">The value</param>
        public MatchData(long value)
        {
            init(value.ToString(), ValueType.INT);
        }

        /// <summary>
        /// Create a new instance of matchData that stores a ulong
        /// </summary>
        /// <param name="value">The value</param>
        public MatchData(ulong value)
        {
            init(((long)value).ToString(), ValueType.INT);
        }

        /// <summary>
        /// Create a new instance of matchData that stores a string
        /// </summary>
        /// <param name="value">The value</param>
        public MatchData(string value)
        {
            init(value, ValueType.STRING);
        }

        /// <summary>
        /// Called by the various constructors to actually set stuff.
        /// </summary>
        /// <param name="value">The value as a string</param>
        /// <param name="valueType">The ValueType to store the value as</param>
        void init(string value, ValueType valueType = ValueType.STRING)
        {
            this.value = value;
            this.valueType = valueType;
        }

        /// <summary>
        /// Serialize the match data for sending to the matchmaking server.
        /// </summary>
        /// <returns>A string representing the MatchData</returns>
        public string Serialize(string key) 
        {
            if (value == null) value = "";
            string preparedValue = value;
            if (valueType == ValueType.STRING) preparedValue = '"' + value.Replace("\"", "\\\"") + '"';
            return (int)valueType + "," + key + "," + preparedValue;
        }

        /// <summary>
        /// Converts an entire Dictionary of MatchData into a string for sending to the matchmaking server.
        /// </summary>
        /// <param name="data">The dictionary of MatchData to serialize</param>
        /// <returns>A string represenation of all of the data.</returns>
        public static string SerializeDictionary(Dictionary<string, MatchData> data)
        {
            if (data == null || data.Count() == 0) return "";

            string[] dataAsStrings = new string[data.Count];
            int i = 0;
            foreach (KeyValuePair<string, MatchData> kv in data)
            {
                dataAsStrings[i] = kv.Value.Serialize(kv.Key);
                i++;
            }
            return string.Join("|", dataAsStrings);
        }

        /// <summary>
        /// Deserialize part of a response from the matchmaking server to get a Dictionary of MatchData entries.
        /// </summary>
        /// <param name="text">The response from the matchmaking server</param>
        /// <param name="startPos">Where to start parsing</param>
        /// <param name="endPos">Where parsing ended</param>
        /// <returns>A Dictionary of MatchData entries</returns>
        public static Dictionary<string, MatchData> DeserializeDictionary(string text, int startPos, ref int endPos)
        {
            var matchData = new Dictionary<string, MatchData>();
            // Number of match data entries
            endPos = text.IndexOf('|', startPos);
            
            if (endPos == -1) return matchData;

            int numDatas = int.Parse(text.Substring(startPos, (endPos - startPos)));
            startPos = endPos + 1;

            int count = 0;
            while (endPos != -1 && endPos < text.Length && count < numDatas)
            {
                string key;
                var data = new MatchData(text, startPos, ref endPos, out key);
                matchData[key] = data;
                startPos = endPos + 1;

                count++;
            }

            return matchData;
        }

        /// <summary>
        /// Implicitly converts an int to a MatchData object.
        /// </summary>
        /// <remarks>
        /// These operators make storing MatchData a little easier.
        /// So you can do things like:
        /// <code>var matchData = new Dictionary<string, MatchData>( { "eloScore", 200 } );</code>
        /// instead of:
        /// <code>var matchData = new Dictionary<string, MatchData>( { "eloScore", new MatchData(200) } );</code>
        /// </remarks>
        /// <param name="value"></param>
        public static implicit operator MatchData(int value)
        {
            return new MatchData(value);
        }

        /// <summary>
        /// Implicitly converts an unsigned int to a MatchData object.
        /// </summary>
        /// <remarks>
        /// These operators make storing MatchData a little easier.
        /// So you can do things like:
        /// <code>var matchData = new Dictionary<string, MatchData>( { "eloScore", 200 } );</code>
        /// instead of:
        /// <code>var matchData = new Dictionary<string, MatchData>( { "eloScore", new MatchData(200) } );</code>
        /// </remarks>
        /// <param name="value"></param>
        public static implicit operator MatchData(uint value)
        {
            return new MatchData(value);
        }

        /// <summary>
        /// Implicitly converts a long to a MatchData object.
        /// </summary>
        /// <remarks>
        /// These operators make storing MatchData a little easier.
        /// So you can do things like:
        /// <code>var matchData = new Dictionary<string, MatchData>( { "eloScore", 200 } );</code>
        /// instead of:
        /// <code>var matchData = new Dictionary<string, MatchData>( { "eloScore", new MatchData(200) } );</code>
        /// </remarks>
        /// <param name="value"></param>
        public static implicit operator MatchData(long value)
        {
            return new MatchData(value);
        }

        /// <summary>
        /// Implicitly converts an unsigned long to a MatchData object.
        /// </summary>
        /// <remarks>
        /// These operators make storing MatchData a little easier.
        /// So you can do things like:
        /// <code>var matchData = new Dictionary<string, MatchData>( { "eloScore", 200 } );</code>
        /// instead of:
        /// <code>var matchData = new Dictionary<string, MatchData>( { "eloScore", new MatchData(200) } );</code>
        /// </remarks>
        /// <param name="value"></param>
        public static implicit operator MatchData(ulong value)
        {
            return new MatchData(value);
        }

        /// <summary>
        /// Implicitly converts a double to a MatchData object.
        /// </summary>
        /// <remarks>
        /// These operators make storing MatchData a little easier.
        /// So you can do things like:
        /// <code>var matchData = new Dictionary<string, MatchData>( { "eloScore", 200.0 } );</code>
        /// instead of:
        /// <code>var matchData = new Dictionary<string, MatchData>( { "eloScore", new MatchData(200.0) } );</code>
        /// </remarks>
        /// <param name="value"></param>
        public static implicit operator MatchData(double value)
        {
            return new MatchData(value);
        }

        /// <summary>
        /// Implicitly converts a float to a MatchData object.
        /// </summary>
        /// <remarks>
        /// These operators make storing MatchData a little easier.
        /// So you can do things like:
        /// <code>var matchData = new Dictionary<string, MatchData>( { "eloScore", 200.0f } );</code>
        /// instead of:
        /// <code>var matchData = new Dictionary<string, MatchData>( { "eloScore", new MatchData(200.0f) } );</code>
        /// </remarks>
        /// <param name="value"></param>
        public static implicit operator MatchData(float value)
        {
            return new MatchData(value);
        }

        /// <summary>
        /// Implicitly converts a string to a MatchData object.
        /// </summary>
        /// <remarks>
        /// These operators make storing MatchData a little easier.
        /// So you can do things like:
        /// <code>var matchData = new Dictionary<string, MatchData>( { "password", "hunter5"} );</code>
        /// instead of:
        /// <code>var matchData = new Dictionary<string, MatchData>( { "password", new MatchData("hunter5") } );</code>
        /// </remarks>
        /// <param name="value"></param>
        public static implicit operator MatchData(string value)
        {
            return new MatchData(value);
        }
        
        /// <summary>
        /// Implicitly converts a MatchData object to an int
        /// </summary>
        /// <remarks>
        /// These operators make accessing MatchData a little easier.
        /// So you can do things like:
        ///     int eloScore = matchData["eloScore"];
        /// instead of:
        ///     int eloScore = matchData["eloScore"].intValue;
        /// </remarks>
        /// <param name="value"></param>
        public static implicit operator int(MatchData matchData)
        {
            if (matchData == null) return 0;
            return matchData.intValue;
        }

        /// <summary>
        /// Implicitly converts a MatchData object to an unsigned int
        /// </summary>
        /// <remarks>
        /// These operators make accessing MatchData a little easier.
        /// So you can do things like:
        ///     int eloScore = matchData["eloScore"];
        /// instead of:
        ///     int eloScore = matchData["eloScore"].intValue;
        /// </remarks>
        /// <param name="value"></param>
        public static implicit operator uint(MatchData matchData)
        {
            if (matchData == null) return 0;
            return matchData.uintValue;
        }

        /// <summary>
        /// Implicitly converts a MatchData object to a long
        /// </summary>
        /// <remarks>
        /// These operators make accessing MatchData a little easier.
        /// So you can do things like:
        ///     int eloScore = matchData["eloScore"];
        /// instead of:
        ///     int eloScore = matchData["eloScore"].intValue;
        /// </remarks>
        /// <param name="value"></param>
        public static implicit operator long(MatchData matchData)
        {
            if (matchData == null) return 0;
            return matchData.longValue;
        }

        /// <summary>
        /// Implicitly converts a MatchData object to an unsigned long
        /// </summary>
        /// <remarks>
        /// These operators make accessing MatchData a little easier.
        /// So you can do things like:
        ///     int eloScore = matchData["eloScore"];
        /// instead of:
        ///     int eloScore = matchData["eloScore"].intValue;
        /// </remarks>
        /// <param name="value"></param>
        public static implicit operator ulong(MatchData matchData)
        {
            if (matchData == null) return 0;
            return matchData.ulongValue;
        }

        /// <summary>
        /// Implicitly converts a MatchData object to a double
        /// </summary>
        /// <remarks>
        /// These operators make accessing MatchData a little easier.
        /// So you can do things like:
        ///     double eloScore = matchData["eloScore"];
        /// instead of:
        ///     double eloScore = matchData["eloScore"].doubleValue;
        /// </remarks>
        /// <param name="value"></param>
        public static implicit operator double(MatchData matchData)
        {
            if (matchData == null) return 0;
            return matchData.doubleValue;
        }

        /// <summary>
        /// Implicitly converts a MatchData object to a float
        /// </summary>
        /// <remarks>
        /// These operators make accessing MatchData a little easier.
        /// So you can do things like:
        ///     double eloScore = matchData["eloScore"];
        /// instead of:
        ///     double eloScore = matchData["eloScore"].doubleValue;
        /// </remarks>
        /// <param name="value"></param>
        public static implicit operator float(MatchData matchData)
        {
            if (matchData == null) return 0;
            return matchData.floatValue;
        }

        /// <summary>
        /// Implicitly converts a MatchData object to a string
        /// </summary>
        /// <remarks>
        /// These operators make accessing MatchData a little easier.
        /// So you can do things like:
        ///     string password = matchData["password"];
        /// instead of:
        ///     string password = matchData["password"].stringValue;
        /// </remarks>
        /// <param name="value"></param>
        public static implicit operator string(MatchData matchData)
        {
            if (matchData == null) return "";
            return matchData.stringValue;
        }
        
        public override string ToString()
        {
            return value;
        }

    }
}
