# Decentraland
Decentraland is an open-source initiative to build a decentralized virtual reality. Blockchain technology is used to claim and transfer land, keeping a permanent record of ownership. [Try it now](https://decentraland.org/app/).


## Decentraland Browser
The browser is built using Unity and its amazing WebGL compilation, so it can run on every modern web browser.

The player’s surroundings are instantiated on runtime. This is done by fetching the file content of each tile from a Decentraland node.

There is no communication between browsers, so players will explore the world alone. Multiplayer support will be added on the next major release (*Iron Age*), we would love to hear your comments about how to implement it.


## World Editor
To edit Decentraland world you first need to become a land lord. Please [run a node](https://github.com/decentraland/decentraland-node) and mine some land.

Once you own some land, you want to [download Unity](https://unity3d.com/get-unity/download), clone this repo and open the Unity project.

In Unity, open the *World Editor* Scene and include  `Window -> Decentraland Editor`  panel somewhere in your workspace. Make sure to complete your node address in the panel, this will let you publish changes directly from the editor.

Build your land by placing all content under `My Tile`  game object’s hierarchy. Once you are ready, select your tail and click `Publish Tile` on the `Decentraland Editor` panel. This will serialize the tile content and push it to the node which will craft, sign and broadcast a transaction and seed the original file through torrent protocol.

There are some limitations on what you can do on your tile:

* Only primitive meshs: Cube, Plane, Sphere, Cylinder and Capsule.
* Only Colors and 2D Textures ([see notes](./docs/EDITOR.md)).
* Hard limit of 1024 game objects inside the tile.
* Can’t place objects beyond the bounds of the tile.
* Not yet supported: Scripts, Animations, Shaders, etc.


# Music Credits
Music by [D.I.E.T.R.I.C.H](http://www.d-i-e-t-r-i-c-h.com/), an amazing Argentine band, who generously allowed us to use their music for this project.
