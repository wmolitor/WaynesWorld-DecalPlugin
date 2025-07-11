using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Constants;
using Decal.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Timers;


// think about using an action queue to handle the actions and some logic to manage timing between actions
// this will allow us to queue up actions and process them in a controlled manner
// when the queue is empty, we can go back to scanning for corpses
// We should probably create a seperate List for corpses we have already looted

// not sure if we will need a state machine for this, but it might be useful
// it would be a simple state machine with only a few states: idle, busy



namespace WaynesWorld
{
    //public class AutoLootStateMachine  - I need to figure out which goes inside of which class, or even if it should be inside or a seperate class, or extention of the class...
    //{
     public partial class PluginCore : PluginBase
     {
        public class AutoLootStateMachine
        {
            private LootState previousState = LootState.Unknown;
            private LootState currentState = LootState.Idle;
            private Timer tickTimer;
            private int waitTicks = 0;

            private int itemToGrabId = 0;
            private int currentCorpseId = 0;
            HashSet<int> itemsToLootIds = new HashSet<int>();
            HashSet<int> corpsesToLootIds = new HashSet<int>();
            HashSet<int> corpsesLootedIds = new HashSet<int>();

            private int WaitForOpeningCorpseCurrentTickCount = 0;
            private int WaitForGrabItemTargetTickCount = 0;
            private int WaitForGrabItemCurrentTickCount = 0;
            
            private bool isBusy = false;
            private bool isRunning = false;

            private PluginCore pluginCore;


            public AutoLootStateMachine(PluginCore core)
            {
                tickTimer = new Timer(1000); // 0.5 second loop
                tickTimer.Elapsed += OnTick;
                pluginCore = core;
            }

            public void Start()
            {
                tickTimer.Start();
                isRunning = true;
                ErrorLogging.log($"[FSM] Plugin started.", 3);
            }
            public void Stop()
            {
                tickTimer.Stop();
                isRunning = false;
                ErrorLogging.log($"[FSM] Plugin stopped.", 3);
            }

            public HashSet<int> GetcorpsesLootedIds()
            {
                return corpsesLootedIds;
            }

            public HashSet<int> GetcorpsesToLootIds()
            {
                return corpsesToLootIds;
            }

            public void SetState(LootState state)
            {
                if (!isRunning)
                {
                    ErrorLogging.log($"[FSM] SetState() ERROR: Attempted to set state while FSM is not running, stopping FSM timer.", 3);
                    this.Stop();
                    return;
                }
                if (state == LootState.Unknown)
                {
                    ErrorLogging.log($"[FSM] SetState() ERROR: Attempted to set state while LootState.Unknown, stopping FSM timer.", 3);
                    this.Stop();
                    return;
                }

                previousState = currentState;
                currentState = state;
                isBusy = false; // reset busy state
                ErrorLogging.log($"[FSM] SetState() SETSTATE: currentState: {previousState} --> NextState: {currentState}", 3);
                return;
            }

            private void OnTick(object sender, ElapsedEventArgs e)
            {
                if (CoreManager.Current.Actions.BusyState != Decal.Constants.BusyState.Idle)
                {
                    ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] OnTick: currentState: {currentState} -  BusyState: {CoreManager.Current.Actions.BusyState}, skipping clock tick.", 3);
                    return; // skip this tick if we are busy
                }
                if (isBusy)
                {
                    ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] OnTick: currentState: {currentState} -  isBusy: {isBusy}, skipping clock tick.", 3);
                    return; // skip this tick if we are busy
                }
                ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] OnTick: currentState: {currentState}  All good, proceed to switch({currentState})", 3);


