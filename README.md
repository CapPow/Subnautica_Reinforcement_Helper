# Subnautica_Reinforcement_Helper
An effort to expose Subnautica to reinforcement training agents using the [QModManager](https://github.com/SubnauticaModding/QModManager) framework.

The Subnautica's Unity Engine is acting as a local host server responding to python requests for in-game information.
The initial use case is to train an agent to follow in-game creatures, keeping them visible in frame.

This mod exposes various commands through the NetMQ REQ/REP model at "tcp://localhost:12346".
Additionally, schooling fish are removed (via [Harmony](https://github.com/pardeike/Harmony) methods) as they appear to have no measurable bounds.

**Available REQ commands**
<pre><code>
"get_outputs" : returns a "|" delineated string containing a score, as well as agent coordinates. 
e.g., "0.2013|29.202|-102.234|-10.521". 
The score is based on the screen area occupied by creatures.
It also adds a small bonus for multiple creatures in frame.

"resetview" : resets the agent view to "0, 0, 0"

"warp|{x}, {y}, {z}" : warps the agent to the provided x, y, z coordaintes.
</code></pre>

See the provided Python class for an example implementation.
