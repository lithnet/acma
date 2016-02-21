using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Novacode;

namespace Lithnet.Acma.Presentation
{
    public class DocXExporter
    {

        public static void ExportUnitTestAsDocX(string fileName, UnitTestFileViewModel unitTestFile)
        {
            DocX doc = DocX.Create(fileName);
            doc.AddFooters();
            doc.Footers.even.InsertParagraph(unitTestFile.FileName);
            doc.Footers.odd.InsertParagraph(unitTestFile.FileName);

            Paragraph p = doc.InsertParagraph();
            
            p = doc.InsertParagraph();
            p.StyleName = "Title";
            p.InsertText("Acma Unit Test Definitions");

            doc.InsertSectionPageBreak();
           
            foreach (var unitTest in unitTestFile.UnitTestObjects)
            {
                if (unitTest is UnitTestGroupViewModel)
                {
                    DocXExporter.WriteUnitTestGroup(unitTest as UnitTestGroupViewModel, doc);
                }
                else if (unitTest is UnitTestViewModel)
                {
                    DocXExporter.WriteUnitTest(unitTest as UnitTestViewModel, doc);
                }
                else
                {
                    throw new ArgumentException("Unknown unit test object");
                }

                doc.InsertSectionPageBreak();
            }

            doc.Save();
        }

        public static void ExportConfigAsDocX(string fileName, XmlConfigFileViewModel xmlConfigFile)
        {
            DocX doc = DocX.Create(fileName);
            doc.AddFooters();
            doc.Footers.even.InsertParagraph(xmlConfigFile.FileName);
            doc.Footers.odd.InsertParagraph(xmlConfigFile.FileName);

            Paragraph p = doc.InsertParagraph();

            p = doc.InsertParagraph();
            p.StyleName = "Title";
            p.InsertText("Acma Configuration");

            p = doc.InsertParagraph();
            p.StyleName = "Heading1";
            p.InsertText("Description");

            p = doc.InsertParagraph(xmlConfigFile.Description);

            doc.InsertSectionPageBreak();
            p = doc.InsertParagraph();
            p.StyleName = "Heading1";
            p.InsertText("Class Constructors");

            foreach (var classConstructor in xmlConfigFile.Constructors)
            {
                p = doc.InsertParagraph();
                p.StyleName = "Heading2";
                p.InsertText(classConstructor.Name);

                DocXExporter.WriteConstructorObject(classConstructor.Constructors, doc);
                doc.InsertSectionPageBreak();
            }

            doc.Save();
        }

        private static void WriteConstructorObject(IEnumerable<ExecutableConstructorObjectViewModel> constructors, DocX doc)
        {
            Paragraph p;
            //http://python-docx.readthedocs.org/en/latest/user/styles.html

            foreach (ExecutableConstructorObjectViewModel constructor in constructors)
            {
                p = doc.InsertParagraph();
                p.StyleName = "Heading3";
                p.InsertText(constructor.DisplayName);

                p = doc.InsertParagraph();
                p.StyleName = "Normal";
                p.InsertText(constructor.Description);

                if (constructor.RuleGroup != null && constructor.RuleGroup.Rules.Count > 0)
                {
                    p = doc.InsertParagraph();
                    p.StyleName = "Heading5";
                    p.InsertText("Execution conditions");

                    p = doc.InsertParagraph();
                    p.StyleName = "Normal";
                    p.InsertText(DocXExporter.WriteRuleGroup(constructor.RuleGroup, 0));

                    // System.Diagnostics.Debug.WriteLine(p.Text);
                }

                if (constructor is AttributeConstructorGroupViewModel)
                {
                    DocXExporter.WriteConstructorObject(((AttributeConstructorGroupViewModel)constructor).Constructors, doc);
                }
                else if (constructor is AttributeConstructorViewModel)
                {
                    if (constructor is DeclarativeValueConstructorViewModel)
                    {
                        DeclarativeValueConstructorViewModel dvc = constructor as DeclarativeValueConstructorViewModel;

                        if (dvc.ModificationType == AcmaAttributeModificationType.Conditional)
                        {
                            p = doc.InsertParagraph();
                            p.StyleName = "Heading5";
                            p.InsertText("Presence Conditions");

                            p = doc.InsertParagraph();
                            p.StyleName = "Normal";
                            p.InsertText(DocXExporter.WriteRuleGroup(dvc.PresenceConditions, 0));
                        }

                        string action;

                        switch (dvc.ModificationType)
                        {
                            case AcmaAttributeModificationType.Add:
                                action = "The following values will be added";
                                break;

                            case AcmaAttributeModificationType.Replace:
                                action = "The following values will replace any existing values";
                                break;

                            case AcmaAttributeModificationType.Delete:
                                action = "The following values will be deleted";
                                break;

                            case AcmaAttributeModificationType.Conditional:
                                action = "The following values will be added or deleted based on the defined presence conditions";
                                break;

                            default:
                                throw new InvalidOperationException();
                        }

                        p = doc.InsertParagraph();
                        p.StyleName = "Heading5";
                        p.InsertText("Value Modifications");

                        p = doc.InsertParagraph();
                        p.StyleName = "Normal";
                        p.InsertText(action);

                        Table t = doc.InsertTable(dvc.ValueDeclarations.Count + 1, 2);

                        t.Alignment = Alignment.left;
                        t.Design = TableDesign.LightListAccent1;

                        t.Rows[0].Cells[0].Paragraphs.First().Append("Declaration");
                        t.Rows[0].Cells[1].Paragraphs.First().Append("Transforms");

                        for (int i = 0; i < dvc.ValueDeclarations.Count; i++)
                        {
                            t.Rows[i + 1].Cells[0].Paragraphs.First().Append(dvc.ValueDeclarations[i].Declaration);
                            t.Rows[i + 1].Cells[1].Paragraphs.First().Append(dvc.ValueDeclarations[i].TransformsString);

                        }
                    }
                }
            }
        }

