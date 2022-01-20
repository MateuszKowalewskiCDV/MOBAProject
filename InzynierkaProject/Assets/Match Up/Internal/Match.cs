using System.Collections.Generic;

namespace MatchUp
{
    /// <summary>Info about a match</summary>
    /// <remarks>
    /// When you call CreateMatch() or GetMatchList() the Match object(s) will be created for you.
    /// matchData and clientID will not be set on clients until after the match is joined.
    /// </remarks>
    public class Match
    {
        /// <summary>The id of the match</summary>
        /// <remarks>
        /// This will be the same for all clients in the match.
        /// </summary>
        public long id = -1;

        /// <summary>The id of this particular client.</summary>
        /// <remarks>
        /// This will be different for each client in the match.
        /// clientID is not set on clients until after the match is joined.
        /// </remarks>
        public long clientID;

        /// <summary>Data related to the match</summary>
        /// <remarks>
        /// This can be anything you want, but generally it is used to store the host's connection info.
        /// </remarks>
        public Dictionary<string, MatchData> matchData = new Dictionary<string, MatchData>();

        /// <summary>Create a new match.</summary>
        /// <remarks>
        /// You should never need to do this yourself. Matches are constructed for you by the
        /// Matchmaker when you call CreateMatch() or GetMatchList()
        /// </remarks>
        /// <param name="matchID"></param>
        /// <param name="matchData"></param>
        public Match(long matchID = -1, Dictionary<string, MatchData> matchData = null)
        {
            this.id = matchID;
            this.matchData = matchData;
        }

        /// <summary>Serialize the match for sending to the matchmaking server.</summary>
        /// <remarks>
        /// Escapes quotes in the match name and serializes the data.
        /// You should not be using this directly unless really know what you're doing.
        /// </remarks>
        /// <param name="maxClients">The maximum number of clients that will be allowed to join this match.</param>
        /// <returns></returns>
        public string Serialize(int maxClients)
        {
            return Serialize(maxClients, matchData);
        }

        public static string Serialize(int maxClients, Dictionary<string, MatchData> matchData = null)
        {
            // Match name isn't used any more but it's still here so I don't have to update the server and break things for everyone
            string escapedMatchName = "\"\"";
            string s = escapedMatchName + "," + maxClients;
            if (matchData != null && matchData.Count > 0)
            {
                s += "|" + MatchData.SerializeDictionary(matchData);
            }
            return s;
        }

        // All this is so that you can compare matches with == and use matches in dictionaries and stuff like that
        public override bool Equals(object obj)
        {
            Match other = (Match)obj;
            if (obj == null) return false;
            return id == other.id;
        }

        public override int GetHashCode()
        {
            var hashCode = 1289958070;
            hashCode = hashCode * -1521134295 + id.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Match obA, Match obB)
        {
            if ((object)obA == null && (object)obB == null) return true;
            if ((object)obA == null) return false;
            return obA.Equals(obB);
        }

        public static bool operator !=(Match obA, Match obB)
        {
            if ((object)obA == null && (object)obB == null) return false;
            if ((object)obA == null) return true;
            return !obA.Equals(obB);
        }
    }
}
