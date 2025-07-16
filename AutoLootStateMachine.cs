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
using System.Text;
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
            public Timer openCorpseTimer;
            public Timer grabItemTimer;
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

            bool isTrainedCreatureEnchantment = false;
            bool isTrainedLifeMagic = false;
            bool isTrainedItemEnchantment = false;
            bool isTrainedWarMagic = false;
            bool isTrainedVoidMagic = false;
            bool isTrainedSummoningEnchantment = false;

            Stopwatch stopwatch0 = new Stopwatch();
            Stopwatch stopwatch1 = new Stopwatch();
            Stopwatch stopwatch2 = new Stopwatch();
            Stopwatch stopwatch3 = new Stopwatch();
            Stopwatch stopwatch4 = new Stopwatch();

            // Constructor
            public AutoLootStateMachine(PluginCore core)
            {
                tickTimer = new Timer(1000); // 1.0 second loop
                tickTimer.Elapsed += OnTick;
                tickTimer.AutoReset = true; // Set the timer to repeat

                openCorpseTimer = new Timer(4000); // 4 seconds to wait for corpse to open
                openCorpseTimer.Elapsed += (sender, e) => SetState(LootState.ScanForItems);
                openCorpseTimer.AutoReset = false; // Set the timer to not repeat

                grabItemTimer = new Timer(4000); // 4 seconds to wait for item to be grabbed
                grabItemTimer.Elapsed += (sender, e) => SetState(LootState.PickupItems);
                grabItemTimer.AutoReset = false; // Set the timer to not repeat

                pluginCore = core;
            }

            public void Start()
            {
                isTrainedCreatureEnchantment = Decal.Adapter.CoreManager.Current.CharacterFilter.Skills[Decal.Adapter.Wrappers.CharFilterSkillType.CreatureEnchantment].Training >= TrainingType.Trained;
                isTrainedLifeMagic = Decal.Adapter.CoreManager.Current.CharacterFilter.Skills[Decal.Adapter.Wrappers.CharFilterSkillType.LifeMagic].Training >= TrainingType.Trained;
                isTrainedItemEnchantment = Decal.Adapter.CoreManager.Current.CharacterFilter.Skills[Decal.Adapter.Wrappers.CharFilterSkillType.ItemEnchantment].Training >= TrainingType.Trained;
                isTrainedWarMagic = Decal.Adapter.CoreManager.Current.CharacterFilter.Skills[Decal.Adapter.Wrappers.CharFilterSkillType.WarMagic].Training >= TrainingType.Trained;
                isTrainedVoidMagic = Decal.Adapter.CoreManager.Current.CharacterFilter.Skills[Decal.Adapter.Wrappers.CharFilterSkillType.VoidMagic].Training >= TrainingType.Trained;
                isTrainedSummoningEnchantment = Decal.Adapter.CoreManager.Current.CharacterFilter.Skills[Decal.Adapter.Wrappers.CharFilterSkillType.Summoning].Training >= TrainingType.Trained;
                tickTimer.Start();
                isRunning = true;
                isBusy = false;
                currentState = LootState.Idle;
                ErrorLogging.log($"[FSM] Plugin started.", int.Parse(pluginCore.editLogLevel.Text));
            }
            public void Stop()
            {
                tickTimer.Stop();
                isRunning = false;
                currentState = LootState.Idle;
                isBusy = false;
                ErrorLogging.log($"[FSM] Plugin stopped.", int.Parse(pluginCore.editLogLevel.Text));
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
                    ErrorLogging.log($"[FSM] SetState() ERROR: Attempted to set state while FSM is not running, stopping FSM timer.", int.Parse(pluginCore.editLogLevel.Text));
                    this.Stop();
                    return;
                }
                if (state == LootState.Unknown)
                {
                    ErrorLogging.log($"[FSM] SetState() ERROR: Attempted to set state while LootState.Unknown, stopping FSM timer.", int.Parse(pluginCore.editLogLevel.Text));
                    this.Stop();
                    return;
                }

                previousState = currentState;
                currentState = state;
                isBusy = false; // reset busy state
                ErrorLogging.log($"[FSM] SetState() SETSTATE: currentState: {previousState} --> NextState: {currentState}", int.Parse(pluginCore.editLogLevel.Text));
                return;
            }

            private void OnTick(object sender, ElapsedEventArgs e)
            {
                if (CoreManager.Current.Actions.BusyState != Decal.Constants.BusyState.Idle)
                {
                    ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] OnTick: currentState: {currentState} -  BusyState: {CoreManager.Current.Actions.BusyState}, skipping clock tick.", int.Parse(pluginCore.editLogLevel.Text));
                    return; // skip this tick if we are busy
                }
                if (isBusy)
                {
                    ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] OnTick: currentState: {currentState} -  isBusy: {isBusy}, skipping clock tick.", int.Parse(pluginCore.editLogLevel.Text));
                    return; // skip this tick if we are busy
                }
                ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] OnTick: currentState: {currentState}  All good, proceed to switch({currentState})", int.Parse(pluginCore.editLogLevel.Text));


                isBusy = true; // set busy state to true to prevent re-entrancy issues
                switch (currentState)
                {
                    case LootState.Idle:
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 1.0 currentState: {currentState}", int.Parse(pluginCore.editLogLevel.Text));
                        if (corpsesToLootIds.Count > 0)
                        {
                            previousState = currentState;
                            currentCorpseId = 0;
                            itemsToLootIds.Clear();
                            currentState = LootState.OpenCorpse;
                            ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 1.1 currentState: {previousState} --> NextState: {currentState}", int.Parse(pluginCore.editLogLevel.Text));
                        }
                        else
                        {
                            previousState = currentState; 
                            currentState = LootState.Idle; // go back to waiting
                            ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 1.2 currentState: {previousState} --> NextState: {currentState} - No corpses to loot, waiting for corpses to be created.", int.Parse(pluginCore.editLogLevel.Text));
                        }
                        isBusy = false; // reset busy state
                        break;


                    case LootState.OpenCorpse:
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 3.0 currentState: {currentState}", int.Parse(pluginCore.editLogLevel.Text));
                        previousState = currentState;
                        currentCorpseId = corpsesToLootIds.First();
                        corpsesToLootIds.Remove(currentCorpseId);
                        corpsesLootedIds.Add(currentCorpseId);

                        // is it close enough that we care about it?
                        double dts = CoreManager.Current.WorldFilter.Distance(CoreManager.Current.CharacterFilter.Id, currentCorpseId);
                        if (dts < 0.03)  // make this a config value
                        {

                            ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 3.1 currentState: OpenCorpse -  Request Open corpse...", int.Parse(pluginCore.editLogLevel.Text));
                            openCorpseTimer.Start(); // Start the timer to wait for the corpse to open
                            currentState = LootState.WaitForOpenCorpse; // go to waiting for corpse to open state
                            CoreManager.Current.Actions.UseItem(currentCorpseId, 0);
                            ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 3.2 currentState: {previousState} --> NextState: {currentState}", int.Parse(pluginCore.editLogLevel.Text));
                        }
                        else
                        {
                            ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 3.3 currentState: OpenCorpse -  Corpse {currentCorpseId} is too far away ({dts}), skipping.", int.Parse(pluginCore.editLogLevel.Text));
                            currentState = LootState.Idle; // go back to waiting
                        }
                        isBusy = false; // reset busy state
                        break;

                    case LootState.WaitForOpenCorpse:
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 4.0 currentState: {currentState}", int.Parse(pluginCore.editLogLevel.Text));
                        previousState = currentState;
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 4.1 currentState: {previousState} --> NextState: {currentState} :: Waited {WaitForOpeningCorpseCurrentTickCount} tick counts for {currentCorpseId} Corpse to open", int.Parse(pluginCore.editLogLevel.Text));
                        isBusy = false; // reset busy state
                        break;


                    case LootState.ScanForItems:
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 5.0 currentState: {currentState}", int.Parse(pluginCore.editLogLevel.Text));
                        previousState = currentState;
                        FindLootableItems(e, currentCorpseId);
                        if (itemsToLootIds.Count > 0)
                        {
                            currentState = LootState.PickupItems;
                        }
                        else
                        {
                            currentState = LootState.Idle;
                        }
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 5.1 currentState: {previousState} --> NextState: {currentState} :: Found {itemsToLootIds.Count} items.", int.Parse(pluginCore.editLogLevel.Text));
                        isBusy = false; // reset busy state
                        break;

                    case LootState.PickupItems:
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 6.0 currentState: {currentState}", int.Parse(pluginCore.editLogLevel.Text));
                        previousState = currentState;
                        // assume the corpse is open and we can loot items
                        // grab item/wiat for grab/go back into PicupItems state
                        if (itemsToLootIds.Count == 0)
                        {
                            currentState = LootState.Idle;
                        }
                        else
                        {
                            itemToGrabId = itemsToLootIds.First();
                            itemsToLootIds.Remove(itemToGrabId);
                            grabItemTimer.Start(); // Start the timer to wait for the item to be grabbed
                            currentState = LootState.WaitForPickupItems;
                            CoreManager.Current.Actions.MoveItem(itemToGrabId, CoreManager.Current.CharacterFilter.Id);
                        }
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 6.1 currentState: {previousState} --> NextState: {currentState} :: Looting {itemsToLootIds.Count} items.", int.Parse(pluginCore.editLogLevel.Text));
                        isBusy = false; // reset busy state
                        break;

                    case LootState.WaitForPickupItems:
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 7.0 currentState: {currentState}", int.Parse(pluginCore.editLogLevel.Text));
                        previousState = currentState;
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 7.1 currentState: {previousState} --> NextState: {currentState} :: Waited {WaitForGrabItemCurrentTickCount} ticks for item {itemToGrabId}", int.Parse(pluginCore.editLogLevel.Text));
                        isBusy = false; // reset busy state    
                        break;

                    default:
                        ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] 9.0 currentState: {currentState} -  ERROR: Unknown state .... Stopping FSM TIMER ", int.Parse(pluginCore.editLogLevel.Text));
                        this.Stop();
                        currentState = LootState.Idle;
                        isBusy = false; // reset busy state
                        break;
                }
            }
                  
            private void FindLootableItems(ElapsedEventArgs e, int corpseId)
            {
                stopwatch0.Restart();

                int spellID;
                FileService fileService;
                SoundPlayer soundPlayerCreate = new SoundPlayer(@"C:\DP3\found.wav");
                SoundPlayer soundPlayerDestroy = new SoundPlayer(@"C:\DP3\gone.wav");
                // I need to figure out a way to access stuff defined in the PluginCore class, like the error log file path
                string errorLogFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + DIR_SEP + "Decal Plugins" +DIR_SEP + PLUGIN + DIR_SEP + FILENAME_ERRORLOG;
                fileService = (FileService)Decal.Adapter.CoreManager.Current.FileService;

                ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] method: FindLootableItems() - Finding items in corpse {corpseId}", int.Parse(pluginCore.editLogLevel.Text));
                foreach (WorldObject obj in CoreManager.Current.WorldFilter.GetByContainer(corpseId))
                {
                    stopwatch1.Restart();
                    if (MatchesAnyRule(buildSearchString(obj)))
                    {
                        itemsToLootIds.Add(obj.Id);
                        continue;
                    }
                    stopwatch1.Stop();
                    long elapsedMilliseconds_t1 = stopwatch1.ElapsedMilliseconds;
                    ErrorLogging.log($"[TIM] [EVT: {e.SignalTime}] method: FindLootableItems() - {obj.Name} ({obj.Id}) - Regex Check: {elapsedMilliseconds_t1} ms", int.Parse(pluginCore.editLogLevel.Text));

                    // If the item is a spell
                    stopwatch2.Restart();
                    if (obj.ObjectClass == ObjectClass.Scroll)
                    {
                        try
                        {   // If the item is a scroll, check to see if I know it
                            // If the spell ID is 0, then it is not a spell scroll
                            spellID = obj.Values(Decal.Adapter.Wrappers.LongValueKey.AssociatedSpell, 0);
                            Spell spell = fileService.SpellTable.GetById(spellID);
                            SpellSchool spellSchool = spell.School;

                            if ((spellSchool.Id == Decal.Constants.SchoolID.Creature) && (isTrainedCreatureEnchantment) ||
                                    (spellSchool.Id == Decal.Constants.SchoolID.Life) && (isTrainedLifeMagic) ||
                                    (spellSchool.Id == Decal.Constants.SchoolID.Item) && (isTrainedItemEnchantment) ||
                                    (spellSchool.Id == Decal.Constants.SchoolID.War)  && (isTrainedWarMagic) ||
                                    (spellSchool.Id == Decal.Constants.SchoolID.Void) && (isTrainedVoidMagic) ||
                                    (spellSchool.Id == Decal.Constants.SchoolID.Summoning) && (isTrainedSummoningEnchantment)) // Add more schools as needed
                            {
                                if (!Decal.Adapter.CoreManager.Current.CharacterFilter.IsSpellKnown(spellID))
                                {
                                    ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] method: FindLootableItems() You do not know the spell: " + obj.Name + "  == GRAB IT !!! ==", Decal.Constants.TextColors.AC_PURPLE);
                                    itemsToLootIds.Add(obj.Id);
                                }
                            }
                            else
                            {
                                ErrorLogging.log($"[FSM] [EVT: {e.SignalTime}] method: FindLootableItems() You do not have this school trained: " + obj.Name + "  == SKIP IT !!! ==", Decal.Constants.TextColors.AC_PURPLE);
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorLogging.LogError(errorLogFile, ex);
                        }
                    }
                    stopwatch2.Stop();

                    long elapsedMilliseconds_t2 = stopwatch2.ElapsedMilliseconds;
                    ErrorLogging.log($"[TIM] [EVT: {e.SignalTime}] method: FindLootableItems() - {obj.Name} ({obj.Id}) - Spell Check: {elapsedMilliseconds_t2} ms", int.Parse(pluginCore.editLogLevel.Text));
                }
                stopwatch0.Stop();
                long elapsedMilliseconds = stopwatch0.ElapsedMilliseconds;
                ErrorLogging.log($"[TIM] [EVT: {e.SignalTime}] method: FindLootableItems() - executed in {elapsedMilliseconds} ms", int.Parse(pluginCore.editLogLevel.Text));
            }

            private string buildSearchString(WorldObject worldObject)
            {
                stopwatch3.Restart();
                if (worldObject == null) return string.Empty;

                // use StringBuilder for better performance when concatenating strings
                StringBuilder sb = new StringBuilder();
                sb.Append("SpellCount:").Append(worldObject.SpellCount).Append(';');
                sb.Append("Behavior:").Append(worldObject.Behavior).Append(';');
                sb.Append("Category:").Append(worldObject.Category).Append(';');
                sb.Append("Container:").Append(worldObject.Container).Append(';');
                sb.Append("Id:").Append(worldObject.Id).Append(';');
                sb.Append("Name:").Append(worldObject.Name).Append(';');
                sb.Append("ObjectClass:").Append(worldObject.ObjectClass).Append(';');
                sb.Append("Type:").Append(worldObject.Type).Append(';');
                sb.Append("Value:").Append(worldObject.Values(LongValueKey.Value)).Append(';');
                sb.Append("Material:").Append(worldObject.Values(LongValueKey.Material)).Append(';');

                stopwatch3.Stop();
                long elapsedMilliseconds = stopwatch3.ElapsedMilliseconds;
                ErrorLogging.log($"[TIM] buildSearchString() - executed in {elapsedMilliseconds} ms", int.Parse(pluginCore.editLogLevel.Text));
                ErrorLogging.log($"[FSM] buildSearchString() - {sb.ToString()}", int.Parse(pluginCore.editLogLevel.Text));

                return (sb.ToString());
            }

            public bool MatchesAnyRule(string searchString)
            {
                // Start measuring time
                stopwatch4.Restart();

                if (pluginCore.pluginSettings.Items == null || pluginCore.pluginSettings.Items.Count == 0 || string.IsNullOrWhiteSpace(searchString))
                    return false;

                foreach (var rule in pluginCore.pluginSettings.Items)
                {
                    if (!rule.enabled || string.IsNullOrWhiteSpace(rule.regex) || (rule.CompiledRegex == null))
                    { 
                        ErrorLogging.log($"[FSM] Method: MatchesAnyRule - Rule '{rule.rulename}' is disabled or has an invalid regex, skipping.", int.Parse(pluginCore.editLogLevel.Text));
                        continue; // Skip disabled rules or rules with empty regex
                    }

                    try
                    {
                        //if (Regex.IsMatch(searchString, rule.Regex, RegexOptions.IgnoreCase))
                        if (rule.CompiledRegex.IsMatch(searchString))
                        {
                            ErrorLogging.log($"[FSM] Method: MatchesAnyRule - Regex in rule matched object == Rule - '{rule.rulename}' Regex - {rule.regex} {searchString}", int.Parse(pluginCore.editLogLevel.Text));
                            return true;
                        }
                        else
                        {
                            ErrorLogging.log($"[FSM] Method: MatchesAnyRule - Regex in rule did not match object == Rule - '{rule.rulename}' Regex - {rule.regex} {searchString}", int.Parse(pluginCore.editLogLevel.Text));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Optional: log invalid regex here
                        ErrorLogging.LogError(pluginCore.errorLogFile, ex);
                    }
                }

                // Stop measuring time
                stopwatch4.Stop();
                long elapsedMilliseconds = stopwatch4.ElapsedMilliseconds;
                ErrorLogging.log($"[TIM] MatchesAnyRule() - executed in {elapsedMilliseconds} ms", int.Parse(pluginCore.editLogLevel.Text));

                return false;
            }

        }


        //---------------------------------------------------------------------------------------------------
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
                    ErrorLogging.log($"[OWC: {timestamp}] method: FSM_WorldFilter_CreateObject() - Found corpse {e.New.Id} in corpsesToLootIds, skipping.", int.Parse(editLogLevel.Text));
                }
                else if (autoLootStateMachine.GetcorpsesLootedIds().Contains(e.New.Id))
                {
                    ErrorLogging.log($"[OWC: {timestamp}] method: FSM_WorldFilter_CreateObject() - Found corpse {e.New.Id} in corpsesLootedIds, skipping.", int.Parse(editLogLevel.Text));
                }
                else
                {
                    autoLootStateMachine.GetcorpsesToLootIds().Add(e.New.Id);
                    ErrorLogging.log($"[OWC: {timestamp}] method: FSM_WorldFilter_CreateObject() - Adding corpse {e.New.Id} to corpsesToLootIds.", int.Parse(editLogLevel.Text));
                }
            }
            catch (Exception ex)
            {
                CoreManager.Current.Actions.AddChatText($"[FSM] ERROR: {ex.Message}", 5);
            }
        }
    }
}

