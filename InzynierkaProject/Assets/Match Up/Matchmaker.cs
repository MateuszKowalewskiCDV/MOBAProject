using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Net.NetworkInformation;

#pragma warning disable 0618 

namespace MatchUp
{
    /// <summary>Provides functionality for creating, listing, joining, and leaving matches.</summary>
    /// <remarks>
    /// Opens up a tcp socket connection to the matchmaking server in order to send
    /// commands and receive responses.
    /// Most methods take an optional callback parameter which you can use to get
    /// the response once it is received.
    /// </remarks>
    public class Matchmaker : MonoBehaviour
    {
        public const string TAG = "Match Up: ";

        /// <summary>How often in seconds to send keepalive messages to the matchmaking server</summary>
        /// <remarks>
        /// This helps the matchmaking server know when clients disconnect without getting a chance to
        /// send a disconnect message. This way old matches will be cleaned up quickly and won't linger
        /// in your match lists.
        /// </remarks>
        const int KEEP_ALIVE_TIMEOUT = 5; // Seconds

        /// <summary>The externalIP which is fetched from an external server</summary>
        public static string externalIP = null;

        #region -- Properties ---------------------------------------------------------------------
        
        /// <summary>The url of the matchmaking server.</summary>
        /// <remarks>
        /// You can use mine for testing but it could go offline at any time so don't even
        /// think of trying to release a game without hosting your own matchmaking server.
        /// </remarks>
        [Tooltip("The url of the matchmaking server")]
        public string matchmakerURL = "noblewhale.com";

        /// <summary>The port to connect to on the matchmaking server.</summary>
        /// <remarks>
        /// Leave this as default unless you started the server on a specific port using
        /// <code>
        ///     ./MatchUp -p [PORT NUMBER]
        /// </code>
        /// </remarks>
        [Tooltip("The port to connect to on the matchmaking server")]
        public int matchmakerPort = 20205;

        /// <summary>How long to wait for a response before giving up</summary>
        [Tooltip("How long to wait for a response before giving up")]
        public float timeout = 5;

        /// <summary>How many times to attempt to re-connect to the matchmaking server if connection is lost.</summary>
        /// <remarks>
        /// When the connection is lost the Matchmaker will automatically try and reconnect this many times.
        /// If you wish to implement your own reconnect behaviour, use the onLostConnectonToMatchmakingServer action
        /// and set this to 0 to disable the default behaviour.
        /// </remarks>
        [Tooltip("How many times to attempt to re-connect to the matchmaking server if connection is lost")]
        public int maxReconnectionAttempts = 5;

        /// <summary>A web service to query to retrieve the external IP of this computer.</summary>
        [Tooltip("A web service to query to retrieve the external IP of this computer")]
        public string externalIPSource = "ipv4.noblewhale.com";
        
        /// <summary>You can use this action to be informed if connection is lost to the matchmaking server</summary>
        /// <remarks>
        /// Generally this means someone pulled out the internet plug or something equally catastrophic.
        /// </remarks>
        public Action<Exception> onLostConnectionToMatchmakingServer = null;

        public bool autoConnect = true;

        #endregion

        #region -- Runtime data -------------------------------------------------------------------

        IPAddress matchmakerIP;
#if MIRROR
        Mirror.NetworkManager networkManager;
#elif UNET
        UnityEngine.Networking.NetworkManager networkManager;
#endif

        /// <summary>The connection to the matchmaking server.</summary>
        TcpClient matchmakingClient;
        /// <summary>Used for sending to the matchmaking server.</summary>
        NetworkStream networkStream;

        /// <summary>Used for receiving from the matchmaking server.</summary>
        StreamReader streamReader;

        /// <summary>Returns true when the matchmaking server has been succesfully connected to.</summary>
        public bool IsReady {
            get {
                bool ready;
                lock (socketLock)
                {
                    ready = !isConnecting && matchmakingClient != null && networkStream != null && streamReader != null && networkStream.CanWrite;
                }
                return ready;
            }
        }

        /// <summary>Keep track of open Transactions so we can call the appropriate onResponse method when a response is received.</summary>
        /// <remarks>
        /// Each request that is sent generates a Transaction with a unique transaction ID.
        /// When the matchmaking server response to a request it will include the ID in the response.
        /// When the response is received the transaction ID is used to look up the transaction
        /// so that it can be completed and the onResponse handler can be called.
        /// </remarks>
        Dictionary<string, Transaction> transactions = new Dictionary<string, Transaction>();

