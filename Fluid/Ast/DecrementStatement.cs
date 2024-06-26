﻿using Fluid.Values;
using System.Text.Encodings.Web;

namespace Fluid.Ast
{
    public sealed class DecrementStatement : Statement
    {
        public DecrementStatement(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; }

        public override ValueTask<Completion> WriteToAsync(TextWriter writer, TextEncoder encoder, TemplateContext context)
        {
            context.IncrementSteps();

            // We prefix the identifier to prevent collisions with variables.
            // Variable identifiers don't represent the same slots as inc/dec ones.
            // c.f. https://shopify.github.io/liquid/tags/variable/

            var prefixedIdentifier = IncrementStatement.Prefix + Identifier;

            var value = context.GetValue(prefixedIdentifier);

            if (value.IsNil())
            {
                value = NumberValue.Zero;
            }
            else
            {
                value = NumberValue.Create(value.ToNumberValue() - 1);
            }

            context.SetValue(prefixedIdentifier, value);

            value.WriteTo(writer, encoder, context.CultureInfo);

            return Normal();
        }

        protected internal override Statement Accept(AstVisitor visitor) => visitor.VisitDecrementStatement(this);
    }
}
