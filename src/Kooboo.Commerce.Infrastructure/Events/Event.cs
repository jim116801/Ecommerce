﻿using Kooboo.Commerce.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using Kooboo.CMS.Common.Runtime;
using System.Reflection;
using System.Linq;

namespace Kooboo.Commerce.Events
{
    [Serializable]
    public abstract class Event : IEvent
    {
        public DateTime TimestampUtc { get; set; }

        protected Event()
        {
            TimestampUtc = DateTime.UtcNow;
        }

        public static void Raise<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            EventHost.Instance.Raise<TEvent>(@event);
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

        public static void Listen<TEvent>(Action<TEvent> handler)
            where TEvent : IEvent
        {
            EventHost.Instance.Listen<TEvent>(handler);
        }
    }
}
