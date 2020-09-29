# Subnautica_Reinforcement_Helper
An effort to expose Subnautica to reinforcement training agents using the [QModManager](https://github.com/SubnauticaModding/QModManager) framework.

The Subnautica's Unity Engine is acting as a local host server responding to python requests for in-game information.
The initial use case is to train an agent to follow in game creatures, keeping them visible in frame.

The primary goals for this mod are to ask the unity engine to:
 - Calculate a score based on the percent screen area occupied by active creatures
 - Capture a screenshot soon after the score is calculated
    - encoded it as jpg
    - convert it to a base64string
 - Deliver both the score and base64 image string to a requesting python client

A secondary goal for this mod is to accept input joystick commands
  - Although this can be achieved with python libraries, the potential benefits of unity are: 
    - it may be more performant
    - it would provide a cleaner all-in-one interface for the training process


