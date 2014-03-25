﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Kooboo.CMS.Common.Runtime.Dependency;
using Kooboo.Commerce.API.LocalProvider;
using System.Collections;

namespace DAF.Core.Map
{
    [Dependency(typeof(IMapper<,>), ComponentLifeStyle.Transient)]
    public class ReflectionMapProvider<T, U> : IMapper<T, U>
        where T : class, new()
        where U : class, new()
    {
        private object Map(object fobj, object tobj, string prefix, params string[] includeComplexPropertyNames)
        {
            if (fobj == null)
                return null;
            var fobjProps = fobj.GetType().GetProperties();
            var tobjProps = tobj.GetType().GetProperties();

            foreach (var top in tobjProps)
            {
                if (!top.CanWrite)
                    continue;
                var op = fobjProps.FirstOrDefault(o => o.Name == top.Name);
                if (op != null)
                {
                    if (op.PropertyType != top.PropertyType)
                    {
                        if (top.PropertyType.IsEnum && op.PropertyType.IsEnum)
                        {
                            object val = op.GetValue(fobj, null);
                            object nval = Enum.Parse(top.PropertyType, Enum.GetName(op.PropertyType, val));
                            top.SetValue(tobj, nval, null);
                        }
                        else if (includeComplexPropertyNames != null && includeComplexPropertyNames.Any(o => o == (string.IsNullOrEmpty(prefix) ? top.Name : prefix + "." + top.Name)))
                        {
                            // 1. if is array(or enumerable), then map the item in the collection
                            if (typeof(IEnumerable).IsAssignableFrom(op.PropertyType))
                            {
                                // TODO: need recursive map? this will cause repository attrive all properties from database and may cause recursive overflow.
                                // add mutual collection-array mapping.
                                IEnumerable evals = op.GetValue(fobj, null) as IEnumerable;
                                var topType = top.PropertyType.GetElementType();
                                Type listType = typeof(List<>).MakeGenericType(new[] { topType });
                                object nvals = Activator.CreateInstance(listType);
                                foreach (var val in evals)
                                {
                                    var nval = Activator.CreateInstance(topType);
                                    nval = Map(val, nval, string.IsNullOrEmpty(prefix) ? top.Name : prefix + "." + top.Name, includeComplexPropertyNames);
                                    listType.GetMethod("Add").Invoke(nvals, new object[] { nval });
                                }
                                if (top.PropertyType.IsArray)
                                {
                                    var gType = listType.GetMethod("ToArray");
                                    var array = gType.Invoke(nvals, null);
                                    top.SetValue(tobj, array, null);
                                }
                                else
                                {
                                    top.SetValue(tobj, nvals, null);
                                }
                            }
                            // 2. if is a complex type, then map it
                            else
                            {
                                object val = op.GetValue(fobj, null);
                                object nval = Activator.CreateInstance(top.PropertyType);
                                nval = Map(val, nval, string.IsNullOrEmpty(prefix) ? top.Name : prefix + "." + top.Name, includeComplexPropertyNames);
                                top.SetValue(tobj, nval, null);
                            }
                        }
                    }
                    else
                    {
                        // 3. else if is a value type, set the value
                        top.SetValue(tobj, op.GetValue(fobj, null), null);
                    }
                }
            }
            return tobj;
        }

        public T MapTo(U obj, params string[] includeComplexPropertyNames)
        {
            T nobj = new T();
            return Map(obj, nobj, null, includeComplexPropertyNames) as T;
        }

        public U MapFrom(T obj, params string[] includeComplexPropertyNames)
        {
            U nobj = new U();
            return Map(obj, nobj, null, includeComplexPropertyNames) as U;
        }
    }
}
