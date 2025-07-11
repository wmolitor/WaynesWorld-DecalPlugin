using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Constants;
using Decal.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;

//using System.Collections.Generic;
//using System.Media;
//using System.IO.Pipes;

namespace WaynesWorld
{
    public partial class PluginCore
    {
        private FileService fileService = null;
        private void initWorldFilter()
        {
            //////////////////////////////////////////////////////////////
            // To initialize any of the World Filter Events,            //
            // simply uncomment the appropriate initialization          //
            // statement(s) below to enable the event handler           //
            //////////////////////////////////////////////////////////////

            // Initialize the ResetTrade event handler
            // Core.WorldFilter.ResetTrade += new EventHandler<ResetTradeEventArgs>(WorldFilter_ResetTrade);

            // Initialize the ReleaseObject event handler
            Core.WorldFilter.ReleaseObject += new EventHandler<ReleaseObjectEventArgs>(WorldFilter_ReleaseObject);

            // Initialize the ReleaseDone event handler
            // Core.WorldFilter.ReleaseDone += new EventHandler(WorldFilter_ReleaseDone);

            // Initialize the MoveObject event handler
            // Core.WorldFilter.MoveObject += new EventHandler<MoveObjectEventArgs>(WorldFilter_MoveObject);

            // Initialize the FailToCompleteTrade event handler
            // Core.WorldFilter.FailToCompleteTrade += new EventHandler(WorldFilter_FailToCompleteTrade);

            // Initialize the FailToAddTradeItem event handler
            // Core.WorldFilter.FailToAddTradeItem += new EventHandler<FailToAddTradeItemEventArgs>(WorldFilter_FailToAddTradeItem);

            // Initialize the EnterTrade event handler
            // Core.WorldFilter.EnterTrade += new EventHandler<EnterTradeEventArgs>(WorldFilter_EnterTrade);

            // Initialize the EndTrade event handler
            // Core.WorldFilter.EndTrade += new EventHandler<EndTradeEventArgs>(WorldFilter_EndTrade);

            // Initialize the DeclineTrade event handler
            // Core.WorldFilter.DeclineTrade += new EventHandler<DeclineTradeEventArgs>(WorldFilter_DeclineTrade);

            // Initialize the CreateObject event handler
            //Core.WorldFilter.CreateObject += new EventHandler<CreateObjectEventArgs>(WorldFilter_CreateObject);
            Core.WorldFilter.CreateObject += new EventHandler<CreateObjectEventArgs>(FSM_WorldFilter_CreateObject);

            // Initialize the ChangeObject event handler
            // Core.WorldFilter.ChangeObject += new EventHandler<ChangeObjectEventArgs>(WorldFilter_ChangeObject);

            // Initialize the ApproachVendor event handler
            // Core.WorldFilter.ApproachVendor += new EventHandler<ApproachVendorEventArgs>(WorldFilter_ApproachVendor);

            // Initialize the AddTradeItem event handler
            // Core.WorldFilter.AddTradeItem += new EventHandler<AddTradeItemEventArgs>(WorldFilter_AddTradeItem);

            // Initialize the AcceptTrade event handler
            // Core.WorldFilter.AcceptTrade += new EventHandler<AcceptTradeEventArgs>(WorldFilter_AcceptTrade);
        }

        void WorldFilter_AcceptTrade(object sender, AcceptTradeEventArgs e)
        {
            // DO STUFF HERE
        }

        void WorldFilter_AddTradeItem(object sender, AddTradeItemEventArgs e)
        {
            // DO STUFF HERE
        }

        void WorldFilter_ApproachVendor(object sender, ApproachVendorEventArgs e)
        {
            // DO STUFF HERE
        }

