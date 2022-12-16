Unity Multiplayer
==========================

Plugins:
Serilog.Sinks.Unity3D
PullToRefresh

Serilog.Sinks.Unity3D:  Wird für das Logging verwendet. Findet Anwendung in der der UnityLogger.cs
PulloToRefresh: Refresh Möglichkeit der Liste. Wird in der Overview Scene für das Refreshing der Anfragen und aktuellen Spiele verwendet.

Scripts und Scenes sind in dem jeweiligen Ordner unter Multiplayer zu finden.

Wichtig zu erwähnenden Anmerkungen der Skripte:

PopupPlayerName: Erscheint beim erstmaligen Starten im Multiplayer, wenn der PlayerName in der Save Datei leer ist. 

CheckInternetConnection: Auf jeder Szene ist ein GameObject vorhanden, dass mit dieser Klasse verknüpft wird. Es wird beim Laden jeder Szene geprüft, ob auch ncoh eine Internetverbindung vorhanden ist. Falls nicht wird ein Hinweis über die Szene gelegt und der User kann nur noch den Multiplayer Bereich verlassen.

FirebaseHandler: Ist derzeit in der Overview Szene eingebunden. Prüft Firebase und Subscriped sich zu dem Topic der aus einem festgelegten String plus PlayerId besteht. 

CheckReqGamePopUp: Kann in einer Szene eingebunden werden. Prüft im Hintegrund, ob neue Anfragen oder Game Infos gekommen sind und blendet ein Popup ein.

CutOutMaskUI: Wird in der RequestOverview Szene verwendet um um den Kopf vom Spieler noch einen Rahmen zu legen.


