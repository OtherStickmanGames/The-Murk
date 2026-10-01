namespace Game.DevTools
{
    /// <summary>
    /// Averages frame times over a short window; also tracks the worst frame, which shows hitches the average hides.
    /// </summary>
    public sealed class FpsMeter
    {
        private readonly float _windowSeconds;

        private float _elapsed;
        private int _frames;
        private float _worst;

        public FpsMeter(float windowSeconds = 0.5f)
        {
            _windowSeconds = windowSeconds;
        }

        public float Fps { get; private set; }
        public float AverageMs { get; private set; }
        public float WorstMs { get; private set; }

        /// <summary>Returns true when a new window has completed and the values were updated.</summary>
        public bool Sample(float unscaledDeltaTime)
        {
            _elapsed += unscaledDeltaTime;
            _frames++;
            if (unscaledDeltaTime > _worst)
                _worst = unscaledDeltaTime;

            if (_elapsed < _windowSeconds)
                return false;

            Fps = _frames / _elapsed;
            AverageMs = _elapsed * 1000f / _frames;
            WorstMs = _worst * 1000f;

            _elapsed = 0;
            _frames = 0;
            _worst = 0;
            return true;
        }
    }
}
