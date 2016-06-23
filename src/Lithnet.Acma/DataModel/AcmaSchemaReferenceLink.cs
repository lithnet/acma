using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Logging;
using Lithnet.Acma;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Acma.DataModel
{
    public partial class AcmaSchemaReferenceLink
    {
        /// <summary>
        /// Creates and removes links on the referenced objects
        /// </summary>
        /// <param name="sourceObject">The object to apply the link rule to</param>
        /// <returns>A list of objects that were modified</returns>
        public IList<MAObjectHologram> ProcessLinks(MAObjectHologram sourceObject)
        {
            if (sourceObject.InternalAttributeChanges.Any(t => t.Name == this.ForwardLinkAttribute.Name))
            {
                return this.UpdateLinks(sourceObject);
            }

            return new List<MAObjectHologram>();
        }

        /// <summary>
        /// Updates the links to the referenced objects
        /// </summary>
        /// <param name="sourceObject">The object to apply the link rule to</param>
        /// <returns>A list of objects that were modified</returns>
        private IList<MAObjectHologram> UpdateLinks(MAObjectHologram sourceObject)
        {
            List<MAObjectHologram> updatedObjects = new List<MAObjectHologram>();
            List<MAObjectHologram> targetsToReference = new List<MAObjectHologram>();
            List<MAObjectHologram> targetsToDereference = new List<MAObjectHologram>();

            IEnumerable<MAObjectHologram> targetsProposed = sourceObject.GetReferencedObjects(this.ForwardLinkAttribute, System.Data.DataRowVersion.Proposed);
            IEnumerable<MAObjectHologram> targetsCurrent = sourceObject.GetReferencedObjects(this.ForwardLinkAttribute, System.Data.DataRowVersion.Current);

            targetsToReference.AddRange(targetsProposed.Except(targetsCurrent));
            targetsToDereference.AddRange(targetsCurrent.Except(targetsProposed));

            foreach (MAObjectHologram target in targetsToDereference)
            {
                Logger.WriteLine("Unlinking object {0} from {1} on object {2} referenced by {3}", LogLevel.Debug, sourceObject.DisplayText, this.BackLinkAttribute.Name, target.DisplayText, this.ForwardLinkAttribute.Name);
                target.SetObjectModificationType(ObjectModificationType.Update, false);
                this.RemoveReference(sourceObject, target);
                updatedObjects.Add(target);
            }

            foreach (MAObjectHologram target in targetsToReference)
            {
                Logger.WriteLine("Linking object {0} to {1} on object {2} referenced by {3}", LogLevel.Debug, sourceObject.DisplayText, this.BackLinkAttribute.Name, target.DisplayText, this.ForwardLinkAttribute.Name);
                target.SetObjectModificationType(ObjectModificationType.Update, false);
                this.AddReference(sourceObject, target);
                updatedObjects.Add(target);
            }

            return updatedObjects;
        }

        /// <summary>
        /// Removes a reference to the source object on the specified target object
        /// </summary>
        /// <param name="sourceObject">The object to remove the reference to</param>
        /// <param name="targetObject">The object to remove the reference from</param>
        private void RemoveReference(MAObjectHologram sourceObject, MAObjectHologram targetObject)
        {
            if (this.BackLinkAttribute.IsMultivalued)
            {
                targetObject.UpdateAttributeValue(this.BackLinkAttribute, new List<ValueChange>() { ValueChange.CreateValueDelete(sourceObject.ObjectID) });
            }
            else
            {
                targetObject.DeleteAttribute(this.BackLinkAttribute);
            }
        }

        /// <summary>
        /// Adds a reference to the source object on the specified target object
        /// </summary>
        /// <param name="sourceObject">The object to add a reference to</param>
        /// <param name="targetObject">The object to add the reference on</param>
        private void AddReference(MAObjectHologram sourceObject, MAObjectHologram targetObject)
        {
            if (this.BackLinkAttribute.IsMultivalued)
            {
                if (!targetObject.HasAttributeValue(this.BackLinkAttribute, sourceObject.ObjectID))
                {
                    targetObject.UpdateAttributeValue(this.BackLinkAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd(sourceObject.ObjectID) });
                }
            }
            else
            {
                targetObject.SetAttributeValue(this.BackLinkAttribute, sourceObject.ObjectID);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}->{2}:{3}", this.ForwardLinkObjectClass.Name, this.ForwardLinkAttribute.Name, this.BackLinkObjectClass.Name, this.BackLinkAttribute.Name);
        }
    }
}
