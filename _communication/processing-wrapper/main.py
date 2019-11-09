#!/bin/local/python

from signalprocessingwrapper import SignalProcessingWrapper
from redisclient import RedisClient

import signal
import sys
import time

do_exit = False

def signal_handler(sig, frame):
    do_exit = True

def main():
    global do_exit

    # Get headset name - initially as argv[1]
    headsetname = 'unknown1'

    redisClient = RedisClient(headsetname)
    signalProcessor = SignalProcessingWrapper(redisClient)

    # Add handler for SIGINT
    signal.signal(signal.SIGINT, signal_handler)

    if redisClient.setup_connection() == -1:
        sys.exit(-1)

    redisClient.setup_callback(signalProcessor.notify_state_requested)

    # Run underlying signal processing
    try:
        signalProcessor.spawn_client(headsetname)
        while signalProcessor.alive() and not do_exit:
            time.sleep(1)
    except KeyboardInterrupt:
        print('KEYBOARD INTERRUPT')
        pass

    redisClient.shutdown()

if __name__ == "__main__":
    main()