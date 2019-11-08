from signalprocessingwrapper import SignalProcessingWrapper
from redisclient import RedisClient

def main():
    redisClient = RedisClient()
    signalProcessor = SignalProcessingWrapper(redisClient)

    # Undertake any calibration
    signalProcessor.spawn_processing_calibration()

    # Run underlying signal processing
    signalProcessor.spawn_processing()

    pass

def setup_data_streaming(ip, port):
    pass

if __name__ == "__main__":
    main()