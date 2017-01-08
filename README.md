# Decentraland
A blockchain-based virtual reality world.

Decentraland is an open-source initiative to build a decentralized virtual reality. Blockchain technology is used to claim and transfer land, keeping a permanent record of ownership.

## Decentraland Beta: Bronze Age
Bronze Age is the second proof of concept of the Decentraland Project. Land is modeled in three-dimensional space, a Bitcoin-like proof of work algorithm is used to allocate land to users and verify it’s content.

[Try it now](https://decentraland.github.io/bronzeage).


## Our Stack
We stand on the shoulders of giants:

* [bcoin](https://github.com/bcoin-org/bcoin): we forked an awesome bitcoin full-node implementation in JS.
* [Webtorrent](https://github.com/feross/webtorrent): leveraged torrent protocol to distribute world’s land content.
* [Unity](https://unity3d.com/): used to build Decentraland’s *Browser* and *World Editor*.


## How It Works?
Decentraland runs on top of its own blockchain. We modified Bitcoin’s blockchain to represent a non fungible asset, a.k.a. *land*. Transactions in Decentraland’s protocol can transfer land’s ownership and change its content.

The ownership of land is handled just like bitcoin, by using asymmetric cryptography and a stack-based scripting language.

The land’s content can be any type of file and the blockchain will only store a hash of it. The original file will be distributed via torrent’s protocol.

Finally, the network is secured by Bitcoin’s proof-of-work algorithm. Rewarding its miners not with coins, but with *land*.

Below a summary of the main differences with Bitcoin’s blockchain:

### Transaction:
* Removed *Output*’s *value* field.
* Add *x*, *y* and *content* fields to *Output*.
* Each *Output* must have the same *x* and *y* as the corresponding *Input*.
* A *Landbase* transaction must claim land adjacent to existing one.

### RPC:
* *gettile* call to fetch land’s file url.
* *settile* call to create a transaction that updates land’s file content.

### Network:
* Time between blocks: 10 minutes (for testnet).
* Seed land’s file descriptors through torrent network.

For more information about how to run your own miner [click here](https://github.com/decentraland/decentraland-node).

## Decentraland Browser
The browser is built using Unity and its amazing WebGL compilation, so it can run on every modern web browser.

The player’s surroundings are instantiated on runtime. This is done by fetching the file content of each tile from a Decentraland node.

There is no communication between browsers, so players will explore the world alone. We expect to add multiplayer support on the next major release (*Iron Age*), we would love to hear your comments about how to implement it.

## World Editor
To edit Decentraland world you first need to become a land lord. Please [run a node](https://github.com/decentraland/decentraland-node) and mine some land.

Once you own some land, you want to [download Unity](https://unity3d.com/get-unity/download), clone this repo and open the Unity project.

In Unity, open the *World Editor* Scene and include  `Window -> Decentraland Editor`  panel somewhere in your workspace. Make sure to complete your node address in the panel, this will let you publish changes directly from the editor.

Build your land by placing all content under `My Tile`  game object’s hierarchy. Once you are ready, select your tail and click `Publish Tile` on the `Decentraland Editor` panel. This will serialize the tile content and push it to the node which will craft, sign and broadcast a transaction and seed the original file through torrent protocol.

There are some limitations on what you can do on your tile:

* Only primitive meshs: Cube, Plane, Sphere, Cylinder and Capsule.
* Only Colors and 2D Textures (see notes).
* Hard limit of 1024 game objects inside the tile.
* Can’t place objects beyond the bounds of the tile.
* Not yet supported: Scripts, Animations, Shaders, etc.
