import redis

REDIS_HOST = "localhost"
REDIS_PORT = 6379

BUFFER_TIMEOUT = 150
QUEUE_NAME = 'input'

# Utility functions around pushing data into Redis
class RedisClient:

    connection = None
    next_buffer = None
    known_player_ids = []

    def __init__(self):
        self.reset_buffer()

    def shutdown(self):
        self.connection.close()

    def setup_connection(self):
        print('[*] Connecting to Redis instance...')
        self.connection = redis.Redis(host=REDIS_HOST, port=REDIS_PORT, db=0)

        try:
            response = self.connection.client_list()
        except redis.ConnectionError:
            print('[!] Failed to connect to Redis instance!')
            return -1

        return 0

    # Assumes two player IDs
    def reset_buffer(self):
        self.next_buffer = [-1, -1]

    # Indexes any given player ID to an internal tuple index
    def index_for_player(self, playerid):
        if playerid in self.known_player_ids:
            return self.known_player_ids.index(playerid)
        else:
            self.known_player_ids.append(playerid)
            return len(self.known_player_ids) - 1

    def buffer_filled(self):
        is_missing = False

        for item in self.next_buffer:
            if item == -1:
                is_missing = True
                break

        return is_missing == False

    def stringify_buffer(self):
        output = ''
        for i in range(len(self.next_buffer)):
            output = output + (',' if i > 0 else '') + self.next_buffer[i]

        return output

    def push_data(self, player, datapoint):
        index = self.index_for_player(player)
        if self.next_buffer[index] > -1:
            print('[!] WARNING: Player ' + player + ' already has pending data in the next buffer')

        self.next_buffer[index] = datapoint

        if self.buffer_filled():
            print('[*] Pushing buffer: ' + self.stringify_buffer())
            self.connection.rpush(QUEUE_NAME, self.stringify_buffer())
            self.reset_buffer()
        else:
            print('[*] Holding buffer')

    # Called by signal processing wrapper on stdout messages
    def on_new_data(self, stream_update):
        self.push_data(stream_update[0], stream_update[1])