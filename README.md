# SvissOdyssey

A 2D roguelike game made at the end of the first year at UCA.

Authors: 
- Farouk Abidi
- Mathis Chalandard
- Romain Veydarier
- Colin Ferraton
- Enzo Santiago

## Implemented features: 
- Top down movement 
- Attacks and Abilities 
- NPC(Merchant)
- Items
- Inventory
- Multiple Levels
- Cinematic
- Multiplayer
- 2 Bosses
- 9 Enemies
- UI
  - Merchant
  - Main Menu
  - Game Over Screen

### Abilities and skill tree

Choice of 3 abilities among 6 different abilities thanks to a skill tree. 
  -  Choosing a skill in one of the trees will block the other half of the will block the other half of the tree. Skills can be upgraded with skill points. Levels are gained by killing enemies. 
A total of **42 abilities and upgrades** are available.

### NPCs

Purchase with coins earned from various enemies. Interface. Random merchant inventory, 4 items are chosen from all the items 
among all the items present in the game. Item chosen according to their rarity and a fixed percentage of luck.

### Items

Two are unique, once they appear, they cannot reappear. The rarity of an item depends on the stats it modifies, if it decreases, it will be less rare than items that only increase it.

### Inventory

Limited to three items, if you take another one, you will have to choose an item to replace and it will fall on the ground. Also contains the number of coins of the player.

### Levels

The player has no choice of which level to play. The levels follow a "guideline".

### Cinematic

Available in the home screen. Tells the story of the game.

### Multiplayer

Multiplayer on LAN, limited to 2 players. There is the possibility to play on the Internet thanks to port forwarding

### Bosses

Players must defeat one at the end of each dungeon.

### Enemies
9 different enemies:
- Zombie: Simple enemy, hits the player when he sees him.
- Skeleton: Throws bones when it sees the player
- Kamikaze: Explodes on contact with the player
- Goblin: Steals coins from the player and runs away, after a random time, he disappears
- Chemist: Leaves a toxic trail behind him, if the player touches it, he takes continuous damage
- Zombie gargoyle: Same function as the zombie
- Skeleton boss: Randomly throws bones at the player or makes a skeleton appear
- Goblin looter: Steals coins from the player and takes life from him
- Ice sorceress: Throws a projectile that does damage to the player

## Game keybinding

- Movement keys: **ZQSD**
- Keys to insert an item in the inventory: **1, 2, 3**
- Attack keys: Right click (basic attack), **A E** and **R** (abilities)
- **L** To open the skill tree
- **M** To open the merchant interface  

In the render file, there is a build to play in multiplayer and a build to play solo.

[A trailer of the game is available](https://www.youtube.com/watch?v=qsO4ZWtIBog)

Thank you for reading everything!
