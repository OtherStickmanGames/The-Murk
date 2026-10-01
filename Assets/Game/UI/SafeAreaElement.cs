using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
    /// <summary>
    /// Full-screen container whose padding keeps children out of notches and system bars.
    /// </summary>
    public sealed class SafeAreaElement : VisualElement
    {
        private const long RefreshIntervalMs = 500;

        private Rect _appliedSafeArea;
        private Vector2Int _appliedScreenSize;

        public SafeAreaElement()
        {
            pickingMode = PickingMode.Ignore;
            style.position = Position.Absolute;
            style.left = 0;
            style.top = 0;
            style.right = 0;
            style.bottom = 0;

            RegisterCallback<AttachToPanelEvent>(_ => Refresh(force: true));
            // Safe area changes on rotation or when system bars toggle; polling is cheaper than wiring every source.
            schedule.Execute(() => Refresh(force: false)).Every(RefreshIntervalMs);
        }

        private void Refresh(bool force)
        {
            if (panel == null)
                return;

            var safeArea = Screen.safeArea;
            var screenSize = new Vector2Int(Screen.width, Screen.height);
            if (!force && safeArea == _appliedSafeArea && screenSize == _appliedScreenSize)
                return;

            _appliedSafeArea = safeArea;
            _appliedScreenSize = screenSize;

            // Screen space has origin at bottom-left, panel space at top-left.
            var topLeft = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(safeArea.xMin, screenSize.y - safeArea.yMax));
            var bottomRight = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(safeArea.xMax, screenSize.y - safeArea.yMin));
            var panelSize = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(screenSize.x, screenSize.y));

            style.paddingLeft = topLeft.x;
            style.paddingTop = topLeft.y;
            style.paddingRight = panelSize.x - bottomRight.x;
            style.paddingBottom = panelSize.y - bottomRight.y;
        }
    }
}