        /// <summary>The current Match. This is set whenever a Match is created or joined.</summary>
        public Match currentMatch;

        bool isConnecting;

        float lastKeepAliveTime = 0;
        object socketLock = new object();
        string partialMessage = "";
        char[] buffer = new char[1024];

        #endregion

        #region -- Unity stuff --------------------------------------------------------------------

        /// <summary>Set up the networking stuff</summary>
        void Start()
        {
            // We need to get the external IP
            if (autoConnect)
            {
                StartCoroutine(ConnectToMatchmaker());
            }
        }
        
        /// <summary>Close the socket connection.</summary>
        private void OnDestroy()
        {
            if (matchmakingClient != null) matchmakingClient.Close();
        }

        /// <summary>Check for incoming responses from the matchmaking server.</summary>
        protected virtual void Update()
        {
            ReadData();

            if (matchmakingClient != null && matchmakingClient.Connected)
            {
                if (Time.realtimeSinceStartup - lastKeepAliveTime > KEEP_ALIVE_TIMEOUT)
                {
                    KeepAlive();
                    lastKeepAliveTime = Time.realtimeSinceStartup;
                }
            }
        }

        #endregion

        #region -- Public interface ---------------------------------------------------------------
        
        /// <summary>
        /// Connect to the matchmaking server. This called automatically in Start().
        /// </summary>
        /// <remarks>
        /// Called automatically on start and also when reconnecting after a lost connection.
        /// You can also call it yourself if you implement your own re-connection scheme or
        /// for whatever other reason you may find.
        /// </remarks>
        /// <returns></returns>
        public IEnumerator ConnectToMatchmaker()
        {
            if (isConnecting) yield break;
            isConnecting = true;

            if (string.IsNullOrEmpty(externalIP))
            {
                yield return StartCoroutine(FetchExternalIP(externalIPSource));
            }

            foreach (var transaction in transactions)
            {
                transaction.Value.Failed("Lost connection to MatchUp server.");
                StopCoroutine(transaction.Value.timeoutProcess);
            }

            int maxTries = maxReconnectionAttempts;
            int attempts = 0;
            while (attempts < maxTries && (matchmakingClient == null || !matchmakingClient.Connected))
            {
                yield return StartCoroutine(ResolveMatchmakerURL());

                IAsyncResult connectResult;
                lock (socketLock)
                {
                    if (matchmakingClient != null) matchmakingClient.Close();
                    matchmakingClient = new TcpClient(AddressFamily.InterNetwork);
                    connectResult = matchmakingClient.BeginConnect(matchmakerIP, matchmakerPort, null, null);
                }

                while (!connectResult.IsCompleted) yield return 0;

                try
                {
                    lock (socketLock)
                    {
                        matchmakingClient.EndConnect(connectResult);
                    }
                }
                catch (SocketException e)
                {
                    Debug.LogWarning(TAG + "Failed attempting to connect to the matchmaking server. Is it running?\n" + e);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(TAG + "Failed to connect to the matchmaking server.\n" + e);
                }

                bool isConnected;
                lock (socketLock)
                {
                    isConnected = matchmakingClient.Connected;
                }
                if (!isConnected)
                {
                    yield return new WaitForSeconds(5);
                }

                attempts++;
            }

            lock (socketLock)
            {
                if (matchmakingClient != null && matchmakingClient.Connected)
                {
                    lock (socketLock)
                    {
                        if (networkStream != null) networkStream.Dispose();
                        if (streamReader != null) streamReader.Dispose();
                        networkStream = matchmakingClient.GetStream();
                        streamReader = new StreamReader(networkStream);
                    }
                }
                else
                {
                    Debug.LogError(TAG + "Final failed to connect to the matchmaking server. Is it running?");
                }
            }
            isConnecting = false;
        }

        public void Disconnect()
        {
            matchmakingClient.Close();
            isConnecting = false;
            networkStream.Dispose();
            streamReader.Dispose();
            matchmakingClient = null;
            networkStream = null;
            streamReader = null;

        }

