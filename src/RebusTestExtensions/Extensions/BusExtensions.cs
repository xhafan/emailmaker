using System;
using System.Collections.Generic;
using FakeItEasy;
using Rebus.Bus;

namespace RebusTestExtensions.Extensions
{
    public static class BusExtensions
    {
        public static void ExpectMessageSent<TMessage>(
            this IBus bus, 
            Action<TMessage> onSend
            ) 
            where TMessage : class, new()
        {
            A.CallTo(() => bus.Send(A<object>._, A<Dictionary<string, string>>._))
                .Invokes((object messages, Dictionary<string, string> optionalHeaders) =>
                {
                    onSend(messages as TMessage);
                });
        }

        public static void ExpectMessageSentLocal<TMessage>(
            this IBus bus, 
            Action<TMessage> onSend
        )
            where TMessage : class, new()
        {
            A.CallTo(() => bus.SendLocal(A<object>._, A<Dictionary<string, string>>._))
                .Invokes((object messages, Dictionary<string, string> optionalHeaders) =>
                {
                    onSend(messages as TMessage);
                });
        }

        public static void ExpectMessagePublished<TMessage>(
            this IBus bus,
            Action<TMessage> onPublish
        )
            where TMessage : class, new()
        {
            A.CallTo(() => bus.Publish(A<object>._, A<Dictionary<string, string>>._))
                .Invokes((object messages, Dictionary<string, string> optionalHeaders) =>
                {
                    onPublish(messages as TMessage);
                });
        }
    }
}
