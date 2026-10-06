# 3D videgame decisions

## Genre: Roguelike Narrative-Driven

Primary Inspiration: Signalis  
Secondary Inspirations: Empty Shell, Resident Evil, Metal Gear  
Cloning process for respawning (the character falls into the ground, fresh and slimy from the liquids with an integrated "terminal" that let's them get the information the player knows trough the previous plays)  
Procedural generated rooms for exploring the abandoned lab, with some "key rooms" to ensure the narrative-driven game.  
Low-poly models + raw pixel textures, to ensure a pixelated, vibrant CRT visual style.  
Mechanical Inspirations:

| Game Feature        | Injected Element                          | How it works                                                                                                                                                                                                                                       |
| ------------------- | ----------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Visual Art          | _Signalis_                                | Low-poly models + raw pixel textures + black cell shading accented by vibrant colors.                                                                                                                                                              |
| Screen Filter       | _Empty Shell_                             | Heavy CRT filter, and screen-space distortion but operating on a vibrant, punchy color palette.                                                                                                                                                    |
| Progression         | _Signalis_ + _Empty Shell_                | Individual permadeath; overarching narrative items and environmental puzzle data persist through a main interface database.                                                                                                                        |
| Narrative           | _Signalis_ + _Empty Shell_ + _Metal Gear_ | Dystopian world where society is divided into two different groups: Humans and Clones. The protagonist is forced into a loop ruled by a rogue decaying AI that slowly kills him (causing the respawn feature and procedural generated playtrough). |
| Movement & Evade    | _Signalis_ + _Metal Gear_                 | The player has a limited, heavy stamina bar. Sprinting is used strictly to escape a bad corner, not to traverse the map. There are no sci-fi dashes or double jumps.                                                                               |
| Exploration         | _Empty Shell_ + _Signalis_                | Modular 3D rooms generate with locked heavy bulkheads, broken elevators, and flooded maintenance shafts. You explore vertically via slow ladders, cargo lifts, and dropping through shattered ceilings.                                            |
| Combat Rhythm       | _Signalis_ + _Metal Gear_                 | Enemies are slower but incredibly lethal, aggressive, and unpredictable. Fighting more than two at once is a death sentence. You must use the environment (bottlenecks, closing blast doors) to control the space.                                 |
| Inventory & Looting | _Signalis_ + _Empty Shell_                | Opening your integrated database terminal or managing your 6-slot inventory does _not_ pause the game. You must find a dark, quiet corner to manage items.                                                                                         |

- The Core (Loop): The core narrative of this story is an existential battle of attrition between a decaying AI trying to preserve an endless war, and a breaking soldier using his own deaths to dismantle it.
- The Gameplay/Narrative Parallel: The "procedural generation" isn't just a mechanic: it is the AI actively rewriting the world to prevent the player from scaping.
- The Protagonist’s Defiance: While the AI uses the player deaths to optimize its own data, the player uses his own deaths to memorize the permanent flaws in the machine. He turns his data (passed-down from his past failures) into a door to eventually break the system.

Its focused on the grinding, industrial dread of cosmic horror. At its absolute center, the narrative explores three defining concepts:

- War as an Automated Prison: The story traps the protagonist (a human mistaken for a clone) in a literal, inescapable cycle. War is no longer fought for an ideology, it is simulated by a senile, rotting AI (The Mother) simply because fighting is the only function it has left.
- The Weaponization of Failure: The protagonist cannot outsmart the system through standard means because the world actively rewires itself (_procedural generation_) to block his escape. Instead, his only weapon is his own corpses. While the machine recycles his flesh, it fails to cleanly wipe his data. The protagonist uses the data of his past deaths to destroy the AI’s decaying mind and finally scape from hell.
- Identity Dissolution: The true horror is not dying, but surviving. Every time The protagonist dies and respawns, it isn't a clean "re-stitch." It is an industrial, agonizing recycling process. The facility's automated systems dredge up your mangled remains, paste them together with synthetic flesh and cold metal, and shove you back out, he is crudely patched back together while losing pieces of his humanity, his mind, and his memories with every loop. He is running a race against time to break the system before he completely forgets why he is trying to escape in the first place.  
    Ultimately, it is a story about defying an omnipotent, bureaucratic nightmare by holding onto the scars it leaves behind.

## Camera View

Id like to combine 2 kind of perspectives, over-the-shoulder and fixed isometric view:

1. Exploration Mode: Fixed Isometric

- When it triggers: When your weapon is holstered, or when you are traversing the procedurally generated corridors.
- The Vibe: High-angle, surveillance-style camera looking down. You can see the architecture, the multi-tiered vertical shafts, and the environmental puzzles.
- Narrative Connection: This represents The Mother’s view. The AI is watching your movement from its security cameras network. You have a wider view, but everything feels distant and cold.

1. Combat/Investigation Mode: Over-the-Shoulder

- When it triggers: The absolute millisecond you press the "Aim Weapon" or "Toggle Flashlight Focus" button.
- The Vibe: The camera smoothly but rapidly swings down, snapping tightly right behind the character's rusted, metal-grafted shoulder. The field of view narrows drastically.
- The Visual Horror: Because the models are low-poly and raw-textured, zooming in close under a heavy, distorted CRT filter makes everything look incredibly raw, jagged, and terrifying. You can see the copper staples holding your character's jaw together.
- The Mechanical Terror: Your peripheral vision vanishes. You can no longer see what is crouching on the catwalk above or sneaking up from behind. You are entirely dependent on your narrow flashlight cone cutting through the dithered darkness.  
