using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenAI;
using OpenAI.Chat;

namespace SourceGit.Models
{
    public partial class OpenAIResponse
    {
        public OpenAIResponse(Action<string> onUpdate)
        {
            _onUpdate = onUpdate;
        }

        public void Append(string text)
        {
            var buffer = text;

            if (_thinkTail.Length > 0)
            {
                _thinkTail.Append(buffer);
                buffer = _thinkTail.ToString();
                _thinkTail.Clear();
            }

            buffer = REG_COT().Replace(buffer, "");

            var startIdx = buffer.IndexOf('<');
            if (startIdx >= 0)
            {
                if (startIdx > 0)
                    OnReceive(buffer.Substring(0, startIdx));

                var endIdx = buffer.IndexOf('>', startIdx + 1);
                if (endIdx <= startIdx)
                {
                    if (buffer.Length - startIdx <= 15)
                        _thinkTail.Append(buffer.AsSpan(startIdx));
                    else
                        OnReceive(buffer.Substring(startIdx));
                }
                else if (endIdx < startIdx + 15)
                {
                    var tag = buffer.Substring(startIdx + 1, endIdx - startIdx - 1);
                    if (_thinkTags.Contains(tag))
                        _thinkTail.Append(buffer.AsSpan(startIdx));
                    else
                        OnReceive(buffer.Substring(startIdx));
                }
                else
                {
                    OnReceive(buffer.Substring(startIdx));
                }
            }
            else
            {
                OnReceive(buffer);
            }
        }

        public void End()
        {
            if (_thinkTail.Length > 0)
            {
                OnReceive(_thinkTail.ToString());
                _thinkTail.Clear();
            }
        }

        private void OnReceive(string text)
        {
            if (!_hasTrimmedStart)
            {
                text = text.TrimStart();
                if (string.IsNullOrEmpty(text))
                    return;

                _hasTrimmedStart = true;
            }

            _onUpdate?.Invoke(text);
        }

        [GeneratedRegex(@"<(think|thought|thinking|thought_chain)>.*?</\1>", RegexOptions.Singleline)]
        private static partial Regex REG_COT();

        private Action<string> _onUpdate = null;
        private StringBuilder _thinkTail = new StringBuilder();
        private HashSet<string> _thinkTags = ["think", "thought", "thinking", "thought_chain"];
        private bool _hasTrimmedStart = false;
    }

    public class OpenAIService : ObservableObject, IAIService
    {
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Server
        {
            get => _server;
            set => SetProperty(ref _server, value);
        }

        public string ApiKey
        {
            get => _apiKey;
            set => SetProperty(ref _apiKey, value);
        }

        public bool ReadApiKeyFromEnv
        {
            get => _readApiKeyFromEnv;
            set => SetProperty(ref _readApiKeyFromEnv, value);
        }

        public string Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        public bool Streaming
        {
            get => _streaming;
            set => SetProperty(ref _streaming, value);
        }

        public string AnalyzeDiffPrompt
        {
            get => _analyzeDiffPrompt;
            set => SetProperty(ref _analyzeDiffPrompt, value);
        }

        public string GenerateSubjectPrompt
        {
            get => _generateSubjectPrompt;
            set => SetProperty(ref _generateSubjectPrompt, value);
        }

        public OpenAIService()
        {
            AnalyzeDiffPrompt = IAIService.DefaultAnalyzeDiffPrompt;
            GenerateSubjectPrompt = IAIService.DefaultGenerateSubjectPrompt;
        }

        public async Task ChatAsync(string prompt, string question, CancellationToken cancellation, Action<string> onUpdate)
        {
            var key = _readApiKeyFromEnv ? Environment.GetEnvironmentVariable(_apiKey) : _apiKey;
            var endPoint = new Uri(_server);
            var credential = new ApiKeyCredential(key);
            var client = _server.Contains("openai.azure.com/", StringComparison.Ordinal)
                ? new AzureOpenAIClient(endPoint, credential)
                : new OpenAIClient(credential, new() { Endpoint = endPoint });

            var chatClient = client.GetChatClient(_model);
            var messages = new List<ChatMessage>()
            {
                _model.Equals("o1-mini", StringComparison.Ordinal) ? new UserChatMessage(prompt) : new SystemChatMessage(prompt),
                new UserChatMessage(question),
            };

            try
            {
                var rsp = new OpenAIResponse(onUpdate);

                if (_streaming)
                {
                    var updates = chatClient.CompleteChatStreamingAsync(messages, null, cancellation);

                    await foreach (var update in updates)
                    {
                        if (update.ContentUpdate.Count > 0)
                            rsp.Append(update.ContentUpdate[0].Text);
                    }
                }
                else
                {
                    var completion = await chatClient.CompleteChatAsync(messages, null, cancellation);

                    if (completion.Value.Content.Count > 0)
                        rsp.Append(completion.Value.Content[0].Text);
                }

                rsp.End();
            }
            catch
            {
                if (!cancellation.IsCancellationRequested)
                    throw;
            }
        }

        private string _name;
        private string _server;
        private string _apiKey;
        private bool _readApiKeyFromEnv = false;
        private string _model;
        private bool _streaming = true;
        private string _analyzeDiffPrompt;
        private string _generateSubjectPrompt;
    }
}
