# Rubik's Cube Simulator
An interactive Rubik's Cube Simulator built in Unity that scrambles and attempts to solve a Rubik's Cube using the beginner method

## Demo
![Scramble Demo]<img width="800" height="450" alt="RubiksRotation" src="https://github.com/user-attachments/assets/3150952b-7620-46b2-92d2-48f023ded7f0" />


## Features
* Full cube Rotation and camera control
* Each layer moves independently, mapped to keyboard letters
* Scramble function to randomise the cube
* Solve function attempt to solves the cube using the beginner method (see Limitations)
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


how it works
* how scramble is generated:
* * picks 20 random moves to apply onto the cube
* how solve works:
  * uses the steps in the well known beginner method to attempt to solve
  * follows the steps until the desired "cube state" is found (e.g. the yellow cross has been made)
  * and ultimately until the cube is solved
  * the issue lied with the raycasting. where the ray casts were prematurely reading the cube state and applying the next move before the previous state was achieved
      this resulted in incorrect solutions being formed
  * is is compared using a line graph (rather unimpressive honestly) simply it counts how many moves the algorithm makes to solve the cube.
    once counted it is plotted on a graph where there is a highlighted region showing the average time. (i checked the entire documentation twice i dont think i wrote down what numbers that region represented ive honestly forgotten but it mustve been the average moves or something)

limitation
* as explained above

reflection 
* 


