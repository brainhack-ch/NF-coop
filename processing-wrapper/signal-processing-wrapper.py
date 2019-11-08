class SignalProcessingWrapper:

    delegate = None

    # Constructor
    # @params delegate The objetc to notify when new data becomes available
    def __init__(delegate):
        self.delegate = delegate

    def emit_data(stream_update):
        self.delegate.on_new_data(stream_update)

    def spawn_processing():
        pass

    def spawn_processing_calibration():
        pass