using Game.DevTools;
using Game.Sim;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.App
{
    /// <summary>
    /// The single entry point: creates the simulation and wires every system explicitly.
    /// </summary>
    public sealed class Bootstrap : MonoBehaviour
    {
        [SerializeField] private PanelSettings _panelSettings;

        private GameSimulation _simulation;
        private SimClock _clock;
        private DevConsole _devConsole;

        private void Awake()
        {
            if (_panelSettings == null)
                Debug.LogError("Bootstrap: PanelSettings is not assigned. Run 'The Murk/Rebuild Bootstrap Scene'.", this);

            _simulation = new GameSimulation();
            _clock = new SimClock();

            var uiDocument = CreateUiDocument("DevConsoleUI", sortingOrder: 1000);
            _devConsole = new DevConsole(uiDocument.rootVisualElement, new DebugCommandRegistry());
            SimCommands.Register(_devConsole.Commands, _simulation, _clock);
        }

        private void Update()
        {
            var ticks = _clock.Advance(Time.unscaledDeltaTime);
            for (var i = 0; i < ticks; i++)
                _simulation.Step();

            _devConsole.Update(Time.unscaledDeltaTime);
        }

        private UIDocument CreateUiDocument(string name, int sortingOrder)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            var document = go.AddComponent<UIDocument>();
            document.panelSettings = _panelSettings;
            document.sortingOrder = sortingOrder;
            return document;
        }
    }
}
