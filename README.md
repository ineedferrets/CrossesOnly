# CrossesOnly
Crosses only variant of naughts and crosses with an MCTS AI opponent.

## Monte-Carlo Tree Search
Monte-Carlo Tree Search (MCTS) is a decision-making AI algorithm that maps out available actions and evaluates their value based upon a pseudo-random exploration of the potential action space.
MCTS is most famously used in AlphaGo Zero to become the world champion at Go.

MCTS works by mapping out a tree of the available actions.
It starts from the current state $G_0$ and checks whether all available actions have been evaluated.
If an action from the current state $A_0^j$ has not been evaluated, then MCTS selects that action.
The action is evaluated by running a simulation in which all players act randomly.
If the MCTS wins, it adds a win count to action $A_0^j$ and a total count of evaluations to that action.

If all available actions from the current state $G_0$ have been evaluated, MCTS chooses a new game state to evaluate from based upon the best action evaluated so far.
The best action is given by the Upper Confidence Bound (UCB) as follows:

$UCT_j = X_j + C * \sqrt{\frac{ln(n)}{n_j}}$

where $X_j$ is the win ratio of the action, $C$ is a constant that adjusts the weight towards exploration, $ln(n)$ is the number of times the original game state has been visited, and $n_j$ is the number of times the child has been visited.

MCTS then iterates over these steps constantly to explore the action space as much as possible.
MCTS will terminate when all actions have been explored, but for games like Chess or Go this would take an extraordinary amount of time.
As such, many implementations place caps on the time it takes or the amount it iterates.

The random exploration of actions, the Monte-Carlo aspect of the algorithm, can be inefficient.
There are multiple ways to overcome this. AlphaGo evaluates each game state by referring to an ML algorithm that has learnt to evaluate a given game state based on it's win chance over multiple thousands, if not millions, of games.
I have not done this for this game.
