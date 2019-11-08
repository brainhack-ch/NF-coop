import redis

REDIS_HOST = "localhost"
REDIS_PORT = 6379

# Utility functions around pushing data into Redis
class RedisClient:

    connection = None

    def __init__(self):
        pass

    def setup_connection(self):
        print('[*] Connecting to Redis instance...')
        self.connection = redis.Redis(host=REDIS_HOST, port=REDIS_PORT, db=0)

        try:
            response = self.connection.client_list()
        except redis.ConnectionError:
            print('[!] Failed to connect to Redis instance!')
            return -1

        return 0

    def push_data(self, datapoint, queue_name):
        pass

    # Called by signal processing wrapper on stdout messages
    def on_new_data(self, stream_update):
        self.push_data(stream_update[0], stream_update[1:])