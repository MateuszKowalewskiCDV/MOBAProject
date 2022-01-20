Plugin Version: 3.08

## Welcome
Match Up provides easy-to-use, hassle free matchmaking support for any networking system. Just call CreateMatch() or GetMatchList() to get started. Once you've selected a match from the list you can join it with JoinMatch(). It's as simple as that.

Also includes advanced filtering options and the ability to store additional data related to the match.

An example scene is included that features **creating**, **listing**, **joining**, **leaving**, and **destroying** matches as well as setting **match data** and **filtering** matches.

*Supports Windows, Linux, OSX, Android, IOS, WebPlayer, and probably anything else Unity runs on.*

*Full source code is included for both the client and the matchmaking server.*

*Note: Matchmaking requires a matchmaking server. By default the plugin will use a testing server that we've set up. You are welcome to use the test server while developing your game, but you will want to host your own matchmaking server before releasing, as the test server could go down at any time.*

<a href="(https://www.assetstore.unity3d.com/#!/content/104411)">![Available on the asset store](http://grabblesgame.com/nat-traversal/docs/asset_store_button.png)</a>

## Features

**Painless to use:**
Just drop it in and go. There is no set up and nothing to configure. Even the most advanced features are a breeze to use.

**Advanced features:**
MatchData and MatchFilters can easily be used to implement an ELO system or any other kind of filtering you desire.

**Actually works:**
UNET's matchmaking claims to support match data and filtering via matchAttributes but it doesn't actually work!

**More flexible than UNet Matchmaking**
With Match Up you can store strings, floats, doubles, longs, or ints as match data. UNET only allows longs.

**Easier to Use**
Instead of a dealing with Unity's clunky and sometimes non-funtional system you get a clean interface with concise syntax:<br />
The **UNET** way:
```
// This only even works if your string value is less than 8 bytes.
byte[] stringBytes = Encoding.ASCII.GetBytes("some string");
Array.Resize(ref stringBytes, sizeof(long));
matchInfoSnapshot.matchAttributes["someKey"] = BitConverter.ToInt64(stringBytes, 0);
```
The **Match Up** way:
```
// The string can be as long as you want.
match.matchData["someKey"] = "some string"
```

**Professional support:**
Absolutely everything is fully commented and documented including the example scripts. The API is available online and updated with each release.
We have a consistent record of prompt and effective support. Contact us via email, forums, or discord any time and we will work with you to resolve any issue.

## How to Use
1. Import the plugin and add the Matchmaker component to any game object.
2. On the Host, wherever you call StartHost() or StartServer(), add a call to **CreateMatch()**
3. On the Clients, call **GetMatchList()** and pass one of the returned Matches in to **JoinMatch()**.
4. When the call to JoinMatch() returns pass the Match into StartClient() to connect to the match host.
5. When you disconnect on the Client you should call **LeaveMatch()**. When you disconnect on the Host you should call **DestroyMatch()**.

*Note: Matchmaking requires a matchmaking server.*

When you are ready to host your own matchmaking server simply copy the MatchUpServer file that is provided with the plugin to any linux server and run it. Also make sure to update the matchmakerURL on your MatchUp component to point to your server.

## Advanced Features
You can use MatchData to store extra data related to match:
```
SetMatchData("eloScore", 200);
```

And then clients can use MatchFilters to filter the match list:
```
// Filter so that we only receive matches with an eloScore between 100 and 300
var filters = new List<MatchFilter>(){
    new MatchFilter("eloScore", 100, MatchFilter.OperationType.GREATER),
    new MatchFilter("eloScore", 300, MatchFilter.OperationType.LESS)
};
// Get the filtered match list. The results will be received in OnMatchList()
matchUp.GetMatchList(OnMatchList, 0, 10, filters);
```

## How it Works
The plugin connects to the matchmaking server via Sockets. Commands and match data are serialized to the server which stores the match data and the current state of each match. The server is a multithreaded Socket server written in c++. It listens for commands from the plugin and acts accordingly by creating / modifying / deleting records in an sqlite database.

## But Why?
We set out to improve on Unity's matchmaking in every way possible. For our own games we needed to be able to store various data associated with each match and we quickly realized it was simple not possible with Unity's matchmaking.
The docs make vague references to MatchAttributes but if you actually try and use them you find that they don't work at all. And even if they did using them would be such an headache!<br/>
Every matchmaking system ever needs some sort of filtering as well and Unity's matchmaking offers none.<br />
The secondary motivation for creating this was so that small time developers like myself wouldn't be so dependent on Unity's multiplayer services. Having a matchmaking server that we can host ourselves frees us from Unity's services and allows us to choose our own hosting option.

## Known Issues
If you're using the default matchmaking server your Applicaton.productName is used as a filter so that you don't see matches belonging to other developer's games. If your product name happens to be the same as someone else's you will see their matches and they will see yours. 
The solution is to either change your product name or host your own matchmaking server using the provided MatchUpServer.
