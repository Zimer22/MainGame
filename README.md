Biggest feature right now is the inventory system, it is a grid based inventory displayed in 3d as a hologram
(2d sprite in 3d space) infront of the player, It supports irregular(non square-rectangular) shapes meaning there can be a lot more 
variety in the shapes of items and inventories. Every item has 2 rotations(horizontal and vertical) + 4 "flips" per rotation (facing directions, left right, facing down or up)
Sprites are handled in a per slot basis, allowing for easier image scaling with different sizes and avoiding the problems that come with not using rectangular items in a grid inventory (images being hard to stretch without going out of "bounds")
The project is being developed with multiplayer in mind using locally hosted servers by the users. connection either by steam relay(not implemented) or direct ip at the users discretion
Many features are incomplete, Currently working on finishing the inventory system (Mainly UI interactions with the system and full multiplayer sync)
