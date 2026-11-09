// Công cụ hỗ trợ nạp nhanh Lineup từ TikTok LineupMaster (@lineupmaster) vào file lineups.json
// Chạy bằng lệnh: node scripts/import_lineup.js

const fs = require('fs');
const path = require('path');

const jsonPath = path.join(__dirname, '..', 'lineups.json');

function addLineup({ map, agent, ability, side, site, title, throwType, standDesc, aimDesc, videoUrl, notes }) {
    if (!fs.existsSync(jsonPath)) {
        console.error("Không tìm thấy file lineups.json!");
        return;
    }

    const raw = fs.readFileSync(jsonPath, 'utf8');
    const list = JSON.parse(raw);

    const newLineup = {
        Id: `lm-${(map || 'custom').toLowerCase()}-${(agent || 'agent').toLowerCase()}-${Date.now().toString(36)}`,
        Map: map || "Ascent",
        Agent: agent || "Brimstone",
        Ability: ability || "Incendiary (Lựu đạn lửa)",
        Side: side || "Attacker",
        Site: site || "A",
        Title: title || "Lineup mới từ TikTok LineupMaster",
        ThrowType: throwType || "Đứng yên - Bắn chuột trái",
        StandDescription: standDesc || "Đứng vào góc kẹt theo hướng dẫn video.",
        AimDescription: aimDesc || "Căn tâm ngắm hoặc vạch HUD theo video.",
        StandImageUrl: "",
        AimImageUrl: "",
        VideoUrl: videoUrl || "https://www.tiktok.com/@lineupmaster",
        Author: "LineupMaster",
        Difficulty: "Trung bình",
        Notes: notes || "Lineup sưu tầm từ kênh TikTok @lineupmaster."
    };

    list.unshift(newLineup);
    fs.writeFileSync(jsonPath, JSON.stringify(list, null, 2), 'utf8');
    console.log(`✅ Đã thêm thành công: [${newLineup.Agent}] ${newLineup.Title} (${newLineup.Map} - Site ${newLineup.Site})`);
}

// Ví dụ mẫu:
console.log("=== BỘ NẠP LINEUP TỰ ĐỘNG CHO VALORANT LINEUP COMPANION ===");
console.log(`Đang lưu trữ ${JSON.parse(fs.readFileSync(jsonPath, 'utf8')).length} lineups trong thư viện.`);
console.log("Để nạp thêm video từ TikTok, bạn có thể gọi hàm addLineup() hoặc bấm nút '+ Thêm Lineup' trực tiếp trên giao diện app!");
