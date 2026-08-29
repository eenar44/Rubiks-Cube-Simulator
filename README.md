# Rubik's Cube Simulator
An interactive Rubik's Cube Simulator built in Unity that scrambles and attempts to solve a Rubik's Cube using the beginner method

## Demo
<img width="800" height="450" alt="RubiksRotation" src="https://github.com/user-attachments/assets/3150952b-7620-46b2-92d2-48f023ded7f0" />


## Features
* Full cube Rotation and camera control
* Each layer moves independently, mapped to keyboard letters
* Scramble function to randomise the cube
* Solve function attempt to solves the cube using the beginner method (see [Limitations](https://github.com/eenar44/Rubiks/edit/main/README.md#limitations) )
* Instructions scene explain controls
* Information scene displays information about the algorithm
* Back navigation between scenes

## Tech Stack
* Unity
* C#

## How it works
### Scramble generation
Twenty random moves are chosen and applied to the cube.

### Solve algorithm
Follows the infamous beginner method step by step, checking the cube's state after each move (e.g. confirming the yellow cross has been formed) before progressing to the next stage. This continues until the cube is solved.

### Performance comparison
The performance is plotted on a lone graph, where the number of moves the algorithm makes to solve the cube are counted, then it is plotted against a highlighted region, representing the average human performance.

## Limitations
The solver doesn't currently solve the cube correctly. The issue lies in the ray-casting used to read the cube state: the ray casts prematurely read the cube state and applied the net move before the previous move has properly been resolved. This caused the algorithm to act on inaccurate information and produce incorrect solutions. 

Manual cube manipulation (scrambling, moving layers and the whole cube) work as intended. 

## Reflection
This was my A Level year 13 NEA project an one of my first real experiences using Unity, GitHub and creating a project of this scale. So apologies to anyone who looks into my code and commits. Initially i worked through a Flappy Bird tutorial before starting. From there, most of the technical growth came from solving problems as they appeared: using ray-casts to read each panel's colour state, fixing a floating-point drift in cube layer positions with a custom threshold comparison, and using IEnumerators to stop layer-rotation animations from overlapping.

The solving algorithm itself was the hardest part- solving methods are intuitive to a person but not code. Therefore, I worked out the underlying patterns by solving a physical cube, then translating that into an algorithm. Ultimately, the automated solver never worked correctly, which came down to the ray-casting. The casts were reading the cube's state before a previous move had been fully resolved. This resulted in later moves acting on the wrong information.

## Setup
1. Clone the repository
2. Open the project in Unity
3. Open the main scene and press Play

## Future Improvements
* Resolve or replace solve method with a more effective technique
* Revisit the comparison graph and make it more clear


