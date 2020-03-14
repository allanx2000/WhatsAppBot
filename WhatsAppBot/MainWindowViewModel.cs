using Innouvous.Utils;
using Innouvous.Utils.Merged45.MVVM45;
using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WhatsAppBot.Client;

namespace WhatsAppBot
{
    public class MainWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private readonly WhatsAppClient waClient;
        private readonly AZLyricsClient azClient;
        private readonly Window window;
        private Timer timer;

        public MainWindowViewModel(Window window, WhatsAppClient waClient, AZLyricsClient azClient)
        {
            this.waClient = waClient;
            this.azClient = azClient;

            this.window = window;
        }

        public bool Ready
        {
            get
            {
                return Get<bool>();
            }
            private set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public void StartUp()
        {
            waClient.OpenLogin();
            Status = "Login...";
        }

        public ICommand SendSongCommand
        {
            get
            {
                return new CommandHelper(SendSong);
            }
        }

        private void SendSong()
        {
            try
            {
                waClient.SendMessage(azClient.GetRandom());
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e, owner: window);
            }
        }

        public ICommand HookupUICommand
        {
            get
            {
                return new CommandHelper(HookupUI);
            }
        }

        public ICommand SendRandomMessageCommand
        {
            get
            {
                return new CommandHelper(SendRandomMessage);
            }
        }

        private void SendRandomMessage()
        {
            try
            {
                waClient.SendRandomMessage();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand ToggleTimerCommand
        {
            get
            {
                return new CommandHelper(ToggleTimer);
            }
        }

        private void ToggleTimer()
        {
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer = null;
                Status = "Stopped";
            }
            else
            {
                timer = new Timer(OnTick, null, 0, Timeout.Infinite);
                Status = "Started";
            }


        }

        Random rand = new Random();
        private void OnTick(object state)
        {
            waClient.SendMessage(azClient.GetRandom());
            timer.Change(rand.Next(7000, 30000), Timeout.Infinite);
        }

        public string Status
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        private void HookupUI()
        {
            try
            {
                waClient.HookupUI();
                Status = "Hooked Up";
                Ready = true;
            }
            catch (Exception e)
            {
                Status = "Error";
                MessageBoxFactory.ShowError(e, owner: window);
            }
        }

        internal void Close()
        {
            waClient.Close();
            azClient.Close();
        }
    }
}
