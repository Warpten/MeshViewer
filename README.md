# MeshViewer

Target build: 4.3.4.15595

![](https://i.imgur.com/pSVTD9I.png)
![](https://i.imgur.com/xecPI8c.jpg)

Features

* Reads everything from client memory. No function calls. Completely passive.
* Gameobjects that have collision data are rendered. They can be toggled on and off.
* Camera view matrix is extracted from the client memory.

Bugs

* GameObjects don't animate their movements (transports)
* Lots of exceptions at shutdown.
