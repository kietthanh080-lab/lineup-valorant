Add-Type -AssemblyName System.Drawing

$assetsDir = "D:\thanhkiet\Assets\Lineups"
if (!(Test-Path $assetsDir)) { New-Item -ItemType Directory -Path $assetsDir -Force }

function Create-Tactical-Image {
    param(
        [string]$Filename,
        [string]$Type, # "STAND" or "AIM"
        [string]$Map,
        [string]$Site,
        [string]$Agent,
        [string]$Instruction,
        [string]$Subtext,
        [string]$SkyColor = "#1A2E3B",
        [string]$GroundColor = "#222A26"
    )

    $w = 800
    $h = 480
    $bmp = New-Object System.Drawing.Bitmap($w, $h, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit

    $cSky = [System.Drawing.ColorTranslator]::FromHtml($SkyColor)
    $cGround = [System.Drawing.ColorTranslator]::FromHtml($GroundColor)

    if ($Type -eq "STAND") {
        # First-person view looking down at corner/floor
        $brushGround = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
            (New-Object System.Drawing.Point(0, 0)),
            (New-Object System.Drawing.Point(0, $h)),
            [System.Drawing.Color]::FromArgb(255, 30, 38, 48),
            [System.Drawing.Color]::FromArgb(255, 12, 16, 22)
        )
        $g.FillRectangle($brushGround, 0, 0, $w, $h)

        # Draw 3D corner lines (wall & crate)
        $penWall = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255, 45, 58, 75), 4)
        $g.DrawLine($penWall, 0, 80, 350, 480)
        $g.DrawLine($penWall, 350, 480, 800, 320)
        $g.DrawLine($penWall, 350, 0, 350, 480)

        # Foot standing position ring
        $penFoot = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(200, 0, 240, 255), 3)
        $penFoot.DashStyle = [System.Drawing.Drawing2D.DashStyle]::Dash
        $g.DrawEllipse($penFoot, 300, 320, 100, 70)

        # Center marker dot
        $markerBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(230, 0, 240, 255))
        $g.FillEllipse($markerBrush, 335, 340, 30, 30)

        # Standing Tag
        $fontTag = New-Object System.Drawing.Font("Segoe UI", 12, [System.Drawing.FontStyle]::Bold)
        $brushTag = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 0, 240, 255))
        $g.DrawString("VI TRI DUNG KET GOC (STAND SPOT)", $fontTag, $brushTag, 230, 285)
    }
    else {
        # First-person view looking up at sky & buildings
        $brushSky = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
            (New-Object System.Drawing.Point(0, 0)),
            (New-Object System.Drawing.Point(0, $h)),
            $cSky,
            [System.Drawing.Color]::FromArgb(255, 18, 24, 32)
        )
        $g.FillRectangle($brushSky, 0, 0, $w, $h)

        # Rooftop / building polygon
        $roofPoints = @(
            (New-Object System.Drawing.Point(0, 480)),
            (New-Object System.Drawing.Point(0, 260)),
            (New-Object System.Drawing.Point(260, 220)),
            (New-Object System.Drawing.Point(400, 140)), # Aim point landmark
            (New-Object System.Drawing.Point(540, 210)),
            (New-Object System.Drawing.Point(800, 250)),
            (New-Object System.Drawing.Point(800, 480))
        )
        $brushBuilding = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 24, 32, 42))
        $g.FillPolygon($brushBuilding, $roofPoints)
        $penBuilding = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255, 60, 78, 100), 2)
        $g.DrawPolygon($penBuilding, $roofPoints)

        # Red target reticle circle
        $penTarget = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255, 255, 70, 85), 3)
        $penTarget.DashStyle = [System.Drawing.Drawing2D.DashStyle]::Dash
        $g.DrawEllipse($penTarget, 370, 110, 60, 60)
        
        $penAimCross = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255, 0, 255, 120), 2)
        $g.DrawLine($penAimCross, 385, 140, 415, 140)
        $g.DrawLine($penAimCross, 400, 125, 400, 155)

        # Arrow indicator
        $penArrow = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255, 255, 70, 85), 3)
        $penArrow.EndCap = [System.Drawing.Drawing2D.LineCap]::ArrowAnchor
        $g.DrawLine($penArrow, 460, 190, 415, 150)

        $fontAim = New-Object System.Drawing.Font("Segoe UI", 12, [System.Drawing.FontStyle]::Bold)
        $brushAim = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 255, 70, 85))
        $g.DrawString("DIEM CAN TAM / MEP VACH HUD", $fontAim, $brushAim, 450, 195)

        # Ability HUD bar at the bottom
        $g.FillRectangle((New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(180, 10, 14, 20))), 240, 410, 320, 60)
        $fontHud = New-Object System.Drawing.Font("Segoe UI", 11, [System.Drawing.FontStyle]::Bold)
        $brushHud = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 236, 232, 225))
        $g.DrawString("[C]       [Q]       [E]       [X]", $fontHud, $brushHud, 275, 430)
    }

    # Frame border
    $penBorder = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(120, 255, 70, 85), 2)
    $g.DrawRectangle($penBorder, 10, 10, $w - 20, $h - 20)

    # Top-left Badge
    $g.FillRectangle((New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(220, 15, 20, 28))), 20, 20, 240, 64)
    $g.DrawRectangle((New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255, 0, 240, 255), 1.5)), 20, 20, 240, 64)

    $fontTitle = New-Object System.Drawing.Font("Segoe UI", 13, [System.Drawing.FontStyle]::Bold)
    $brushWhite = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 236, 232, 225))
    $g.DrawString("$($Map.ToUpper()) - SITE $Site", $fontTitle, $brushWhite, 30, 26)

    $fontSub = New-Object System.Drawing.Font("Segoe UI", 10, [System.Drawing.FontStyle]::Regular)
    $brushCyan = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 0, 240, 255))
    $g.DrawString("Agent: $Agent - $Type", $fontSub, $brushCyan, 30, 52)

    # Bottom Guide Text Panel
    $g.FillRectangle((New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(220, 10, 14, 20))), 20, 390, 760, 75)
    $fontDesc = New-Object System.Drawing.Font("Segoe UI", 11, [System.Drawing.FontStyle]::Bold)
    $brushDesc = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 240, 244, 248))
    $g.DrawString("> $Instruction", $fontDesc, $brushDesc, 30, 400)

    $fontNotes = New-Object System.Drawing.Font("Segoe UI", 10, [System.Drawing.FontStyle]::Regular)
    $brushNotes = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 160, 175, 190))
    $g.DrawString("$Subtext", $fontNotes, $brushNotes, 30, 430)

    $dest = Join-Path $assetsDir $Filename
    $bmp.Save($dest, [System.Drawing.Imaging.ImageFormat]::Png)
    $g.Dispose()
    $bmp.Dispose()
    Write-Output "Generated: $Filename"
}

