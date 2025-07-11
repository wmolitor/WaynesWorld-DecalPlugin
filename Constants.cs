using Decal.Filters;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Decal.Constants
{
    public static class MessageTypes
    {
        /// <summary>Sent every time an object you are aware of ceases to exist. Merely running out of range does not generate this message - in that case, the client just automatically stops tracking it after receiving no updates for a while (which I presume is a very short while).</summary>
        public const int DestroyObject = 0x0024;

        /// <summary>For stackable items, this changes the number of items in the stack.</summary>
        public const int AdjustStackSize = 0x0197;

        /// <summary>A Player Kill occurred nearby (also sent for suicides). This could be interesting to monitor for tournements.</summary>
        public const int PlayerKilled = 0x019E;

        /// <summary>Indirect '/e' text.</summary>
        public const int IndirectText = 0x01E0;

        /// <summary>Contains the text associated with an emote action.</summary>
        public const int EmoteText = 0x01E2;

        /// <summary>A message to be displayed in the chat window, spoken by a nearby player, NPC or creature</summary>
        public const int CreatureMessage = 0x02BB;

        /// <summary>A message to be displayed in the chat window, spoken by a nearby player, NPC or creature</summary>
        public const int CreatureMessage_Ranged = 0x02BC;

        /// <summary>Set or update a Character DWORD property value</summary>
        public const int SetCharacterDword = 0x02CD;

        /// <summary>Set or update an Object DWORD property value</summary>
        public const int SetObjectDword = 0x02CE;

        /// <summary>Set or update a Character QWORD property value</summary>
        public const int SetCharacterQword = 0x02CF;

        /// <summary>Set or update a Character Boolean property value</summary>
        public const int SetCharacterBoolean = 0x02D1;

        /// <summary>Set or update an Object Boolean property value</summary>
        public const int SetObjectBoolean = 0x02D2;

        /// <summary>Set or update an Object String property value</summary>
        public const int SetObjectString = 0x02D6;

        /// <summary>Set or update an Object Resource property value</summary>
        public const int SetObjectResource = 0x02D8;

        /// <summary>Set or update a Character Link property value</summary>
        public const int SetCharacterLink = 0x02D9;

        /// <summary>Set or update an Object Link property value</summary>
        public const int SetObjectLink = 0x02DA;

        /// <summary>Set or update a Character Position property value</summary>
        public const int SetCharacterPosition = 0x02DB;

        /// <summary>Set or update a Character Skill value</summary>
        public const int SetCharacterSkillLevel = 0x02DD;

        /// <summary>Set or update a Character Skill state</summary>
        public const int SetCharacterSkillState = 0x02E1;

        /// <summary>Set or update a Character Attribute value</summary>
        public const int SetCharacterAttribute = 0x02E3;

        /// <summary>Set or update a Character Vital value</summary>
        public const int SetCharacterVital = 0x02E7;

        /// <summary>Set or update a Character Vital value</summary>
        public const int SetCharacterCurrentVital = 0x02E9;

        /// <summary>Sent when a character rematerializes at the lifestone after death.</summary>
        public const int LifestoneMaterialize = 0xF619;

        /// <summary>Sent whenever a character changes their clothes. It contains the entire description of what their wearing (and possibly their facial features as well). This message is only sent for changes, when the character is first created, the body of this message is included inside the creation message.</summary>
        public const int ChangeModel = 0xF625;

        /// <summary>Uncracked - Character creation screen initilised.</summary>
        public const int CharCreationInitilisation = 0xF643;

        /// <summary>Instructs the client to return to 2D mode - the character list.</summary>
        public const int End3DMode = 0xF653;

        /// <summary>A character was marked for  delete.</summary>
        public const int CharDeletion = 0xF655;

        /// <summary>The character to log in.</summary>
        public const int RequestLogin = 0xF657;

        /// <summary>The list of characters on the current account.</summary>
        public const int CharacterList = 0xF658;

        /// <summary>Failure to log in</summary>
        public const int CharacterLoginFailure = 0xF659;

        /// <summary>Create an object somewhere in the world</summary>
        public const int CreateObject = 0xF745;

        /// <summary></summary>
        public const int LoginCharacter = 0xF746;

        /// <summary>Sent whenever an object is removed from the scene.</summary>
        public const int RemoveItem = 0xF747;

        /// <summary>Set position - the server pathologically sends these after every actions - sometimes more than once. If has options for setting a fixed velocity or an arc for thrown weapons and arrows.</summary>
        public const int SetPositionAndMotion = 0xF748;

        /// <summary>Multipurpose message.  So far object wielding has been decoded.  Lots of unknowns</summary>
        public const int WieldObject = 0xF749;

        /// <summary></summary>
        public const int MoveObjectIntoInventory = 0xF74A;

        /// <summary>Signals your client to end the portal animation for you or another char and also is fired when war spells dissapear as they hit an object blocking their path.</summary>
        public const int ToggleObjectVisibility = 0xF74B;

        /// <summary>These are animations. Whenever a human, monster or object moves - one of these little messages is sent. Even idle emotes (like head scratching and nodding) are sent in this manner.</summary>
        public const int Animation = 0xF74C;

        /// <summary>An object has jumped</summary>
        public const int Jumping = 0xF74E;

        /// <summary>Applies a sound effect.</summary>
        public const int ApplySoundEffect = 0xF750;

        /// <summary>Instructs the client to show the portal graphic.</summary>
        public const int EnterPortalMode = 0xF751;

        /// <summary>Applies an effect with visual and sound.</summary>
        public const int ApplyVisualOrSoundEffect = 0xF755;

        /// <summary>Game Events are messages that are sequenced.</summary>
        public const int GameEvent = 0xF7B0;

        /// <summary>Game Actions are outgoing messages that are sequenced.</summary>
        public const int GameAction = 0xF7B1;

        /// <summary>The user has clicked 'Enter'. This message does not contain the ID of the character logging on; that comes later.</summary>
        public const int EnterGame = 0xF7C8;

        /// <summary>Update an existing object's data.</summary>
        public const int UpdateObject = 0xF7DB;

        /// <summary>Send or receive a message using Turbine Chat.</summary>
        public const int TurbineChat = 0xF7DE;

        /// <summary>Switch from the character display to the game display.</summary>
        public const int Start3DMode = 0xF7DF;

        /// <summary>Display a message in the chat window.</summary>
        public const int ServerMessage = 0xF7E0;

        /// <summary>The name of the current world.</summary>
        public const int ServerName = 0xF7E1;

        /// <summary>Add or update a dat file Resource.</summary>
        public const int UpdateResource = 0xF7E2;

        /// <summary>A list of dat files that need to be patched</summary>
        public const int DatFilePatchList = 0xF7E7;
    }

    static class GameEvents
    {
        /// <summary>Display a message in a popup message window.</summary>
        public const int MessageBox = 0x0004;
        /// <summary>Information describing your character.</summary>
        public const int LoginCharacter = 0x0013;
        /// <summary>Returns info related to your monarch, patron and vassals.</summary>
        public const int AllegianceInfo = 0x0020;
        /// <summary>Store an item in a container.</summary>
        public const int InsertInventoryItem = 0x0022;
        /// <summary>Equip an item.</summary>
        public const int WearItem = 0x0023;
        /// <summary>Titles for the current character.</summary>
        public const int TitleList = 0x0029;
        /// <summary>Set a title for the current character.</summary>
        public const int SetTitle = 0x002b;
        /// <summary>Close Container - Only sent when explicitly closed</summary>
        public const int CloseContainer = 0x0052;
        /// <summary>Open the buy/sell panel for a merchant.</summary>
        public const int ApproachVendor = 0x0062;
        /// <summary>Failure to give an item</summary>
        public const int FailureToGiveItem = 0x00A0;
        /// <summary>Member left fellowship</summary>
        public const int FellowshipMemberQuit = 0x00A3;
        /// <summary>Member dismissed from fellowship</summary>
        public const int FellowshipMemberDismissed = 0x00A4;
        /// <summary>Sent when you first open a book, contains the entire table of contents.</summary>
        public const int ReadTableOfContents = 0x00B4;
        /// <summary>Contains the text of a single page of a book, parchment or sign.</summary>
        public const int ReadPage = 0x00B8;
        /// <summary>The result of an attempt to assess an item or creature.</summary>
        public const int IdentifyObject = 0x00C9;
        /// <summary>Group Chat</summary>
        public const int GroupChat = 0x0147;
        /// <summary>Set Pack Contents</summary>
        public const int SetPackContents = 0x0196;
        /// <summary>Removes an item from inventory (when you place it on the ground or give it away)</summary>
        public const int DropFromInventory = 0x019A;
        /// <summary>Melee attack completed</summary>
        public const int AttackCompleted = 0x01A7;
        /// <summary>Delete a spell from your spellbook.</summary>
        public const int DeleteSpellFromSpellbook = 0x01A8;
        /// <summary>You just died.</summary>
        public const int YourDeath = 0x01AC;
        /// <summary>Message for a death, something you killed or your own death message.</summary>
        public const int KillOrDeathMessage = 0x01AD;
        /// <summary>You have hit your target with a melee attack.</summary>
        public const int InflictMeleeDamage = 0x01B1;
        /// <summary>You have been hit by a creature's melee attack.</summary>
        public const int ReceiveMeleeDamage = 0x01B2;
        /// <summary>Your target has evaded your melee attack.</summary>
        public const int OtherMeleeEvade = 0x01B3;
        /// <summary>You have evaded a creature's melee attack.</summary>
        public const int SelfMeleeEvade = 0x01B4;
        /// <summary>Start melee attack</summary>
        public const int StartMeleeAttack = 0x01B8;
        /// <summary>Update a creature's health bar.</summary>
        public const int UpdateHealth = 0x01C0;
        /// <summary>Age Command Result - happens when you do /age in the game</summary>
        public const int AgeCommandResult = 0x01C3;
        /// <summary>Ready.  Previous action complete</summary>
        public const int ReadyPreviousActionComplete = 0x01C7;
        /// <summary>Update Allegiance info, sent when allegiance panel is open</summary>
        public const int UpdateAllegianceInfo = 0x01C8;
        /// <summary>Close Assess Panel</summary>
        public const int CloseAssessPanel = 0x01CB;
        /// <summary>Ping Reply</summary>
        public const int PingReply = 0x01EA;
        /// <summary>Squelched Users List</summary>
        public const int SquelchedUsersList = 0x01F4;
        /// <summary>Send to begin a trade and display the trade window</summary>
        public const int EnterTrade = 0x01FD;
        /// <summary>End trading</summary>
        public const int EndTrade = 0x01FF;
        /// <summary>Item was added to trade window - you will receive a Create Object (0xF745) with details of the item</summary>
        public const int AddTradeItem = 0x0200;
        /// <summary>The trade was accepted</summary>
        public const int AcceptTrade = 0x0202;
        /// <summary>The trade was un-accepted</summary>
        public const int UnacceptTrade = 0x0203;
        /// <summary>The trade window was reset</summary>
        public const int ResetTrade = 0x0205;
        /// <summary>Failure to add a trade item</summary>
        public const int FailureToAddATradeItem = 0x0207;
        /// <summary>Failure to complete a trade</summary>
        public const int FailureToCompleteATrade = 0x0208;
        /// <summary>Buy a dwelling or pay maintenance</summary>
        public const int DisplayDwellingPurchaseOrMaintenancePanel = 0x021D;
        /// <summary>House panel information for owners.</summary>
        public const int HouseInformationForOwners = 0x0225;
        /// <summary>House panel information for non-owners.</summary>
        public const int HouseInformationForNonowners = 0x0226;
        /// <summary>House Guest List, Sent in response to asking for one.</summary>
        public const int HouseGuestList = 0x0257;
        /// <summary>Update an item's mana bar.</summary>
        public const int UpdateItemManaBar = 0x0264;
        /// <summary>Display a list of available dwellings in the chat window.</summary>
        public const int HousesAvailable = 0x0271;
        /// <summary>Display a confirmation panel.</summary>
        public const int ConfirmationPanel = 0x0274;
        /// <summary>A player has closed your confirmation panel.</summary>
        public const int ConfirmationPanelClosed = 0x0276;
        /// <summary>Display an allegiance login/logout message in the chat window.</summary>
        public const int AllegianceMemberLoginOrOut = 0x027A;
        /// <summary>Display a status message in the chat window.</summary>
        public const int DisplayStatusMessage = 0x028A;
        /// <summary>Display a parameterized status message in the chat window.</summary>
        public const int DisplayParameterizedStatusMessage = 0x028B;
        /// <summary>Set Turbine Chat channel numbers.</summary>
        public const int SetTurbineChatChannels = 0x0295;
        /// <summary>Someone has sent you a @tell.</summary>
        public const int Tell = 0x02BD;
        /// <summary>Create or join a fellowship</summary>
        public const int CreateFellowship = 0x02BE;
        /// <summary>Disband your fellowship.</summary>
        public const int DisbandFellowship = 0x02BF;
        /// <summary>Add a member to your fellowship.</summary>
        public const int AddFellowshipMember = 0x02C0;
        /// <summary>Add a spell to your spellbook.</summary>
        public const int AddSpellToSpellbook = 0x02C1;
        /// <summary>Apply an enchantment to your character.</summary>
        public const int AddCharacterEnchantment = 0x02C2;
        /// <summary>Remove an enchantment from your character.</summary>
        public const int RemoveCharacterEnchantment = 0x02C3;
        /// <summary>Remove multiple enchantments from your character.</summary>
        public const int RemoveMultipleCharacterEnchantments = 0x02C5;
        /// <summary>Silently remove all enchantments from your character, e.g. when you die (no message in the chat window).</summary>
        public const int RemoveAllCharacterEnchantments_Silent = 0x02C6;
        /// <summary>Silently remove An enchantment from your character.</summary>
        public const int RemoveCharacterEnchantment_Silent = 0x02C7;
        /// <summary>Silently remove multiple enchantments from your character (no message in the chat window).</summary>
        public const int RemoveMultipleCharacterEnchantments_Silent = 0x02C8;
        /// <summary>A portal storm is brewing.</summary>
        public const int MildPortalStorm = 0x02C9;
        /// <summary>A portal storm is imminent.</summary>
        public const int HeavyPortalStorm = 0x02CA;
        /// <summary>You have been portal stormed.</summary>
        public const int PortalStormed = 0x02CB;
        /// <summary>The portal storm has subsided.</summary>
        public const int EndPortalStorm = 0x02CC;
        /// <summary>Display a status message on the Action Viewscreen (the red text overlaid on the 3D area).</summary>
        public const int StatusMessage = 0x02EB;
    }

    static class GameActions
    {
        /// <summary>Set a single character option.</summary>
        public const int SetSingleCharacterOption = 0x0005;
        /// <summary>Set AFK message.</summary>
        public const int SetAfkMessage = 0x0010;
        /// <summary>Store an item in a container.</summary>
        public const int StoreItem = 0x0019;
        /// <summary>Equip an item.</summary>
        public const int EquipItem = 0x001A;
        /// <summary>Drop an item.</summary>
        public const int DropItem = 0x001B;
        /// <summary>Attempt to use an item.</summary>
        public const int UseItem = 0x0036;
        /// <summary>Spend XP to raise a vital.</summary>
        public const int RaiseVital = 0x0044;
        /// <summary>Spend XP to raise an attribute.</summary>
        public const int RaiseAttribute = 0x0045;
        /// <summary>Spend XP to raise a skill.</summary>
        public const int RaiseSkill = 0x0046;
        /// <summary>Spend skill credits to train a skill.</summary>
        public const int TrainSkill = 0x0047;
        /// <summary>Cast a spell.</summary>
        public const int CastSpell = 0x0048;
        /// <summary>Cast a spell.</summary>
        public const int CastSpellOnObject = 0x004A;
        /// <summary>The client is ready for the character to materialize after portalling or logging on.</summary>
        public const int Materialize = 0x00A1;
        /// <summary>Give an item to someone.</summary>
        public const int GiveItem = 0x00CD;
        /// <summary>Add an item to the shortcut bar.</summary>
        public const int MakeShortcut = 0x019C;
        /// <summary>Remove an item from the shortcut bar.</summary>
        public const int RemoveShortcut = 0x019D;
        /// <summary>Set multiple character options.</summary>
        public const int SetCharacterOptions = 0x01A1;
        /// <summary>Add a spell to a spell bar.</summary>
        public const int AddSpellToSpellbar = 0x01E3;
        /// <summary>Remove a spell from a spell bar.</summary>
        public const int RemoveSpellFromSpellbar = 0x01E4;
    }

    // I am not sure if the below is correct as it was autogenerated by the IDE
    public static class ObjectTypes
    {
        public const int Player = 0x0001;
        public const int Creature = 0x0002;
        public const int Item = 0x0003;
        public const int Container = 0x0004;
        public const int Portal = 0x0005;
        public const int Door = 0x0006;
        public const int Wall = 0x0007;
        public const int Terrain = 0x0008;
        public const int Effect = 0x0009;
    }

    // I am not sure if the below is correct as it was autogenerated by the IDE
    public static class ObjectProperties
    {
        public const int Name = 0x0001;
        public const int Description = 0x0002;
        public const int Model = 0x0003;
        public const int Position = 0x0004;
        public const int Velocity = 0x0005;
        public const int Orientation = 0x0006;
        public const int Scale = 0x0007;
        public const int Color = 0x0008;
        public const int AnimationState = 0x0009;
    }

    // I pulled the below definitions from protocol pages - https://skunkworks.sourceforge.net/protocol/Protocol.php
    public static class ObjectCategories
    {
        public const long MeleeWeapon = 0x00000001;
        public const long Armor = 0x00000002;
        public const long Clothing = 0x00000004;
        public const long Jewelry = 0x00000008;
        public const long Creature = 0x00000010; // (Player /NPC/Monster)
        public const long Food = 0x00000020;
        public const long Pyreals = 0x00000040;
        public const long Miscellaneous = 0x00000080;
        public const long MissileWeapons_Ammunition = 0x00000100;
        public const long Containers = 0x00000200;
        public const long WrappedFletchingSupplies_HouseDecorations = 0x00000400;
        public const long Gems_Packdolls_DecorativeStatues = 0x00000800;
        public const long SpellComponents = 0x00001000;
        public const long Books_Parchment_Scrolls_Signs_Statues = 0x00002000;
        public const long Keys_Lockpicks = 0x00004000;
        public const long CastingItem_wand_orb_staff = 0x00008000;
        public const long Portal = 0x00010000;
        public const long Lockable = 0x00020000;
        public const long TradeNotes = 0x00040000;
        public const long ManaStones_ManaCharges = 0x00080000;
        public const long Services = 0x00100000;
        public const long unknown_1 = 0x00200000;
        public const long CookingIngredients_Supplies_Plants_DyePots = 0x00400000;
        public const long LooseFletchingSupplies = 0x00800000;
        public const long unknown_2 = 0x01000000;
        public const long unknown_3 = 0x02000000;
        public const long AlchemyIngredients_Supplies_Oils_DyeVials = 0x04000000;
        public const long unknown_4 = 0x08000000;
        public const long Lifestone = 0x10000000;
        public const long Ust = 0x20000000;
        public const long Salvage = 0x40000000;
        public const long unknown_5 = 0x80000000;
    }

    // I pulled the below definitions from protocol pages - https://skunkworks.sourceforge.net/protocol/Protocol.php
    public static class ObjectMaterialTypes
    {
        public const uint Ceramic = 0x00000001;
        public const uint Porcelain = 0x00000002;
        public const uint Linen = 0x00000004;
        public const uint Satin = 0x00000005;
        public const uint Silk = 0x00000006;
        public const uint Velvet = 0x00000007;
        public const uint Wool = 0x00000008;
        public const uint Agate = 0x0000000A;
        public const uint Amber = 0x0000000B;
        public const uint Amethyst = 0x0000000C;
        public const uint Aquamarine = 0x0000000D;
        public const uint Azurite = 0x0000000E;
        public const uint BlackGarnet = 0x0000000F;
        public const uint BlackOpal = 0x00000010;
        public const uint Bloodstone = 0x00000011;
        public const uint Carnelian = 0x00000012;
        public const uint Citrine = 0x00000013;
        public const uint Diamond = 0x00000014;
        public const uint Emerald = 0x00000015;
        public const uint FireOpal = 0x00000016;
        public const uint GreenGarnet = 0x00000017;
        public const uint GreenJade = 0x00000018;
        public const uint Hematite = 0x00000019;
        public const uint ImperialTopaz = 0x0000001A;
        public const uint Jet = 0x0000001B;
        public const uint LapisLazuli = 0x0000001C;
        public const uint LavenderJade = 0x0000001D;
        public const uint Malachite = 0x0000001E;
        public const uint Moonstone = 0x0000001F;
        public const uint Onyx = 0x00000020;
        public const uint Opal = 0x00000021;
        public const uint Peridot = 0x00000022;
        public const uint RedGarnet = 0x00000023;
        public const uint RedJade = 0x00000024;
        public const uint RoseQuartz = 0x00000025;
        public const uint Ruby = 0x00000026;
        public const uint Sapphire = 0x00000027;
        public const uint SmokeyQuartz = 0x00000028;
        public const uint Sunstone = 0x00000029;
        public const uint TigerEye = 0x0000002A;
        public const uint Tourmaline = 0x0000002B;
        public const uint Turquoise = 0x0000002C;
        public const uint WhiteJade = 0x0000002D;
        public const uint WhiteQuartz = 0x0000002E;
        public const uint WhiteSapphire = 0x0000002F;
        public const uint YellowGarnet = 0x00000030;
        public const uint YellowTopaz = 0x00000031;
        public const uint Zircon = 0x00000032;
        public const uint Ivory = 0x00000033;
        public const uint Leather = 0x00000034;
        public const uint ArmoredilloHide = 0x00000035;
        public const uint GromnieHide = 0x00000036;
        public const uint ReedSharkHide = 0x00000037;
        public const uint Brass = 0x00000039;
        public const uint Bronze = 0x0000003A;
        public const uint Copper = 0x0000003B;
        public const uint Gold = 0x0000003C;
        public const uint Iron = 0x0000003D;
        public const uint Pyreal = 0x0000003E;
        public const uint Silver = 0x0000003F;
        public const uint Steel = 0x00000040;
        public const uint Alabaster = 0x00000042;
        public const uint Granite = 0x00000043;
        public const uint Marble = 0x00000044;
        public const uint Obsidian = 0x00000045;
        public const uint Sandstone = 0x00000046;
        public const uint Serpentine = 0x00000047;
        public const uint Ebony = 0x00000049;
        public const uint Mahogany = 0x0000004A;
        public const uint Oak = 0x0000004B;
        public const uint Pine = 0x0000004C;
        public const uint Teak = 0x0000004D;
    }

    public static class ObjectWieldType
    {
        public const byte MeleeWeapon = 0x01;
        public const byte RangedWeapon = 0x02;
        public const byte MissileAmmunition = 0x03;
        public const byte Shield = 0x04;

    }

    public static class ObjectDWORDPropertyIDs
    {
        public const uint Species = 0x02;
        public const uint Burden = 0x05;
        public const uint EquippedSlots = 0x0A;
        public const uint RareID = 0x11;
        public const uint Value = 0x13;
        public const uint TotalPyreals = 0x14;
        public const uint SkillCreditsAvailable = 0x18;
        public const uint CreatureLevel = 0x19;
        public const uint RestrictedtoAC_ToDPurchasers = 0x1A;
        public const uint ArmorLevel = 0x1C;
        public const uint Rank = 0x1E;
        public const uint Bonded = 0x21;
        public const uint NumberofFollowers = 0x23;
        public const uint Unenchantable = 0x24;
        public const uint LockpickDifficulty = 0x26;
        public const uint Deaths = 0x2B;
        public const uint WandElementalDamageBonus_DamageType = 0x2D;
        public const uint MinimumLevelRestriction = 0x56;
        public const uint MaximumLevelRestriction = 0x57;
        public const uint LockpickSkillBonus = 0x58;
        public const uint AffectsVital_VitalID = 0x59;
        public const uint AffectsVital_Amount_alsoHealingKitSkillBonus = 0x5A;
        public const uint UsesTotal = 0x5B;
        public const uint UsesRemaining = 0x5C;
        public const uint DateofBirth = 0x62;
        public const uint Workmanship = 0x69;
        public const uint Spellcraft = 0x6A;
        public const uint CurrentMana = 0x6B;
        public const uint MaximumMana = 0x6C;
        public const uint ActivationRequirement_ArcaneLoreDifficulty = 0x6D;
        public const uint ActivationRequirement_Rank = 0x6E;
        public const uint PortalRestrictionFlags = 0x6F;
        public const uint Gender = 0x71;
        public const uint Attuned = 0x72;
        public const uint ActivationRequirement_SkillLevel = 0x73;
        public const uint ManaCost = 0x75;
        public const uint AgeSeconds = 0x7D;
        public const uint XPneededfornextpointofVitaePenaltyreduction = 0x81;
        public const uint Material = 0x83;
        public const uint WieldRequirement_Type = 0x9E;
        public const uint WieldRequirement_Attribute = 0x9F;
        public const uint WieldRequirement_Value = 0xA0;
        public const uint SlayerSpecies = 0xA6;
        public const uint NumberofItemsSalvagedFrom = 0xAA;
        public const uint NumberofTimesTinkered = 0xAB;
        public const uint DescriptionFormat = 0xAC;
        public const uint PagesUsed = 0xAE;
        public const uint PagesTotal = 0xAF;
        public const uint ActivationRequirement_SkillID = 0xB0;
        public const uint GemstoneSettingQuantity = 0xB1;
        public const uint GemstoneSettingType = 0xB2;
        public const uint Imbued = 0xB3;
        public const uint ChessRank = 0xB5;
        public const uint Heritage = 0xBC;
        public const uint FishingSkill = 0xC0;
        public const uint KeysHeld = 0xC1;
        public const uint ElementalDamageBonus = 0xCC;
        public const uint Augmentation_ReinforcementoftheLugians = 0xDA;
        public const uint Augmentation_BleearghsFortitude = 0xDB;
        public const uint Augmentation_OswaldsEnhancement = 0xDC;
        public const uint Augmentation_SiraluunsBlessing = 0xDD;
        public const uint Augmentation_EnduringCalm = 0xDE;
        public const uint Augmentation_SteadfastWill = 0xDF;
        public const uint Augmentation_CiandrasEssence = 0xE0;
        public const uint Augmentation_YoshisEssence = 0xE1;
        public const uint Augmentation_JibrilsEssence = 0xE2;
        public const uint Augmentation_CeldisethsEssence = 0xE3;
        public const uint Augmentation_KogasEssence = 0xE4;
        public const uint Augmentation_ShadowoftheSeventhMule = 0xE5;
        public const uint Augmentation_MightoftheSeventhMule = 0xE6;
        public const uint Augmentation_ClutchoftheMiser = 0xE7;
        public const uint Augmentation_EnduringEnchantment = 0xE8;
        public const uint Augmentation_CriticalProtection = 0xE9;
        public const uint Augmentation_QuickLearner = 0xEA;
        public const uint Augmentation_CiandrasFortune = 0xEB;
        public const uint Augmentation_CharmedSmith = 0xEC;
        public const uint Augmentation_InnateRenewal = 0xED;
        public const uint Augmentation_ArchmagesEndurance = 0xEE;
        public const uint Augmentation_EnchancementoftheBladeTurner = 0xF0;
        public const uint Augmentation_EnchancementoftheArrowTurner = 0xF1;
        public const uint Augmentation_EnchancementoftheMaceTurner = 0xF2;
        public const uint Augmentation_CausticEnhancement = 0xF3;
        public const uint Augmentation_FieryEnchancement = 0xF4;
        public const uint Augmentation_IcyEnchancement = 0xF5;
        public const uint Augmentation_StormsEnhancement = 0xF6;
        public const uint TitlesEarned = 0x106;
    }

    public static class TextColors
    {
        public const int AC_GREEN = 0x01;
        public const int AC_WHITE = 0x02;
        public const int AC_YELLOW = 0x03;
        public const int AC_BROWN = 0x04;
        public const int AC_PURPLE = 0x05;
        public const int AC_RED = 0x06;
        public const int AC_BLUE = 0x07;
        public const int AC_PINK = 0x08;
        public const int AC_LTPINK = 0x09;
        public const int AC_GREY = 0x0C;
        public const int AC_CYAN = 0x0D;
        public const int AC_LTCYAN = 0x0E;
        public const int AC_LTRED = 0x16;

    }

    public static class MoveFlags
    {
        public const int normal = 0;  // Default(normal move)
        public const int stack = 1;  // Move as a stacked item(combine with existing)
        public const int nostack = 2;  // Place in empty container slot, no stack
        public const int special = 4;  // Special(possibly internal use or ignore overflow)
        public const int silent = 8;  // Move without playing sound(speculative)
        public const int topmost = 16; // Move into topmost container only
    }

    public static class CurVitalID
    {
        public const int CurrentHealth = 0x02; // Health Vital
        public const int CurrentStamina = 0x04; // Stamina Vital
        public const int CurrentMana = 0x06; // Mana Vital
    }

    public static class SkillType
    {
        public const int Axe = 0x01;
        public const int Bow = 0x02;
        public const int Crossbow = 0x03;
        public const int Dagger = 0x04;
        public const int Mace = 0x05;
        public const int MeleeDefense = 0x06;
        public const int MissileDefense = 0x07;
        public const int Spear = 0x09;
        public const int Staff = 0x0A;
        public const int Sword = 0x0B;
        public const int ThrownWeapons = 0x0C;
        public const int UnarmedCombat = 0x0D;
        public const int ArcaneLore = 0x0E;
        public const int MagicDefense = 0x0F;
        public const int ManaConversion = 0x10;
        public const int ItemTinkering = 0x12;
        public const int AssessPerson = 0x13;
        public const int Deception = 0x14;
        public const int Healing = 0x15;
        public const int Jump = 0x16;
        public const int Lockpick = 0x17;
        public const int Run = 0x18;
        public const int AssessCreature = 0x1B;
        public const int WeaponTinkering = 0x1C;
        public const int ArmorTinkering = 0x1D;
        public const int MagicItemTinkering = 0x1E;
        public const int CreatureEnchantment = 0x1F;
        public const int ItemEnchantment = 0x20;
        public const int LifeMagic = 0x21;
        public const int WarMagic = 0x22;
        public const int Leadership = 0x23;
        public const int Loyalty = 0x24;
        public const int Fletching = 0x25;
        public const int Alchemy = 0x26;
        public const int Cooking = 0x27;
        public const int Salvaging = 0x28;
    }

    public static class SchoolID
    {
        public const int War       = 0x01; // War Magic
        public const int Life      = 0x02; // Life Magic
        public const int Item      = 0x03; // Item Enchantment
        public const int Creature  = 0x04; // Creature Enchantment
        public const int Void      = 0x05; // Void Magic
        public const int Summoning = 0x06; // Summoning Spells
    }

    public static class BusyState
    {
        public const int Idle = 0;
        public const int CombiningStack = 1;
        public const int SplittingStack = 2;
        public const int Unknown = 3;
        public const int PickupItemFromGround = 4;
        public const int MovingUnequippingItem = 5;
        public const int DropItemToGround = 6;
        public const int EquipItem = 7;
    }

    public enum LootState
    {
        Idle,
        ScanForCorpse,
        OpenCorpse,
        WaitForOpenCorpse,
        ScanForItems,
        PickupItems,
        WaitForPickupItems,
        Done,
        Unknown 
    }

}