        void WorldFilter_ChangeObject(object sender, ChangeObjectEventArgs e)
        {
            // DO STUFF HERE
        }

        
        void WorldFilter_CreateObject(object sender, CreateObjectEventArgs e)
        {
            int spellID;

            // If the item is an Item, then check for spells and seek items
            if (!loginComplete)
            {
                return;
            }
            else if (e.New.ObjectClass == ObjectClass.Scroll)
            {
                try
                {   // If the item is a scroll, check to see if I know it
                    // If the spell ID is 0, then it is not a spell scroll
                    spellID = e.New.Values(Decal.Adapter.Wrappers.LongValueKey.AssociatedSpell, 0);
                    fileService = (FileService)Core.FileService;
                    Spell spell = fileService.SpellTable.GetById(spellID);
                    SpellSchool spellSchool = spell.School;
                    bool creatureTrained = false;
                    bool lifeTrained = false;
                    bool itemTrained = false;
                    bool warTrained = false;
                    bool voidTrained = false;
                    if (Core.CharacterFilter.Skills[Decal.Adapter.Wrappers.CharFilterSkillType.CreatureEnchantment].Training >= TrainingType.Trained)
                        creatureTrained = true;
                    if (Core.CharacterFilter.Skills[Decal.Adapter.Wrappers.CharFilterSkillType.LifeMagic].Training >= TrainingType.Trained)
                        lifeTrained = true;
                    if (Core.CharacterFilter.Skills[Decal.Adapter.Wrappers.CharFilterSkillType.ItemEnchantment].Training >= TrainingType.Trained)
                        itemTrained = true;
                    if (Core.CharacterFilter.Skills[Decal.Adapter.Wrappers.CharFilterSkillType.WarMagic].Training >= TrainingType.Trained)
                        warTrained = true;
                    if (Core.CharacterFilter.Skills[Decal.Adapter.Wrappers.CharFilterSkillType.VoidMagic].Training >= TrainingType.Trained)
                        voidTrained = true;
                    if ((spellSchool.Id == Decal.Constants.SchoolID.Creature) && (creatureTrained) ||
                            (spellSchool.Id == Decal.Constants.SchoolID.Life) && (lifeTrained) ||
                            (spellSchool.Id == Decal.Constants.SchoolID.Item) && (itemTrained) ||
                            (spellSchool.Id == Decal.Constants.SchoolID.War) && (warTrained) ||
                            (spellSchool.Id == Decal.Constants.SchoolID.Void) && (voidTrained))
                    {
                        if (!Core.CharacterFilter.IsSpellKnown(spellID))
                        {
                            Host.Actions.AddChatText("== You do not know the spell: " + e.New.Name + "  == GRAB IT !!! ==", Decal.Constants.TextColors.AC_PURPLE);
                            soundPlayerCreate.Play(); // Play the sound for an unknown spell being found
                            Timer delay = new Timer { Interval = 1000 };
                            delay.Elapsed += (se, ea) =>
                            {
                                delay.Stop();
                                delay.Dispose();
                                //Host.Actions.SelectItem(e.New.Id); // Select the item
                                Host.Actions.AddChatText("== Picking it up now ==", Decal.Constants.TextColors.AC_PURPLE);
                                Host.Actions.MoveItem(e.New.Id, Core.CharacterFilter.Id); // Move the item to my inventory
                            };
                            delay.Start();
                        }
                    }
                    else
                    {
                        Host.Actions.AddChatText("== You do not have this school trained: " + e.New.Name + "  == SKIP IT !!! ==", Decal.Constants.TextColors.AC_PURPLE);
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogging.LogError(errorLogFile, ex);
                }
            }
            else
            {
                // Seek items with a name that matches the text in editSeek_*
                if (cbSeek_1.Checked)
                {
                    seekCheckName(e, 1);
                }
                if (cbSeek_2.Checked)
                {
                    seekCheckName(e, 2);
                }
                if (cbSeek_3.Checked)
                {
                    seekCheckName(e, 3);
                }
                if (cbSeek_4.Checked)
                {
                    seekCheckName(e, 4);
                }
                if (cbSeek_5.Checked)
                {
                    seekCheckName(e, 5);
                }
                if (cbSeek_6.Checked)
                {
                    seekCheckName(e, 6);
                }

                // seek items with a material that matches the text in editSeek_*
                if (cbMaterial_1.Checked)
                {
                    seekCheckMaterial(e, 1);
                }
                if (cbMaterial_2.Checked)
                {
                    seekCheckMaterial(e, 2);
                }
                if (cbMaterial_3.Checked)
                {
                    seekCheckMaterial(e, 3);
                }
                if (cbMaterial_4.Checked)
                {
                    seekCheckMaterial(e, 4);
                }
                if (cbMaterial_5.Checked)
                {
                    seekCheckMaterial(e, 5);
                }
                if (cbMaterial_6.Checked)
                {
                    seekCheckMaterial(e, 6);
                }
            }   // End of if item is an Item
        }


        void WorldFilter_DeclineTrade(object sender, DeclineTradeEventArgs e)
        {
            // DO STUFF HERE
        }

        void WorldFilter_EndTrade(object sender, EndTradeEventArgs e)
        {
            // DO STUFF HERE
        }

        void WorldFilter_EnterTrade(object sender, EnterTradeEventArgs e)
        {
            // DO STUFF HERE
        }

        void WorldFilter_FailToAddTradeItem(object sender, FailToAddTradeItemEventArgs e)
        {
            // DO STUFF HERE
        }

        void WorldFilter_FailToCompleteTrade(object sender, EventArgs e)
        {
            // DO STUFF HERE
        }

        void WorldFilter_MoveObject(object sender, MoveObjectEventArgs e)
        {
            // DO STUFF HERE
        }

        void WorldFilter_ReleaseDone(object sender, EventArgs e)
        {
            // DO STUFF HERE
        }

        void WorldFilter_ReleaseObject(object sender, ReleaseObjectEventArgs e)
        {
            if ((cbSeek_1.Checked) ||
                (cbSeek_2.Checked) ||
                (cbSeek_3.Checked) ||
                (cbSeek_4.Checked) ||
                (cbSeek_5.Checked) ||
                (cbSeek_6.Checked) ||
                (cbMaterial_1.Checked) ||
                (cbMaterial_2.Checked) ||
                (cbMaterial_3.Checked) ||
                (cbMaterial_4.Checked) ||
                (cbMaterial_5.Checked) ||
                (cbMaterial_6.Checked))
            {
                try
                {   // Log the release of the object
                    // Host.Actions.AddChatText("Released Object: " + e.Released.Name + " " + e.Released.Id, 6);

                    for (int i = 0; i < seekListbox_1.RowCount; i++)
                    {
                        if (e.Released.Id == seekList[i].Id) // Check if the released object matches the one in the seek list
                        {
                            seekListbox_1.Delete(i); // Remove the item from the seek list
                            seekList.RemoveAt(i);    // Remove the item from the seekList
                            Host.Actions.AddChatText(e.Released.Name + ": removed from seek list", 6);
                            soundPlayerDestroy.Play(); // Play the sound for an item being removed from the seek list
                            break; // Exit the loop after removing the item
                        }
                    }
                    // repeat the check one more time to ensure the item is not still in the seek list
                    for (int i = 0; i < seekListbox_1.RowCount; i++)
                    {
                        if (e.Released.Id == seekList[i].Id) // Check if the released object matches the one in the seek list
                        {
                            seekListbox_1.Delete(i); // Remove the item from the seek list
                            seekList.RemoveAt(i);    // Remove the item from the seekList
                            Host.Actions.AddChatText(e.Released.Name + ": removed from seek list", 6);
                            soundPlayerDestroy.Play(); // Play the sound for an item being removed from the seek list
                            break; // Exit the loop after removing the item
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogging.LogError(errorLogFile, ex);
                }
            }
            // If the item released is a corpse, remove it from the corpseLooted list
            if (e.Released.ObjectClass == ObjectClass.Corpse)
            {
                try
                {
                    // Check if the corpse is in the corpseLooted list
                    HashSet<int> corpsesLootedIds = autoLootStateMachine.GetcorpsesLootedIds();
                    HashSet<int> corpsesToLootIds = autoLootStateMachine.GetcorpsesToLootIds(); // Create a copy of the corpsesLooted list
                    if (corpsesLootedIds.Contains(e.Released.Id))
                    {
                        // If the corpse is in the list, remove it
                        CoreManager.Current.Actions.AddChatText($"Corpse: {e.Released.Name}  removed from loot list", 5);
                        corpsesLootedIds.Remove(e.Released.Id); // remove this corpse
                    }
                    if (corpsesToLootIds.Contains(e.Released.Id))
                    {
                        // If the corpse is in the list, remove it
                        CoreManager.Current.Actions.AddChatText($"Corpse: {e.Released.Name}  removed from loot list", 5);
                        corpsesToLootIds.Remove(e.Released.Id); // remove this corpse
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogging.LogError(errorLogFile, ex);
                }
            }
        }

        void WorldFilter_ResetTrade(object sender, ResetTradeEventArgs e)
        {
            // DO STUFF HERE
        }

        private void destroyWorldFilter()
        {
            // UnInitialize the ResetTrade event handler
            // Core.WorldFilter.ResetTrade -= new EventHandler<ResetTradeEventArgs>(WorldFilter_ResetTrade);

            // UnInitialize the ReleaseObject event handler
            Core.WorldFilter.ReleaseObject -= new EventHandler<ReleaseObjectEventArgs>(WorldFilter_ReleaseObject);

            // UnInitialize the ReleaseDone event handler
            // Core.WorldFilter.ReleaseDone -= new EventHandler(WorldFilter_ReleaseDone);

            // UnInitialize the MoveObject event handler
            // Core.WorldFilter.MoveObject -= new EventHandler<MoveObjectEventArgs>(WorldFilter_MoveObject);

            // UnInitialize the FailToCompleteTrade event handler
            // Core.WorldFilter.FailToCompleteTrade -= new EventHandler(WorldFilter_FailToCompleteTrade);

            // UnInitialize the FailToAddTradeItem event handler
            // Core.WorldFilter.FailToAddTradeItem -= new EventHandler<FailToAddTradeItemEventArgs>(WorldFilter_FailToAddTradeItem);

            // UnInitialize the EnterTrade event handler
            // Core.WorldFilter.EnterTrade -= new EventHandler<EnterTradeEventArgs>(WorldFilter_EnterTrade);

            // UnInitialize the EndTrade event handler
            // Core.WorldFilter.EndTrade -= new EventHandler<EndTradeEventArgs>(WorldFilter_EndTrade);

            // UnInitialize the DeclineTrade event handler
            // Core.WorldFilter.DeclineTrade -= new EventHandler<DeclineTradeEventArgs>(WorldFilter_DeclineTrade);

            // UnInitialize the CreateObject event handler
            // Core.WorldFilter.CreateObject -= new EventHandler<CreateObjectEventArgs>(WorldFilter_CreateObject);

            // UnInitialize the ChangeObject event handler
            // Core.WorldFilter.ChangeObject -= new EventHandler<ChangeObjectEventArgs>(WorldFilter_ChangeObject);

            // UnInitialize the ApproachVendor event handler
            // Core.WorldFilter.ApproachVendor -= new EventHandler<ApproachVendorEventArgs>(WorldFilter_ApproachVendor);

            // UnInitialize the AddTradeItem event handler
            // Core.WorldFilter.AddTradeItem -= new EventHandler<AddTradeItemEventArgs>(WorldFilter_AddTradeItem);

            // UnInitialize the AcceptTrade event handler
            // Core.WorldFilter.AcceptTrade -= new EventHandler<AcceptTradeEventArgs>(WorldFilter_AcceptTrade);
        }

        private void seekCheckName(CreateObjectEventArgs e, int cbNumber)
        {
            string seekText = string.Empty;
            switch (cbNumber)
            {
                case 1:
                    seekText = editSeek_1.Text.ToLower();
                    break;
                case 2:
                    seekText = editSeek_2.Text.ToLower();
                    break;
                case 3:
                    seekText = editSeek_3.Text.ToLower();
                    break;
                case 4:
                    seekText = editSeek_4.Text.ToLower();
                    break;
                case 5:
                    seekText = editSeek_5.Text.ToLower();
                    break;
                case 6:
                    seekText = editSeek_6.Text.ToLower();
                    break;
            }
            if (e.New.Name.ToLower().Contains(seekText))
            {
                // Need to check for the item being in the seekListbox_1 already
                if (seekList.Contains(e.New))
                {
                    // If the item is already in the seekList, do not add it again
                    Host.Actions.AddChatText(e.New.Name + ": already in seek list", 6);
                    return; // Exit the method to avoid adding duplicates
                }
                Host.Actions.AddChatText(e.New.Name + ": matches name", 6);
                // Create a new ListRow in the seekListbox_1
                ListRow lr = seekListbox_1.Add();
                seekListbox_1[seekListbox_1.RowCount - 1][0][0] = e.New.Name;
                seekList.Add(e.New); // Add the object Name to the seekList
                seekListbox_1[seekListbox_1.RowCount - 1][0].Color = Color.FromArgb(255, 0, 255, 0); // Set the color of the row to green
                soundPlayerCreate.Play(); // Play the sound for a matching item being found

                
            }
        }
        private void seekCheckMaterial(CreateObjectEventArgs e, int cbNumber)
        {
            string seekText = string.Empty;
            switch (cbNumber)
            {
                case 1:
                    seekText = editSeek_1.Text.ToLower();
                    break;
                case 2:
                    seekText = editSeek_2.Text.ToLower();
                    break;
                case 3:
                    seekText = editSeek_3.Text.ToLower();
                    break;
                case 4:
                    seekText = editSeek_4.Text.ToLower();
                    break;
                case 5:
                    seekText = editSeek_5.Text.ToLower();
                    break;
                case 6:
                    seekText = editSeek_6.Text.ToLower();
                    break;
            }
            if (e.New.Values(Decal.Adapter.Wrappers.LongValueKey.Material, 0) != 0)
            {
                string itemMaterialName;
                materials.TryGetValue(e.New.Values(Decal.Adapter.Wrappers.LongValueKey.Material, 0), out itemMaterialName);
                if (itemMaterialName.ToLower().Contains(seekText))
                {
                    // Need to check for the item being in the seekListbox_1 already
                    if (seekList.Contains(e.New))
                    {
                        // If the item is already in the seekList, do not add it again
                        Host.Actions.AddChatText(e.New.Name + ": already in seek list", 6);
                        return; // Exit the method to avoid adding duplicates
                    }
                    Host.Actions.AddChatText(e.New.Name + ": matches the material (" + seekText + ")", 6);
                    ListRow lr = seekListbox_1.Add();
                    seekListbox_1[seekListbox_1.RowCount - 1][0][0] = e.New.Name;
                    seekList.Add(e.New); // Add the object Name to the seekList
                    lr[0].Color = Color.FromArgb(255, 255, 0, 0); // Set the color of the row to red
                    soundPlayerCreate.Play(); // Play the sound for a matching item being found
                }
            }
        }
    }
}