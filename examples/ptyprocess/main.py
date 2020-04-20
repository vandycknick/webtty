import tty
import sys
import io
import subprocess

from ptyprocess import PtyProcess

from timeit import default_timer as timer


proc = PtyProcess.spawn(["pwsh", "-command", "Get-Date; ls --color=auto; Get-Date; stty -a; tty"], cwd="/Users/nickvd/Projects/webtty")


mode = tty.tcgetattr(proc.fileno())
print(mode)

stdout = io.open(proc.fd, 'rb', 0)

while proc.isalive():
    try:
        start = timer()
        result = stdout.read(1024)
        print("Read took {}s".format(timer() - start))
    except:
        pass

    sys.stdout.write(result.decode("utf-8"))
    sys.stdout.flush()

# print(st)
print("finished")
