// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Services;
using System.Diagnostics;
using System.Windows.Interop;

namespace MKW.GUI.SingleInstance
{
    public class SingleInstanceApplicationManager
    {
        private readonly ISingleInstanceApplication application;
        private readonly MessageService messageService;

        public SingleInstanceApplicationManager(ISingleInstanceApplication application)
        {
            this.application = application;

            messageService = new MessageService(SingleInstanceConstants.ApplicationMagic);
            messageService.MessageReceived += MessageReceived;
        }

        public bool Run(RunRequest request)
        {
            byte[] encoded = RunRequestSerializer.Serialize(request);

            IReadOnlyCollection<IOtherAppWindow> windows = messageService.GetOtherAppWindows();
            foreach (IOtherAppWindow window in windows)
            {
                window.SendDataMessage(SingleInstanceConstants.DataMessageId.RunRequest, encoded);

                // messages broadcasted successfully -> no new host required
                return false;
            }

            return true;
        }

        public void AddMessageSource(HwndSource source)
        {
            messageService.AddMessageSource(source);
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                if (e.MessageId == SingleInstanceConstants.DataMessageId.RunRequest)
                {
                    RunRequest request = RunRequestSerializer.Deserialize(e.Data);

                    application.InvokeExternalInstance(request);
                }
                else
                {
                    Debug.WriteLine($"Unknown Data Message ({e.MessageId}).");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception while handling Data Message: {ex.Message}");
            }
        }
    }
}
