SLASH_XY1 = "/xy"
SlashCmdList["XY"] = function(msg)
   print(getPosition())
end

SLASH_FRAMESTK1 = "/fs"
SlashCmdList.FRAMESTK = function()
	LoadAddOn('Blizzard_DebugTools')
	FrameStackTooltip_Toggle()
end

--------------------------------------------------------------

for i = 1, NUM_CHAT_WINDOWS do
	_G["ChatFrame"..i.."EditBox"]:SetAltArrowKeyMode(false)
end

--------------------------------------------------------------

function getPosition()
	local position, x, y
	
	local map = C_Map.GetBestMapForUnit("player")
	
	if map then
		position = C_Map.GetPlayerMapPosition(map, "player")
		
		if position then
			x = math.ceil(position.x * 10000)/100
			y = math.ceil(position.y * 10000)/100
		end
	end
	
	x = x or 0
	y = y or 0
	
	local mapName = string.upper(GetRealZoneText())
	
	return mapName.." ["..x.." "..y.."]"
end

function setTooltip(message)
	if (message == nil) then message = "" end

	GameTooltip_SetDefaultAnchor(GameTooltip, UIParent)
	GameTooltip:SetText(message)
	GameTooltip:Show()
end