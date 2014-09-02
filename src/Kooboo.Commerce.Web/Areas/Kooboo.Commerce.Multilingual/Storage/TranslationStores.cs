﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kooboo.Commerce.Multilingual.Storage
{
    public static class TranslationStores
    {
        static readonly Dictionary<string, ITranslationStore> _storesByInstance = new Dictionary<string, ITranslationStore>();

        public static ITranslationStore Get(string instance)
        {
            return _storesByInstance[instance];
        }

        public static void Register(string instance, ITranslationStore store)
        {
            Remove(instance);
            _storesByInstance.Add(instance, store);
        }

        public static void Remove(string instance)
        {
            if (_storesByInstance.ContainsKey(instance))
            {
                var store = _storesByInstance[instance];
                if (store is IDisposable)
                {
                    ((IDisposable)store).Dispose();
                }

                _storesByInstance.Remove(instance);
            }
        }
    }
}