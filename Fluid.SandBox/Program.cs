﻿using Fluid;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;

var source = @"<table>
{%- for f in fortunes -%}
    <tr><td>{{ f.Id }}</td><td>{{ f.Message }}</td></tr>
{%- endfor -%}
</table>";

var templates = new Dictionary<string, IFluidTemplate>();

var sw = Stopwatch.StartNew();

var parser = new FluidParser();

var sb = new StringBuilder(2048);
var writer = new StringWriter(sb);

var fortunes = new Fortune[] {
    new (0, "Additional fortune added at request time."),
    new (1, "fortune: No such file or directory"),
    new (2, "A computer scientist is someone who fixes things that aren't broken."),
    new (3, "After enough decimal places, nobody gives a damn."),
    new (4, "A bad random number generator: 1, 1, 1, 1, 1, 4.33e+67, 1, 1, 1"),
    new (5, "A computer program does what you tell it to do, not what you want it to do."),
    new (6, "Emacs is a nice operating system, but I prefer UNIX. — Tom Christaensen"),
    new (7, "Any program that runs right is obsolete."),
    new (8, "A list is only as strong as its weakest link. — Donald Knuth"),
    new (9, "Feature: A bug with seniority."),
    new (10, "Computers make very fast, very accurate mistakes."),
    new (11, "<script>alert(\"This should not be displayed in a browser alert box.\");</ script >"),
    new (12, "フレームワークのベンチマーク"),
};


templates["Interpreted"] = parser.Parse(source);
templates["Custom"] = new CustomTemplate(fortunes);
templates["CustomAsync"] = new CustomTemplateAsync(fortunes);

sw.Restart();
templates["Compiled"] = parser.Compile<ViewModel>(source);
Console.WriteLine($"Compiled in {sw.Elapsed}");
templates["CompiledObject"] = parser.Compile<object>(source);

var options = new TemplateOptions();
options.MemberAccessStrategy.Register<ViewModel>();
options.MemberAccessStrategy.Register<Fortune>();
// options.TemplateCompilationThreshold = 0;

var templateContext = new TemplateContext(new ViewModel { Fortunes = fortunes }, options);
templateContext.SetValue("fortunes", fortunes);

var iterations = 100000;

// Test

foreach (var template in templates)
{
    sb.Clear();
    Console.WriteLine(template.Key);
    await template.Value.RenderAsync(Console.Out, HtmlEncoder.Default, templateContext);
    Console.WriteLine();
}

// Warmup

foreach (var template in templates.Values)
{
    for (var k = 1; k < 1000; k++)
    {
        sb.Clear();
        await template.RenderAsync(writer, HtmlEncoder.Default, templateContext);
    }
}

// Run

foreach (var (name, template) in templates)
{
    sw.Restart();

    for (var k = 1; k < iterations; k++)
    {
        sb.Clear();
        await template.RenderAsync(writer, HtmlEncoder.Default, templateContext);
    }

    Console.WriteLine();
    Console.WriteLine($"{name}: {sw.ElapsedMilliseconds} ms {Math.Round(iterations/ (decimal)sw.ElapsedMilliseconds * 1000, 2)}/s");
}

public class Fortune
{
    public Fortune(int id, string message)
    {
        Id = id;
        Message = message;
    }

    public int Id { get; set; }
    public string Message { get; init; }
}

public class ViewModel
{
    public Fortune[] Fortunes = Array.Empty<Fortune>();
}

class CustomTemplate : IFluidTemplate
{
    private readonly Fortune[] _fortunes;

    public CustomTemplate(Fortune[] fortunes)
    {
        _fortunes = fortunes;
    }
    public ValueTask RenderAsync(TextWriter writer, TextEncoder encoder, TemplateContext context)
    {
        writer.Write(@"<table>");

        foreach (var f in _fortunes)
        {
            writer.Write(@"<tr><td>");
            writer.Write(encoder.Encode(f.Id.ToString()));
            writer.Write(@"</td><td>");
            writer.Write(encoder.Encode(f.Message));
            writer.Write(@"</td></tr>");
        }

        writer.Write(@"</table>");

        return ValueTask.CompletedTask;
    }
}

class CustomTemplateAsync : IFluidTemplate
{
    private readonly Fortune[] _fortunes;

    public CustomTemplateAsync(Fortune[] fortunes)
    {
        _fortunes = fortunes;
    }
    public async ValueTask RenderAsync(TextWriter writer, TextEncoder encoder, TemplateContext context)
    {
        await writer.WriteAsync(@"<table>");

        foreach (var f in _fortunes)
        {
            await writer.WriteAsync(@"<tr><td>");
            await writer.WriteAsync(encoder.Encode(f.Id.ToString()));
            await writer.WriteAsync(@"</td><td>");
            await writer.WriteAsync(encoder.Encode(f.Message));
            await writer.WriteAsync(@"</td></tr>");
        }

        await writer.WriteAsync(@"</table>");
    }
}

class GeneratedTemplate : IFluidTemplate
{
    private readonly Fortune[] _fortunes;

    public GeneratedTemplate(Fortune[] fortunes)
    {
        _fortunes = fortunes;
    }

    public ValueTask RenderAsync(TextWriter writer, TextEncoder encoder, TemplateContext context)
    {
        //LiquidTemplates.RenderFortunes(_fortunes, writer);
        
        return ValueTask.CompletedTask;
    }
}
