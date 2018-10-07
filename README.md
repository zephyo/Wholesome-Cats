![header image](https://img.itch.zone/aW1nLzE1MjE0MzgucG5n/original/FGpGqx.png)
***
## A cute, kind RPG: Explore, befriend, then collect wholesome cats!

**Watch the [trailer](https://www.youtube.com/watch?v=Obwk8nxC7No), or play today for free:**


<a href="https://apps.facebook.com/wholesome_cats" target="_blank">
  <p align="center"><img src="https://zephyo.github.io/img/messenger.png" 
height="60" /></p></a>
_Messenger currently in review_
<a href="https://play.google.com/store/apps/details?id=com.AngelaHe.WholesomeCats" target="_blank">
  <p align="center">
    <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/c/cd/Get_it_on_Google_play.svg/1000px-Get_it_on_Google_play.svg.png" 
height = "60" /></p></a>
  <a href="https://itunes.apple.com/us/app/wholesome-cats/id1436937938" target="_blank">
  <p align="center"><img src="https://devimages-cdn.apple.com/app-store/marketing/guidelines/images/badge-download-on-the-app-store.svg" 
height="60" /></p></a>


***
<img align="left" height="120" src="https://img.itch.zone/aW1hZ2UvMzA3MTk2LzE1MjExNDIuZ2lm/315x250%23c/VR6uaR.gif" 
/> I created this game during my summer 2018 internship at Zynga for an internal hackathon. Feedback was great so I acquired the rights for the game from the company, finished the game, then published. 

I just love cats so much!  




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
* Reduced Facebook Messenger WebGL build from 24.1 MB to 6.7 MB
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
* AWS S3
* HTML/Javascript
* Adobe After Effects
* Adobe Photoshop

Licensing
------
Copyright (C) 2018 Angela He
