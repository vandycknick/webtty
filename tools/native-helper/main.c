#include <stdio.h>
#include <errno.h>
#include <sys/ioctl.h>
#include <unistd.h>
#include <fcntl.h>

void print_errno();
void print_ioctl();
void print_fcntl();

int main()
{

    print_errno();

    print_ioctl();

    print_fcntl();
}

void print_errno()
{
    printf("=== ERRNO ===\n");
    printf("EPERM is (int) 0x%x\n", EPERM);
    printf("ENOENT is (int) 0x%x\n", ENOENT);
    printf("ESRCH is (int) 0x%x\n", ESRCH);
    printf("EINTR is (int) 0x%x\n", EINTR);
    printf("EIO is (int) 0x%x\n", EIO);
    printf("ENXIO is (int) 0x%x\n", ENXIO);
    printf("E2BIG is (int) 0x%x\n", E2BIG);
    printf("ENOEXEC is (int) 0x%x\n", ENOEXEC);
    printf("EBADF is (int) 0x%x\n", EBADF);
    printf("ECHILD is (int) 0x%x\n", ECHILD);
    printf("EAGAIN is (int) 0x%x\n", EAGAIN);
    printf("ENOMEM is (int) 0x%x\n", ENOMEM);
    printf("EACCES is (int) 0x%x\n", EACCES);
    printf("EFAULT is (int) 0x%x\n", EFAULT);
    printf("ENOTBLK is (int) 0x%x\n", ENOTBLK);
    printf("EBUSY is (int) 0x%x\n", EBUSY);
    printf("EEXIST is (int) 0x%x\n", EEXIST);
    printf("EXDEV is (int) 0x%x\n", EXDEV);
    printf("ENODEV is (int) 0x%x\n", ENODEV);
    printf("ENOTDIR is (int) 0x%x\n", ENOTDIR);
    printf("EISDIR is (int) 0x%x\n", EISDIR);
    printf("EINVAL is (int) 0x%x\n", EINVAL);
    printf("ENFILE is (int) 0x%x\n", ENFILE);
    printf("EMFILE is (int) 0x%x\n", EMFILE);
    printf("ENOTTY is (int) 0x%x\n", ENOTTY);
    printf("ETXTBSY is (int) 0x%x\n", ETXTBSY);
    printf("EFBIG is (int) 0x%x\n", EFBIG);
    printf("ENOSPC is (int) 0x%x\n", ENOSPC);
    printf("ESPIPE is (int) 0x%x\n", ESPIPE);
    printf("EROFS is (int) 0x%x\n", EROFS);
    printf("EMLINK is (int) 0x%x\n", EMLINK);
    printf("EPIPE is (int) 0x%x\n", EPIPE);
    printf("EDOM is (int) 0x%x\n", EDOM);
    printf("ERANGE is (int) 0x%x\n", ERANGE);
    printf("EDEADLK is (int) 0x%x\n", EDEADLK);
    printf("ENAMETOOLONG is (int) 0x%x\n", ENAMETOOLONG);
    printf("ENOLCK is (int) 0x%x\n", ENOLCK);
    printf("ENOSYS is (int) 0x%x\n", ENOSYS);
    printf("ENOTEMPTY is (int) 0x%x\n", ENOTEMPTY);
    printf("ELOOP is (int) 0x%x\n", ELOOP);
    printf("EWOULDBLOCK is (int) 0x%x\n", EWOULDBLOCK);
    printf("ENOMSG is (int) 0x%x\n", ENOMSG);
    printf("EIDRM is (int) 0x%x\n", EIDRM);
    printf("=== ERRNO ===\n\n");
}

void print_ioctl()
{
    printf("=== IOCTL ===\n");
    printf("TIOCSWINSZ is (long) 0x%lx\n", TIOCSWINSZ);
    printf("TIOCSCTTY is (int) 0x%x\n", TIOCSCTTY);
    printf("TIOCNOTTY is (int) 0x%x\n", TIOCNOTTY);
    printf("=== IOCTL ===\n\n");
}

void print_fcntl()
{
    printf("=== FCNTL ===\n");
    printf("F_SETFD is (int) 0x%x\n", F_SETFD);
    printf("O_CLOEXEC is (int) 0x%x\n", O_CLOEXEC);
    printf("FD_CLOEXEC is (int) 0x%x\n", FD_CLOEXEC);
    printf("=== FCNTL ===\n\n");
}
