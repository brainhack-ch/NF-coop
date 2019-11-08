from signalprocessingwrapper import SignalProcessingWrapper
from redisclient import RedisClient

import signal
import sys

def signal_handler(sig, frame):
    print('You pressed Ctrl+C!')
    sys.exit(0)

def main():
    redisClient = RedisClient()
    signalProcessor = SignalProcessingWrapper(redisClient)

    # Add handler for SIGINT
    signal.signal(signal.SIGINT, signal_handler)

    if redisClient.setup_connection() == -1:
        sys.exit(-1)

    # Undertake any calibration
    signalProcessor.spawn_processing_calibration()

    # Run underlying signal processing
    signalProcessor.spawn_processing()

    pass

def setup_data_streaming(ip, port):
    pass

if __name__ == "__main__":
    main()