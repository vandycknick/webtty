#!/usr/bin/env python
# -*- coding: utf-8 -*-

import errno
import os
import tty
import sys
import subprocess
import termios
import struct
import fcntl

def set_winsize(fd, row, col, xpix=0, ypix=0):
    winsize = struct.pack("HHHH", row, col, xpix, ypix)
    fcntl.ioctl(fd, termios.TIOCSWINSZ, winsize)

pty_width = 80
pty_height = 24
args = ['/usr/local/bin/pwsh', '-command', 'Get-Date; ls; Get-Date; tty']
# args = ['python', 'tty-names.py']

master_fd, slave_fd = os.openpty()

print("Slave:", os.ttyname(slave_fd))
set_winsize(master_fd, pty_height, pty_width)

# spawn child process and redirect output to PTY
proc = subprocess.Popen(args, bufsize=0,stdout=slave_fd, stderr=slave_fd, cwd="/Users/nickvd/Projects/webtty")
os.close(slave_fd) # if we do not write to process, close this.

while True:
    try:
        output = os.read(master_fd, 1024).decode("utf-8")
        if sys.version_info.major == 3:
            sys.stdout.write(output)
            sys.stdout.flush()
        else:
            sys.stdout.write(output.encode("utf-8"))
            sys.stdout.flush()
    except OSError as e:
        if e.errno != errno.EIO: raise
        output = ""

    if not output:
        proc.wait()
        break
