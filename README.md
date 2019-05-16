# CSPSolver

C# code for a csp solver. 

Based on the CSC384 assignment #2 at University of Toronto. 


# Usage: 

Run the Make.cmd file in the Visual Studio Development Terminal. A nongram.exe file should be created. 

In the terminal execute the exe. 
> nongram.exe

# Classes 

## cspbase.cs 
Contains classes for the Variable, Contraints, the Constraint Satisfaction Problem and the Back Trace Search. The Back Trace will keep details about speed and number for prunings. 

## Orderings.cs 
A class used for testing purposes

## Propagators.cs 
A class for finding valid variable assignments using different types of Propagators. This class is used as a parameter for the Back Trace search in the cspbase.cs 

## nongram.cs 
This is a class a representign a sample Constraint Satisfaction Problem. (https://en.wikipedia.org/wiki/Nonogram) Takes an input representing a Grid representing the column contraints and then the row contraints. 

## teseCspbase.cs 
This is where the CSP is tested. A Sample Bird nonogram puzzle is written. Will print out the grid in the terminal once the problem is solveed.  
