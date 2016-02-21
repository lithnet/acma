using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using Lithnet.Common.Presentation;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.Presentation
{
    public  class IconProvider : IIconProvider
    {
        public  BitmapSource GetImageForItem(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            Type itemType = item.GetType();

            if (itemType == typeof(MainWindowViewModel))
            {
               return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Schema.png", UriKind.Absolute));
            }
            else if (itemType == typeof(AcmaConstantsViewModel))
            {
                return this.GetIcon(item as AcmaConstantsViewModel);
            }
            else if (itemType == typeof(AcmaDatabaseViewModel))
            {
                return this.GetIcon(item as AcmaDatabaseViewModel);
            }
            else if (itemType == typeof(AcmaSchemaAttributesViewModel))
            {
                return this.GetIcon(item as AcmaSchemaAttributesViewModel);
            }
            else if (itemType == typeof(AcmaSchemaAttribute))
            {
                return this.GetIcon(item as AcmaSchemaAttribute);
            }
            else if (itemType == typeof(AcmaSchemaMappingsViewModel))
            {
                return this.GetIcon(item as AcmaSchemaMappingsViewModel);
            }
            else if (itemType == typeof(AcmaSchemaMappingViewModel))
            {
                return this.GetIcon(item as AcmaSchemaMappingViewModel);
            }
            else if (itemType == typeof(AcmaSchemaObjectsViewModel))
            {
                return this.GetIcon(item as AcmaSchemaObjectsViewModel);
            }
            else if (itemType == typeof(AcmaSchemaObjectViewModel))
            {
                return this.GetIcon(item as AcmaSchemaObjectViewModel);
            }
            else if (itemType == typeof(AcmaSchemaReferenceLinksViewModel))
            {
                return this.GetIcon(item as AcmaSchemaReferenceLinksViewModel);
            }
            else if (itemType == typeof(AcmaSchemaReferenceLinkViewModel))
            {
                return this.GetIcon(item as AcmaSchemaReferenceLinkViewModel);
            }
            else if (itemType == typeof(AcmaSchemaShadowObjectLinksViewModel))
            {
                return this.GetIcon(item as AcmaSchemaShadowObjectLinksViewModel);
            }
            else if (itemType == typeof(AcmaSchemaShadowObjectLinkViewModel))
            {
                return this.GetIcon(item as AcmaSchemaShadowObjectLinkViewModel);
            }
            else if (itemType == typeof(AcmaSequencesViewModel))
            {
                return this.GetIcon(item as AcmaSequencesViewModel);
            }
            else if (itemType == typeof(AcmaSequenceViewModel))
            {
                return this.GetIcon(item as AcmaSequenceViewModel);
            }
            else if (itemType == typeof(SafetyRulesViewModel))
            {
                return this.GetIcon(item as SafetyRulesViewModel);
            }
            else if (itemType == typeof(SafetyRuleViewModel))
            {
                return this.GetIcon(item as SafetyRuleViewModel);
            }
            else if (itemType == typeof(AcmaExitEventsViewModel))
            {
                return this.GetIcon(item as AcmaExitEventsViewModel);
            }
            else if (itemType == typeof(AcmaOperationEventsViewModel))
            {
                return this.GetIcon(item as AcmaExitEventsViewModel);
            }
            else if (itemType == typeof(AcmaInternalExitEventViewModel))
            {
                return this.GetIcon(item as AcmaInternalExitEventViewModel);
            }
            else if (itemType == typeof(AcmaExternalExitEventCmdViewModel))
            {
                return this.GetIcon(item as AcmaExternalExitEventCmdViewModel);
            }
            else if (itemType == typeof(AcmaExternalExitEventExtensibleViewModel))
            {
                return this.GetIcon(item as AcmaExternalExitEventExtensibleViewModel);
            }
            else if (itemType == typeof(AcmaOperationEventViewModel))
            {
                return this.GetIcon(item as AcmaOperationEventViewModel);
            }
            else if (itemType == typeof(AttributeConstructorGroupViewModel))
            {
                return this.GetIcon(item as AttributeConstructorGroupViewModel);
            }
            else if (itemType == typeof(RuleGroupViewModel))
            {
                return this.GetIcon(item as RuleGroupViewModel);
            }
            else if (itemType == typeof(XmlConfigFileViewModel))
            {
                return this.GetIcon(item as XmlConfigFileViewModel);
            }
            else if (itemType == typeof(ClassConstructorViewModel))
            {
                return this.GetIcon(item as ClassConstructorViewModel);
            }
            else if (itemType == typeof(DBQueryObjectsViewModel))
            {
                return this.GetIcon(item as DBQueryObjectsViewModel);
            }
            else if (itemType == typeof(DBQueryGroupViewModel))
            {
                return this.GetIcon(item as DBQueryGroupViewModel);
            }
            else if (itemType == typeof(DBQueryByValueViewModel))
            {
                return this.GetIcon(item as DBQueryByValueViewModel);
            }
            else if (itemType == typeof(DeclarativeValueConstructorViewModel))
            {
                return this.GetIcon(item as DeclarativeValueConstructorViewModel);
            }
            else if (itemType == typeof(UniqueValueConstructorViewModel))
            {
                return this.GetIcon(item as UniqueValueConstructorViewModel);
            }
            else if (itemType == typeof(ReferenceLookupConstructorViewModel))
            {
                return this.GetIcon(item as ReferenceLookupConstructorViewModel);
            }
            else if (itemType == typeof(SequentialIntegerAllocationConstructorViewModel))
            {
                return this.GetIcon(item as SequentialIntegerAllocationConstructorViewModel);
            }
            else if (itemType == typeof(AttributeValueDeleteConstructorViewModel))
            {
                return this.GetIcon(item as AttributeValueDeleteConstructorViewModel);
            }
            else if (itemType == typeof(UnitTestFileViewModel))
            {
                return this.GetIcon(item as UnitTestFileViewModel);
            }
            else if (itemType == typeof(UnitTestViewModel))
            {
                return this.GetIcon(item as UnitTestViewModel);
            }
            else if (itemType == typeof(UnitTestGroupViewModel))
            {
                return this.GetIcon(item as UnitTestGroupViewModel);
            }
            else if (itemType == typeof(ClassConstructorsViewModel))
            {
                return this.GetIcon(item as ClassConstructorsViewModel);
            }
            else if (itemType == typeof(ConstructorsViewModel))
            {
                return this.GetIcon(item as ConstructorsViewModel);
            }
            else if (itemType == typeof(UnitTestStepObjectCreationViewModel))
            {
                return this.GetIcon(item as UnitTestStepObjectCreationViewModel);
            }
            else if (itemType == typeof(UnitTestStepObjectEvaluationViewModel))
            {
                return this.GetIcon(item as UnitTestStepObjectEvaluationViewModel);
            }
            else if (itemType == typeof(UnitTestStepObjectModificationViewModel))
            {
                return this.GetIcon(item as UnitTestStepObjectModificationViewModel);
            }
            else if (itemType == typeof(SchemaAttributeUsageViewModel))
            {
                return this.GetIcon(item as SchemaAttributeUsageViewModel);
            }

            if (itemType.IsSubclassOf(typeof(RuleViewModel)))
            {
                return this.GetIcon(item as RuleViewModel);
            }


            return null;
        }

        private  BitmapSource GetIcon(AcmaConstantsViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Constant.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(AcmaDatabaseViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Database.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(AcmaSchemaAttributesViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Attribute.png", UriKind.Absolute));
        }

        private BitmapSource GetIcon(AcmaSchemaAttribute item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Attribute.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(AcmaSchemaMappingsViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Mapping.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(AcmaSchemaMappingViewModel item)
        {
            if (item.IsInherited)
            {
                return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/MappingInherited.png", UriKind.Absolute));
            }
            else
            {
                return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Mapping.png", UriKind.Absolute));
            }
        }

        private  BitmapSource GetIcon(AcmaSchemaObjectsViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/ObjectClasses.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(AcmaSchemaObjectViewModel item)
        {
            if (item.Model.IsShadowObject)
            {
                return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/ShadowObjectClass.png", UriKind.Absolute));
            }
            else
            {
                return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/ObjectClass.png", UriKind.Absolute));
            }
        }

        private  BitmapSource GetIcon(AcmaSchemaReferenceLinksViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/BackLink.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(AcmaSchemaReferenceLinkViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/BackLink.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(AcmaSchemaShadowObjectLinksViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/ShadowLink.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(AcmaSchemaShadowObjectLinkViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/ShadowLink.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(AcmaSequencesViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Sequence.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(AcmaSequenceViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Sequence.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(SafetyRulesViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Transforms.Presentation;component/Resources/transforms.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(SafetyRuleViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/AttributeTemp.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(AcmaExitEventsViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Event.png", UriKind.Absolute));
        }

        private BitmapSource GetIcon(AcmaOperationEventsViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Event.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(AcmaInternalExitEventViewModel item)
        {
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Event.png", UriKind.Absolute));

            if (item.IsDisabled)
            {
                BitmapImage disabled = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/DisabledOverlay.png", UriKind.Absolute));
                return this.MergeImages(image, disabled);
            }
            else
            {
                return image;
            }
        }

        private BitmapSource GetIcon(AcmaExternalExitEventCmdViewModel item)
        {
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Event.png", UriKind.Absolute));

            if (item.IsDisabled)
            {
                BitmapImage disabled = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/DisabledOverlay.png", UriKind.Absolute));
                return this.MergeImages(image, disabled);
            }
            else
            {
                return image;
            }
        }

        private BitmapSource GetIcon(AcmaExternalExitEventExtensibleViewModel item)
        {
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Event.png", UriKind.Absolute));

            if (item.IsDisabled)
            {
                BitmapImage disabled = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/DisabledOverlay.png", UriKind.Absolute));
                return this.MergeImages(image, disabled);
            }
            else
            {
                return image;
            }
        }

        private BitmapSource GetIcon(AcmaOperationEventViewModel item)
        {
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Event.png", UriKind.Absolute));

            if (item.IsDisabled)
            {
                BitmapImage disabled = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/DisabledOverlay.png", UriKind.Absolute));
                return this.MergeImages(image, disabled);
            }
            else
            {
                return image;
            }
        }

        private  BitmapSource GetIcon(AttributeConstructorGroupViewModel item)
        {
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/ConstructorGroup.png", UriKind.Absolute));

            if (item.Disabled)
            {
                BitmapImage disabled = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/DisabledOverlay.png", UriKind.Absolute));
                return this.MergeImages(image, disabled);
            }
            else
            {
                return image;
            }
        }

        private  BitmapSource GetIcon(RuleGroupViewModel item)
        {
            if (item.ParentConstructor == null)
            {
                return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/RuleGroup.png", UriKind.Absolute));
            }
            else
            {
                return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/ExecutionConditions.png", UriKind.Absolute));
            }
        }

        private  BitmapSource GetIcon(RuleViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Rule.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(XmlConfigFileViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/XmlConfigFile.png", UriKind.Absolute));
        }


        private  BitmapSource GetIcon(DBQueryByValueViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/DBQuery.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(DBQueryGroupViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/DBQueryGroup.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(DBQueryObjectsViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/DBQueryGroup.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(SequentialIntegerAllocationConstructorViewModel item)
        {
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Sequence.png", UriKind.Absolute));

            if (item.Disabled)
            {
                BitmapImage disabled = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/DisabledOverlay.png", UriKind.Absolute));
                return this.MergeImages(image, disabled);
            }
            else
            {
                return image;
            }
        }

        private  BitmapSource GetIcon(DeclarativeValueConstructorViewModel item)
        {
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/DeclarativeValueConstructor.png", UriKind.Absolute));

            if (item.Disabled)
            {
                BitmapImage disabled = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/DisabledOverlay.png", UriKind.Absolute));
                return this.MergeImages(image, disabled);
            }
            else
            {
                return image;
            } 
        }

        private BitmapSource GetIcon(SchemaAttributeUsageViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/DeclarativeValueConstructor.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(ReferenceLookupConstructorViewModel item)
        {
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/ReferenceLookupConstructor.png", UriKind.Absolute));

            if (item.Disabled)
            {
                BitmapImage disabled = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/DisabledOverlay.png", UriKind.Absolute));
                return this.MergeImages(image, disabled);
            }
            else
            {
                return image;
            } 
        }

        private  BitmapSource GetIcon(UniqueValueConstructorViewModel item)
        {
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/UniqueValueConstructor.png", UriKind.Absolute));

            if (item.Disabled)
            {
                BitmapImage disabled = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/DisabledOverlay.png", UriKind.Absolute));
                return this.MergeImages(image, disabled);
            }
            else
            {
                return image;
            } 
        }

        private  BitmapSource GetIcon(AttributeValueDeleteConstructorViewModel item)
        {
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/ValueDeleteConstructor.png", UriKind.Absolute));

            if (item.Disabled)
            {
                BitmapImage disabled = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/DisabledOverlay.png", UriKind.Absolute));
                return this.MergeImages(image, disabled);
            }
            else
            {
                return image;
            }
        }

        private  BitmapSource GetIcon(UnitTestFileViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/UnitTests.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(UnitTestGroupViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/UnitTests.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(UnitTestStepObjectModificationViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/EditObject.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(UnitTestStepObjectEvaluationViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/EvaluateObject.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(UnitTestStepObjectCreationViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/NewObject.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(UnitTestViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/UnitTest.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(ClassConstructorViewModel item)
        {
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/ObjectClass.png", UriKind.Absolute));

            if (item.Disabled)
            {
                BitmapImage disabled = new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/DisabledOverlay.png", UriKind.Absolute));
                return this.MergeImages(image, disabled);
            }
            else
            {
                return image;
            }
        }

        private  BitmapSource GetIcon(ClassConstructorsViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/ObjectClasses.png", UriKind.Absolute));
        }

        private  BitmapSource GetIcon(ConstructorsViewModel item)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Lithnet.Acma.Presentation;component/Resources/Constructors.png", UriKind.Absolute));
        }

        private  BitmapSource MergeImages(BitmapImage image1, BitmapImage image2)
        {
            // Gets the size of the images (I assume each image has the same size)
            int imageWidth = image1.PixelWidth;
            int imageHeight = image2.PixelHeight;

            // Draws the images into a DrawingVisual component
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(image1, new Rect(0, 0, imageWidth, imageHeight));
                drawingContext.DrawImage(image2, new Rect(0, 0, imageWidth, imageHeight));
            }

            // Converts the Visual (DrawingVisual) into a BitmapSource
            RenderTargetBitmap bmp = new RenderTargetBitmap(imageWidth, imageHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            return bmp;
        }
    }
}
