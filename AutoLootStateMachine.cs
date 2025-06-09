using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Constants;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Media;
namespace WaynesWorld
{
    public class AutoLootStateMachine
    {
        public partial class PluginCore : PluginBase
        {
            private LootState currentState = LootState.Idle;
            private Timer tickTimer;
            private int currentCorpseId = 0;
            private List<WorldObject> itemsToLoot = new List<WorldObject>();

            public AutoLootStateMachine()
            {
                tickTimer = new Timer(1000); // 1 second loop
                tickTimer.Elapsed += OnTick;
            }

            public void Start() => tickTimer.Start();
            public void Stop() => tickTimer.Stop();

            protected override void Startup()
            {
                Host.Actions.AddChatText("[LootFSM] Plugin started.", 5);
            }

            protected override void Shutdown()
            {
                tickTimer.Stop();
                Host.Actions.AddChatText("[LootFSM] Plugin stopped.", 5);
            }
            private void OnTick(object sender, ElapsedEventArgs e)
            {
                switch (currentState)
                {
                    case LootState.Idle:
                        Host.Actions.AddChatText("[LootFSM] Starting scan...", 5);
                        currentState = LootState.ScanForCorpse;
                        break;

                    case LootState.ScanForCorpse:
                        WorldObject corpse = FindNearestCorpse();
                        if (corpse != null)
                        {
                            currentCorpseId = corpse.Id;
                            Host.Actions.AddChatText("[LootFSM] Found corpse: " + corpse.Name, 5);
                            currentState = LootState.OpenCorpse;
                        }
                        else
                        {
                            Host.Actions.AddChatText("[LootFSM] No corpses found.", 5);
                            currentState = LootState.Idle;
                        }
                        break;

                    case LootState.OpenCorpse:
                        Host.Actions.UseItem(currentCorpseId);
                        Host.Actions.AddChatText("[LootFSM] Opening corpse...", 5);
                        currentState = LootState.ScanForItems;
                        break;

                    case LootState.ScanForItems:
                        itemsToLoot = FindLootableItems(currentCorpseId);
                        if (itemsToLoot.Count > 0)
                        {
                            Host.Actions.AddChatText($"[LootFSM] Found {itemsToLoot.Count} items.", 5);
                            currentState = LootState.PickupItems;
                        }
                        else
                        {
                            Host.Actions.AddChatText("[LootFSM] No items to loot.", 5);
                            currentState = LootState.Done;
                        }
                        break;

                    case LootState.PickupItems:
                        foreach (var item in itemsToLoot)
                        {
                            Host.Actions.MoveItem(item.Id, Core.CharacterFilter.Id);
                            Host.Actions.AddChatText($"[LootFSM] Looting {item.Name}", 5);
                        }
                        currentState = LootState.Done;
                        break;

                    case LootState.Done:
                        Host.Actions.AddChatText("[LootFSM] Done looting.", 5);
                        currentCorpseId = 0;
                        itemsToLoot.Clear();
                        currentState = LootState.Idle;
                        break;
                }
            }

            private WorldObject FindNearestCorpse()
            {
                foreach (WorldObject obj in Core.WorldFilter.GetByObjectClass(ObjectClass.Corpse))
                {
                    // get the below to work with the new API
                    if (DistanceToSelf(obj) < 3.0)
                        return obj;

                    //if (obj.DistanceTo(Core.CharacterFilter) < 10.0)
                    //    return obj;
                }
                return null;
            }

            private List<WorldObject> FindLootableItems(int corpseId)
            {
                List<WorldObject> foundItems = new List<WorldObject>();
                foreach (WorldObject obj in Core.WorldFilter)
                {
                    if (obj.Container == corpseId && obj.Values(LongValueKey.StackCount, 1) > 0)
                        foundItems.Add(obj);
                }
                return foundItems;
            }
        }
    }
}