        private static string WriteRuleGroup(RuleGroupViewModel rule, int indentLevel)
        {
            StringBuilder s = new StringBuilder();
            int rulePadLength = (indentLevel * 4);
            indentLevel++;

            switch (rule.Operator)
            {
                case GroupOperator.None:
                    s.AppendLine(string.Empty.PadRight(rulePadLength) + "None of the following conditions");

                    break;
                case GroupOperator.All:
                    s.AppendLine(string.Empty.PadRight(rulePadLength) + "All of the following conditions");

                    break;
                case GroupOperator.Any:
                    s.AppendLine(string.Empty.PadRight(rulePadLength) + "Any of the following conditions");

                    break;
                case GroupOperator.One:
                    s.AppendLine(string.Empty.PadRight(rulePadLength) + "One of the following conditions");

                    break;
                default:
                    break;
            }

            for (int i = 0; i < rule.Rules.Count; i++)
            {
                if (rule.Rules[i] is RuleGroupViewModel)
                {
                    s.AppendLine(string.Empty.PadRight(rulePadLength) + DocXExporter.WriteRuleObject(rule.Rules[i], indentLevel));
                }
                else
                {
                    s.AppendLine(string.Empty.PadRight(rulePadLength + 4) + DocXExporter.WriteRuleObject(rule.Rules[i], indentLevel));
                }
            }

            return s.ToString().TrimEnd('\n');
        }

        private static string WriteRuleObject(RuleObjectViewModel rule, int indentLevel)
        {
            if (rule is RuleGroupViewModel)
            {
                return DocXExporter.WriteRuleGroup((RuleGroupViewModel)rule, indentLevel);
            }
            else
            {
                return rule.DisplayNameLong;
            }
        }

        private static void WriteUnitTestGroup(UnitTestGroupViewModel group, DocX doc)
        {
            Paragraph p;

            p = doc.InsertParagraph();
            p.StyleName = "Heading1";
            p.InsertText(group.TestId);

            p = doc.InsertParagraph();
            p.StyleName = "Normal";
            p.Italic();
            p.Append(string.Format("{0}", group.ParentPath));

            foreach (UnitTestObjectViewModel item in group.UnitTestObjects)
            {
                if (item is UnitTestGroupViewModel)
                {
                    DocXExporter.WriteUnitTestGroup(item as UnitTestGroupViewModel, doc);
                }
                else if (item is UnitTestViewModel)
                {
                    DocXExporter.WriteUnitTest(item as UnitTestViewModel, doc);
                }
                else
                {
                    throw new ArgumentException("Unknown unit test object");
                }

                doc.InsertSectionPageBreak();
            }
        }

