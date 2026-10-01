using System;
using Game.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.DevTools
{
    /// <summary>
    /// Code-built UI Toolkit tree for the FPS readout and the console panel. Holds no command logic.
    /// </summary>
    public sealed class DevConsoleView
    {
        private const int MaxLogLines = 200;

        // Sizes in reference pixels (1080 wide): ~16 sp text and 48 dp touch targets on a typical phone.
        private const int FontSize = 42;
        private const int Padding = 16;
        private const int TouchTarget = 132;

        private static readonly Color PanelColor = new Color(0f, 0f, 0f, 0.8f);
        private static readonly Color TextColor = new Color(0.9f, 0.9f, 0.85f);
        private static readonly Color ButtonColor = new Color(0.25f, 0.25f, 0.25f, 0.9f);

        private readonly Label _fpsLabel;
        private readonly VisualElement _panel;
        private readonly ScrollView _log;
        private readonly TextField _input;

        public event Action<string> Submitted;

        public DevConsoleView(VisualElement root)
        {
            var safeArea = new SafeAreaElement();
            root.Add(safeArea);

            var topBar = new VisualElement { pickingMode = PickingMode.Ignore };
            topBar.style.flexDirection = FlexDirection.Row;
            topBar.style.justifyContent = Justify.SpaceBetween;
            topBar.style.alignItems = Align.FlexStart;
            safeArea.Add(topBar);

            _fpsLabel = new Label { pickingMode = PickingMode.Ignore };
            StyleText(_fpsLabel);
            _fpsLabel.style.backgroundColor = PanelColor;
            _fpsLabel.style.paddingLeft = Padding;
            _fpsLabel.style.paddingRight = Padding;
            topBar.Add(_fpsLabel);

            var toggle = MakeButton("Console", Toggle);
            topBar.Add(toggle);

            _panel = new VisualElement();
            _panel.style.flexGrow = 0;
            _panel.style.height = Length.Percent(45);
            _panel.style.backgroundColor = PanelColor;
            _panel.style.paddingLeft = Padding;
            _panel.style.paddingRight = Padding;
            _panel.style.paddingTop = Padding;
            _panel.style.paddingBottom = Padding;
            _panel.style.display = DisplayStyle.None;
            safeArea.Add(_panel);

            _log = new ScrollView(ScrollViewMode.Vertical);
            _log.style.flexGrow = 1;
            _panel.Add(_log);

            var inputRow = new VisualElement();
            inputRow.style.flexDirection = FlexDirection.Row;
            inputRow.style.marginTop = Padding;
            _panel.Add(inputRow);

            _input = new TextField();
            _input.style.flexGrow = 1;
            _input.style.fontSize = FontSize;
            _input.style.minHeight = TouchTarget;
            _input.RegisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);
            inputRow.Add(_input);

            inputRow.Add(MakeButton("Run", Submit));
        }

        public bool IsOpen => _panel.style.display == DisplayStyle.Flex;

        public void SetFpsVisible(bool visible)
        {
            _fpsLabel.style.visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }

        public void SetFpsText(string text)
        {
            _fpsLabel.text = text;
        }

        public void Toggle()
        {
            _panel.style.display = IsOpen ? DisplayStyle.None : DisplayStyle.Flex;
            if (IsOpen)
                _input.Focus();
        }

        public void AppendLog(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var line = new Label(text);
            StyleText(line);
            line.style.whiteSpace = WhiteSpace.Normal;
            _log.Add(line);

            while (_log.contentContainer.childCount > MaxLogLines)
                _log.contentContainer.RemoveAt(0);

            _log.schedule.Execute(() => _log.ScrollTo(line));
        }

        public void ClearLog()
        {
            _log.Clear();
        }

        private void OnInputKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Return && evt.keyCode != KeyCode.KeypadEnter)
                return;

            evt.StopPropagation();
            Submit();
        }

        private void Submit()
        {
            var line = _input.value;
            _input.value = string.Empty;
            if (!string.IsNullOrWhiteSpace(line))
                Submitted?.Invoke(line.Trim());
            _input.Focus();
        }

        private static Button MakeButton(string text, Action onClick)
        {
            var button = new Button(onClick) { text = text };
            button.style.fontSize = FontSize;
            button.style.color = TextColor;
            button.style.backgroundColor = ButtonColor;
            button.style.minWidth = TouchTarget * 2;
            button.style.minHeight = TouchTarget;
            return button;
        }

        private static void StyleText(Label label)
        {
            label.style.fontSize = FontSize;
            label.style.color = TextColor;
        }
    }
}
