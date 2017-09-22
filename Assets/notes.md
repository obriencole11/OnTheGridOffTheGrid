Left off working on the end turn method
- Basically this needs to determine what dies and what upgrades

Grid Manager
- Should be mostly the same except should be private
- Instead use the grid tools library
- Grid is a dictionary of Vector2 : GridObject

Grid Objects
- A grid object is something that exists on the grid
- A grid object registers itself at start
Important notes
- The blocked state shouldn't exist anymore
- All objects should check if they can move before they do
- Declare actions step is when they check

- A grid object has a grid state. On setting it alerts the object that the state has changed.
- Grid objects can be moved and turned, this updates the state
- Grid objects can be animated, this is purely visual and inputs a blend value and a position
- Grid objects can declare actions

Grid State
- Contains information that could be used to recreate the objects state
- An exist bool
- A grid position
- A grid direction

Interfaces
- IDestroyable is able to be destroyed
	- All destroyable objects have a color
	- When destroyed, the state is set to not exist
- IPushable is able to be pushed
	- If pushed this usually moves the object
- IActive is able to declare an action

-----------------

There are a couple subclasses of gridobjects

PawnObjects : IDestroyable, IPushable, IActive
- When declaring an action they check for a square that exists and flip if it doesn't
- When blocked, they turn around

RookObjects : IDestroyable, IPushable
- Blocked doesn't really do anything

PlayerObjects : IActive
- Registers itself to a special list 
- Listens for player input, if it detects a direction it sets it's direction and it starts the round
- When declaring an action they declare the next direction

-------------------

The Round
- When a round is started
- Each round there are two turns, one taken by the player, one by all pawns
- Each turn starts with the acting objects declaring their next move
- Each move is added to a list, the game manager looks at this data and determines the result of each action
	- Basically checks each action, checks if the next space is filled or is declared for another action
- The game manager then records the state of each gridobject
- A timer is then started and each gridobject is animated based on its action (move, block, or destroyed)
- When the timer is up, the gridobject is acted upon

Undo
- When undoing, each grid object is set to not exist
- Then each gridobject in the historyList is assigned their Grid State
- And the last entry is removed