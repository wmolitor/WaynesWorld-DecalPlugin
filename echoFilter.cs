// Author: Lone, Sawz
// Date: 05-18-2025
// Description: This file contains the code for the echo filter.
// This code is used to filter messages sent to and from the server.
// It is used to intercept messages and perform actions based on the message type.

using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Constants;
using System;

//
// See this project for examples of how to get values from echo filter messages
//  https://github.com/justindz/DrunkenBoxing/blob/master/PluginCore/Fellowship.cs


namespace WaynesWorld
{
    public partial class PluginCore
    {
        private void initEcho2Filter()
        {

            // Uncomment the following line to enable the echo filter
            Core.EchoFilter.ServerDispatch += new EventHandler<Decal.Adapter.NetworkMessageEventArgs>(EchoFilter_ServerDispatch);
            //CoreManager.Current.EchoFilter.ServerDispatch += new EventHandler<Decal.Adapter.NetworkMessageEventArgs>(FSM_EchoFilter_ServerDispatch);
            //Core.EchoFilter.ClientDispatch += new EventHandler<Decal.Adapter.NetworkMessageEventArgs>(EchoFilter_ClientDispatch);
        }
        private void destroyEcho2Filter()
        {
            Core.EchoFilter.ServerDispatch -= new EventHandler<Decal.Adapter.NetworkMessageEventArgs>(EchoFilter_ServerDispatch);
            //Core.EchoFilter.ClientDispatch -= new EventHandler<Decal.Adapter.NetworkMessageEventArgs>(EchoFilter_ClientDispatch);
        }

        private void EchoFilter_ServerDispatch(object sender, NetworkMessageEventArgs e)
        {
            // DO STUFF HERE
            try
            {

                //HEALING
                if (cbAutoHeal.Checked)
                {
                    if (((e.Message.Type == Decal.Constants.MessageTypes.GameEvent)&&(e.Message.Value<int>("event") == Decal.Constants.GameEvents.ReceiveMeleeDamage)) || 
                    ((e.Message.Type == Decal.Constants.MessageTypes.SetCharacterCurrentVital) && (e.Message.Value<uint>("CurVitalID") == Decal.Constants.CurVitalID.CurrentHealth)))
                    {
                        // Check current health and apply healing item if necessary
                        int healthCurrent = Core.CharacterFilter.Vitals[CharFilterVitalType.Health].Current;
                        int healthBuffed = Core.CharacterFilter.Vitals[CharFilterVitalType.Health].Buffed;
                        float healthRatio = (float) healthCurrent / healthBuffed;
                        if (healthRatio < (sbHealth.Position/100))
                        {
                            // Example: Notify the user if health is below X% and attempt to heal.
                            // Need to find a way to interupt the current action if healing is required, then continue action. (Or queue the heal)
                            WriteToChat("Health Current: " + healthCurrent + " | Health Buffed: " + healthBuffed + " | Ratio: " + healthRatio.ToString("P2"));
                            WriteToChat("WARNING: Your health is below " + sbHealth.Position + "%! ATTEMPTING TO HEAL!!");
                            Host.Actions.ApplyItem(Find_Heal_Kit(), Core.CharacterFilter.Id);
                        }
                    }
                }

                //
                ////////////////////////////////////////////////
                //

                /*
                if (e.Message.Type == Decal.Constants.MessageTypes.GameEvent)
                    ErrorLogging.log("Game Event 0xF7B0: 0x" + e.Message.Value<int>("event").ToString("X4"), editLogLevel.Text);
                else if (e.Message.Type == Decal.Constants.MessageTypes.GameAction)
                    ErrorLogging.log("Game Action 0xF7B1: 0x" + e.Message.Value<int>("action").ToString("X4"), editLogLevel.Text);
                else
                    ErrorLogging.log(e.Message.Type.ToString("X4"), editLogLevel.Text); // Log the message type
                */

                ////////////////////////////////////////////////
                //  Just opened a container and server set its contents
                if (loginComplete)
                {
                    if ((e.Message.Type == Decal.Constants.MessageTypes.GameEvent) && (e.Message.Value<int>("event") == Decal.Constants.GameEvents.SetPackContents))
                    {
                        ErrorLogging.log("[ECHO] 0xF7B1: 0x" + e.Message.Value<int>("action").ToString("X4") + " Set Pack Contenets (" + e.Message.Value<int>("itemCount") + ")", int.Parse(editLogLevel.Text));
                        autoLootStateMachine.openCorpseTimer.Stop();
                        autoLootStateMachine.SetState(LootState.ScanForItems);
                    }

                    ////////////////////////////////////////////////
                    //  Just added an item to the player's inventory
                    if ((e.Message.Type == Decal.Constants.MessageTypes.GameEvent) && (e.Message.Value<int>("event") == Decal.Constants.GameEvents.InsertInventoryItem))
                    {
                        int itemId = e.Message.Value<int>("item");
                        // get a list of all items in the player's inventory
                        foreach (WorldObject obj in CoreManager.Current.WorldFilter.GetByContainer(Core.CharacterFilter.Id))
                        {
                            // Is the item of interest in the player's inventory?
                            if (obj.Id == itemId)
                            {
                                soundPlayerCreate.Play(); // Play sound when an item is inserted into inventory
                                ErrorLogging.log("[ECHO]: 0xF7B0: event 0x0022: Insert Inventory Item: " + obj.Name, int.Parse(editLogLevel.Text));
                                autoLootStateMachine.SetState(LootState.PickupItems);
                                autoLootStateMachine.grabItemTimer.Stop(); // Stop the grab item timer
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }

        /// 21:52:55 You took 6 damage from a melee attack by Olthoi Slayer
        /// 21:52:55 Health Current: 242 | Health Buffed: 248 | Health Base: 225 | Ratio: 0.00%
        /// 21:52:55 WARNING: Your health is below 45%! ATTEMPTING TO HEAL!!


        private void EchoFilter_ClientDispatch(object sender, NetworkMessageEventArgs e)
        {
            // DO STUFF HERE
        }


    }
}
