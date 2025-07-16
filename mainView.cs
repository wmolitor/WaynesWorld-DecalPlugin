using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;
using static WaynesWorld.PluginCore;

namespace WaynesWorld
{
    [View("WaynesWorld.ViewXML.mainView.xml")]
    [WireUpControlEvents]
    public partial class PluginCore
    {
        // This is the main view for the plugin. It contains all the controls and events for the "main" tab.
        [ControlReference("TestEdit")]
        private TextBoxWrapper TestEdit;

        [ControlReference("instructions")]
        private StaticWrapper instructions;

        [ControlReference("cbAutoHeal")]
        private CheckBoxWrapper cbAutoHeal;

        [ControlReference("editLogLevel")]
        private TextBoxWrapper editLogLevel;

        [ControlReference("sbHealth")]
        private SliderWrapper sbHealth;

        [ControlReference("editOpenTimer")]
        private TextBoxWrapper editOpenTimer;

        [ControlReference("editLootTimer")]
        private TextBoxWrapper editLootTimer;

        [ControlReference("AutoLoot")]
        private StaticWrapper AutoLoot;

        [ControlReference("pbAutoLoot")]
        private PushButtonWrapper pbAutoLoot;

        [ControlEvent("pbAutoLoot", "Click")]
        private void onClickAutoLoot(object sender, ControlEventArgs args)
        {
            try
            {
                WriteToChat("pbAutoLoot Hit");
                // Toggle auto loot functionality
                if (AutoLootEnabled)
                {
                    AutoLootEnabled = false;
                    autoLootStateMachine.Stop();
                    pbAutoLoot.Text = "Start Auto Loot";
                }
                else
                {
                    AutoLootEnabled = true;
                    autoLootStateMachine.Start();
                    pbAutoLoot.Text = "Stop Auto Loot";
                }
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }
        public bool AutoLootEnabled
        {
            get { return autoLootEnabled; }
            set
            {
                //WriteToChat("Setting autoLootEnabled: " + value);
                autoLootEnabled = value;
                if (autoLootEnabled)
                {
                    //lootTimer.Start();
                    autoLootStateMachine.Start();
                    //WriteToChat("Auto Loot Timer Started");
                }
                else
                {
                    //lootTimer.Stop();
                    autoLootStateMachine.Stop();
                    //WriteToChat("Auto Loot Timer Stopped");
                }
            }
        }



        // These are controls and events on the "About" tab.



        // These are controls and events on the "Seek" tab.
        [ControlReference("editSeek_1")]
        private TextBoxWrapper editSeek_1;

        [ControlReference("editSeek_2")]
        private TextBoxWrapper editSeek_2;

        [ControlReference("editSeek_3")]
        private TextBoxWrapper editSeek_3;

        [ControlReference("editSeek_4")]
        private TextBoxWrapper editSeek_4;

        [ControlReference("editSeek_5")]
        private TextBoxWrapper editSeek_5;

        [ControlReference("editSeek_6")]
        private TextBoxWrapper editSeek_6;

        [ControlReference("cbSeek_1")]
        private CheckBoxWrapper cbSeek_1;

        [ControlReference("cbSeek_2")]
        private CheckBoxWrapper cbSeek_2;

        [ControlReference("cbSeek_3")]
        private CheckBoxWrapper cbSeek_3;

        [ControlReference("cbSeek_4")]
        private CheckBoxWrapper cbSeek_4;

        [ControlReference("cbSeek_5")]
        private CheckBoxWrapper cbSeek_5;

        [ControlReference("cbSeek_6")]
        private CheckBoxWrapper cbSeek_6;

        [ControlReference("cbMaterial_1")]
        private CheckBoxWrapper cbMaterial_1;

        [ControlReference("cbMaterial_2")]
        private CheckBoxWrapper cbMaterial_2;

        [ControlReference("cbMaterial_3")]
        private CheckBoxWrapper cbMaterial_3;

        [ControlReference("cbMaterial_4")]
        private CheckBoxWrapper cbMaterial_4;

        [ControlReference("cbMaterial_5")]
        private CheckBoxWrapper cbMaterial_5;

        [ControlReference("cbMaterial_6")]
        private CheckBoxWrapper cbMaterial_6;

        [ControlReference("labelSeek_1")]
        private StaticWrapper labelSeek_1;

        [ControlReference("seekListbox_1")]
        private ListWrapper seekListbox_1;

        [ControlEvent("seekListbox_1", "Selected")]
        private void onSelectSeekListbox(object sender, ListSelectEventArgs args)
        {
            try
            {
                // Handle the select event for the seek listbox.
                WriteToChat("SeekListbox_1 was selected");
                if (args.Row >= 0 && args.Row < seekList.Count)
                {
                    WorldObject selectedItem = seekList[args.Row];
                    CoreManager.Current.Actions.SelectItem(selectedItem.Id);
                    WriteToChat("Selected Item: " + selectedItem);
                }
                else
                {
                    WriteToChat("No item selected or invalid selection.");
                }
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }


        [ControlReference("pbClearList")]
        private PushButtonWrapper pbClearList;

        [ControlEvent("pbClearList", "Click")]
        private void onClickClearList(object sender, ControlEventArgs args)
        {
            try
            {
                // Clear the seek listbox.
                this.seekListbox_1.Clear();
                seekList.Clear();
                WriteToChat("Seek Listbox has been cleared.");
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }


        [ControlEvent("editSeek_1", "Begin")]
        private void onChangeEditSeek_1_Begin(object sender, ControlEventArgs args)
        {
            try
            {
                WriteToChat("ControlID = " + args.Id + "    Event: Beg    Text in box is: " + this.editSeek_1.Text);
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }
        [ControlEvent("editSeek_1", "Change")]
        private void onChangeEditSeek_1_Change(object sender, ControlEventArgs args)
        {
            try
            {
                WriteToChat("ControlID = " + args.Id + "    Event: Chg    Text in box is: " + this.editSeek_1.Text);
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }
        [ControlEvent("editSeek_1", "Destroy")]
        private void onChangeEditSeek_1_Destroy(object sender, ControlEventArgs args)
        {
            try
            {
                WriteToChat("ControlID = " + args.Id + "    Event: Des    Text in box is: " + this.editSeek_1.Text);
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }
        [ControlEvent("editSeek_1", "End")]
        private void onChangeEditSeek_1_End(object sender, ControlEventArgs args)
        {
            try
            {
                WriteToChat("Control ID = " + args.Id + "    Event: End    Text in box is: " + this.editSeek_1.Text);
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }


        [ControlEvent("seekListbox_1", "Click")]
        private void onClickSeekListbox(object sender, ControlEventArgs args)
        {
            try
            {
                // Handle the click event for the seek listbox.
                // This is where you would add your logic to handle the click event.
                WriteToChat("SeekListbox_1 was clicked");
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }
    }
}

