﻿using Fluid.Values;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;

namespace Fluid.Compilation
{
    public class CompiledTemplateBase
    {
        public static FluidValue BuildArray(int start, int end)
        {
            // If end < start, we create an empty array
            var list = new FluidValue[Math.Max(0, end - start + 1)];

            for (var i = 0; i < list.Length; i++)
            {
                list[i] = NumberValue.Create(start + i);
            }

            return new ArrayValue(list);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task WriteAsync(string value, TextWriter writer, TextEncoder encoder, TemplateContext context)
        {
            return writer.WriteAsync(encoder.Encode(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task WriteAsync(int value, TextWriter writer, TextEncoder encoder, TemplateContext context)
        {
            return writer.WriteAsync(encoder.Encode(value.ToString(context.CultureInfo)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task WriteAsync(float value, TextWriter writer, TextEncoder encoder, TemplateContext context)
        {
            return writer.WriteAsync(encoder.Encode(value.ToString(context.CultureInfo)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task WriteAsync(double value, TextWriter writer, TextEncoder encoder, TemplateContext context)
        {
            return writer.WriteAsync(encoder.Encode(value.ToString(context.CultureInfo)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task WriteAsync(decimal value, TextWriter writer, TextEncoder encoder, TemplateContext context)
        {
            return writer.WriteAsync(encoder.Encode(value.ToString(context.CultureInfo)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task WriteAsync(bool value, TextWriter writer, TextEncoder encoder, TemplateContext context)
        {
            return writer.WriteAsync(value ? "true" : "false");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task WriteAsync(DateTime value, TextWriter writer, TextEncoder encoder, TemplateContext context)
        {
            return writer.WriteAsync(value.ToString("u", context.CultureInfo));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task WriteAsync(DateTimeOffset value, TextWriter writer, TextEncoder encoder, TemplateContext context)
        {
            return writer.WriteAsync(value.ToString("u", context.CultureInfo));
        }

        // TODO: Add other overload for Write()

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task WriteAsync(object value, TextWriter writer, TextEncoder encoder, TemplateContext context)
        {
            var wrapped = FluidValue.Create(value, context.Options);
            wrapped.WriteTo(writer, encoder, context.CultureInfo);
            return Task.CompletedTask;
        }
    }
}
