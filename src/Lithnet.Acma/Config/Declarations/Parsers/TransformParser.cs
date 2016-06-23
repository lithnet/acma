using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;

namespace Lithnet.Acma
{
    public class TransformParser
    {
        private string context;

        public TransformParser(string transformString, string context = null)
        {
            this.context = context ?? transformString;
            this.ValidateExpression(transformString);
        }

        public TransformParser(StringTokenizer t, TokenKind endToken, string context)
        {
            this.context = context;
            this.ValidateExpression(t, endToken);
        }

        public List<Transform> Transforms { get; private set; }

        public List<TokenError> Errors { get; private set; }

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

        private void ValidateExpression(string declaration)
        {
            StringTokenizer t = new StringTokenizer(declaration);
            this.ValidateExpression(t, TokenKind.EOF);
        }

        private void ValidateExpression(StringTokenizer t, TokenKind endToken)
        {
            this.Transforms = new List<Transform>();
            this.Errors = new List<TokenError>();

            while (!t.EOF)
            {
                Token token = t.Next();
                
                if (token.Kind != TokenKind.Word)
                {
                    this.Errors.Add(new TokenError("A transform name was expected", token.Line, token.Column, token.Value, this.context));
                    return;
                }

                if (!ActiveConfig.XmlConfig.Transforms.Contains(token.Value))
                {
                    this.Errors.Add(new TokenError("The transform was not found", token.Line, token.Column, token.Value, this.context));
                }
                else
                {
                    this.Transforms.Add(ActiveConfig.XmlConfig.Transforms[token.Value]);
                }

                token = t.Next();

                if (token.Kind == TokenKind.EOF)
                {
                    if (endToken == TokenKind.EOF)
                    {
                        return;
                    }
                    else
                    {
                        this.Errors.Add(new TokenError(string.Format("Expected '>>' or '{0}'", endToken.ToString()), token.Line, token.Column, token.Value, this.context));
                    }
                }
                else if (token.Kind == endToken)
                {
                    return;
                }
                else if (token.Kind == TokenKind.TransformOperator)
                {
                    continue;
                }
                else 
                {
                    this.Errors.Add(new TokenError(string.Format("Expected '>>' or '{0}'", endToken.ToString()), token.Line, token.Column, token.Value, this.context));
                    return;
                }
            }
        }
    }
}