                isBusy = true; // set busy state to true to prevent re-entrancy issues
                switch (currentState)
                {
                    case LootState.Idle:
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 1.0 currentState: {currentState}", 3);
                        if (corpsesToLootIds.Count > 0)
                        {
                            previousState = currentState;
                            currentState = LootState.OpenCorpse;
                            ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 1.1 currentState: {previousState} --> NextState: {currentState}", 3);
                        }
                        else
                        {
                            previousState = currentState; 
                            currentState = LootState.Idle; // go back to waiting
                            ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 1.2 currentState: {previousState} --> NextState: {currentState} - No corpses to loot, waiting for corpses to be created.", 3);
                        }
                        isBusy = false; // reset busy state
                        break;


                    case LootState.ScanForCorpse:
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 2.0 currentState: {currentState}", 3);
                        previousState = currentState;
                        FindNearCorpses(e);
                        currentState = LootState.Unknown;
                        if (corpsesToLootIds.Count > 0)
                        {
                            currentState = LootState.OpenCorpse;
                        }
                        else
                        {
                            currentState = LootState.Idle;
                        }
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 2.1 currentState: ScanForCorpse -  Found corpses: " + corpsesToLootIds.Count, 3);
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 2.2 currentState: {previousState} --> NextState: {currentState}", 3);
                        isBusy = false; // reset busy state
                        break;


                    case LootState.OpenCorpse:
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 3.0 currentState: {currentState}", 3);
                        previousState = currentState;
                        currentState = LootState.Unknown;
                        currentCorpseId = corpsesToLootIds.First();
                        corpsesToLootIds.Remove(currentCorpseId);
                        corpsesLootedIds.Add(currentCorpseId);

                        // is it close enough that we care about it?
                        double dts = CoreManager.Current.WorldFilter.Distance(CoreManager.Current.CharacterFilter.Id, currentCorpseId);
                        if (dts < 0.03)  // make this a config value
                        {

                            CoreManager.Current.Actions.UseItem(currentCorpseId, 0);
                            ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 3.1 currentState: OpenCorpse -  Opening corpse...", 3);
                            waitTicks = 4; // make this a config value
                            WaitForOpeningCorpseCurrentTickCount = 0;
                            currentState = LootState.WaitForOpenCorpse;
                            ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 3.2 currentState: {previousState} --> NextState: {currentState}", 3);
                        }
                        else
                        {
                            ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 3.3 currentState: OpenCorpse -  Corpse {currentCorpseId} is too far away ({dts}), skipping.", 3);
                            currentState = LootState.Idle; // go back to waiting
                        }
                        isBusy = false; // reset busy state
                        break;

                    case LootState.WaitForOpenCorpse:
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 4.0 currentState: {currentState}", 3);
                        previousState = currentState;
                        currentState = LootState.Unknown;
                        // Each tick is 500ms, so count how many times we have ticked
                        // and if we have ticked waitTicks, change state to ScanForItems
                        if (waitTicks > WaitForOpeningCorpseCurrentTickCount)
                        {
                            WaitForOpeningCorpseCurrentTickCount++;
                            currentState = LootState.WaitForOpenCorpse;
                        }
                        else
                        {
                            currentState = LootState.ScanForItems;
                        }
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 4.1 currentState: {previousState} --> NextState: {currentState} :: Waited {WaitForOpeningCorpseCurrentTickCount} tick counts for {currentCorpseId} Corpse to open", 3);
                        isBusy = false; // reset busy state
                        break;


                    case LootState.ScanForItems:
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 5.0 currentState: {currentState}", 3);
                        previousState = currentState;
                        currentState = LootState.Unknown;
                        FindLootableItems(e, currentCorpseId);
                        if (itemsToLootIds.Count > 0)
                        {
                            currentState = LootState.PickupItems;
                        }
                        else
                        {
                            currentState = LootState.Done;
                        }
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 5.1 currentState: {previousState} --> NextState: {currentState} :: Found {itemsToLootIds.Count} items.", 3);
                        isBusy = false; // reset busy state
                        break;

                    case LootState.PickupItems:
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 6.0 currentState: {currentState}", 3);
                        previousState = currentState;
                        currentState = LootState.Unknown;
                        // assume the corpse is open and we can loot items
                        // grab item/wiat for grab/go back into PicupItems state
                        // rework this logic to use the itemsToLoot list and make it re-entrant
                        if (itemsToLootIds.Count == 0)
                        {
                            currentState = LootState.Done;
                        }
                        else
                        {
                            WaitForGrabItemCurrentTickCount = 0;
                            WaitForGrabItemTargetTickCount = 2; // make this a config value
                            itemToGrabId = itemsToLootIds.First();
                            itemsToLootIds.Remove(itemToGrabId);
                            currentState = LootState.WaitForPickupItems;
                            CoreManager.Current.Actions.MoveItem(itemToGrabId, CoreManager.Current.CharacterFilter.Id);
                        }
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 6.1 currentState: {previousState} --> NextState: {currentState} :: Looting {itemsToLootIds.Count} items.", 3);
                        isBusy = false; // reset busy state
                        break;

                    case LootState.WaitForPickupItems:
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 7.0 currentState: {currentState}", 3);
                        previousState = currentState;
                        currentState = LootState.Unknown;
                        // Each tick is 500ms, so count how many times we have ticked
                        // and if we have ticked waitTicks, change state to ScanForItems
                        if (waitTicks > WaitForGrabItemCurrentTickCount)
                        {
                            WaitForGrabItemCurrentTickCount++;
                            currentState = LootState.WaitForPickupItems;
                        }
                        else
                        {
                            currentState = LootState.PickupItems;
                        }
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 7.1 currentState: {previousState} --> NextState: {currentState} :: Waited {WaitForGrabItemCurrentTickCount} ticks for item {itemToGrabId}", 3);
                        isBusy = false; // reset busy state    
                        break;

                    case LootState.Done:
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 8.0 currentState: {currentState}", 3);
                        previousState = currentState;
                        currentState = LootState.Unknown;
                        currentCorpseId = 0;
                        itemsToLootIds.Clear();
                        // check if there are more corpses to loot
                        if (corpsesToLootIds.Count > 0)
                        {
                            currentState = LootState.OpenCorpse; // go back to opening the next corpse
                        }
                        else
                        {
                            currentState = LootState.Idle;
                        }
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 8.1 currentState: {previousState} --> NextState: {currentState} :: Found {corpsesToLootIds.Count} more corpses to loot.", 3);
                        isBusy = false; // reset busy state
                        break;

                    default:
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 9.0 currentState: {currentState} -  ERROR: Unknown state .... Stopping FSM TIMER ", 3);
                        this.Stop();
                        currentState = LootState.Idle;
                        isBusy = false; // reset busy state
                        break;
                }
            }

            private void FindNearCorpses(ElapsedEventArgs e)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                bool found = false; // flag to indicate if we found any corpses to be added to the corpsesToLoot list

                ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] method: FindNearCorpses()", 3);
                foreach (WorldObject obj in CoreManager.Current.WorldFilter.GetByObjectClass(ObjectClass.Corpse))
                {
                    found = false;
                    // is it close enough that we care about it?
                    double dts = CoreManager.Current.WorldFilter.Distance(CoreManager.Current.CharacterFilter.Id, obj.Id);
                    if (dts > 0.03)  // make this a config value
                    {
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] method: FindNearCorpses() - {obj.Name}: {obj.Id}) at distance {dts} - too far away, skip", 3);
                        found = true; // set found to true, too far away, but same effect, it skips this corpse
                        continue; // skip this corpse if it is too far away
                    }
                    if (!found)
                    {
                        if (corpsesToLootIds.Contains(obj.Id))
                        {
                            // in theory, this should never happen, but just in case
                            ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] method: FindNearCorpses() - Found corpse {obj.Id} in corpsesToLootIds set, skipping.", 3);
                            found = true; // set found to true so we can skip this corpse
                            continue; // skip this corpse
                        }
                    }
                    if (!found)
                    {
                        if (corpsesLootedIds.Contains(obj.Id))
                        {
                            ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] method: FindNearCorpses() - Found corpse {obj.Id} in corpsesLootedIds set, skipping.", 3);
                            found = true; // set found to true so we can skip this corpse
                            continue; // skip this corpse
                        }
                    }
                    // if we get here, the corpse is close, is not in either list, so we can add it to the corpsesToLoot list
                    if (!found)
                    {
                        corpsesToLootIds.Add(obj.Id);
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] method: FindNearCorpses() - {obj.Name}: {obj.Id}) at distance {dts} - corpsesToLoot.Add()", 3);
                    }
                }

                stopwatch.Stop();
                long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] method: FindNearCorpses() - executed in {elapsedMilliseconds} ms", 3);

                return;
            }

            private void FindLootableItems(ElapsedEventArgs e, int corpseId)
            {
                int spellID;
                FileService fileService;
                SoundPlayer soundPlayerCreate = new SoundPlayer(@"C:\DP3\found.wav");
                SoundPlayer soundPlayerDestroy = new SoundPlayer(@"C:\DP3\gone.wav");
                // I need to figure out a way to access stuff defined in the PluginCore class, like the error log file path
                string errorLogFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + DIR_SEP + "Decal Plugins" + DIR_SEP + PLUGIN + DIR_SEP + FILENAME_ERRORLOG;

                ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] method: FindLootableItems() - Finding items in corpse {corpseId}", 3);
                foreach (WorldObject obj in CoreManager.Current.WorldFilter.GetByContainer(corpseId))
                {
                    //if (obj.Container == corpseId && obj.Values(LongValueKey.Value, 1) >= 4000)
                    //{
                    //   itemsToLootIds.Add(obj.Id);
                    //    continue;
                    //}
                    if (obj.Container == corpseId)
                    {
                        if (MatchesAnyRule(buildSearchString(obj)))
                        {
                            ErrorLogging.log($"[FSM] Object matched a rule: {obj.Name}", 3);
                            itemsToLootIds.Add(obj.Id);
                            continue;
                        }
                    }

                    // If the item is a spell
                    if (obj.ObjectClass == ObjectClass.Scroll)
                    {
                        try
                        {   // If the item is a scroll, check to see if I know it
                            // If the spell ID is 0, then it is not a spell scroll
                            spellID = obj.Values(Decal.Adapter.Wrappers.LongValueKey.AssociatedSpell, 0);
                            fileService = (FileService)Decal.Adapter.CoreManager.Current.FileService;
                            Spell spell = fileService.SpellTable.GetById(spellID);
                            SpellSchool spellSchool = spell.School;
                            bool creatureTrained = false;
                            bool lifeTrained = false;
                            bool itemTrained = false;
                            bool warTrained = false;
                            bool voidTrained = false;
                            if (Decal.Adapter.CoreManager.Current.CharacterFilter.Skills[Decal.Adapter.Wrappers.CharFilterSkillType.CreatureEnchantment].Training >= TrainingType.Trained)
                                creatureTrained = true;
                            if (Decal.Adapter.CoreManager.Current.CharacterFilter.Skills[Decal.Adapter.Wrappers.CharFilterSkillType.LifeMagic].Training >= TrainingType.Trained)
                                lifeTrained = true;
                            if (Decal.Adapter.CoreManager.Current.CharacterFilter.Skills[Decal.Adapter.Wrappers.CharFilterSkillType.ItemEnchantment].Training >= TrainingType.Trained)
                                itemTrained = true;
                            if (Decal.Adapter.CoreManager.Current.CharacterFilter.Skills[Decal.Adapter.Wrappers.CharFilterSkillType.WarMagic].Training >= TrainingType.Trained)
                                warTrained = true;
                            if (Decal.Adapter.CoreManager.Current.CharacterFilter.Skills[Decal.Adapter.Wrappers.CharFilterSkillType.VoidMagic].Training >= TrainingType.Trained)
                                voidTrained = true;
                            if ((spellSchool.Id == Decal.Constants.SchoolID.Creature) && (creatureTrained) ||
                                    (spellSchool.Id == Decal.Constants.SchoolID.Life) && (lifeTrained) ||
                                    (spellSchool.Id == Decal.Constants.SchoolID.Item) && (itemTrained) ||
                                    (spellSchool.Id == Decal.Constants.SchoolID.War) && (warTrained) ||
                                    (spellSchool.Id == Decal.Constants.SchoolID.Void) && (voidTrained))
                            {
                                if (!Decal.Adapter.CoreManager.Current.CharacterFilter.IsSpellKnown(spellID))
                                {
                                    ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] method: FindLootableItems() You do not know the spell: " + obj.Name + "  == GRAB IT !!! ==", Decal.Constants.TextColors.AC_PURPLE);
                                    soundPlayerCreate.Play(); // Play the sound for an unknown spell being found
                                    itemsToLootIds.Add(obj.Id);
                                }
                            }
                            else
                            {
                                ErrorLogging.log("$[FSM] [EVT: {e.SignalTime}] method: FindLootableItems() You do not have this school trained: " + obj.Name + "  == SKIP IT !!! ==", Decal.Constants.TextColors.AC_PURPLE);
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorLogging.LogError(errorLogFile, ex);
                        }
                    }
                }
            }

            public static string buildSearchString(WorldObject worldObject)
            {
                if (worldObject == null) return string.Empty;
                string searchString = (
                      "f1=" + worldObject.SpellCount.ToString() + ";"
                    + "f2=" + worldObject.Behavior.ToString() + ";"
                    + "f3=" + worldObject.Category.ToString() + ";"
                    + "f4=" + worldObject.Container.ToString() + ";"
                    + "f5=" + worldObject.Id.ToString() + ";"
                    + "f6=" + worldObject.Name.ToString() + ";"
                    + "f7=" + worldObject.ObjectClass.GetType() + ";"
                    + "f8=" + worldObject.Type.ToString() + ";"
                    + "f9=" + worldObject.Values(LongValueKey.Value) + ";"
                    + "f10=" + worldObject.Values(LongValueKey.Material) + ";"
                    );
                return (searchString);
            }

            public bool MatchesAnyRule(string searchString)
            {
                if (pluginCore.pluginSettings.Items == null || pluginCore.pluginSettings.Items.Count == 0 || string.IsNullOrWhiteSpace(searchString))
                    return false;

                foreach (var rule in pluginCore.pluginSettings.Items)
                {
                    if (!rule.Enabled || string.IsNullOrWhiteSpace(rule.Regex))
                        continue;

                    try
                    {
                        if (Regex.IsMatch(searchString, rule.Regex, RegexOptions.IgnoreCase))
                        {
                            ErrorLogging.log($"[FSM] Regex in rule matched object: '{rule.RuleName}': {rule.Regex}: {searchString}", 3);
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Optional: log invalid regex here
                        ErrorLogging.LogError(pluginCore.errorLogFile, ex);
                    }
                }

                return false;
            }

        }



        // Below this point is PluginCore.cs code that is used to handle events and create objects.
        // You are no longer in the AutoLootStateMachine class, but in the PluginCore class
        public void FSM_WorldFilter_CreateObject(object sender, CreateObjectEventArgs e)
        {
                // See about collecting the corpse information as they are created, and then using that information to populate the corpsesToLootIds list
                if (!loginComplete)
                {
                    return;
                }
                if (e.New.ObjectClass != ObjectClass.Corpse)
                {
                    return; // only interested in corpses
                }
            // DO STUFF HERE
            try
            {
                DateTime now = DateTime.Now;
                string timestamp = now.ToString("HH:mm:ss.fff");

                if (autoLootStateMachine.GetcorpsesToLootIds().Contains(e.New.Id))
                {
                    // in theory, this should never happen, but just in case
                    ErrorLogging.log($"[OWC: {timestamp}] method: FSM_WorldFilter_CreateObject() - Found corpse {e.New.Id} in corpsesToLootIds, skipping.", 3);
                }
                else if (autoLootStateMachine.GetcorpsesLootedIds().Contains(e.New.Id))
                {
                    ErrorLogging.log($"[OWC: {timestamp}] method: FSM_WorldFilter_CreateObject() - Found corpse {e.New.Id} in corpsesLootedIds, skipping.", 3);
                }
                else
                {
                    autoLootStateMachine.GetcorpsesToLootIds().Add(e.New.Id);
                    ErrorLogging.log($"[OWC: {timestamp}] method: FSM_WorldFilter_CreateObject() - Adding corpse {e.New.Id} to corpsesToLootIds.", 3);
                }
            }
            catch (Exception ex)
            {
                CoreManager.Current.Actions.AddChatText($"[FSM] ERROR: {ex.Message}", 5);
            }
        }
    }
}

