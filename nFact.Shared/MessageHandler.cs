using System;
using System.Collections.Concurrent;
using System.Text;

namespace nFact.Shared
{
    public enum MessageState {OK, Error};

    public class MessageHandler
    {
        private ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();
        public MessageState State { get; private set; }
        public volatile bool HasMessages;

        private static MessageHandler _instance;
        public static MessageHandler Instance
        {
            get { return _instance ?? (_instance = new MessageHandler()); }
        }

        public void Reset()
        {
            HasMessages = false;
            State = MessageState.OK;
            _messages = new ConcurrentQueue<string>();
        }

        public void AddMessage(string messageItem, MessageState state)
        {
            if (State != MessageState.Error)
                State = state;

            _messages.Enqueue(messageItem);
            HasMessages = true;
        }

        public string GetMessages()
        {
            var sb = new StringBuilder();
            var result = String.Empty;

            while (!_messages.IsEmpty)
            {
                if (_messages.TryDequeue(out result))
                {
                    var messageItem = result;
                    if (!string.IsNullOrEmpty(messageItem.Trim()))
                        sb.Append(messageItem);
                }
            }

            HasMessages = false;
            return sb.ToString();
        }
    }
}
