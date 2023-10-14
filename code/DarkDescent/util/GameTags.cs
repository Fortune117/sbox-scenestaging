namespace DarkDescent;

public static class GameTags
{
    public static class Physics
    {
        public const string Solid = "solid";
        public const string Trigger = "trigger";
        public const string Ladder = "ladder";
        public const string Water = "water";
        public const string Debris = "debris";
        
        public const string Player = "player";
        public const string NPC = "npc";
        public const string Weapon = "weapon";
        public const string Ragdoll = "weapon";
        public const string Entity = "entity";
        public const string NoCollide = "nocollide";
    }

    public static class HitBox
    {
        public const string Head = "head";
        public const string Arm = "arm";
        public const string Leg = "leg";
        public const string Chest = "chest";
        public const string Stomach = "stomach";
        public const string Left = "left";
        public const string Right = "right";
        public const string Eye = "eye"; 
        public const string Shield = "shield"; 
    }

    public static class Damage
    {
        public const string Physical = "physical";
        public const string Magical = "magical";
        
        public const string Fire = "fire";
        public const string Frost = "frost";
        public const string Electric = "electric";
        public const string Poison = "poison";
        public const string Necrotic = "necrotic";
        
        public const string Arcane = "arcane";
        public const string Divine = "divine";
        public const string Occult = "occult";

        public const string Absolute = "absolute";
        public const string Chaotic = "chaotic";

        public static class Flags
        {
	        public const string IgnoreResistance = "ignore_resistance";
        }
    }
    
    public static class Label
    {
    }
    
    public static class NPCAnims
    {
	 
    }

    public static class Ammo
    {
    }
    
    public static class Debug
    {
	    public const string GodMode = "ghosts_godmode";
	    public const string InfiniteAmmo = "ghosts_infinite_ammo";
	    public const string GhostMode = "ghosts_ghost_mode";
    }

    public static class Input
    {
	    public const string Forward = "forward";
	    public const string Backward = "backward";
	    public const string Left = "Left";
	    public const string Right = "right";
	    public const string Jump = "jump";
	    public const string Run = "run";
	    public const string Duck = "duck";
	    public const string Walk = "walk";
	    public const string AttackPrimary = "attack1";
	    public const string AttackSecondary = "attack2";
	    public const string Reload = "reload";
	    public const string Interact = "interact";
	    public const string Slot1 = "slot1";
	    public const string Slot2 = "slot2";
	    public const string Slot3 = "slot3";
	    public const string Slot4 = "slot4";
	    public const string Slot5 = "slot5";
	    public const string Slot6 = "slot6";
	    public const string Slot7 = "slot7";
	    public const string Slot8 = "slot8";
	    public const string Slot9 = "slot9";
	    public const string Slot0 = "slot0";
	    public const string GameMenu = "game_menu";
	    public const string Drop = "drop";
	    public const string DebugMenu = "debugmenu";
    }

    public static class ItemTags
    {
	    /// <summary>
	    /// Items with this tag cannot be dropped.
	    /// </summary>
	    public const string NoDrop = "nodrop";
	    
	    /// <summary>
	    /// Items with this tag will be accepted as payment by all merchants.
	    /// </summary>
	    public const string Currency = "currency";

	    /// <summary>
	    /// Items with this tag should have a value of zero.
	    /// </summary>
	    public const string NoValue = "novalue";

	    /// <summary>
	    /// Items with this tag cannot be traded.
	    /// </summary>
	    public const string NoTrade = "notrade";

	    /// <summary>
	    /// Items with this tag cannot be stored in your stash.
	    /// </summary>
	    public const string NoStash = "nostash";

	    /// <summary>
	    /// Items with this tag will have no weight.
	    /// </summary>
	    public const string NoWeight = "noweight";
    }