# 1. Lotus C Mound
Create-Tactical-Image -Filename "lotus_c_stand.png" -Type "STAND" -Map "Lotus" -Site "C" -Agent "Brimstone" -Instruction "Leo len go da tai C Mound, ep sat lung vao goc tuong da co." -Subtext "Nhin xuong dat khoa chan de dan bay chuan xac."
Create-Tactical-Image -Filename "lotus_c_aim.png" -Type "AIM" -Map "Lotus" -Site "C" -Agent "Brimstone" -Instruction "Can icon chuot trai HUD cham vao chop nhon buc phu dieu tren vom Site C." -Subtext "Ban chuot trai - Dan bay 4.5s no phu kin C Default." -SkyColor "#304245"

# 2. Lotus A Root
Create-Tactical-Image -Filename "lotus_a_stand.png" -Type "STAND" -Map "Lotus" -Site "A" -Agent "Brimstone" -Instruction "Dung nep vao re cay co thu khong lo tai sanh ngoai A Root." -Subtext "Dung yen ap sat goc re cay."
Create-Tactical-Image -Filename "lotus_a_aim.png" -Type "AIM" -Map "Lotus" -Site "A" -Agent "Brimstone" -Instruction "Can vach kim cuong ben trai tam ngam thang hang voi ngon la sen." -Subtext "Dan vuot qua tuong da cao roi trung A Default." -SkyColor "#2D3E35"

# 3. Sunset A Lobby
Create-Tactical-Image -Filename "sunset_a_stand.png" -Type "STAND" -Map "Sunset" -Site "A" -Agent "Brimstone" -Instruction "Dung ket vao goc quay nuoc giai khat o A Lobby." -Subtext "Nep sat goc tuong tranh doi thu ban tu A Elbow."
Create-Tactical-Image -Filename "sunset_a_aim.png" -Type "AIM" -Map "Sunset" -Site "A" -Agent "Brimstone" -Instruction "Can mep tren thanh nap dan trung mep day cap dien cao the." -Subtext "Dan no bao trum toan bo goc dat bom A Default." -SkyColor "#4A2B2B"

# 4. Sunset B Main
Create-Tactical-Image -Filename "sunset_b_stand.png" -Type "STAND" -Map "Sunset" -Site "B" -Agent "Brimstone" -Instruction "Lui sat goc tuong gach ben ngoai ngo vao B Main." -Subtext "Khoang cach xa tuyet doi an toan."
Create-Tactical-Image -Filename "sunset_b_aim.png" -Type "AIM" -Map "Sunset" -Site "B" -Agent "Brimstone" -Instruction "Can goc duoi thanh mau tiep giap ngon cay co cao nhat." -Subtext "Lua lan kin mat truoc va sau khoi hop B Site." -SkyColor "#3E2A36"

