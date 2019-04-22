using System;
using Newtonsoft.Json;

namespace LFRun
{
    /// <summary>
    /// Represents a dated Command Record
    /// </summary>
    [JsonObject]
    public struct CommandRecord : IEquatable<CommandRecord>
    {
        public DateTime CommandDate { get; }
        public string Command { get; }

        public CommandRecord(string command)
            :this(command, DateTime.UtcNow)
        {
        }

        [JsonConstructor]
        public CommandRecord(string command, DateTime commandDate)
        {
            Command = command;
            CommandDate = commandDate;
        }

        public static bool operator ==(CommandRecord first, CommandRecord second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(CommandRecord first, CommandRecord second)
        {
            return !(first == second);
        }

        /// <summary>
        /// Compares two <see cref="CommandRecord"/> <see cref="Command"/>s using <see cref="StringComparison"/>
        /// </summary>
        /// <param name="other">The CommandRecord to compare to this one</param>
        /// <returns>True if the commands match; otherwise, false</returns>
        public bool Equals(CommandRecord other)
        {
            return string.Equals(Command, other.Command, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object other)
        {
            if (other is CommandRecord cr)
                return Equals(cr);
            return false;
        }

        public override int GetHashCode()
            => Command.GetHashCode();
    }
}
