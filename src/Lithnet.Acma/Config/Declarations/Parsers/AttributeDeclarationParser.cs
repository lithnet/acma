using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma
{
    public class AttributeDeclarationParser
    {
        private string context;

        private AcmaSchemaObjectClass objectClass;

        public AttributeDeclarationParser(StringTokenizer t, string context)
        {
            this.context = context;
            this.ValidateExpression(t);
        }

        public AttributeDeclarationParser(string declaration)
            : this(declaration, null, null)
        {
        }

        public AttributeDeclarationParser(string declaration, string context)
            : this(declaration, null, context)
        {
        }

        public AttributeDeclarationParser(string declaration, AcmaSchemaObjectClass objectClass)
            : this(declaration, objectClass, null)
        {
        }

        public AttributeDeclarationParser(string declaration, AcmaSchemaObjectClass objectClass, string context)
        {
            this.objectClass = objectClass;
            this.context = context ?? declaration;
            this.ValidateExpression(declaration);
        }

        public IList<Transform> Transforms { get; private set; }

        public IList<AcmaSchemaAttribute> EvaluationPath { get; private set; }

        public AcmaSchemaAttribute Attribute { get; private set; }

        public IList<TokenError> Errors { get; private set; }

        public HologramView View { get; private set; }

        public string Declaration { get; private set; }

        public string PreText { get; private set; }

        public string PostText { get; private set; }

        public bool HasErrors
        {
            get
            {
                if (this.Errors == null)
                {
                    return false;
                }
                else
                {
                    return this.Errors.Count > 0;
                }
            }
        }

        public AttributeDeclaration GetAttributeDeclaration()
        {
            if (this.HasErrors)
            {
                throw new InvalidDeclarationStringException(
                  string.Format(
                      "The declaration contained one or more errors\n{0}",
                      this.Errors.Select(u => u.ToString()).ToNewLineSeparatedString()));
            }

            return new AttributeDeclaration(
                this.Attribute,
                this.View,
                this.Transforms,
                this.PreText,
                this.PostText,
                this.EvaluationPath,
                this.Declaration
                );
        }

        private void ValidateExpression(string declaration)
        {
            StringTokenizer t = new StringTokenizer(declaration);
            this.ValidateExpression(t);
        }

        private void ValidateExpression(StringTokenizer t)
        {
            this.EvaluationPath = new List<AcmaSchemaAttribute>();
            this.Transforms = new List<Transform>();
            this.Errors = new List<TokenError>();
            this.View = HologramView.Proposed;
            bool hasOptionalData = false;
            Token token;

            if (t.CurrentToken == null)
            {
                token = t.Next();
            }
            else
            {
                token = t.CurrentToken;
            }

            int startingPosition = token.Position;

            if (token.Kind == TokenKind.OpenSquareBracket)
            {
                hasOptionalData = true;

                if (!this.GetPreText(t))
                {
                    this.Errors.Add(new TokenError("'{' expected", t.CurrentToken.Line, t.CurrentToken.Column, null, this.context));
                    return;
                }
            }
            else if (token.Kind != TokenKind.OpenBrace)
            {
                this.Errors.Add(new TokenError("'{' expected", token.Line, token.Column, token.Value, this.context));
                return;
            }

            token = t.Next();

            if (token.Kind == TokenKind.Hash)
            {
                this.View = HologramView.Current;
                token = t.Next();
            }
            else if (token.Kind != TokenKind.Word)
            {
                this.Errors.Add(new TokenError("'#' or attribute name expected", token.Line, token.Column, token.Value, this.context));
                return;
            }

            while (true)
            {
                Token attributeToken = null;

                if (token.Kind != TokenKind.Word)
                {
                    this.Errors.Add(new TokenError("Expected attribute name", token.Line, token.Column, token.Value, this.context));
                    return;
                }

                AcmaSchemaAttribute attribute = null;

                if (!ActiveConfig.DB.HasAttribute(token.Value))
                {
                    this.Errors.Add(new TokenError("The attribute was not found", token.Line, token.Column, token.Value, this.context));
                    return;
                }
                else
                {
                    attribute = ActiveConfig.DB.GetAttribute(token.Value);
                    attributeToken = token;
                }

                token = t.Next();

                if (token.Kind == TokenKind.ClosedBrace)
                {
                    if (this.EvaluationPath.Count == 0)
                    {
                        if (this.objectClass != null)
                        {
                            if (!this.objectClass.HasAttribute(attribute.Name))
                            {
                                this.Errors.Add(new TokenError(string.Format("Attribute not found in object class '{0}'", this.objectClass.Name), attributeToken.Line, attributeToken.Column, attributeToken.Value, this.context));
                                return;
                            }
                        }
                    }

                    if (hasOptionalData)
                    {
                        if (!this.GetPostText(t))
                        {
                            this.Errors.Add(new TokenError("']' expected", t.CurrentToken.Line, t.CurrentToken.Column, null, this.context));
                            return;
                        }
                    }

                    this.Attribute = attribute;
                    this.Declaration = t.Data.Substring(startingPosition, t.CurrentPosition - startingPosition);

                    return;
                }
                else if (token.Kind == TokenKind.ReferenceOperator)
                {
                    if (attribute.Type != ExtendedAttributeType.Reference)
                    {
                        this.Errors.Add(new TokenError("Non-reference attribute found in reference chain", attributeToken.Line, attributeToken.Column, attributeToken.Value, this.context));
                        return;
                    }
                    else
                    {
                        if (this.EvaluationPath.Count == 0)
                        {
                            if (this.objectClass != null)
                            {
                                if (!this.objectClass.HasAttribute(attribute.Name))
                                {
                                    this.Errors.Add(new TokenError(string.Format("Attribute not found in object class '{0}'", this.objectClass.Name), attributeToken.Line, attributeToken.Column, attributeToken.Value, this.context));
                                    return;
                                }
                            }
                        }

                        this.EvaluationPath.Add(attribute);
                        token = t.Next();
                        continue;
                    }

                }
                else if (token.Kind == TokenKind.TransformOperator)
                {
                    this.Attribute = attribute;

                    TransformParser transformTokenizer = new TransformParser(t, TokenKind.ClosedBrace, this.context);

                    if (transformTokenizer.HasErrors)
                    {
                        foreach (TokenError error in transformTokenizer.Errors)
                        {
                            this.Errors.Add(error);
                        }

                        return;
                    }
                    else
                    {
                        if (hasOptionalData)
                        {
                            if (!this.GetPostText(t))
                            {
                                this.Errors.Add(new TokenError("']' expected", t.CurrentToken.Line, t.CurrentToken.Column, null, this.context));
                                return;
                            }
                        }

                        this.Transforms = transformTokenizer.Transforms;
                        this.Declaration = t.Data.Substring(startingPosition, t.CurrentPosition - startingPosition);
                        return;
                    }
                }
                else
                {
                    this.Errors.Add(new TokenError("Expected '}', '->' or '>>'", token.Line, token.Column, token.Value, this.context));
                    return;
                }
            }
        }

        private bool GetPreText(StringTokenizer t)
        {
            Token token;
            int startPreText = t.CurrentPosition;
            bool foundOpenBrace = false;

            while (!t.EOF)
            {
                token = t.Next();

                if (token.Kind == TokenKind.OpenBrace)
                {
                    foundOpenBrace = true;
                    this.PreText = t.Data.Substring(startPreText, t.CurrentPosition - startPreText - 1);
                    break;
                }
            }

            return foundOpenBrace;
        }

        private bool GetPostText(StringTokenizer t)
        {
            Token token;

            int postTextStart = t.CurrentPosition;
            bool foundCloseSquareBracket = false;

            while (!t.EOF)
            {
                token = t.Next();

                if (token.Kind == TokenKind.CloseSquareBracket)
                {
                    foundCloseSquareBracket = true;
                    this.PostText = t.Data.Substring(postTextStart, t.CurrentPosition - postTextStart - 1);
                    break;
                }
            }

            return foundCloseSquareBracket;
        }
    }
}
