using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Game.DevTools
{
    /// <summary>
    /// Debug console: FPS readout plus a command line. Driven by the bootstrap each frame; not a MonoBehaviour.
    /// </summary>
    public sealed class DevConsole
    {
        private readonly DevConsoleView _view;
        private readonly FpsMeter _fpsMeter = new FpsMeter();

        public DevConsole(VisualElement root, DebugCommandRegistry commands)
        {
            Commands = commands;
            _view = new DevConsoleView(root);
            _view.Submitted += OnSubmitted;
            _view.SetFpsText("-- fps");
            RegisterBuiltInCommands();
        }

        public DebugCommandRegistry Commands { get; }

        public void Update(float unscaledDeltaTime)
        {
            if (_fpsMeter.Sample(unscaledDeltaTime))
                _view.SetFpsText($"{_fpsMeter.Fps:0} fps  {_fpsMeter.AverageMs:0.0} ms  max {_fpsMeter.WorstMs:0.0}");

            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.backquoteKey.wasPressedThisFrame)
                _view.Toggle();
        }

        public void Log(string text)
        {
            _view.AppendLog(text);
        }

        private void OnSubmitted(string line)
        {
            _view.AppendLog("> " + line);
            _view.AppendLog(Commands.Execute(line));
        }

        private void RegisterBuiltInCommands()
        {
            Commands.Register("help", "List commands.", _ =>
                string.Join("\n", Commands.Commands.Select(c => $"{c.Name} — {c.Help}")));

            Commands.Register("clear", "Clear the console log.", _ =>
            {
                _view.ClearLog();
                return string.Empty;
            });

            Commands.Register("fps", "fps on|off — show or hide the FPS counter.", args =>
            {
                if (args.Length != 1 || (args[0] != "on" && args[0] != "off"))
                    return "Usage: fps on|off";
                _view.SetFpsVisible(args[0] == "on");
                return "FPS counter " + args[0];
            });
        }
    }
}
