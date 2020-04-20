import os
import sys

print(os.ttyname(sys.stdin.fileno()))
print(os.ttyname(sys.stdout.fileno()))
print(os.ttyname(sys.stderr.fileno()))
