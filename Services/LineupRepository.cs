using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ValoLineupViewer.Models;

namespace ValoLineupViewer.Services
{
    public class LineupRepository
    {
        private readonly string _filePath;

        public LineupRepository()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string localPath = Path.Combine(baseDir, "lineups.json");
            
            // Nếu chạy từ dev/source, ưu tiên file lineups.json trong thư mục dự án
            string rootProjectPath = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..", "lineups.json"));
            
            if (File.Exists(localPath))
            {
                _filePath = localPath;
            }
            else if (File.Exists(rootProjectPath))
            {
                _filePath = rootProjectPath;
            }
            else
            {
                _filePath = localPath;
            }
        }

        public List<Lineup> LoadLineups()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    var list = JsonSerializer.Deserialize<List<Lineup>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (list != null && list.Count > 0)
                        return list;
                }
                catch { }
            }

            return new List<Lineup>();
        }

        public void SaveLineups(List<Lineup> lineups)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(lineups, options);
            File.WriteAllText(_filePath, json);
        }
    }
}
