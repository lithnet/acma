using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections;

namespace Lithnet.Common.ObjectModel
{
    public static class Extensions
    {
        public static IEnumerable<Type> GetDerivedTypes(this Type baseType, Assembly assembly)
        {
            return assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
        }

        public static string GetTypeDescription(this Type type)
        {
            DescriptionAttribute displayName = type.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault() as DescriptionAttribute;

            if (displayName == null)
            {
                return type.Name;
            }
            else
            {
                return displayName.Description;
            }
        }

        public static string GetEnumDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attribute = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;

            return attribute != null ? attribute.Description : value.ToString();
        }

        public static void MoveUp<T>(this IList list, T item)
        {
            int itemIndex = list.IndexOf(item);

            if (itemIndex == -1)
            {
                throw new ArgumentOutOfRangeException("item");
            }

            if (itemIndex == 0)
            {
                return;
            }

            if (list is ObservableCollection<T>)
            {
                ((ObservableCollection<T>)list).Move(itemIndex, itemIndex - 1);
            }
            else
            {
                list.RemoveAt(itemIndex);
                list.Insert(itemIndex - 1, item);
            }
        }

        public static void MoveDown<T>(this IList list, T item)
        {
            int itemIndex = list.IndexOf(item);

            if (itemIndex == -1)
            {
                throw new ArgumentOutOfRangeException("item");
            }

            if (itemIndex == list.Count - 1)
            {
                return;
            }

            if (list is ObservableCollection<T>)
            {
                ((ObservableCollection<T>)list).Move(itemIndex, itemIndex + 1);
            }
            else
            {
                list.RemoveAt(itemIndex);
                list.Insert(itemIndex > list.Count ? list.Count : itemIndex + 1, item);
            }
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            foreach (T item in range)
            {
                collection.Add(item);
            }
        }
    }
}
