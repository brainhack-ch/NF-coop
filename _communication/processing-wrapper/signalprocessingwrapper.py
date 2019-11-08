import subprocess, threading, Queue
import os
import sys

SIGNAL_PROCESSING_SCRIPT_LOCATION = "/mockdata.py"

class SignalProcessingWrapper:

    delegate = None
    subprocess = None

    # Constructor
    # @params delegate The objetc to notify when new data becomes available
    def __init__(self, delegate):
        self.delegate = delegate

    def emit_data(self, stream_update):
        self.delegate.on_new_data(stream_update)

    def alive(self):
        return self.subprocess.poll() is None

    def spawn_processing(self):
        # Assuming that the signal processing is a self-contained script that
        # writes to stdout

        ownLocation = sys.path[0] + '/'
        path = ownLocation + SIGNAL_PROCESSING_SCRIPT_LOCATION

        print('[*] Starting signal processing subprocess at: ' + path)

        if not os.path.isfile(path):
            print('[!] Cannot find subprocess for signal processing')
            return

        try:
            self.subprocess = subprocess.Popen(["python", path],
                stdout=subprocess.PIPE,
                bufsize=-1,
                universal_newlines=False)
        except:
            print('[!] Failed to initialise subprocess for signal processing')
            return

        # Spawn listening loop as a thread for process response
        self.output_thread = threading.Thread(target=self.recv_thread)
        self.output_thread.daemon=True
        self.output_thread.start()

    def spawn_processing_calibration(self):
        pass

    def recv_thread(self):
        while self.subprocess.poll() is None:
            out = self.subprocess.stdout.readline()

            if not out:
                continue

            # Parse and emit
            self.emit_data(self.parse_subprocess_line(out.rstrip()))

        print('[*] Cleaning up subprocess')
        self.subprocess.stdout.close()

    def parse_subprocess_line(self, line):
        # Assuming format is:
        # playerId,val
        return line.split(',')