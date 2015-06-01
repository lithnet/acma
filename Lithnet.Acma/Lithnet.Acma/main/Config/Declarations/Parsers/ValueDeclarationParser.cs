using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma
{
    public class ValueDeclarationParser
    {
        private string declaration;

        private string context;

        public ValueDeclarationParser(string declaration)
            : this(declaration, null)
        {
        }

        public ValueDeclarationParser(string declaration, string context)
        {
            this.declaration = declaration;
            this.context = context ?? declaration;

            this.Parse(declaration);
        }

        public IEnumerable<VariableDeclaration> VariableDeclarations
        {
            get
            {
                return this.VariableDeclarationParsers.Select(t => t.GetVariableDeclaration());
            }
        }

        public IEnumerable<AttributeDeclaration> AttributeDeclarations
        {
            get
            {
                return this.AttributeDeclarationParsers.Select(t => t.GetAttributeDeclaration());
            }
        }

        private IList<VariableDeclarationParser> VariableDeclarationParsers { get; set; }

        private IList<AttributeDeclarationParser> AttributeDeclarationParsers { get; set; }

        public IList<TokenError> Errors { get; private set; }

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

        private void Parse(string declaration)
        {
            StringTokenizer t = new StringTokenizer(declaration);
            this.Parse(t);
        }

        private void Parse(StringTokenizer t)
        {
            this.Errors = new List<TokenError>();
            this.VariableDeclarationParsers = new List<VariableDeclarationParser>();
            this.AttributeDeclarationParsers = new List<AttributeDeclarationParser>();

            t.Next();

            while (t.CurrentToken.Kind != TokenKind.EOF)
            {
                //char peek = t.CurrentChar;

                if (t.CurrentToken.Kind == TokenKind.OpenBrace || t.CurrentToken.Kind == TokenKind.OpenSquareBracket)
                {
                    AttributeDeclarationParser p = new AttributeDeclarationParser(t, this.context);
                    
                    if (p.HasErrors)
                    {
                        foreach (TokenError error in p.Errors)
                        {
                            this.Errors.Add(error);
                        }

                        return;
                    }
                    else
                    {
                        this.AttributeDeclarationParsers.Add(p);
                    }
                }
                else if (t.CurrentToken.Kind == TokenKind.PercentSign)
                {
                    VariableDeclarationParser p = new VariableDeclarationParser(t, this.context);

                    if (p.HasErrors)
                    {
                        foreach (TokenError error in p.Errors)
                        {
                            this.Errors.Add(error);
                        }

                        return;
                    }
                    else
                    {
                        this.VariableDeclarationParsers.Add(p);
                    }
                }

                t.Next();
            }
        }
    }
}