using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma
{
    public class VariableDeclarationParser
    {
        private string context;

        public VariableDeclarationParser(StringTokenizer t, string context)
        {
            this.context = context;
            this.ValidateExpression(t);
        }

        public VariableDeclarationParser(string declaration)
            : this(declaration, null)
        {
        }

        public VariableDeclarationParser(string declaration, string context)
        {
            this.context = context ?? declaration;
            this.ValidateExpression(declaration);
        }

        public IList<Transform> Transforms { get; private set; }

        public string VariableName { get; private set; }

        public string VariableParameter { get; private set; }

        public IList<TokenError> Errors { get; private set; }

        public string Declaration { get; private set; }

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

        public VariableDeclaration GetVariableDeclaration()
        {
            if (this.HasErrors)
            {
                throw new InvalidDeclarationStringException(
                  string.Format(
                      "The declaration contained one or more errors\n{0}",
                      this.Errors.Select(u => u.ToString()).ToNewLineSeparatedString()));
            }

            return new VariableDeclaration(this.VariableName, this.VariableParameter, this.Transforms, this.Declaration);
        }

        private void ValidateExpression(string declaration)
        {
            StringTokenizer t = new StringTokenizer(declaration);
            this.ValidateExpression(t);
        }

        private void ValidateExpression(StringTokenizer t)
        {
            this.Transforms = new List<Transform>();
            this.Errors = new List<TokenError>();
            this.VariableParameter = null;
            this.VariableName = null;

            Token token ;

            if (t.CurrentToken == null)
            {
                token = t.Next();
            }
            else
            {
                token = t.CurrentToken;
            }

            int startPos = token.Position;

            // Ensure we are starting with a %
            if (token.Kind != TokenKind.PercentSign)
            {
                this.Errors.Add(new TokenError("'%' expected", token.Line, token.Column, token.Value, this.context));
                return;
            }

            token = t.Next();

            // Make sure the second token is a word
            if (token.Kind != TokenKind.Word)
            {
                this.Errors.Add(new TokenError("Expected variable name", token.Line, token.Column, token.Value, this.context));
                return;
            }

            // Make sure the variable name we found actually exists
            if (!VariableDeclaration.GetVariableNames().Contains(token.Value))
            {
                this.Errors.Add(new TokenError("Unknown variable", token.Line, token.Column, token.Value, this.context));
                return;
            }
            else
            {
                this.VariableName = token.Value;
            }

            token = t.Next();

            // We have reached the end of the declaration
            if (token.Kind == TokenKind.PercentSign)
            {
                this.Declaration = t.Data.Substring(startPos, t.CurrentPosition - startPos);
                return;
            }
            else if (token.Kind == TokenKind.Colon)
            {
                token = t.Next();

                // Make sure we have a word for the variable parameter
                if (token.Kind != TokenKind.Word)
                {
                    this.Errors.Add(new TokenError("Expected variable parameter", token.Line, token.Column, token.Value, this.context));
                    return;
                }

                if (VariableDeclaration.DoesVariableSupportParameters(this.VariableName))
                {
                    if (VariableDeclaration.IsValidParameter(this.VariableName, token.Value))
                    {
                        this.VariableParameter = token.Value;
                    }
                    else
                    {
                        this.Errors.Add(new TokenError(string.Format("Specified parameter was not valid for Variable '{0}'", this.VariableName), token.Line, token.Column, token.Value, this.context));
                        return;
                    }
                }
                else
                {
                    this.Errors.Add(new TokenError(string.Format("Variable '{0}' does not support parameters", this.VariableName), token.Line, token.Column, token.Value, this.context));
                    return;
                }

                token = t.Next();

                // Check to see if we have reached the end of the declaration
                if (token.Kind == TokenKind.PercentSign)
                {
                    this.Declaration = t.Data.Substring(startPos, t.CurrentPosition - startPos);
                    return;
                }
            }

            if (token.Kind == TokenKind.TransformOperator)
            {
                TransformParser transformTokenizer = new TransformParser(t, TokenKind.PercentSign, this.context);

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
                    this.Transforms = transformTokenizer.Transforms;
                    this.Declaration = t.Data.Substring(startPos, t.CurrentPosition - startPos);
                    return;
                }
            }
            else
            // We expect either an declaration end (%), a parameter operator (:), or a transform operator (>>)
            {
                this.Errors.Add(new TokenError("Expected '%', ':', '>>'", token.Line, token.Column, token.Value, this.context));
                return;
            }


        }
    }
}
