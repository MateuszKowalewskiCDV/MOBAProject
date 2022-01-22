using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Mirror;

namespace MatchUp.Examples.Basic
{

    [RequireComponent(typeof(Matchmaker))]
    public class ExampleBasicMatchmaker : MonoBehaviour
    {
        public GameObject characterSelect, teamChoose;
        public NetworkManager nt;
        public NetworkConnection conn;

        // A reference to the MatchUp Matchmaker component that will be used for matchmaking
        Matchmaker matchUp;

        // A list of matches returned by the MatchUp server
        // This is populated in the GetMatchList() method
        Match[] matches;

        bool isHost, isClient;

        string hostAddress;
        int hostPort;

        // Get a references to components we will use often
        void Awake()
        {
            matchUp = GetComponent<Matchmaker>();
            matchUp.onLostConnectionToMatchmakingServer = OnLostConnectionToMatchmakingServer;
        }

        // If connection to the matchmaking server is lost then shutdown the host or client
        private void OnLostConnectionToMatchmakingServer(Exception e)
        {
            isHost = false;
            isClient = false;
        }

        // Display buttons for hosting, listing, joining, and leaving matches.
        void OnGUI()
        {
            if (!matchUp.IsReady || Matchmaker.externalIP == null) GUI.enabled = false;
            else GUI.enabled = true;

            if (!isHost && !isClient)
            {
                // Host a match
                if (GUI.Button(new Rect(10, 10, 150, 48), "Host"))
                {
                    HostAMatch();
                }

                // List matches
                if (GUI.Button(new Rect(10, 85, 150, 48), "List matches"))
                {
                    GetMatchList();
                }

                // Display the match list
                if (matches != null)
                {
                    for (int i = 0; i < matches.Length; i++)
                    {
                        DisplayJoinMatchButton(i, matches[i]);
                    }
                }
            }
            else
            {
                GUI.Label(new Rect(10, 10, 250, 48), "Host Address: " + hostAddress);
                GUI.Label(new Rect(10, 35, 250, 48), "Host Port: " + hostPort);
                if (isClient)
                {
                    GUI.Label(new Rect(10, 60, 250, 48), "Status: Match Joined");
                }
                else if (isHost)
                {
                    GUI.Label(new Rect(10, 60, 250, 48), "Status: Match Created");
                }

                // Leave match
                if (GUI.Button(new Rect(10, 85, 150, 48), "Disconnect"))
                {
                    Disconnect();
                }
            }
        }

        // Display a button to join a match
        void DisplayJoinMatchButton(int i, Match match)
        {
            // Grab some match data to display on the button
            var data = matches[i].matchData;

            // Join the match
            if (GUI.Button(new Rect(170, 10 + i * 26, 600, 25), data["Match name"]))
            {
                matchUp.JoinMatch(matches[i], OnJoinMatch);
            }
        }

        // Host a match
        void HostAMatch()
        {
            // Start hosting here using whatever networking system you use
            // For example if you were using UNet you would call something like NetworkManager.StartHost() here

            // Once you have the host's connection info add it as match data and create the match
            hostAddress = Matchmaker.externalIP;
            hostPort = 12345;

            // You can set MatchData when creating the match. (string, float, double, int, or long)
            var matchData = new Dictionary<string, MatchData>() {
                { "Match name", "Serwer Projektu Inzynierskiego" },
                { "Host Address", hostAddress },
                { "Host Port", hostPort }
            };

            // Create the Match with the associated MatchData
            matchUp.CreateMatch(16, matchData, OnMatchCreated);
        }

        // Called when a response is received from the CreateMatch request.
        void OnMatchCreated(bool success, Match match)
        {
            if (success)
            {
                isHost = true;

                Debug.Log("Created match: " + match.matchData["Match name"]);
                
                nt.StartHost();

                characterSelect.SetActive(false);
                teamChoose.SetActive(false);
            }
        }

        // Get a filtered list of matches
        void GetMatchList()
        {
            Debug.Log("Fetching match list");

            // Get the match list. The results will be received in OnMatchList()
            matchUp.GetMatchList(OnMatchListGot, 0, 10);
        }

        // Called when the match list is retreived via GetMatchList
        void OnMatchListGot(bool success, Match[] matches)
        {
            if (!success) return;

            Debug.Log("Received match list.");
            this.matches = matches;
        }

        // Called when a response is received from a JoinMatch request
        void OnJoinMatch(bool success, Match match)
        {
            if (!success) return;

            isClient = true;

            // Get the host's connection info
            hostAddress = match.matchData["Host Address"];
            hostPort = match.matchData["Host Port"];

            nt.networkAddress = hostAddress;

            Debug.Log("Joined match: " + match.matchData["Match name"] + " " + hostAddress + ":" + hostPort);



            nt.StartClient();


            NetworkClient.Ready();
            if (NetworkClient.localPlayer == null)
            {
                NetworkClient.AddPlayer();
            }

            characterSelect.SetActive(false);
            teamChoose.SetActive(false);

        }

        // Disconnect and leave the Match
        void Disconnect()
        {
            // Stop hosting and destroy the match
            if (isHost)
            {
                Debug.Log("Destroying match");
                isHost = false;
                matchUp.DestroyMatch();

                // Do whatever you need to here to stop hosting
            }

            // Disconnect from the host and leave the match
            else
            {
                Debug.Log("Left match");
                isClient = false;
                matchUp.LeaveMatch();

                // Do whatever you need to here to disconnect the client
            }
        }
    }
}