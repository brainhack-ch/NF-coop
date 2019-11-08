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

    redisClient = RedisClient()
    signalProcessor = SignalProcessingWrapper(redisClient)

    # Add handler for SIGINT
    signal.signal(signal.SIGINT, signal_handler)

    if redisClient.setup_connection() == -1:
        sys.exit(-1)

    # Undertake any calibration
    signalProcessor.spawn_processing_calibration()

    # Run underlying signal processing
    try:
        signalProcessor.spawn_processing()
        while signalProcessor.alive():
            time.sleep(1)
    except KeyboardInterrupt:
        pass

    redisClient.shutdown()

if __name__ == "__main__":
    main()