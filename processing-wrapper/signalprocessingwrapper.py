import subprocess, threading, Queue
from os import O_NONBLOCK, read
import sys

SIGNAL_PROCESSING_SCRIPT_LOCATION = "../signal-processing/main.py"

class SignalProcessingWrapper:

    delegate = None
    subprocess = None

    # Constructor
    # @params delegate The objetc to notify when new data becomes available
    def __init__(self, delegate):
        self.delegate = delegate

    def emit_data(self, stream_update):
        self.delegate.on_new_data(stream_update)

    def spawn_processing(self):
        # Assuming that the signal processing is a self-contained script that
        # writes to stdout

        ownLocation = sys.path[0] + '/'
        self.subprocess = subprocess.Popen(["python", ownLocation + SIGNAL_PROCESSING_SCRIPT_LOCATION],
            stdin=subprocess.PIPE,
            stdout=subprocess.PIPE,
            bufsize=-1,
            universal_newlines=False)

        # Spawn listening loop as a thread for process response
        output_thread = threading.Thread(target=self.recv_thread)
        output_thread.daemon = True
        output_thread.start()

    def spawn_processing_calibration(self):
        pass

    def recv_thread(self):
        while True:
            try:
                out = self.subprocess.stdout.readline()

                if not out:
                    continue

                # Parse line as appropriate
                print(out.rstrip())

                # Parse and emit
                self.emit_data(self.parse_subprocess_line(out.rstrip()))
            except:
                pass

        self.subprocess.stdout.close()

    def parse_subprocess_line(self, line):
        return line