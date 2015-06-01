using System;
using System.Collections.Generic;
using System.Linq;
using Lithnet.Common.Presentation;
using Lithnet.Fim.Core;
using Microsoft.MetadirectoryServices;
using System.Reflection;
using System.Windows;
using System.Text.RegularExpressions;
using Lithnet.Fim.Transforms;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class NewTransformViewModel : ViewModelBase<Transform>
    {
        private TransformCollectionViewModel transformCollectionViewModel;

        public NewTransformViewModel(TransformCollectionViewModel transforms, Window window)
            : base()
        {
            this.Commands.AddItem("Create", t => this.Create(window), t => this.CanCreate());
            this.transformCollectionViewModel = transforms;
            this.TransformType = TypeMarkupExtension.GetSubclassDescriptors
                (typeof(Transform)).FirstOrDefault(t => !t.Value.GetCustomAttributes(true).Any
                        (
                            u => u.GetType() == typeof(LoopbackTransformAttribute)
                        )
                    ).Value;
        }

        private bool CanCreate()
        {
            if (this.TransformType == null)
            {
                return false;
            }

            this.ValidatePropertyChange("Id");
            return !this.HasErrors;
        }

        private void Create(Window window)
        {
            Transform newTransform = (Transform)Activator.CreateInstance(this.TransformType);
            newTransform.ID = this.Id;
            this.transformCollectionViewModel.Add(newTransform, true);

            window.Close();
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            switch (propertyName)
            {
                case "Id":
                    if (string.IsNullOrWhiteSpace(this.Id))
                    {
                        this.AddError("Id", "An ID must be specified");
                    }
                    else
                    {
                        if (Regex.IsMatch(this.Id, @"[^a-zA-Z0-9]+"))
                        {
                            this.AddError("Id", "The ID must contain only letters, numbers, hyphen, and underscores");
                        }
                        else if (UniqueIDCache.IsIdInUse(this.Id, this.Model, "transform"))
                        {
                            this.AddError("Id", "The specified ID is already in use");
                        }
                        else
                        {
                            this.RemoveError("Id");
                        }
                    }

                    break;

                default:
                    break;
            }
        }

        public string Id { get; set; }

        public Type TransformType { get; set; }
    }
}