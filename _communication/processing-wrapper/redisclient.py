import redis
import time
import threading

REDIS_HOST = "192.168.36.92"
REDIS_PORT = 6379

SERVER_KEY_NAME = 'KeyfromUnity'

# Utility functions around pushing data into Redis
class RedisClient:

    connection = None
    event_thread = None
    queue_name = ''
    state_callback = None
    shutdown_requested = False
    last_server_event = -1

    def __init__(self, headsetname):
        self.queue_name = self.map_demo_headset_name_to_queue(headsetname)

    def shutdown(self):
        self.shutdown_requested = True
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
        if name == 'P04/01':
            return 'player_0'
        elif name == 'P04/02':
            return 'player_1'
        else:
            return ''

    def push_data(self, datapoint):
        # Format:
        # ( timestamp, state, value )
        # state is [0|1]; 0 == resting, 1 == gaming
        # value is [0,1]
        formattedPoint = '(' + str(int(time.time() * 1000)) + ',' + '1' + ',' + str(datapoint) + ')'
        print('[*] Pushing ' + formattedPoint + ' to list: ' + self.queue_name)

        self.connection.rpush(self.queue_name, formattedPoint)

    # Called by signal processing wrapper on stdout messages
    def on_new_data(self, stream_update):
        self.push_data(stream_update)

    def event_thread_fn(self):
        print('[*] Starting server event thread')

        while self.shutdown_requested == False:
            server_event = self.connection.get(SERVER_KEY_NAME)

            if server_event != None and server_event != self.last_server_event:
                self.state_callback(server_event)
                self.last_server_event = server_event

            time.sleep(1)

        print('[*] Cleaning up server event thread')