﻿using Kooboo.CMS.Common.Runtime;
using Kooboo.Commerce.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Kooboo.Commerce.Categories
{
    public class CategoryCache
    {
        public IList<CategoryCacheEntry> Categories { get; private set; }

        public CategoryCache()
        {
            Categories = new List<CategoryCacheEntry>();
        }

        public CategoryCacheEntry Find(int id)
        {
            foreach (var category in Categories)
            {
                if (category.Id == id)
                {
                    return category;
                }

                var result = category.Descendants().FirstOrDefault(c => c.Id == id);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public void ForEach(Action<CategoryCacheEntry> action)
        {
            foreach (var root in Categories)
            {
                action(root);

                foreach (var descendant in root.Descendants())
                {
                    action(descendant);
                }
            }
        }

        public CategoryCache Localize(CultureInfo culture)
        {
            var texts = Localizer.GetText(GetAllCategoryKeys(), "Name", culture);
            var clone = Clone();
            clone.ForEach(entry =>
            {
                var localizedName = texts[new EntityKey(typeof(Category), entry.Id)];
                if (!String.IsNullOrEmpty(localizedName))
                {
                    entry.Name = localizedName;
                }
            });

            return clone;
        }

        public CategoryCache Clone()
        {
            var clone = new CategoryCache();
            foreach (var root in Categories)
            {
                clone.Categories.Add(CloneEntry(root, null));
            }

            return clone;
        }

        private CategoryCacheEntry CloneEntry(CategoryCacheEntry entry, CategoryCacheEntry parent)
        {
            var clone = new CategoryCacheEntry
            {
                Id = entry.Id,
                Name = entry.Name,
                Parent = parent
            };

            foreach (var child in entry.Children)
            {
                clone.Children.Add(CloneEntry(child, entry));
            }

            return clone;
        }

        private List<EntityKey> GetAllCategoryKeys()
        {
            var keys = new List<EntityKey>();
            foreach (var category in Categories)
            {
                keys.Add(new EntityKey(typeof(Category), category.Id));

                foreach (var descendant in category.Descendants())
                {
                    keys.Add(new EntityKey(typeof(Category), descendant.Id));
                }
            }

            return keys;
        }

        static readonly ConcurrentDictionary<string, CategoryCache> _caches = new ConcurrentDictionary<string, CategoryCache>(StringComparer.OrdinalIgnoreCase);

        public static CategoryCache Get(string instance)
        {
            Require.NotNullOrEmpty(instance, "instance");

            return _caches.GetOrAdd(instance, instanceName =>
            {
                var manager = EngineContext.Current.Resolve<ICommerceInstanceManager>();
                return BuildFrom(manager.GetInstance(instanceName).Database.GetRepository<Category>().Query().ToList());
            });
        }

        public static void Remove(string instance)
        {
            Require.NotNullOrEmpty(instance, "instance");

            CategoryCache cache;
            _caches.TryRemove(instance, out cache);
        }

        public static CategoryCache BuildFrom(IEnumerable<Category> all)
        {
            var cache = new CategoryCache();
            var roots = all.Where(c => c.Parent == null);
            foreach (var root in roots)
            {
                cache.Categories.Add(BuildEntry(root, all));
            }

            return cache;
        }

        static CategoryCacheEntry BuildEntry(Category category, IEnumerable<Category> all)
        {
            var entry = new CategoryCacheEntry
            {
                Id = category.Id,
                Name = category.Name
            };

            foreach (var child in all.Where(c => c.Parent != null && c.Parent.Id == category.Id))
            {
                var childEntry = BuildEntry(child, all);
                entry.Children.Add(childEntry);
                childEntry.Parent = entry;
            }

            return entry;
        }
    }
}
