using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.Common.ObjectModel
{
    public static class UniqueIDCache
    {
        private static Dictionary<string, HashSet<IIdentifiedObject>> objectCache = new Dictionary<string, HashSet<IIdentifiedObject>>();

        public static void AddItem(IIdentifiedObject item, string group)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            HashSet<IIdentifiedObject> itemGroup = UniqueIDCache.GetGroupCollection(group);

            if (itemGroup.Any(t => t.ID == item.ID))
            {
                throw new ArgumentException(string.Format("The specified id '{0}' is already in use in group {1}", item.ID, group));
            }
            else
            {
                itemGroup.Add(item);
            }
        }

        public static void RemoveItem(IIdentifiedObject item, string group)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            HashSet<IIdentifiedObject> itemGroup = UniqueIDCache.GetGroupCollection(group);

            if (itemGroup.Contains(item))
            {
                itemGroup.Remove(item);
            }
        }

        public static string GetNextId(string id, IIdentifiedObject obj, string group)
        {
            string suggestedId = id;
            int count = 1;

            while (UniqueIDCache.IsIdInUse(suggestedId, obj, group))
            {
                suggestedId = id + "-" + count.ToString();
                count++;
            }

            return suggestedId;
        }

        public static bool IsIdInUse(string id, IIdentifiedObject obj, string group)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            HashSet<IIdentifiedObject> itemGroup = UniqueIDCache.GetGroupCollection(group);
            return itemGroup.Any(t => t.ID == id && t != obj);
        }

        public static bool CanChangeId(string group, IIdentifiedObject item, string newId)
        {
            HashSet<IIdentifiedObject> itemGroup = UniqueIDCache.GetGroupCollection(group);

            return !itemGroup.Any(t => t.ID == newId && t != item);
        }

        public static string SetID(IIdentifiedObject obj, string proposedValue, string groupName, bool canRename)
        {
            string newID = proposedValue;
            string oldID = obj.ID;

            if (proposedValue != null)
            {
                if (obj.ID == null)
                {
                    if (UniqueIDCache.IsIdInUse(proposedValue, obj, groupName))
                    {
                        if (canRename)
                        {
                            newID = UniqueIDCache.GetNextId(proposedValue, obj, groupName);
                        }
                        else
                        {
                            throw new DuplicateIdentifierException();
                        }
                    }
                    else
                    {
                        newID = proposedValue;
                    }
                }
                else if (UniqueIDCache.CanChangeId(groupName, obj, proposedValue))
                {
                    newID = proposedValue;
                }
                else
                {
                    throw new DuplicateIdentifierException();
                }
            }

            return newID;
        }

        public static bool HasObject(string group, IIdentifiedObject item)
        {
            return UniqueIDCache.GetGroupCollection(group).Contains(item);
        }

        public static void ClearIdCache(string group)
        {
            UniqueIDCache.GetGroupCollection(group).Clear();
        }
        public static void ClearIdCache()
        {
            UniqueIDCache.objectCache.Clear();
        }

        private static HashSet<IIdentifiedObject> GetGroupCollection(string group)
        {
            group = group.ToLowerInvariant();

            if (!UniqueIDCache.objectCache.ContainsKey(group))
            {
                UniqueIDCache.objectCache.Add(group, new HashSet<IIdentifiedObject>());
            }

            return UniqueIDCache.objectCache[group];
        }
    }
}