# 5. Abyss A Recon Sova
Create-Tactical-Image -Filename "abyss_a_stand.png" -Type "STAND" -Map "Abyss" -Site "A" -Agent "Sova" -Instruction "Dung nep vao lan can da ngoai sanh A Main nhin ra A Site." -Subtext "Tranh truot chan roi xuong vuc tham."
Create-Tactical-Image -Filename "abyss_a_aim.png" -Type "AIM" -Map "Abyss" -Site "A" -Agent "Sova" -Instruction "Can goc thanh luc sac vao dinh nhu da cao nhat - Keo 2 vach." -Subtext "Ten cam dinh vach da quet tron Site A va mep vuc." -SkyColor "#142634"

# 6. Abyss A Brimstone
Create-Tactical-Image -Filename "abyss_brim_a_stand.png" -Type "STAND" -Map "Abyss" -Site "A" -Agent "Brimstone" -Instruction "Lui lai sat mep tuong da ben trai loi vao A Main." -Subtext "Dung ap sat goc tuong da vuong."
Create-Tactical-Image -Filename "abyss_brim_a_aim.png" -Type "AIM" -Map "Abyss" -Site "A" -Agent "Brimstone" -Instruction "Can icon chieu E cham re cay ru tu noc hang - Ban chuot trai." -Subtext "Dan bay vuot qua mieng ho sau no dung buc dat Spike." -SkyColor "#162838"

# 7. Bind A Short
Create-Tactical-Image -Filename "bind_a_stand.png" -Type "STAND" -Map "Bind" -Site "A" -Agent "Brimstone" -Instruction "Dung nep vao goc chu U ben phai A Short truoc vach dich chuyen." -Subtext "Goc ket an toan truoc ke dich tu U-Hall."
Create-Tactical-Image -Filename "bind_a_aim.png" -Type "AIM" -Map "Bind" -Site "A" -Agent "Brimstone" -Instruction "Can mep tren thanh mau (HP UI) vao khop noi can cau tren troi." -Subtext "Dan lua no phu kin ca 2 mat xe tai Default A Site." -SkyColor "#4E3E28"

# 8. Bind B Fountain
Create-Tactical-Image -Filename "bind_b_stand.png" -Type "STAND" -Map "Bind" -Site "B" -Agent "Brimstone" -Instruction "Lui sau ve dai phun nuoc B Long, dung ep sat goc chu L." -Subtext "Khoang cach xa tuyet doi an toan."
Create-Tactical-Image -Filename "bind_b_aim.png" -Type "AIM" -Map "Bind" -Site "B" -Agent "Brimstone" -Instruction "Can goc nhon icon chieu C (Stim) thang hang bong den tuong." -Subtext "Molly bay roi chinh xac goc thung dat bom B Site." -SkyColor "#453826"

# 9. Haven A Long
Create-Tactical-Image -Filename "haven_a_stand.png" -Type "STAND" -Map "Haven" -Site "A" -Agent "Brimstone" -Instruction "Lui ve goc tuong A Long, dung sat mep bo rao go doi dien cong vom." -Subtext "Can thang goc tuong go."
Create-Tactical-Image -Filename "haven_a_aim.png" -Type "AIM" -Map "Haven" -Site "A" -Agent "Brimstone" -Instruction "Can mep duoi khung giap trung nhanh la cay xanh thu 3." -Subtext "Dan lua no tron ven diem dat bom goc thung A Site." -SkyColor "#2D3A44"

# 10. Haven C Long
Create-Tactical-Image -Filename "haven_c_stand.png" -Type "STAND" -Map "Haven" -Site "C" -Agent "Brimstone" -Instruction "Dung sat goc chu L ben trai dong bao cat tai C Long." -Subtext "Nep minh sau bao cat."
Create-Tactical-Image -Filename "haven_c_aim.png" -Type "AIM" -Map "Haven" -Site "C" -Agent "Brimstone" -Instruction "Can goc icon chieu Q vao diem giao giua mai dinh va ngon cay tuyet." -Subtext "Dan bay cao dap trung goc dat bom C Default." -SkyColor "#28343D"

# 11. Ascent A Main
Create-Tactical-Image -Filename "ascent_a_stand.png" -Type "STAND" -Map "Ascent" -Site "A" -Agent "Brimstone" -Instruction "Dung nep sat vao goc chu L thung go lon ngoai A Main." -Subtext "Co dinh goc dung."
Create-Tactical-Image -Filename "ascent_a_aim.png" -Type "AIM" -Map "Ascent" -Site "A" -Agent "Brimstone" -Instruction "Can icon sung tren HUD cham vao dau thanh xa kim loai tren mai." -Subtext "Molly bay mat 4.5s no dung tam dat bom A Default." -SkyColor "#243242"

Write-Output "DONE"
