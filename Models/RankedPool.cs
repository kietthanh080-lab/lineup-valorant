using System;
using System.Collections.Generic;

namespace ValoLineupViewer.Models
{
    public static class RankedPool
    {
        // Các map đang trong Bể map Xếp hạng (Active Competitive Pool) hiện tại của Valorant
        // Lưu ý: Ascent và Split đã được Riot xoay tua đưa ra khỏi chế độ Xếp hạng
        public static readonly HashSet<string> ActiveRankedMaps = new(StringComparer.OrdinalIgnoreCase)
        {
            "Abyss",
            "Sunset",
            "Lotus",
            "Bind",
            "Haven"
        };

        public static bool IsRanked(string mapName)
        {
            return ActiveRankedMaps.Contains(mapName);
        }
    }
}
