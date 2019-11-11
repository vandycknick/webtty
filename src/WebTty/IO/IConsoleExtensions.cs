using System;

namespace WebTty.IO
{
    public static class IConsoleExtensions
    {
        /// <summary>
        /// Writes a line terminator to the out stream.
        /// </summary>
        public static void WriteLine(this IConsole console) =>
            console.Out.WriteLine();

        /// <summary>
        /// Writes the given string of characters tot the out stream, followed
        /// by a line terminator.
        /// </summary>
        /// <param name="input">The characters to write.</param>
        public static void WriteLine(this IConsole console, ReadOnlySpan<char> input) =>
            console.Out.WriteLine(input);

        /// <summary>
        /// Writes a given string to the out stream.
        /// </summary>
        /// <param name="input">The characters to write.</param>
        public static void Write(this IConsole console, ReadOnlySpan<char> input) =>
            console.Out.Write(input);
    }
}
