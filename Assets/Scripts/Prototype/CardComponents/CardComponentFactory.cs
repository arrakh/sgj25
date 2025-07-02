using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Prototype.CardComponents
{
    public static class CardComponentFactory
    {
        private static readonly Dictionary<string, Type> _cache = new();

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    // Some assemblies may fail to load types
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        return ex.Types.Where(t => t != null)!;
                    }
                })
                .Where(type => typeof(CardComponent)
                    .IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .ToList();

            foreach (var type in types)
                try
                {
                    var instance = (CardComponent) Activator.CreateInstance(type);
                    foreach (var alias in instance.Aliases)
                        if (!_cache.ContainsKey(alias))
                            _cache[alias] = type;
                        else
                            Debug.LogWarning(
                                $"Duplicate alias '{alias}' found in '{type.FullName}' and '{_cache[alias].FullName}'");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to instantiate type {type.FullName}: {e}");
                }
        }

        public static CardComponent Create(CardInstance cardInstance, string component)
        {
            if (_cache == null)
            {
                Debug.LogError("CardComponentFactory not initialized.");
                return null;
            }

            var split = component.Split(' ');
            var args = split.Skip(1).ToArray();

            if (_cache.TryGetValue(split[0], out var type))
                try
                {
                    var instance = (CardComponent) Activator.CreateInstance(type);
                    instance.Initialize(cardInstance, args);
                    return instance;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to create instance of {type.FullName}: {e}");
                }
            else
                Debug.LogWarning($"No ICardComponent found for alias '{split[0]}'");

            return null;
        }
    }
}