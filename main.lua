local config = {
	updateInterval = 1.0,
}

heartbeatCount = 0

function OnHeartbeat()
	heartbeatCount = heartbeatCount + 1
	
	GoblinDB.log.data[heartbeatCount] = CreateLogItem()
end

function CreateLogItem()
	local timestamp = time() -- epoch
	
	local position, x, y
	local map = C_Map.GetBestMapForUnit("player")
	
	if map then
		position = C_Map.GetPlayerMapPosition(map, "player")
		
		if position then
			x = math.ceil(position.x * 10000) / 100
			y = math.ceil(position.y * 10000) / 100
		end
	end
	
	map = map or 0
	
	x = x or 0
	y = y or 0
	
	local speed = GetUnitSpeed("player")
	if (speed) then
		speed = math.ceil(speed * 100) / 100
	end
	speed = speed and 0
	
	local heading = GetPlayerFacing()
	if (heading) then
		heading = math.ceil(heading * 100) / 100
	end
	heading = heading or 0
	
	local isDead = UnitIsDeadOrGhost("player") and 1 or 0
	local inCombat = InCombatLockdown() and 1 or 0
	local onTaxi = UnitOnTaxi("player") and 1 or 0
	local isFlying = 0 -- IsFlying() and 1 or 0
	local isIndoors = IsIndoors() and 1 or 0
	local isResting = IsResting() and 1 or 0
	local isMounted = IsMounted() and 1 or 0
	local isSwimming = IsSwimming() and 1 or 0
	local isPVP = UnitIsPVP("player") and 1 or 0
	
	local level = UnitLevel("player")
	local xp = UnitXP("player")
	local xpMax = UnitXPMax("player")
	local xpRested = GetXPExhaustion() and 1 or 0
	
	local money = GetMoney()
	
	local health = UnitHealth("player")
	local healthMax = UnitHealthMax("player")
	local power = UnitPower("player")
	local powerMax = UnitPowerMax("player")
	
	return timestamp..";"..map..";"..x..";"..y..";"..speed..";"..heading..";"..isDead..";"..inCombat..";"..onTaxi..";"..isFlying..";"..isIndoors..";"..isResting..";"..isMounted..";"..isSwimming..";"..isPVP..";"..level..";"..xp..";"..xpMax..";"..xpRested..";"..money..";"..health..";"..healthMax..";"..power..";"..powerMax
end

function OnConfigLoaded()
	powerType, powerToken = UnitPowerType("player")
	
	GoblinDB =
	{
		game =
		{
			api = select(4, GetBuildInfo()),
			expanion = GetAccountExpansionLevel(),
		},
		log = 
		{
			beginTime = time(),
			data = {},
			format = "timestamp;map;x;y;speed;heading;isDead;inCombat;onTaxi;isFlying;isIndoors;isResting;isMounted;isSwimming;isPVP;level;xp;xpMax.xpRested;money;health;healthMax;power;powerMax",
		},
		character =
		{
			name = UnitName("player"),
			race = UnitRace("player"),
			class = UnitClass("player"),
			gender = UnitSex("player"), -- 1 = Neutral, 2 = Male, 3 = Female
			realm = GetRealmName(),
			played = nil,
			power = powerToken,
		},
	},
	
	RequestTimePlayed()
end

function OnPlayerLogout()
	GoblinDB.log.endTime = time()
	GoblinDB.log.count = heartbeatCount
end

local f = CreateFrame("Frame", "Goblintools.GPS")

TimeSinceLastUpdate = 0
f:SetScript("OnUpdate", function(self, elapsed)
	TimeSinceLastUpdate = TimeSinceLastUpdate + elapsed

	if (TimeSinceLastUpdate > config.updateInterval) then
		OnHeartbeat()

		TimeSinceLastUpdate = TimeSinceLastUpdate - config.updateInterval
	end
end)

f:RegisterEvent("ADDON_LOADED")
f:RegisterEvent("PLAYER_LOGOUT")
f:RegisterEvent("TIME_PLAYED_MSG")

function f:OnEvent(event, arg1)
	if event == "ADDON_LOADED" and arg1 == "Goblintools.GPS" then
		OnConfigLoaded()
	elseif event == "PLAYER_LOGOUT" then
		OnPlayerLogout()
	elseif event == "TIME_PLAYED_MSG" and GoblinDB.character.played == nil then
		GoblinDB.character.played = arg1
	end
end

f:SetScript("OnEvent", f.OnEvent);
