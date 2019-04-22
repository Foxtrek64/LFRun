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
            return string.Equals(first.Command, second.Command, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator !=(CommandRecord first, CommandRecord second)
        {
            return !(first == second);
        }

        public bool Equals(CommandRecord other)
        {
            return Command == other.Command;
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
