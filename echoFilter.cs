// Author: Lone, Sawz
// Date: 05-18-2025
// Description: This file contains the code for the echo filter.
// This code is used to filter messages sent to and from the server.
// It is used to intercept messages and perform actions based on the message type.

using Decal.Adapter;
using Decal.Adapter.Wrappers;
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
                if (((e.Message.Type == Decal.Constants.MessageTypes.GameEvent)&&(e.Message.Value<int>("event") == Decal.Constants.GameEvents.ReceiveMeleeDamage)) || 
                    ((e.Message.Type == Decal.Constants.MessageTypes.SetCharacterCurrentVital) && (e.Message.Value<uint>("CurVitalID") == Decal.Constants.CurVitalID.CurrentHealth)))
                {

                    if (cbAutoHeal.Checked)
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
