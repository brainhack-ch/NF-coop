# NF-coop
Cooperative Neurofeedback - A multiplayer game controlled with your brain

# Server-side

The server side is a single redis instance.
This can be started on a machine with Docker installed using `_communication/docker/start.sh`

# Client-side

Python dependencies:

- neurodecode
- redis
- pathlib

`NeuroDecode` can be installed via the following steps:

1. `git submodule init` then `git submodule update`
2. `cd _communication/deps/NeuroDecode`
3. `python{3} setup.py develop`

Data is sent to the server-side on a redis queue per player. This is in the format:

"(<unix-timestamp>, [0|1], <value>)", where the second parameter indicates whether the data is sent during
a resting state (0) or gaming state (1).

# Unity

The Unity game sends back commands to clients by writing to a redis key with the following
values:

"resting_<duration>" -> signifies clients to start resting state with the given duration
"EOF" -> signifies end of a game

