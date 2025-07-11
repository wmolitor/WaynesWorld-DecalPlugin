using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;

namespace WaynesWorld
{
    public partial class PluginCore
    {
        private int MessageColor = 5;
        private void initChatEvents()
        {
            // Initialize incoming chat message event handler
            // Core.ChatBoxMessage += new EventHandler<Decal.Adapter.ChatTextInterceptEventArgs>(Core_ChatBoxMessage);
            
            // Initialize the outgoing chat/command message event handler
            // Core.CommandLineText += new EventHandler<Decal.Adapter.ChatParserInterceptEventArgs>(Core_CommandLineText);
        }

        void Core_CommandLineText(object sender, Decal.Adapter.ChatParserInterceptEventArgs e)
        {
            //TODO: outgoing chat handling code or command handling
        }

        void Core_ChatBoxMessage(object sender, Decal.Adapter.ChatTextInterceptEventArgs e)
        {
            //TODO: incoming chat handling code
        }
        private void destroyChatEvents()
        {
            // Core.ChatBoxMessage -= new EventHandler<Decal.Adapter.ChatTextInterceptEventArgs>(Core_ChatBoxMessage);
            // Core.CommandLineText -= new EventHandler<Decal.Adapter.ChatParserInterceptEventArgs>(Core_CommandLineText);
        }

        private void WriteToChat(string message)
        {
            try
            {
                this.Host.Actions.AddChatText(message, MessageColor);
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }
    }
}