import redis

REDIS_HOST = "localhost"
REDIS_PORT = 6379

# Utility functions around pushing data into Redis
class RedisClient:

    connection = None

    def __init__(self):
        self.setup_connection()

    def setup_connection(self):
        self.connection = redis.Redis(host=REDIS_HOST, port=REDIS_PORT, db=0)

    def push_data(self, datapoint, queue_name):
        pass

    # Called by signal processing wrapper on stdout messages
    def on_new_data(self, stream_update):
        pass