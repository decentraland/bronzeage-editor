![](https://raw.githubusercontent.com/decentraland/web/gh-pages/img/banner.png)

## About

Decentraland is an open-source initiative to build a shared virtual reality
world. In Decentraland, the content of each piece of land is determined by its
owner. You can become a land owner either by having someone transfer you some
of their parcels, or by mining new land. A new parcel of land is mined every 10
minutes.

[Explore Decentraland](https://decentraland.org/app/)

## Components

* **Node**: An open and trustless land ownership record and scene content distribution network.
* **Editor**: An easy to use 3D scene editor, that lets you publish scenes directly to any of your parcels.
* **Browser**: A browser for navigating the virtual world of Decentraland.

## Unity Project

This [Unity](https://unity3d.com/) project has two scenes, one that lets you
explore the virtual world, used as the base for compiling the browser, and
another of an empty land parcel, that you can edit and publish to a piece of
land you own.

# Browser

The browser fetches and renders the user's immediate surroundings as she moves
through the Decentraland virtual world. The 3D scene content is provided by a
node's [land content
server](https://github.com/decentraland/bronzeage-node#land-content-server).

The browser is built using [Unity](https://unity3d.com/), targeting WebGL, so
it can run on every modern web browser. There's still no communication between
browsers, so players will explore the world alone. Multiplayer support will be
added on the next major release (Iron Age). We'd love to hear your comments on
how to implement it!

# Editor

The world editor is a Unity3D plugin that connects to your local Decentraland
node and can publish changes you make to your land.

### How to edit your tile's content
To edit the Decentraland world you first need to own some land (one or more tiles). Please [run a node](https://github.com/decentraland/decentraland-node) with mining activated and wait until it mines some tiles. Take note on the coordinates of the tiles you own, as you will need them later.

While the miner is running, you'll want to [download Unity3D](https://unity3d.com/get-unity/download), clone this repo and open the project with Unity3D.

In Unity, open the *World Editor* Scene (found in the Scenes folder) and include the Decentraland Editor (`Window -> Decentraland Editor`) panel somewhere in your workspace. Make sure to fill your node address and port in the panel, in order to publish changes directly from the editor.

Build your land by placing all content under `My Tile` game object’s hierarchy. If you change the root object's name from `My Tile` to anything else, other users will see it when they walk through your land. Once you are ready, select your tile in the object hierarchy, fill the coordinates of a tile you mined in the `Decentraland Editor` panel, and click on `Publish Tile`. This will serialize the tile content and push it to the node. The node will craft, sign and broadcast a transaction, and seed the content through the bittorrent protocol.

There are some limitations on what you can do on your tile:

* Only primitive meshs: Cube, Plane, Sphere, Cylinder and Capsule.
* Only Colors and 2D Textures ([see notes](./docs/EDITOR.md)).
* Hard limit of 1024 game objects inside the tile.
* Can’t place objects beyond the bounds of the tile.
* Not yet supported: Scripts, Animations, Shaders, etc.

Have fun!

# Feedback and support
For get help and give feebdack [join our slack](https://rauchg-slackin-ueglzmcnsv.now.sh/) and talk to the development team. You can also report bugs by [opening a github issue](https://github.com/decentraland/bronzeage-editor/issues/new).


# Music Credits
Music by [D.I.E.T.R.I.C.H](http://www.d-i-e-t-r-i-c-h.com/), an amazing Argentine band, who generously allowed us to use their music for this project.
