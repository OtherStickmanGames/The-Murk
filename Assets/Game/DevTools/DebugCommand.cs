using System;

namespace Game.DevTools
{
    public sealed class DebugCommand
    {
        public DebugCommand(string name, string help, Func<string[], string> handler)
        {
            Name = name;
            Help = help;
            Handler = handler;
        }

        public string Name { get; }
        public string Help { get; }
        public Func<string[], string> Handler { get; }
    }
}