        /// <summary>Send the command to the matchmaking server to create a new match.</summary>
        /// <param name="maxClients">The maximum number of clients to allow. Once a match is full it is no longer returned in match listings (until a client leaves).</param>
        /// <param name="matchData">Optional match data to include with the match. This is a good place to store your connection data.</param>
        /// <param name="matchName">The name of the match.</param>
        /// <param name="onCreateMatch">Optional callback method to call when a response is received from the matchmaking server.</param>
        public void CreateMatch(int maxClients, Dictionary<string, MatchData> matchData = null, Action<bool, Match> onCreateMatch = null)
        {
            if (matchData == null) matchData = new Dictionary<string, MatchData>();

            if (!matchData.ContainsKey("internalIP")) matchData["internalIP"] = GetLocalAddress(AddressFamily.InterNetwork);
            if (!matchData.ContainsKey("externalIP")) matchData["externalIP"] = externalIP;

#if MIRROR
            if (Mirror.NetworkManager.singleton)
            {
                if (!matchData.ContainsKey("port")) matchData["mirrorPort"] = GetTransportPort();
            }
#endif
#if UNET
            if (UnityEngine.Networking.NetworkManager.singleton)
            {
                if (!matchData.ContainsKey("port")) matchData["unetPort"] = UnityEngine.Networking.NetworkManager.singleton.networkPort;
                if (UnityEngine.Networking.NetworkManager.singleton.matchMaker != null && UnityEngine.Networking.NetworkManager.singleton.matchInfo != null)
                {
                    matchData["unetMatchID"] = (ulong)UnityEngine.Networking.NetworkManager.singleton.matchInfo.networkId;
                }
            }
#endif

            if (matchmakerURL == "grabblesgame.com" || matchmakerURL == "noblewhale.com")
            {
                // If you're using my matchmaking server then we need to include some sort of ID to keep your game's matches separate from everyone else's
                if (!matchData.ContainsKey("applicationID")) matchData["applicationID"] = Application.productName;
            }
            currentMatch = null;
            SendCommand(
                Command.CREATE_MATCH,
                Match.Serialize(maxClients, matchData),
                (success, transaction) => OnCreateMatchInternal(success, transaction.response, matchData, onCreateMatch)
            );
        }

        /// <summary>Set a single MatchData value and immediately send it to the matchmaking server.</summary>
        /// <remarks>
        /// Ex:
        /// <code>matchUp.SetMatchData("eloScore", 100);</code>
        /// </remarks>
        /// <param name="key">The key</param>
        /// <param name="matchData">The value</param>
        /// <param name="onSetMatchData">Optional callback method to call when a response is received from the matchmaking server.</param>
        public void SetMatchData(string key, MatchData matchData, Action<bool, Match> onSetMatchData = null)
        {
            if (currentMatch == null || currentMatch.id == -1)
            {
                Debug.LogWarning("Can not SetMatchData until after a match has been created: " + key);
                onSetMatchData(false, null);
                return;
            }
            currentMatch.matchData[key] = matchData;
            SendCommand(
                Command.SET_MATCH_DATA,
                currentMatch.id + "|" + matchData.Serialize(key),
                (success, response) => {
                    if (onSetMatchData != null) onSetMatchData(success, currentMatch);
                }
            );
        }

        /// <summary>Replace all existing match data with new match data.</summary>
        /// <remarks>
        /// Ex:
        /// <code>
        /// var newMatchData = new Dictionary<string, MatchData>() {
        ///    { "Key1", "value1" },
        ///    { "Key2", 3.14159 }
        /// };
        /// matchUp.SetMatchData(newMatchData);
        /// </code>
        /// </remarks>
        /// <param name="matchData">A Dictionary of new MatchData</param>
        /// <param name="onSetMatchData">Optional callback method to call when a response is received from the matchmaking server.</param>
        public void SetMatchData(Dictionary<string, MatchData> matchData, Action<bool, Match> onSetMatchData = null)
        {
            if (currentMatch == null || currentMatch.id == -1)
            {
                Debug.LogWarning("Can not SetMatchData until after a match has been created");
                onSetMatchData(false, null);
                return;
            }

            currentMatch.matchData = matchData;
            UpdateMatchData(onSetMatchData);
        }

