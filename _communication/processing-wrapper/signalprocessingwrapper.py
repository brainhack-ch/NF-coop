import threading
import os
import sys

# Do file relative import
relativeDirectory = os.path.dirname(os.path.abspath(__file__)) + '/../../signal_processing'
sys.path.append(relativeDirectory)

from brainHack_receiver import client, switch_paradigm

class SignalProcessingWrapper:

    delegate = None
    client_thread = None

    # Constructor
    # @params delegate The object to notify when new data becomes available
    def __init__(self, delegate):
        self.delegate = delegate

    def emit_data(self, stream_update):
        self.delegate.on_new_data(stream_update)

    def alive(self):
        return self.client_thread.isAlive()

    def spawn_client(self, headsetname, resting_duration):
        # Spawn listening loop as a thread for process response
        self.client_thread = threading.Thread(target=self.client_thread_fn, args=(headsetname,resting_duration,))
        self.client_thread.daemon = True
        self.client_thread.start()

    def notify_state_requested(self, newState):
        if newState == 0:
            print('[*] Server event: start resting state')
            self.start_resting_state()
        elif newState == 1:
            print('[*] Server event: stop')
            self.stop_client()
        else:
            print('[!] Error: Unknown state requested by the server')

    def start_resting_state(self):
        # Switch to the resting state mode
        switch_paradigm('resting_state')

    def stop_client(self):
        # Switch to the default mode
        switch_paradigm('')

    def on_new_score(self, score):
        self.emit_data(score)

    def on_resting_state_callback(self, state):
        # Start emitting real data up to redis
        switch_paradigm('gaming')

    def client_thread_fn(self, headsetname, resting_duration):
        print('[*] Starting client thread')

        client(headsetname, resting_duration, self.on_resting_state_callback, self.on_new_score)

        print('[*] Cleaning up client thread')