using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Timers;

namespace WaynesWorld
{
    public partial class PluginCore : PluginBase
    {
        // This flag is used to determine if a queued action is currently being processed
        bool inAction = false;

        public void initTimer() 
        {
            // timer for action queue
            lootTimer = new Timer(1000); // Set the timer to .5 second
            lootTimer.AutoReset = true; // Set the timer to repeat
            lootTimer.Elapsed += LootTimer_Tick;
            WriteToChat("AutoLoot Timer initialized. Please hit start button!");
            
            // Todo: uncomment to start the timer
            //lootTimer.Start(); // Start the timer
        }

        public void destroyTimer()
        {
            WriteToChat("AutoLoot Plugin shutting down.");
            lootTimer?.Stop();
            lootTimer?.Dispose();
            CoreManager.Current.WorldFilter.ChangeObject -= WorldFilter_ChangeObject_Timer;
        }


        private void LootTimer_Tick(object sender, EventArgs e)
        {
            WriteToChat($"AutoLoot Timer ticked. isBusy: {isBusy}");
            if (isBusy == 0)
            {
                TryNextAction();
            }
        }

        private void EnqueueAction(Action action)
        {
            actionQueue.Enqueue(action);
        }

        private void TryNextAction()
        {
            WriteToChat($"TryNextAction. isBusy: {isBusy}  actionQueue.Count: {actionQueue.Count}");
            if (isBusy == 0)
            {
                if ((actionQueue.Count > 0) && !inAction)
                {
                    var action = actionQueue.Dequeue();
                    inAction = true; // Set the flag to indicate an action is being processed
                    action.Invoke();
                }
                else if (actionQueue.Count == 0)
                {
                    ScanForCorpses();
                }
            }
        }

        private void ScanForCorpses()
        {
            WriteToChat("Scanning for corpses...");
            foreach (WorldObject obj in CoreManager.Current.WorldFilter.GetByObjectClass(ObjectClass.Corpse))
            {
                int count = CoreManager.Current.WorldFilter.GetByObjectClass(ObjectClass.Corpse).Count;
                int current = 0;
                if (count == 0)
                {
                    WriteToChat("No corpses found.");
                    return;
                }
                WriteToChat($"Checking corpse({++current}/{count}): {obj.Name} at distance {DistanceToSelf(obj)}");
                if (obj.Name != null && DistanceToSelf(obj) < 2.0)
                {
                    WriteToChat($"Queue corpse: {obj.Name}");
                    EnqueueAction(() => OpenCorpse(obj.Id));
                    //break;
                }
            }
        }

        private void OpenCorpse(int corpseId)
        {
            try
            {
                WriteToChat("Opening corpse...");
                CoreManager.Current.Actions.UseItem(corpseId, 0); // Open container
                                                                  // Delay actual looting slightly (corpse takes a moment to open)
                Timer delay = new Timer { Interval = double.Parse(editOpenTimer.Text) };
                delay.Elapsed += (s, e) =>
                {
                    delay.Stop();
                    delay.Dispose();
                    LootFromCorpse(corpseId);
                };
                delay.Start();
            }
            catch (Exception ex)
            {
                WriteToChat($"Error opening corpse: {ex.Message}");
                ErrorLogging.LogError(errorLogFile, ex);
            } 
        }

        private void LootFromCorpse(int corpseId)
        {
            try
            {
                WorldObjectCollection allObjects = CoreManager.Current.WorldFilter.GetAll();
                IEnumerator<WorldObject> woEnumerator = allObjects.GetEnumerator();
                while (woEnumerator.MoveNext())
                {
                    WorldObject obj = woEnumerator.Current;
                    if (obj.Container == corpseId)
                    {
                        if (ShouldLoot(obj))
                        {
                            WriteToChat($"Looting: {obj.Name}");
                            Timer delay = new Timer { Interval = double.Parse(editLootTimer.Text) };
                            delay.Elapsed += (s, e) =>
                            {
                                delay.Stop();
                                delay.Dispose();
                                CoreManager.Current.Actions.MoveItem(obj.Id, CoreManager.Current.CharacterFilter.Id, Decal.Constants.MoveFlags.normal);
                            };
                            delay.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToChat($"Error looting from corpse: {ex.Message}");
                ErrorLogging.LogError(errorLogFile, ex);
            }
            inAction = false;
        }

        private bool ShouldLoot(WorldObject item)
        {
            // Customize this logic as needed
            if (item.Name.Contains(TestEdit.Text))
            {
                WriteToChat($"Looting {item.Name} because it matches the filter text.");
                return true;
            }

            if (item.Values(LongValueKey.Value) > 5000)
            {
                WriteToChat($"Looting {item.Name} because its value is greater than 5000.");
                return true;
            }   

            return false;
        }


        private void WorldFilter_ChangeObject_Timer(object sender, ChangeObjectEventArgs e)
        {
            if ((e.Changed.Id == CoreManager.Current.CharacterFilter.Id) && (isBusy == 0))
            {
                TryNextAction();
            }
        }
              
    }
}
