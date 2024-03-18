namespace PackTracker
{
    public class Settings
    {
        public bool Spoil { get; set; } = false;
        public bool PityOverlay { get; set; } = true;
        public bool Update { get; set; } = true;
        public bool GoldenResetRegularPityTimer { get; set; } = false;
        public bool RightmostPityTimerOverlay { get; set; } = true;
        public string PTCookie { get; set; } = "";
        public string PTAuth { get; set; } = "";
    }
}
