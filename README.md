# Wholesome Cats (Cute RPG)
![header image](https://img.itch.zone/aW1nLzE1MjE0MzgucG5n/original/FGpGqx.png)
***
## A cute, kind RPG: Explore, befriend, then collect wholesome cats!
**Watch the [trailer](https://www.youtube.com/watch?v=Obwk8nxC7No), or play today for free:**

<a href="https://apps.facebook.com/wholesome_cats" target="_blank"><img src="https://zephyo.github.io/img/messenger.png" 
height="80" /></a>
<a href="https://play.google.com/store/apps/details?id=com.AngelaHe.WholesomeCats" target="_blank"><img src="https://upload.wikimedia.org/wikipedia/commons/thumb/c/cd/Get_it_on_Google_play.svg/1000px-Get_it_on_Google_play.svg.png" 
height = "80" /></a>
  <a href="" target="_blank"><img src="https://devimages-cdn.apple.com/app-store/marketing/guidelines/images/badge-download-on-the-app-store.svg" 
height="80" /></a>

***

Features
------
### Map
* Go into battle at random when walking across map; after battle, resume position on map
### Dialogue
* Sort dialogues into Dialogue structs; input into ExploreStory to begin dialogue conversation
* Typewriter effect with adjustable speed
* Voice/"beeping" sound effects
### Battle
* Pit 2 teams of 1-4 characters against each other
* Ability to attack or "act", which will either start dialogue or give a powerup
### Load/Save 
* Saved and loaded PlayerData instance by serializing into JSON using MiniJSON, JsonUtility, and Json.NET
* Save locally or to Firebase Realtime Database
### User authentication
* Create, login, logout, and delete account
* Email verification and password reset features
### Asset Bundling
* Moved memory-heavy assets to asset bundles on AWS S3
* Loaded Asset Bundles with UnityWebRequest instead of WWW.LoadFromCacheOrDownload for performance
* Reduced Facebook Messenger WebGL build from to 6.7 MB
### Monetization
* Android, iOS, and Messenger contain in-app purchases using Unity's Codeless IAP for Android/iOS and Facebook's Game SDK for Messenger
* Android/iOS contain Unity Ads integration


Technologies
------
* C#
* Firebase/Rest API
* Json.NET
* Unity
* Xcode
* HTML/Javascript
* Adobe After Effects
* Adobe Photoshop

Licensing
------
The code in this repo is licensed under the [MIT License](https://opensource.org/licenses/mit-license.php).
