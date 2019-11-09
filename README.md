# NF-coop
Cooperative Neurofeedback - A multiplayer game controlled with your brain

# Client-side

Python dependencies:

- neurodecode
- redis
- pathlib

`NeuroDecode` can be installed via the following steps:

1. `git submodule init` then `git submodule update`
2. `cd _communication/deps/NeuroDecode`
3. `python{3} setup.py develop`



# Server-side

The server side is a single redis instance.
This can be started on a machine with Docker installed using `_communication/docker/start.sh`