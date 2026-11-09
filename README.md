# 🎯 Valorant Lineup Companion HUD (Ranked Competitive Pool Edition)

Ứng dụng Overlay hỗ trợ người chơi **Valorant** tra cứu góc ném chiêu (Lineup) ngay trong trận đấu, tập trung chuyên sâu vào **Bể Map Xếp Hạng (Competitive Rank Pool)** với dữ liệu từ kênh TikTok **LineupMaster ([@lineupmaster](https://www.tiktok.com/@lineupmaster))**.

---

## 🏆 Các Map Đang Trong Bể Rank (Active Competitive Pool)
Ứng dụng đã thiết lập sẵn bộ lọc mặc định **`[🏆 Chỉ Map trong RANK]`** để tập trung vào các map thi đấu chính thức hiện tại:
1. **Abyss (Bản đồ vực thẳm mới nhất):**
   * *Sova:* Max Reveal Recon A Site quét sạch Site & Vực sâu từ A Main.
   * *Brimstone:* Post-plant Molly A Default bay qua vực sâu; Post-plant Molly B Default từ B Lobby.
   * *Viper:* Tường độc phân cách A Site và che chắn đường mép vực.
2. **Sunset:**
   * *Brimstone:* Post-plant Molly A Default từ A Lobby; B Default ngoài B Main.
   * *Sova:* Recon soi toàn bộ B Site & Boba từ B Lobby.
3. **Lotus:**
   * *Brimstone:* Post-plant Molly A Site Default từ A Root; Molly C Site Default từ C Mound.
4. **Bind:**
   * *Brimstone:* Post-plant Molly A Short; Molly B Long (Fountain).
   * *Viper:* Snake Bite Post-plant B Long an toàn.
5. **Haven:**
   * *Brimstone:* Post-plant Molly A Long; Molly C Long.
   * *Sova:* Recon soi trọn C Long đầu Round từ C Site.

*(Lưu ý: **Ascent** và **Split** đã được Riot Games xoay tua đưa ra ngoài chế độ Xếp hạng. Các map này được đánh dấu `(Ngoài Rank)` và vẫn có thể tra cứu khi bạn chọn đích danh map hoặc bỏ tick ô "Chỉ Map trong RANK" để chơi Unrated / Custom.)*

---

## 🛡️ Tại sao ứng dụng an toàn 100% với Riot Vanguard?
- **Zero Memory Injection:** Không can thiệp, không đọc/ghi bộ nhớ game (`No OpenProcess`, `No Read/WriteProcessMemory`).
- **Zero DLL Hooking:** Không hook vào DirectX hay pipeline render của Valorant.
- **Native Windows Overlay:** Ứng dụng chạy độc lập như một cửa sổ trong suốt (`WS_EX_TOPMOST`), chỉ cần đặt Valorant ở chế độ **Borderless Windowed** (Cửa sổ không viền).
- **Safe Hotkeys:** Sử dụng chuẩn `RegisterHotKey` của Windows API.

---

## ⚡ Các phím tắt trong game
| Phím tắt | Chức năng |
| :--- | :--- |
| **`H`** | Bật / Tắt (Toggle) giao diện HUD Lineup ngay trong trận |
| **`Alt + H`** | Phím tắt phụ dự phòng |
| **`ESC`** | Đóng HUD ngay lập tức và trả quyền điều khiển về game |
| **Click ra ngoài HUD** | Tự động ẩn cửa sổ để bạn tập trung bắn súng |

---

## 🚀 Cách mở và sử dụng ứng dụng

Bạn **không cần mở VS Code hay gõ lệnh**:
1. **Cách 1 (Nhanh nhất):** Ra ngoài màn hình máy tính (**Desktop**), nhấp đúp vào biểu tượng phím tắt **`Valorant Lineup HUD`** (đã có icon Valorant đẹp mắt).
2. **Cách 2:** Mở thư mục [`D:\thanhkiet\App`](file:///D:/thanhkiet/App) và nhấp đúp file **`ValoLineupViewer.exe`**.
3. **Cách 3:** Nhấp đúp vào file [`Bat-Dau-App.bat`](file:///D:/thanhkiet/Bat-Dau-App.bat) trong thư mục dự án.

---

## 🎁 Chia sẻ cho bạn bè cùng chơi
Bạn chỉ cần gửi file nén sau cho bạn bè:
👉 **[`D:\thanhkiet\Valorant-Lineup-HUD-Portable.zip`](file:///D:/thanhkiet/Valorant-Lineup-HUD-Portable.zip)**

Người nhận:
- **Không cần cài đặt .NET hay bất cứ phần mềm nào** (Bản Self-contained đã tích hợp sẵn runtime).
- Chỉ cần giải nén và nhấp đúp vào **`ValoLineupViewer.exe`** là chơi được ngay!
- Trong file ZIP đã có sẵn file `HUONG-DAN.txt`.
