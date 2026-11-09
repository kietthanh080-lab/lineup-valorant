using System;
using System.Collections.Generic;

namespace ValoLineupViewer.Models
{
    public static class RankedPool
    {
        // 7 Map chuẩn trong Bể map Xếp hạng (Active Competitive Pool) hiện tại của Valorant:
        // Abyss, Bind, Corrode, Haven, Lotus, Summit, Sunset
        public static readonly HashSet<string> ActiveRankedMaps = new(StringComparer.OrdinalIgnoreCase)
        {
            "Abyss",
            "Bind",
            "Corrode",
            "Haven",
            "Lotus",
            "Summit",
            "Sunset"
        };

        public static bool IsRanked(string mapName)
        {
            return ActiveRankedMaps.Contains(mapName);
        }
    }
}
