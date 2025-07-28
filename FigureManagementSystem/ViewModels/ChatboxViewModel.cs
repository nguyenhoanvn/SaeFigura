using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.Windows.Input;
using FigureManagementSystem.Models;
using FigureManagementSystem.Helpers;

namespace FigureManagementSystem.ViewModels
{
    public class ChatboxViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ChatMessage> ChatMessages { get; set; } = new();
        public ObservableCollection<DatabaseTable> Tables { get; set; } = new();
        public Action CloseWindowAction { get; set; }
        private string _userInput;
        public string UserInput
        {
            get => _userInput;
            set
            {
                if (_userInput != value)
                {
                    _userInput = value;
                    OnPropertyChanged(nameof(UserInput));
                }
            }
        }

        private string _databaseFile = "FigureManagementSystem";
        public string DatabaseFile
        {
            get => _databaseFile;
            set { if (_databaseFile != value) { _databaseFile = value; OnPropertyChanged(nameof(DatabaseFile)); } }
        }

        public ICommand SendMessageCommand { get; }
        public ICommand BackCommand { get; }

        public ChatboxViewModel()
        {
            BackCommand = new RelayCommand(_ => CloseWindowAction?.Invoke());
            SendMessageCommand = new RelayCommand(_ => SendMessage(), _ => !string.IsNullOrWhiteSpace(UserInput));
        }

        private async void SendMessage()
        {
            var userMsg = new ChatMessage { Role = "user", Content = UserInput, Timestamp = DateTime.Now };
            ChatMessages.Add(userMsg);
            string reply = await ApiService.SendChatAsync(ChatMessages, UserInput);
            ChatMessages.Add(new ChatMessage { Role = "assistant", Content = reply, Timestamp = DateTime.Now });
            UserInput = string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
