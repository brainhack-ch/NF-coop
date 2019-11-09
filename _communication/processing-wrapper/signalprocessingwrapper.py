import threading
import os
import sys

# Do file relative import
relativeDirectory = os.path.dirname(os.path.abspath(__file__)) + '/../../signal_processing'
sys.path.append(relativeDirectory)

from brainHack_receiver import client, switch_paradigm, set_resting_state_duration

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

    def spawn_client(self, headsetname):
        # Spawn listening loop as a thread for process response
        self.client_thread = threading.Thread(target=self.client_thread_fn, args=(headsetname,))
        self.client_thread.daemon = True
        self.client_thread.start()

    def notify_state_requested(self, newState):
        if not isinstance(newState, str):
            print('[!] Error: Server requested a non-string state: ' + str(type(newState)))
            return

        if newState.startswith('resting'):
            duration = float(newState.split('_')[1]) # This is in seconds, needs converting to minutes
            duration = duration / 60.0

            print('[*] Server event: start resting state with duration: ' + str(duration) + 'mins')

            self.start_resting_state(duration)
        elif newState.startswith('EOF'):
            print('[*] Server event: stop')
            self.stop_client()
        else:
            print('[!] Error: Unknown state requested by the server: ' + str(newState))

    def start_resting_state(self, param):
        # Set resting duration first
        set_resting_state_duration(param)

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

    def client_thread_fn(self, headsetname):
        print('[*] Starting client thread')

        client(headsetname, self.on_resting_state_callback, self.on_new_score)

        print('[*] Cleaning up client thread')