        /// <summary>Merge new MatchData with existing MatchData and immediately send it all to the matchmaking server.</summary>
        /// <remarks>
        /// Ex:
        /// <code>
        /// var additionalMatchData = new Dictionary<string, MatchData>() {
        ///    { "Key1", new MatchData("value1") },
        ///    { "Key2", new MatchData(3.14159) }
        /// };
        /// matchUp.UpdateMatchData(additionalMatchData);
        /// </code>
        /// </remarks>
        /// <param name="additionalData">A Dictionary of additional MatchData to merge into existing match data</param>
        /// <param name="onUpdateMatchData">Optional callback method to call when a response is received from the matchmaking server.</param>
        public void UpdateMatchData(Dictionary<string, MatchData> additionalData, Action<bool, Match> onUpdateMatchData = null)
        {
            if (currentMatch == null || currentMatch.id == -1)
            {
                Debug.LogWarning("Can not UpdateMatchData until after a match has been created");
                onUpdateMatchData(false, null);
                return;
            }

            // Add new MatchData entries and replace existing one
            foreach (KeyValuePair<string, MatchData> kv in additionalData)
            {
                currentMatch.matchData[kv.Key] = kv.Value;
            }
            UpdateMatchData(onUpdateMatchData);
        }

        /// <summary>Send current MatchData to the matchmaking server.</summary>
        /// <remarks>
        /// Ex:
        /// <code>
        /// matchUp.currentMatch.matchData["Key1"] = 3.14159;
        /// matchUp.currentMatch.matchData["Key2"] = "Hello world";
        /// matchUp.UpdateMatchData();
        /// </code>
        /// </remarks>
        /// <param name="onUpdateMatchData">Optional callback method to call when a response is received from the matchmaking server.</param>
        public void UpdateMatchData(Action<bool, Match> onUpdateMatchData = null)
        {
            if (currentMatch == null || currentMatch.id == -1)
            {
                Debug.LogWarning("Can not UpdateMatchData until after a match has been created");
                onUpdateMatchData(false, null);
                return;
            }

            SendCommand(
                Command.SET_MATCH_DATA,
                currentMatch.id + "|" + MatchData.SerializeDictionary(currentMatch.matchData),
                (success, response) => {
                    if (onUpdateMatchData != null) onUpdateMatchData(success, currentMatch);
                }
            );
        }

        /// <summary>Destroy a match. This also removes all Client entries and MatchData on the matchmaking server.</summary>
        /// <param name="onDestroyMatch">Optional callback method to call when a response is received from the matchmaking server.</param>
        public void DestroyMatch(Action<bool> onDestroyMatch = null)
        {
            if (currentMatch == null || currentMatch.id == -1)
            {
                // There is no match to destroy
                Debug.LogWarning("Can not DestroyMatch because there is no current match.");
                if (onDestroyMatch != null) onDestroyMatch(false);
                return;
            }
            SendCommand(
                Command.DESTROY_MATCH,
                currentMatch.id.ToString(),
                (success, response) => {
                    if (onDestroyMatch != null) onDestroyMatch(success);
                }
            );
        }

        /// <summary>Join one of the matches returned my GetMatchList().</summary>
        /// <remarks>
        /// You can use the callback to get the Match object after it is received from the matchmaking server.
        /// Once the match is joined you'll have access to all the match's MatchData.
        /// </remarks>
        /// <param name="match">The Match to join. Generally this will come from GetMatchList()</param>
        /// <param name="onJoinMatch">Optional callback method to call when a response is received from the matchmaking server.</param>
        public void JoinMatch(Match match, Action<bool, Match> onJoinMatch = null)
        {
            if (match == null || match.id == -1)
            {
                // There is no match to join
                Debug.LogWarning("Can not JoinMatch because the match is invalid (null or id == -1)");
                onJoinMatch(false, match);
                return;
            }
            currentMatch = match;
            SendCommand(
                Command.JOIN_MATCH,
                currentMatch.id.ToString(),
                (success, transaction) => OnJoinMatchInternal(success, transaction.response, match, onJoinMatch)
            );
        }

        /// <summary>Leave a match.</summary>
        /// <param name="onLeaveMatch">Optional callback method to call when a response is received from the matchmaking server.</param>
        public void LeaveMatch(Action<bool> onLeaveMatch = null)
        {
            if (currentMatch == null || currentMatch.id == -1)
            {
                // There is no match to leave
                Debug.LogWarning("Can not LeaveMatch because there is no current match.");
                onLeaveMatch(false);
                return;
            }
            SendCommand(
                Command.LEAVE_MATCH,
                currentMatch.clientID.ToString(),
                (success, response) => {
                    if (onLeaveMatch != null) onLeaveMatch(success);
                }
            );
        }

