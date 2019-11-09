import redis
import time
import threading

REDIS_HOST = "localhost"
REDIS_PORT = 6379

SERVER_KEY_NAME = 'KeyfromUnity'

# Utility functions around pushing data into Redis
class RedisClient:

    connection = None
    event_thread = None
    queue_name = ''
    state_callback = None
    shutdown = False
    last_server_event = -1

    def __init__(self, headsetname):
        self.queue_name = self.map_demo_headset_name_to_queue(headsetname)

    def shutdown(self):
        self.shutdown = True
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

        # Start polling for server events
        # Spawn listening loop as a thread for process response
        self.event_thread = threading.Thread(target=self.event_thread_fn)
        self.event_thread.daemon = True
        self.event_thread.start()

        return 0

    def map_demo_headset_name_to_queue(self, name):
        if name == 'unknown1':
            return 'player_0'
        elif name == 'unknown2':
            return 'player_1'
        else:
            return ''

    def push_data(self, datapoint):
        # Format:
        # ( timestamp, state, value )
        # state is [0|1]; 0 == resting, 1 == gaming
        # value is [0,1]
        formattedPoint = '(' + int(time.time()) + ',' + '1' + ',' + str(datapoint) + ')'
        print('[*] Pushing ' + formattedPoint)

        self.connection.rpush(self.queue_name, formattedPoint)

    # Called by signal processing wrapper on stdout messages
    def on_new_data(self, stream_update):
        self.push_data(stream_update)

    def event_thread_fn(self):
        print('[*] Starting server event thread')

        while self.shutdown == False:
            server_event = self.connection.get(SERVER_KEY_NAME)
            if server_event != None and int(server_event) != self.last_server_event:
                self.state_callback(int(server_event))
                self.last_server_event = int(server_event)

            time.sleep(1)

        print('[*] Cleaning up server event thread')