#!/bin/local/python

import time
import sys

def main():
    while True:
        print('player1,0.45')
        print('player2,0.45')

        # Require to flush print to pipeline runner
        sys.stdout.flush()

        time.sleep(0.5)

if __name__ == '__main__':
    main()