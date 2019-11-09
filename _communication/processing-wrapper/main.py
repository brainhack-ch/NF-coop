#!/bin/local/python

from signalprocessingwrapper import SignalProcessingWrapper
from redisclient import RedisClient

import signal
import sys
import time

import pylsl

do_exit = False

DEBUG_MODE = 1

def signal_handler(sig, frame):
    do_exit = True

def choose_headset():
    streamInfos = pylsl.resolve_streams()
    for i in range(len(streamInfos)):
        si = streamInfos[i]
        if si.name() != 'StreamRecorderInfo':
            print('[' + str(i + 1) + '] Headset: ' + si.name())

    print('Choose headset to use (e.g. 1):')
    userInput = input().strip()

    headset = streamInfos[int(userInput) - 1]

    return headset.name()

def main():
    global do_exit

    # Get headset name
    headsetname = choose_headset()

    redisClient = RedisClient(headsetname)
    signalProcessor = SignalProcessingWrapper(redisClient)

    # Add handler for SIGINT
    signal.signal(signal.SIGINT, signal_handler)

    if redisClient.setup_connection() == -1:
        sys.exit(-1)

    redisClient.setup_callback(signalProcessor.notify_state_requested)

    # Run underlying signal processing
    try:
        start_time = time.time()
        last_mode = -1

        signalProcessor.spawn_client(headsetname)
        while signalProcessor.alive() and not do_exit:
            time.sleep(1)

            if DEBUG_MODE and time.time() - start_time > 5 and last_mode == -1:
                print('[*] DEBUG MODE: Going to resting state calibration, then auto-switch to gaming')
                signalProcessor.notify_state_requested(0)
                last_mode = 0
    except KeyboardInterrupt:
        pass

    redisClient.shutdown()

if __name__ == "__main__":
    main()