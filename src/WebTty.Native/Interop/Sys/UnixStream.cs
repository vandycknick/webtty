using System;
using System.IO;

namespace WebTty.Native.Interop
{
    internal sealed unsafe class UnixStream : Stream, IDisposable
    {
        private const int InvalidFileDescriptor = -1;

        private readonly bool _canRead;
        private readonly bool _canSeek;
        private readonly bool _canWrite;
        private readonly int _fd;

        private stat _stat;

        public UnixStream(int fileDescriptor)
        {
            if (InvalidFileDescriptor == fileDescriptor)
                throw new ArgumentException(nameof(fileDescriptor));

            _fd = fileDescriptor;

            long offset = Libc.lseek(fileDescriptor, (long)0, Libc.SEEK_CUR);
            if (offset != -1)
            {
                _canSeek = true;
            }

            long read = Libc.read(fileDescriptor, (void*)IntPtr.Zero, 0);
            if (read != -1)
            {
                _canRead = true;
            }

            long write = Libc.write(fileDescriptor, (void*)IntPtr.Zero, 0);
            if (write != -1)
            {
                _canWrite = true;
            }
        }

        public override bool CanRead => _canRead;

        public override bool CanSeek => _canSeek;

        public override bool CanWrite => _canWrite;

        public override long Length
        {
            get
            {
                AssertNotDisposed();
                if (!CanSeek)
                    throw new NotSupportedException("File descriptor doesn't support seeking");
                LoadStat();
                return _stat.st_size;
            }
        }

        public override long Position
        {
            get
            {
                AssertNotDisposed();
                if (!CanSeek)
                    throw new NotSupportedException("The stream does not support seeking");

                long pos = Libc.lseek(_fd, 0, Libc.SEEK_CUR);
                if (pos == -1)
                {
                    Sys.ThrowExceptionForLastError();
                }
                return (long)pos;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            AssertNotDisposed();
            AssertValidBuffer(buffer, offset, count);
            if (!CanRead)
                throw new NotSupportedException("Stream does not support reading");

            if (buffer.Length == 0) return 0;

            long retval = 0;
            fixed (byte* buf = &buffer[offset])
            {
                do
                {
                    retval = Libc.read(_fd, buf, (uint)count);
                } while (Sys.ShouldRetrySyscall((int)retval));
            }

            if (retval == -1) Sys.ThrowExceptionForLastError();

            return (int)retval;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            AssertNotDisposed();
            if (!CanSeek)
            {
                throw new NotSupportedException("The File Descriptor does not support seeking");
            }

            var sf = Libc.SEEK_CUR;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    sf = Libc.SEEK_SET;
                    break;
                case SeekOrigin.Current:
                default:
                    sf = Libc.SEEK_CUR;
                    break;
                case SeekOrigin.End:
                    sf = Libc.SEEK_END;
                    break;
            }

            long pos = Libc.lseek(_fd, offset, sf);
            if (pos == -1) Sys.ThrowExceptionForLastError();

            return (long)pos;
        }

        public override void SetLength(long value)
        {
            AssertNotDisposed();
            if (value < 0)
                throw new ArgumentOutOfRangeException("value", "< 0");
            if (!CanSeek && !CanWrite)
                throw new NotSupportedException("You can't truncating the current file descriptor");

            int retval;
            do
            {
                retval = Libc.ftruncate(_fd, value);
            } while (Sys.ShouldRetrySyscall(retval));

            if (retval == -1) Sys.ThrowExceptionForLastError();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            AssertNotDisposed();
            AssertValidBuffer(buffer, offset, count);

            if (!CanWrite) throw new NotSupportedException("File Descriptor does not support writing");

            if (buffer.Length == 0) return;

            long retval = 0;
            fixed (byte* buf = &buffer[offset])
            {
                do
                {
                    retval = Libc.write(_fd, buf, (uint)count);
                } while (Sys.ShouldRetrySyscall((int)retval));
            }

            if (retval == -1) Sys.ThrowExceptionForLastError();
        }

        private void AssertNotDisposed()
        {
            if (_fd == InvalidFileDescriptor)
                throw new ObjectDisposedException("Invalid File Descriptor");
        }

        private void AssertValidBuffer(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "< 0");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "< 0");
            if (offset > buffer.Length)
                throw new ArgumentException("destination offset is beyond array size");
            if (offset > (buffer.Length - count))
                throw new ArgumentException("would overrun buffer");
        }

        private void LoadStat()
        {
            int r = Libc.fstat(_fd, out _stat);
            Sys.ThrowExceptionForLastErrorIf(r);
        }
    }
}
