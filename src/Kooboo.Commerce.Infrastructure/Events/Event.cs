﻿using Kooboo.Commerce.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using Kooboo.CMS.Common.Runtime;
using System.Reflection;
using System.Linq;

namespace Kooboo.Commerce.Events
{
    public static class Event
    {
        public static void Raise<TEvent>(TEvent @event, EventContext context)
            where TEvent : IEvent
        {
            EventHost.Instance.Raise<TEvent>(@event, context);
        }

        public static void Listen(Type eventType, Type handlerType)
        {
            EventHost.Instance.Listen(eventType, handlerType);
        }

        public static void Listen<TEvent>(Type handlerType)
            where TEvent : IEvent
        {
            EventHost.Instance.Listen<TEvent>(handlerType);
        }

        public static void Listen<TEvent>(IHandle<TEvent> handler)
            where TEvent : IEvent
        {
            EventHost.Instance.Listen<TEvent>(handler);
        }

        public static void Listen(Type eventType, Action<IEvent, EventContext> handler)
        {
            EventHost.Instance.Listen(eventType, handler);
        }

        public static void Listen<TEvent>(Action<TEvent, EventContext> handler)
            where TEvent : IEvent
        {
            EventHost.Instance.Listen<TEvent>(handler);
        }
    }
}