    public static class PlayerStatNames
    {
	    public const string HealthRegeneration = "#stat.health.regeneration";
	    public const string BleedRecoveryRate = "#stat.bleedrecovery";
	    public const string RadiationRecoveryRate = "#stat.radiationrecovery";
	    public const string PsychicRegenerationRate = "#stat.psychic.regeneration";
	    public const string ArmSwayMultiplier = "#stat.armswaymultiplier";
	    public const string HungerIncreaseRate = "#stat.hunger.increaserate";
	    public const string ThirstIncreaseRate = "#stat.thirst.increaserate";
	    public const string TemperatureHighThreshold = "#stat.temperature.highthreshold";
	    public const string TemperatureLowThreshold = "#stat.temperature.lowthreshold";
	    public const string PhysicalResistance = "#stat.resistance.physical";
	    public const string BallisticResistance = "#stat.resistance.ballistic";
	    public const string BludgeoningResistance = "#stat.resistance.bludgeoning";
	    public const string SlashingResistance = "#stat.resistance.slashing";
	    public const string ExplosionResistance = "#stat.resistance.explosion";
	    public const string FireResistance = "#stat.resistance.fire";
	    public const string ElectricResistance = "#stat.resistance.electric";
	    public const string ChemicalResistance = "#stat.resistance.chemical";
	    public const string PsychicResistance = "#stat.resistance.psychic";
	    public const string RadiationResistance = "#stat.resistance.radiation";
	    public const string MaxStamina = "#stat.stamina.maximum";
	    public const string StaminaRegenerationMultiplier = "#stat.stamina.regeneration";
	    public const string StaminaCostMultiplier = "#stat.stamina.costmultiplier";
	    public const string MoveSpeedMultiplier = "#stat.movement.speedmult";
	    public const string JumpHeightMultiplier = "#stat.movement.jumpmult";
	    public const string CarryWeight = "#stat.carryweight";
    }

    public static class ItemStatNames
    {
	    public static class Ammo
	    {
		    public const string Damage = "#stats.ammo.damage";
		    public const string Penetration = "#stats.ammo.penetration";
		    public const string MuzzleVelocity = "#stats.ammo.muzzlevelocity";
		    public const string BleedChance = "#stats.ammo.bleedchance";
		    public const string BleedSeverity = "#stats.ammo.bleedseverity";
	    }

	    public static class Consumable
	    {
		    public const string UseTime = "#stats.consumable.usetime";
		    public const string UseTimeInstant = "#stats.consumable.usetime.instant";
	    }

	    public static class Firearm
	    {
		    public const string Accuracy = "#stats.weapon.accuracy";
		    public const string RPM = "#stats.weapon.rpm";
		    public const string RecoilVertical = "#stats.weapon.recoil.vertical";
		    public const string RecoilHorizontal = "#stats.weapon.recoil.horizontal";
		    public const string Handling = "#stats.weapon.handling";
		    public const string Ergonomics = "#stats.weapon.ergonomics";
		    public const string Reliability = "#stats.weapon.reliability";
		    public const string MagazineCapacity = "#stats.weapon.magazinecapacity";
		    public const string Automatic = "#stats.weapon.automatic";
	    }

	    public static class Armor
	    {
		    public const string ArmorRating = "#stats.armor.rating";
		    public const string BallisticResistance = "#stats.armor.ballistic";
		    public const string BludgeoningResistance = "#stats.armor.bludgeoning";
		    public const string SlashingResistance = "#stats.armor.slashing";
		    public const string ExplosionResistance = "#stats.armor.explosion";
		    public const string FireResistance = "#stats.armor.fire";
		    public const string ElectricResistance = "#stats.armor.electric";
		    public const string ChemicalResistance = "#stats.armor.chemical";
		    public const string PsychicResistance = "#stats.armor.psychic";
		    public const string RadiationResistance = "#stats.armor.radiation";
		    public const string MoveSpeedMultiplier = "#stats.armor.movespeed";
		    public const string JumpHeightMultiplier = "#stats.armor.jumpheight";
		    public const string CarryWeight = "#stats.armor.carryweight";
		    public const string StaminaCostMultiplier = "#stats.armor.staminacost";
		    public const string StaminaRegenerationMultiplier = "#stats.armor.staminaregeneration";
	    }

	    public static class Item
	    {
		    public const string Storage = "#stats.item.storage";
	    }
    }
}