        private static void WriteUnitTest(UnitTestViewModel test, DocX doc)
        {
            Paragraph p;

            p = doc.InsertParagraph();
            p.StyleName = "Heading1";
            p.InsertText(test.TestId);

            p = doc.InsertParagraph();
            p.StyleName = "Normal";
            p.Italic();
            p.Append(string.Format("{0}", test.ParentPath));

            foreach (var step in test.Steps)
            {
                if (step is UnitTestStepObjectCreationViewModel)
                {
                    DocXExporter.WriteUnitTestStepObjectCreation(step as UnitTestStepObjectCreationViewModel, doc);
                }
                else if (step is UnitTestStepObjectModificationViewModel)
                {
                    DocXExporter.WriteUnitTestStepObjectModification(step as UnitTestStepObjectModificationViewModel, doc);
                }
                else if (step is UnitTestStepObjectEvaluationViewModel)
                {
                    DocXExporter.WriteUnitTestStepObjectEvaluation(step as UnitTestStepObjectEvaluationViewModel, doc);
                }
                else
                {
                    throw new ArgumentException("Unknown unit test step object");
                }
            }
        }

        private static void WriteUnitTestStepObjectCreation(UnitTestStepObjectCreationViewModel step, DocX doc)
        {
            Paragraph p;

            p = doc.InsertParagraph();
            p.StyleName = "Heading2";
            p.InsertText(string.Format("Step {0}: {1}", step.ParentIndex + 1, step.DisplayName));

            p = doc.InsertParagraph();
            p.StyleName = "Normal";
            p.InsertText("Object class: " + step.ObjectClassName);
            p.AppendLine("Object name: " + step.Name);
            p.AppendLine("Object ID: " + step.ObjectId);

            p = doc.InsertParagraph();
            p.StyleName = "Heading3";
            p.InsertText(string.Format("Attribute modifications"));

            Table t = doc.InsertTable(step.AttributeChanges.Count + 1, 3);

            t.Alignment = Alignment.left;
            t.Design = TableDesign.LightListAccent1;

            t.Rows[0].Cells[0].Paragraphs.First().Append("Attribute");
            t.Rows[0].Cells[1].Paragraphs.First().Append("Modification type");
            t.Rows[0].Cells[2].Paragraphs.First().Append("Value");

            for (int i = 0; i < step.AttributeChanges.Count; i++)
            {
                t.Rows[i + 1].Cells[0].Paragraphs.First().Append(step.AttributeChanges[i].Name);
                t.Rows[i + 1].Cells[1].Paragraphs.First().Append(step.AttributeChanges[i].ModificationType.ToString());
                t.Rows[i + 1].Cells[2].Paragraphs.First().Append(step.AttributeChanges[i].ValueChangesString);
            }
        }

        private static void WriteUnitTestStepObjectModification(UnitTestStepObjectModificationViewModel step, DocX doc)
        {
            Paragraph p;
            
            p = doc.InsertParagraph();
            p.StyleName = "Heading2";
            p.InsertText(string.Format("Step {0}: {1}", step.ParentIndex + 1, step.DisplayName));

            p = doc.InsertParagraph();
            p.StyleName = "Heading3";
            p.InsertText(string.Format("Attribute modifications"));

            Table t = doc.InsertTable(step.AttributeChanges.Count + 1, 3);

            t.Alignment = Alignment.left;
            t.Design = TableDesign.LightListAccent1;

            t.Rows[0].Cells[0].Paragraphs.First().Append("Attribute");
            t.Rows[0].Cells[1].Paragraphs.First().Append("Modification type");
            t.Rows[0].Cells[2].Paragraphs.First().Append("Value");

            for (int i = 0; i < step.AttributeChanges.Count; i++)
            {
                t.Rows[i + 1].Cells[0].Paragraphs.First().Append(step.AttributeChanges[i].Name);
                t.Rows[i + 1].Cells[1].Paragraphs.First().Append(step.AttributeChanges[i].ModificationType.ToString());
                t.Rows[i + 1].Cells[2].Paragraphs.First().Append(step.AttributeChanges[i].ValueChangesString);
            }
        }

        private static void WriteUnitTestStepObjectEvaluation(UnitTestStepObjectEvaluationViewModel step, DocX doc)
        {
            Paragraph p;

            p = doc.InsertParagraph();
            p.StyleName = "Heading2";
            p.InsertText(string.Format("Step {0}: {1}", step.ParentIndex + 1, step.DisplayName));

            p = doc.InsertParagraph();
            p.StyleName = "Normal";
            p.InsertText(DocXExporter.WriteRuleGroup(step.SuccessCriteria, 0));
        }
    }
}
