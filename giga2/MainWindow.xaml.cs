using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GigaChatAPI;
using GigaChatAPI.Models;

namespace giga2
{
    public partial class MainWindow : Window
    {
        private readonly GigaChat _gigaChat;

        public MainWindow()
        {
            InitializeComponent();
            _gigaChat = new GigaChat(Scope.GIGACHAT_API_PERS,"MDcyNTZjMzYtNzhlNC00NGNlLTljM2MtZDc4Yjk5ZDYyM2FjOjIzNGZkMjE2LWYxYjAtNGMwNS05YTc2LWZhZmUxOGY0Njc4NA=="); // Вставьте свои авторизационные данные
        }

        private async void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            string userInput = tbUserInput.Text.Trim();
            if (string.IsNullOrEmpty(userInput))
                return;

            var data = new ModelConfiguration()
            {
                MaxTokens = 1024,
                Messages = new List<Message>()

                {
                    new Message()
                    {
                        Role = Role.system,
                        Content = "Отвечай как научный сотрудник"
                    },
                    new Message()
                    {
                        Content = userInput,
                        Role = Role.user
                    }
                }
            };

            var result = await _gigaChat.SendMessage(data);
            var modelMessage = result.Choices.FirstOrDefault()?.Message;

            if (modelMessage != null)
            {
                tbAnswer.Text = modelMessage.Content;
            }
            else
            {
                tbAnswer.Text = "Не удалось получить ответ от чата.";
            }

            await ShowTokensCount(userInput);
        }

        private async Task ShowTokensCount(string message)
        {
            var tokenCountRequest = new TokenCountRequest()
            {
                Model = "GigaChat:latest",
                Input = new[] { message }
            };

            var tokenResponse = await _gigaChat.GetTokensCount(tokenCountRequest);
            var tokenCount = tokenResponse.FirstOrDefault()?.Tokens;

            if (tokenCount.HasValue)
            {
                MessageBox.Show($"Количество токенов: {tokenCount}");
            }
        }
    }
}