        /// <summary>Get info on a single match</summary>
        /// <param name="onGetMatch">Callback method to call when the response is received from the matchmaking server</param>
        /// <param name="id">The ID of the match to fetch info for</param>
        /// <param name="includeMatchData">Whether or not to include match data in the response</param>
        public void GetMatch(Action<bool, Match> onGetMatch, long id, bool includeMatchData = true)
        {
            char includeMatchDataChar = includeMatchData ? '1' : '0';
            SendCommand(
                Command.GET_MATCH,
                id + "," + includeMatchDataChar,
                (success, transaction) => OnGetMatchInternal(success, transaction.response, onGetMatch)
            );
        }

        /// <summary>Get the match list, optionally filtering the results.</summary>
        /// <remarks>
        /// Ex:
        /// <code>
        /// var filters = new List<MatchFilter>(){
        ///     new MatchFilter("eloScore", 100, MatchFilter.OperationType.GREATER),
        ///     new MatchFilter("eloScore", 300, MatchFilter.OperationType.LESS)
        /// };
        /// matchUp.GetMatchList(OnMatchList, filters);
        /// ...
        /// void OnMatchList(bool success, Match[] matches)
        /// {
        ///     matchUp.JoinMatch(matches[0], OnJoinMatch);
        /// }
        /// </code>
        /// </remarks>
        /// <param name="onMatchList">Callback method to call when a response is received from the matchmaking server.</param>
        /// <param name="pageNumber">Used with resultsPerPage. Determines which page of results to return. Defaults to 0.</param>
        /// <param name="resultsPerPage">User with pageNumber. Determines how many matches to return for each page. Defaults to 10.</param>
        /// <param name="filters">Optional List of Filters to use when fetching the match list</param>
        /// <param name="includeMatchData">
        /// By default match data is included for every match in the list. 
        /// If you don't need / want this you can pass false in here and save some bandwidth. 
        /// If you don't retrieve match data here you can still get it when joining the match.
        /// </param>
        public void GetMatchList(Action<bool, Match[]> onMatchList, int pageNumber = 0, int resultsPerPage = 10, List<MatchFilter> filters = null, bool includeMatchData = true)
        {
            if (matchmakerURL == "grabblesgame.com" || matchmakerURL == "noblewhale.com")
            {
                if (filters == null)
                {
                    filters = new List<MatchFilter>();
                }
                if (filters.Find(x => x.key == "applicationID") == null)
                {
                    // If you're using my matchmaking server then we need to include some sort of ID to keep your game's matches separate from everyone else's
                    filters.Add(new MatchFilter("applicationID", Application.productName));
                }
            }
            string filterString = "";
            if (filters != null) filterString = "|" + MatchFilter.Serialize(filters);
            char includeMatchDataChar = includeMatchData ? '1' : '0';
            SendCommand(
                Command.GET_MATCH_LIST,
                pageNumber + "," + resultsPerPage + "," + includeMatchDataChar + filterString,
                (success, transaction) => OnGetMatchListInternal(success, transaction.response, onMatchList)
            );
        }

        /// <summary>Fetch the external IP</summary>
        /// <param name="ipSource">The url from which to fetch the IP</param>
        public static IEnumerator FetchExternalIP(string ipSource)
        {
#if UNITY_5_6 || UNITY_2017_1 || UNITY_2017_2
            WWW www = new WWW(ipSource);
            yield return www;
            externalIP = IPAddress.Parse(www.text.Trim()).ToString();
#else
            var www = UnityEngine.Networking.UnityWebRequest.Get(ipSource);
            var status = www.SendWebRequest();
            yield return status;
            try
            {
                externalIP = IPAddress.Parse(www.downloadHandler.text.Trim()).ToString();
            }
            catch (Exception e)
            {
                Debug.Log("Failed fetching IP. Response: " + www.downloadHandler.text);
                Debug.Log(e);
            }
#endif
        }

