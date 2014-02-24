﻿using Kooboo.CMS.Common.Runtime;
using Kooboo.CMS.Common.Runtime.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Commerce.Activities
{
    public interface IActivityFactory
    {
        IActivity FindByName(string activityName);

        IEnumerable<IActivity> FindBindableActivities(Type eventType);
    }

    [Dependency(typeof(IActivityFactory))]
    public class ActivityFactory : IActivityFactory
    {
        private IEngine _engine;
        private List<IActivity> _activities;

        private List<IActivity> Activities
        {
            get
            {
                if (_activities == null)
                {
                    _activities = _engine.ResolveAll<IActivity>().ToList();
                }
                return _activities;
            }
        }

        public ActivityFactory()
            : this(EngineContext.Current) { }

        public ActivityFactory(IEngine engine)
        {
            Require.NotNull(engine, "engine");
            _engine = engine;
        }

        public IActivity FindByName(string activityName)
        {
            Require.NotNullOrEmpty(activityName, "activityName");
            return Activities.FirstOrDefault(x => x.Name.Equals(activityName, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<IActivity> FindBindableActivities(Type eventType)
        {
            return Activities.Where(it => it.CanBindTo(eventType)).ToList();
        }
    }
}
