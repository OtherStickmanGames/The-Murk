using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.DevTools
{
    /// <summary>
    /// Named console commands. Each system registers its own cheats here at bootstrap.
    /// A handler receives the arguments after the command name and returns text for the log.
    /// </summary>
    public sealed class DebugCommandRegistry
    {
        private readonly Dictionary<string, DebugCommand> _commands =
            new Dictionary<string, DebugCommand>(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<DebugCommand> Commands => _commands.Values.OrderBy(c => c.Name, StringComparer.Ordinal);

        public void Register(string name, string help, Func<string[], string> handler)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Any(char.IsWhiteSpace))
                throw new ArgumentException($"Invalid command name '{name}'.", nameof(name));
            if (_commands.ContainsKey(name))
                throw new InvalidOperationException($"Command '{name}' is already registered.");

            _commands[name] = new DebugCommand(name, help, handler ?? throw new ArgumentNullException(nameof(handler)));
        }

        public string Execute(string line)
        {
            var tokens = Tokenize(line);
            if (tokens.Count == 0)
                return string.Empty;

            if (!_commands.TryGetValue(tokens[0], out var command))
                return $"Unknown command '{tokens[0]}'. Type 'help'.";

            try
            {
                return command.Handler(tokens.Skip(1).ToArray()) ?? string.Empty;
            }
            catch (Exception e)
            {
                return $"{command.Name} failed: {e.Message}";
            }
        }

        /// <summary>Splits on whitespace; double quotes group words into one argument.</summary>
        public static List<string> Tokenize(string line)
        {
            var tokens = new List<string>();
            if (string.IsNullOrEmpty(line))
                return tokens;

            var current = new StringBuilder();
            var inQuotes = false;
            var hasToken = false;

            foreach (var c in line)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    hasToken = true;
                }
                else if (char.IsWhiteSpace(c) && !inQuotes)
                {
                    if (hasToken)
                        tokens.Add(current.ToString());
                    current.Clear();
                    hasToken = false;
                }
                else
                {
                    current.Append(c);
                    hasToken = true;
                }
            }

            if (hasToken)
                tokens.Add(current.ToString());
            return tokens;
        }
    }
}