        /// <summary>Select between internal and external IP.</summary>
        /// <remarks>
        /// Most of the time we connect to the externalIP but when connecting to another PC on the same local network or 
        /// another build on the same computer we need to use the local address or localhost instead
        /// </remarks>
        /// <param name="hostExternalIP">The host's external IP</param>
        /// <param name="hostInternalIP">The host's internal IP</param>
        /// <returns></returns>
        public static string PickCorrectAddressToConnectTo(string hostExternalIP, string hostInternalIP)
        {
            if (hostExternalIP == externalIP && !string.IsNullOrEmpty(hostInternalIP))
            {
                // Client and host are behind the same router
                if (hostInternalIP == GetLocalAddress(AddressFamily.InterNetwork))
                {
                    // Host is running on the same computer as client, two separate builds
                    return "127.0.0.1";
                }
                else
                {
                    // Host is on the same local network as client
                    return hostInternalIP;
                }
            }
            else
            {
                // Host is somewhere out on the internet
                return hostExternalIP;
            }
        }

        /// <summary>
        /// Gets the a local address by looping through all network interfaces and returning first address from the first interface whose OperationalStatus is Up and whose
        /// address family matches the provided family.
        /// </summary>
        /// <returns>The local address as a string or an empty string if there is none</returns>
        public static string GetLocalAddress(AddressFamily family)
        {
            try
            {
                foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (item.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == family)
                            {
                                return ip.Address.ToString().Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }

            return "";
        }

        #endregion

        #region -- Internal handlers --------------------------------------------------------------

        /// <summary>Parses the CreateMatch response to get the match id and clientID</summary>
        void OnCreateMatchInternal(bool success, string response, Dictionary<string, MatchData> matchData, Action<bool, Match> onCreateMatch)
        {
            if (!success)
            {
                currentMatch = null;
                if (onCreateMatch != null) onCreateMatch(success, currentMatch);
                return;
            }

            string[] parts = response.Split(',');
            try
            {
                long id = long.Parse(parts[0]);
                currentMatch = new Match(id, matchData);
                currentMatch.clientID = long.Parse(parts[1]);
            }
            catch (Exception e)
            {
                Debug.LogError(TAG + "Error parsing CreateMatch response from matchmaking server." + e, gameObject);
                success = false;
            }

            if (onCreateMatch != null) onCreateMatch(success, currentMatch);
        }

        /// <summary>Parses the clientID and MatchData returned by the matchmaking server.</summary>
        void OnJoinMatchInternal(bool success, string response, Match match, Action<bool, Match> onJoinMatch)
        {
            int endPos = 0;
            int startPos = 0;
            try
            {
                // Client id
                endPos = response.IndexOf(',', startPos);
                if (endPos == -1)
                {
                    currentMatch.clientID = int.Parse(response);
                }
                else
                {
                    currentMatch.clientID = int.Parse(response.Substring(startPos, (endPos - startPos)));
                    startPos = endPos + 1;

                    // The rest of the match data
                    match.matchData = MatchData.DeserializeDictionary(response, startPos, ref endPos);
                    startPos = endPos + 1;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(TAG + "Error parsing JoinMatch response from matchmaking server." + e, gameObject);
                success = false;
            }

            if (onJoinMatch != null)
            {
                onJoinMatch(success, match);
            }
        }

        /// <summary>Parses the match list returned by the matchmaking server</summary>
        void OnGetMatchInternal(bool success, string response, Action<bool, Match> onMatch)
        {
            int endPos = 0;
            int startPos = 0;

            if (response == "")
            {
                onMatch(false, null);
                return;
            }

            Match match = null;
            try
            {
                // Match id
                endPos = response.IndexOf(',', startPos);
                int matchID = int.Parse(response.Substring(startPos, (endPos - startPos)));
                startPos = endPos + 1;

                // Create the match object
                match = new Match(matchID);

                // Add the match data to the match
                match.matchData = MatchData.DeserializeDictionary(response, startPos, ref endPos);
                startPos = endPos + 1;
            }
            catch (Exception e)
            {
                Debug.LogError(TAG + "Error parsing GetMatchList response from matchmaking server." + e, gameObject);
                success = false;
            }

            if (onMatch != null)
            {
                onMatch(success, match);
            }
        }

        /// <summary>Parses the match list returned by the matchmaking server</summary>
        void OnGetMatchListInternal(bool success, string response, Action<bool, Match[]> onMatchList)
        {
            if (success == false)
            {
                if (onMatchList != null) onMatchList(success, null);
                return;
            }

            int endPos = 0;
            int startPos = 0;

            List<Match> matches = new List<Match>();
            try
            {
                while (endPos != -1 && endPos < response.Length && startPos < response.Length)
                {
                    // Match id
                    endPos = response.IndexOf(',', startPos);
                    int matchID = int.Parse(response.Substring(startPos, (endPos - startPos)));
                    startPos = endPos + 1;

                    // Match name
                    // This is actually deprecated and unused but left in so I don't have to change the server
                    Message.ParseQuoted(response, startPos, ref endPos);
                    bool hasMatchData = endPos < response.Length && response[endPos] == ',';
                    startPos = endPos + 1;

                    // Create the match object
                    Match match = new Match(matchID);

                    // Check if there is match data that needs parsing
                    if (hasMatchData)
                    {
                        // Add the match data to the match
                        match.matchData = MatchData.DeserializeDictionary(response, startPos, ref endPos);
                        startPos = endPos + 1;
                    }

                    // Add the match to the list
                    matches.Add(match);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(TAG + "Error parsing GetMatchList response from matchmaking server." + e, gameObject);
                success = false;
            }

            if (onMatchList != null)
            {
                onMatchList(success, matches.ToArray());
            }
        }

        #endregion

        #region -- Internal -----------------------------------------------------------------------

		/// <summarySend a command to the matchmaking server.</summary>
		void SendCommand(Command command, string textToSend = "", Action<bool, Transaction> onResponse = null)
        {
            var request = new Message(command, textToSend);
            StartCoroutine(SendCommandAsync(request, onResponse));
        }

        /// <summary>Send a Command to the matchmaking server and wait for a response.</summary>
        /// <remarks>
        /// Creates a transaction for the request if a response is expected.
        /// Also starts a coroutine to timeout the transaction if no response is received.
        /// </remarks>
        IEnumerator SendCommandAsync(Message request, Action<bool, Transaction> onResponse = null)
        {
            if (matchmakingClient == null) yield break;

            string transactionID = "";
            Transaction transaction = null;
            if (onResponse != null)
            {
                transactionID = Transaction.GenerateID();

                transaction = new Transaction(transactionID, request, onResponse);
				transactions[transactionID] = transaction;
			}

            // Send the command
            string textToSend = (int)request.command + "|" + transactionID + "|" + request.payload + "\n";
            byte[] bytesToSend = Encoding.ASCII.GetBytes(textToSend);
            
            try
            {
                lock (socketLock)
                {
                    networkStream.Write(bytesToSend, 0, bytesToSend.Length);
                }
            }
            catch (SocketException e)
            {
                transaction.Failed("Lost connection to MatchUp server.");
                onLostConnectionToMatchmakingServer(e);
                yield break;
            }
            
            if (onResponse != null)
            {
                Coroutine timeoutProcess = StartCoroutine(TimeoutTransaction(transaction));
                transaction.timeoutProcess = timeoutProcess;
            }
        }

        /// <summary>Wait to see if a transaction times out.</summary>
        /// <remarks>
        /// If no response is received then the transaction has failed.
        /// The transaction's onResponse method will be called with success = false.
        /// </remarks>
        /// <param name="transaction">The transaction that is timing out</param>
        IEnumerator TimeoutTransaction(Transaction transaction)
        {
            yield return new WaitForSeconds(timeout);

            // One last chance to complete the transaction
            // This helps prevent false negatives caused by long scene loads
            // Without the extra read the transaction will appear to have timed out
            // when the scene finishes loading because the check below runs before
            // the Update() loop that would normally read the message.
            ReadData();

            if (!transaction.isComplete)
            {
                transactions.Remove(transaction.transactionID);
                transaction.Timeout();
            }
        }

        /// <summary>Resolve the matchmaker url to an ip</summary>
        IEnumerator ResolveMatchmakerURL()
        {
            var asyncResult = Dns.BeginGetHostAddresses(matchmakerURL, null, null);

            while (!asyncResult.IsCompleted) yield return 0;

            IPAddress[] addresses = null;
            try
            {
                addresses = Dns.EndGetHostAddresses(asyncResult);
            }
            catch (Exception)
            {
                // Do nothing, probably no internet
            }

            if (addresses == null || addresses.Length == 0)
            {
                Debug.LogError(TAG + "Failed to resolve matchmakerUrl: " + matchmakerURL);
                yield break;
            }
            
            foreach (var address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    matchmakerIP = address;
                    break;
                }
            }
        }

        /// <summary>Send the keep-alive message to the matchmaking server so it knows we are still here.</summary>
        void KeepAlive()
        {
            try
            {
                lock (socketLock)
                {
                    if (matchmakingClient != null && networkStream != null && streamReader != null && networkStream.CanWrite)
                    {
                        networkStream.Write(new byte[] { (byte)'\n' }, 0, 1);
                    }
                }
            }
            catch (Exception e) 
            {
                lock (socketLock)
                {
                    if (matchmakingClient != null) matchmakingClient.Close();
                }
                if (onLostConnectionToMatchmakingServer != null)
                {
                    onLostConnectionToMatchmakingServer(e);
                }
            }
        }

        /// <summary>Read incoming data from the matchmaking server</summary>
        void ReadData() 
        {
            lock (socketLock)
            {
                // Check if there is anything to read and if there is read it
                while (networkStream != null && networkStream.CanRead && networkStream.DataAvailable)
                {
                    // Read up to buffer length
                    int numCharsRead = streamReader.Read(buffer, 0, buffer.Length);
                    // Convert to string for easy splitting
                    string incoming = new string(buffer, 0, numCharsRead);
                    // Split on delimeter
                    string[] messages = incoming.Split('\n');

                    // There may be 0, 1, or many messages.
                    // If the delimeter does not appear, then the buffer contains a single
                    // partial message that is stored for when the rest of it arrives
                    // If the delimeter is present then messages[] will contain all
                    // of the complete messages, except for the final entry which will
                    // contain the next partial message.
                    for (int i = 0; i < messages.Length - 1; i++)
                    {
                        string message = partialMessage + messages[i];
                        partialMessage = ""; // Only prepended to the first message, so unset after using

                        // Parse the transaction ID and response body
                        int pipePos = message.IndexOf('|');
                        string transactionID = message.Substring(0, pipePos);
                        string response = message.Substring(pipePos + 1);

                        // Find the corresponding transaction that this is a response for
                        Transaction t;
                        bool success = transactions.TryGetValue(transactionID, out t);
                        if (success)
                        {
                            // Complete the transaction and remove it from the list
                            transactions.Remove(transactionID);
                            t.Complete(response);
                        }
                        else
                        {
                            Debug.LogWarning(TAG + "Received a response for which there is no open transaction: " + message);
                        }
                    }
                    // Store the final entry as the new partial message to be used when the next bit of data arrives
                    partialMessage += messages[messages.Length - 1];
                }
            }
        }

#if MIRROR
        public static int GetTransportPort()
        {
            var transportType = Mirror.Transport.activeTransport.GetType();
#if LITENETLIB4MIRROR
            if (transportType == typeof(Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport))
            {
                return ((Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport)Mirror.Transport.activeTransport).port;
            }
#endif
#if IGNORANCE
            if (transportType.IsSubclassOf(typeof(IgnoranceTransport.Ignorance)) || transportType == typeof(IgnoranceTransport.Ignorance))
            {
                return ((IgnoranceTransport.Ignorance)Mirror.Transport.activeTransport).port;
            }
#endif
            if (transportType == typeof(Mirror.TelepathyTransport))
            {
                return ((Mirror.TelepathyTransport)Mirror.Transport.activeTransport).port;
            }
            else if (transportType == typeof(kcp2k.KcpTransport))
            {
                return ((kcp2k.KcpTransport)Mirror.Transport.activeTransport).Port;
            }

            return -1;
        }

        public static void SetTransportPort(int port)
        {
            if (port < 0) return;

            var transportType = Mirror.Transport.activeTransport.GetType();
#if LITENETLIB4MIRROR
            if (transportType == typeof(Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport))
            {
                var liteNet = (Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorTransport)Mirror.Transport.activeTransport;
                liteNet.port = (ushort)port;
            }
#endif
#if IGNORANCE
            if (transportType == typeof(IgnoranceTransport.Ignorance))
            {
                var ignorance = (IgnoranceTransport.Ignorance)Mirror.Transport.activeTransport;
                ignorance.port = port;
            }
#endif
            if (transportType == typeof(Mirror.TelepathyTransport))
            {
                ((Mirror.TelepathyTransport)Mirror.Transport.activeTransport).port = (ushort)port;
            }
            else if (transportType == typeof(kcp2k.KcpTransport))
            {
                ((kcp2k.KcpTransport)Mirror.Transport.activeTransport).Port = (ushort)port;
            }
        }
#endif

        #endregion
    }
}
