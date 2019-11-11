using System;
using System.IO;

namespace WebTty.IO
{
    public interface IConsole
    {
        /// <summary>
        /// Gets the standard input stream.
        /// </summary>
        /// <returns>
        /// A System.IO.TextReader that represents the standard input stream.
        /// </returns>
        TextReader In { get; }

        /// <summary>
        /// Gets the standard output stream.
        /// </summary>
        /// <returns>
        /// A System.IO.TextWriter that represents the standard output stream.
        /// </returns>
        TextWriter Out { get; }

        /// <summary>
        /// Gets the standard error output stream.
        /// </summary>
        /// <returns>
        /// A System.IO.TextWriter that represents the standard error output stream.
        /// </returns>
        TextWriter Error { get; }

        /// <summary>
        /// Gets a value that indicates whether input has been redirected from the standard
        /// input stream.
        /// </summary>
        /// <returns>
        /// true if input is redirected; otherwise, false.
        /// </returns>
        bool IsInputRedirected { get; }

        /// <summary>
        /// Gets a value that indicates whether output has been redirected from the standard
        /// output stream.
        /// </summary>
        /// <returns>
        /// true if output is redirected; otherwise, false.
        /// </returns>
        bool IsOutputRedirected { get; }

        /// <summary>
        /// Gets a value that indicates whether the error output stream has been redirected
        /// from the standard error stream.
        /// </summary>
        /// <returns>
        /// true if error output is redirected; otherwise, false.
        /// </returns>
        bool IsErrorRedirected { get; }

        /// <summary>
        /// Gets or sets the background color of the console.
        /// </summary>
        /// <returns>
        /// A value that specifies the background color of the console; that is, the color
        /// that appears behind each character. The default is black.
        /// </returns>
        ConsoleColor BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color of the console.
        /// </summary>
        /// <returns>
        /// A System.ConsoleColor that specifies the foreground color of the console; that
        /// is, the color of each character that is displayed. The default is gray.
        /// </returns>
        ConsoleColor ForegroundColor { get; set; }

        /// <summary>
        /// Sets the foreground and background console colors to their defaults.
        /// </summary>
        void ResetColor();
    }
}
