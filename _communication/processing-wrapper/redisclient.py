import redis

REDIS_HOST = "localhost"
REDIS_PORT = 6379

# Utility functions around pushing data into Redis
class RedisClient:

    connection = None
    queue_name = ''

    state_callback = None

    def __init__(self, headsetname):
        self.queue_name = self.map_demo_headset_name_to_queue(headsetname)

    def shutdown(self):
        self.connection.close()

    def setup_callback(self, requested_state_changed_callback):
        self.state_callback = requested_state_changed_callback

    def setup_connection(self):
        print('[*] Connecting to Redis instance...')
        self.connection = redis.Redis(host=REDIS_HOST, port=REDIS_PORT, db=0)

        try:
            response = self.connection.client_list()
        except redis.ConnectionError:
            print('[!] Failed to connect to Redis instance!')
            return -1

        return 0

    def map_demo_headset_name_to_queue(self, name):
        if name == 'unknown1':
            return 'player_0'
        elif name == 'unknown2':
            return 'player_1'
        else:
            return ''

    def push_data(self, datapoint):
        self.connection.rpush(self.queue_name, datapoint)

    # Called by signal processing wrapper on stdout messages
    def on_new_data(self, stream_update):
        self.push_data(stream_update)