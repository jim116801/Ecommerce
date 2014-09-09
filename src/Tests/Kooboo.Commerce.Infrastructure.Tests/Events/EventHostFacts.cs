﻿using Kooboo.Commerce.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Kooboo.Commerce.Infrastructure.Tests.Events
{
    public class EventHostFacts
    {
        public class EventRegisteration
        {
            [Fact]
            public void can_register_action()
            {
                var host = new EventHost();
                var value = 0;

                host.Listen<MyEvent>(@event =>
                {
                    value += @event.Value;
                });

                Assert.Equal(0, value);

                host.Raise(new MyEvent { Value = 5 });
                Assert.Equal(5, value);

                host.Raise(new MyEvent { Value = 3 });
                Assert.Equal(8, value);
            }

            [Fact]
            public void can_register_handler_instance()
            {
                var host = new EventHost();
                var handler = new MyEventHandler();

                host.Listen<MyEvent>(handler);

                host.Raise(new MyEvent { Value = 6 });
                Assert.Equal(6, handler.Value);

                host.Raise(new MyEvent { Value = 7 });
                Assert.Equal(13, handler.Value);
            }

            [Fact]
            public void can_register_hanlder_type()
            {
                var host = new EventHost();

                host.Listen<MyEvent>(typeof(MyEventHandlerWithStaticValue));

                host.Raise(new MyEvent { Value = 16 });
                Assert.Equal(16, MyEventHandlerWithStaticValue.Value);

                host.Raise(new MyEvent { Value = 3 });
                Assert.Equal(19, MyEventHandlerWithStaticValue.Value);
            }

            public class MyEvent : Event
            {
                public int Value { get; set; }
            }

            public class MyEventHandler : IHandle<MyEvent>
            {
                public int Value { get; set; }

                public void Handle(MyEvent @event)
                {
                    Value += @event.Value;
                }
            }

            public class MyEventHandlerWithStaticValue : IHandle<MyEvent>
            {
                public static int Value = 0;

                public void Handle(MyEvent @event)
                {
                    Value += @event.Value;
                }
            }
        }

        public class EventInheritance
        {
            [Fact]
            public void can_listen_to_base_events()
            {
                var host = new EventHost();

                host.Listen<BaseEvent>(typeof(BaseEventHandler));

                var @event = new DerivedEvent();
                host.Raise(@event);

                Assert.Equal(1, @event.Value);
            }

            [Fact]
            public void can_listen_to_event_interfaces()
            {
                var host = new EventHost();

                host.Listen<IEvent>(typeof(AllEventHandler));

                var @event = new BaseEvent();

                host.Raise(@event);
                Assert.Equal(5, @event.Value);

                @event = new DerivedEvent();

                host.Raise(@event);
                Assert.Equal(5, @event.Value);
            }

            public class BaseEvent : Event
            {
                public int Value { get; set; }
            }

            public class DerivedEvent : BaseEvent
            {
            }

            public class BaseEventHandler : IHandle<BaseEvent>
            {
                public void Handle(BaseEvent @event)
                {
                    @event.Value++;
                }
            }

            public class DerivedEventHandler : IHandle<DerivedEvent>
            {
                public void Handle(DerivedEvent @event)
                {
                    @event.Value += 2;
                }
            }

            public class AllEventHandler : IHandle<IEvent>
            {
                public void Handle(IEvent @event)
                {
                    var evnt = @event as BaseEvent;
                    evnt.Value += 5;
                }
            }
        }
    }
}
