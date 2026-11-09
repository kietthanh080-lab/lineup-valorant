namespace ValoLineupViewer.Models
{
    public class Lineup
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Map { get; set; } = "Ascent";
        public string Agent { get; set; } = "Brimstone";
        public string Ability { get; set; } = "Incendiary (Molly)";
        public string Side { get; set; } = "Attacker"; // Attacker / Defender
        public string Site { get; set; } = "A";        // A / B / C / Mid
        public string Title { get; set; } = string.Empty;
        public string ThrowType { get; set; } = "Đứng bắn thường"; // Jump, 1-Charge, 2-Bounce, etc.
        public string StandDescription { get; set; } = string.Empty;
        public string AimDescription { get; set; } = string.Empty;
        public string StandImageUrl { get; set; } = string.Empty;
        public string AimImageUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = "https://www.tiktok.com/@lineupmaster";
        public string Author { get; set; } = "LineupMaster";
        public string Difficulty { get; set; } = "Dễ"; // Dễ / Trung bình / Khó
        public string Notes { get; set; } = string.Empty;

        private bool? _isRankedMap;
        public bool IsRankedMap
        {
            get => _isRankedMap ?? RankedPool.IsRanked(Map);
            set => _isRankedMap = value;
        }
    }
}
