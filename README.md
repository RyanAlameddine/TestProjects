# TestProjects
This repo is an amalgamation of a couple side projects I did. Each one is described in a little detail below:

# Pong with Windows

![pong](https://github.com/RyanAlameddine/TestProjects/raw/master/READMEResources/Pong.gif)

As you can see in this ^ gif, I am playing pong using a random process's window as the paddle! Also, you might notice that the score, ball, 
and AI paddle all displayed seemingly magically floating over my desktop. I implemented this by using PInvoke to directly call the windows system API.

More specifically, the score, the ball, and the AI paddle are all separate processes. I use PInvoke to update the ball & paddle window's positions, and share information about the score.
I find the topmost window on the screen, identify it's bounds and use that as the player paddle. The rest is just simple pong logic!

# Arbitrary Graph-Based Data Structure Visualizer

![treevis](https://github.com/RyanAlameddine/TestProjects/raw/master/READMEResources/treevis.gif)

I created a special visualizer which allows users to visualize arbitrary data structures which can take a graph-like form. 
The circles are nodes, the arrowed lines are edges, and the triangles + dashed lines are "pins" which help with visualization.

![graphexample](https://github.com/RyanAlameddine/TestProjects/raw/master/READMEResources/GraphExample.gif)

In the visualizer, users can experiment with pre-provided data structures (BSTs, Linked List, and Doubly Circularly Linked List), or they can add their own using my VisLibrary.


To ease in visualization, "pins" can be placed pulling nodes to hold certain configurations:

![PinExample](https://github.com/RyanAlameddine/TestProjects/raw/master/READMEResources/PinExample.gif)


# Maze Generation and Pathfinding

![pathfinding](https://github.com/RyanAlameddine/TestProjects/raw/master/READMEResources/simpletraversal.gif)

As shown above, I created a simple drawable grid which supports pathfinding using A* from start (green) to finish (red) ignoring walls (black). 

Additionally, I implemented maze generation using Union Find and Primms:

![mazegen](https://github.com/RyanAlameddine/TestProjects/raw/master/READMEResources/mazegen.gif)
