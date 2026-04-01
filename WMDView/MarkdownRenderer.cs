using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Diagnostics;
using Microsoft.Win32;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace WMDView;

public class MarkdownRenderer
{
    private readonly MarkdownPipeline _pipeline;
    private bool _isDarkTheme;

    public MarkdownRenderer()
    {
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
        _isDarkTheme = IsSystemDarkTheme();
    }

    public bool IsDarkTheme => _isDarkTheme;

    public void SetTheme(bool isDark)
    {
        _isDarkTheme = isDark;
    }

    public FlowDocument Render(string markdown)
    {
        var document = new FlowDocument
        {
            PagePadding = new Thickness(40),
            FontFamily = new FontFamily("Microsoft YaHei UI"),
            FontSize = 14,
            Foreground = _isDarkTheme ? Brushes.White : Brushes.Black,
            Background = _isDarkTheme ? new SolidColorBrush(Color.FromRgb(30, 30, 30)) : Brushes.White
        };

        if (string.IsNullOrWhiteSpace(markdown))
        {
            return document;
        }

        var ast = Markdig.Markdown.Parse(markdown, _pipeline);
        RenderBlockElements(ast, document.Blocks);

        return document;
    }

    private void RenderBlockElements(ContainerBlock container, BlockCollection blocks)
    {
        foreach (var block in container)
        {
            switch (block)
            {
                case HeadingBlock heading:
                    blocks.Add(RenderHeading(heading));
                    break;
                case ParagraphBlock paragraph:
                    blocks.Add(RenderParagraph(paragraph));
                    break;
                case ListBlock list:
                    blocks.Add(RenderList(list));
                    break;
                case QuoteBlock quote:
                    blocks.Add(RenderQuote(quote));
                    break;
                case FencedCodeBlock fencedCode:
                    blocks.Add(RenderFencedCodeBlock(fencedCode));
                    break;
                case CodeBlock code:
                    blocks.Add(RenderCodeBlock(code));
                    break;
                case ThematicBreakBlock thematicBreak:
                    blocks.Add(RenderThematicBreak());
                    break;
            }
        }
    }

    private System.Windows.Documents.Block RenderHeading(HeadingBlock heading)
    {
        var paragraph = new Paragraph
        {
            Margin = new Thickness(0, heading.Level == 1 ? 20 : 10, 0, 10)
        };

        var fontSize = heading.Level switch
        {
            1 => 32,
            2 => 24,
            3 => 20,
            4 => 18,
            5 => 16,
            _ => 14
        };

        var span = new Span();
        if (heading.Inline != null)
        {
            RenderInlines(heading.Inline, span.Inlines);
        }
        span.FontSize = fontSize;
        span.FontWeight = FontWeights.Bold;

        paragraph.Inlines.Add(span);
        return paragraph;
    }

    private System.Windows.Documents.Block RenderParagraph(ParagraphBlock paragraph)
    {
        var p = new Paragraph
        {
            Margin = new Thickness(0, 0, 0, 10)
        };

        if (paragraph.Inline != null)
        {
            RenderInlines(paragraph.Inline, p.Inlines);
        }
        return p;
    }

    private System.Windows.Documents.Block RenderList(ListBlock list)
    {
        var container = new Section();
        var listItemIndex = 0;

        foreach (var block in list)
        {
            if (block is ListItemBlock listItem)
            {
                var paragraph = new Paragraph
                {
                    Margin = new Thickness(0, 0, 0, 5),
                    TextIndent = -20,
                    Padding = new Thickness(20, 0, 0, 0)
                };

                var marker = list.IsOrdered 
                    ? $"{1 + listItemIndex}. " 
                    : "• ";

                paragraph.Inlines.Add(new Run(marker));

                foreach (var listItemBlock in listItem)
                {
                    if (listItemBlock is ParagraphBlock para)
                    {
                        if (para.Inline != null)
                        {
                            RenderInlines(para.Inline, paragraph.Inlines);
                        }
                    }
                }

                container.Blocks.Add(paragraph);
                listItemIndex++;
            }
        }

        return container;
    }

    private System.Windows.Documents.Block RenderQuote(QuoteBlock quote)
    {
        var section = new Section
        {
            BorderBrush = GetQuoteBorder(),
            BorderThickness = new Thickness(3, 0, 0, 0),
            Padding = new Thickness(10, 0, 0, 0),
            Margin = new Thickness(0, 5, 0, 10)
        };

        RenderBlockElements(quote, section.Blocks);
        return section;
    }

    private System.Windows.Documents.Block RenderCodeBlock(CodeBlock code)
    {
        var paragraph = new Paragraph
        {
            FontFamily = new FontFamily("Consolas"),
            Background = GetCodeBackground(),
            Padding = new Thickness(10),
            Margin = new Thickness(0, 5, 0, 10)
        };

        paragraph.Inlines.Add(new Run(code.Lines.ToString()));
        return paragraph;
    }

    private System.Windows.Documents.Block RenderFencedCodeBlock(FencedCodeBlock code)
    {
        var paragraph = new Paragraph
        {
            FontFamily = new FontFamily("Consolas"),
            Background = GetCodeBackground(),
            Padding = new Thickness(10),
            Margin = new Thickness(0, 5, 0, 10)
        };

        paragraph.Inlines.Add(new Run(code.Lines.ToString()));
        return paragraph;
    }

    private System.Windows.Documents.Block RenderThematicBreak()
    {
        var paragraph = new Paragraph
        {
            Margin = new Thickness(0, 10, 0, 10)
        };

        var border = new Border
        {
            BorderBrush = GetThematicBreakBrush(),
            BorderThickness = new Thickness(0, 1, 0, 0),
            Height = 1
        };

        var container = new InlineUIContainer(border);
        paragraph.Inlines.Add(container);
        return paragraph;
    }

    private void RenderInlines(ContainerInline container, InlineCollection inlines)
    {
        foreach (var inline in container)
        {
            switch (inline)
            {
                case LiteralInline literal:
                    inlines.Add(new Run(literal.Content.ToString()));
                    break;
                case EmphasisInline emphasis:
                    var span = new Span();
                    RenderInlines(emphasis, span.Inlines);
                    
                    if (emphasis.DelimiterCount == 1)
                    {
                        span.FontStyle = FontStyles.Italic;
                    }
                    else if (emphasis.DelimiterCount == 2)
                    {
                        span.FontWeight = FontWeights.Bold;
                    }
                    
                    inlines.Add(span);
                    break;
                case CodeInline codeInline:
                    var codeSpan = new Span
                    {
                        FontFamily = new FontFamily("Consolas"),
                        Background = GetCodeBackground()
                    };
                    codeSpan.Inlines.Add(new Run(codeInline.Content.ToString()));
                    inlines.Add(codeSpan);
                    break;
                case LinkInline link:
                    if (link.IsImage)
                    {
                        try
                        {
                            var image = new System.Windows.Controls.Image
                            {
                                Width = 200,
                                Height = 150,
                                Stretch = Stretch.Uniform
                            };
                            
                            if (!string.IsNullOrEmpty(link.Url))
                            {
                                var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                                bitmap.BeginInit();
                                bitmap.UriSource = new Uri(link.Url, UriKind.RelativeOrAbsolute);
                                bitmap.EndInit();
                                image.Source = bitmap;
                            }
                            
                            var inlineContainer = new InlineUIContainer(image);
                            inlines.Add(inlineContainer);
                        }
                        catch
                        {
                            inlines.Add(new Run($"[Image: {link.Url}]"));
                        }
                    }
                    else
                    {
                        var hyperlink = new Hyperlink();
                        if (!string.IsNullOrEmpty(link.Url))
                        {
                            hyperlink.NavigateUri = new Uri(link.Url, UriKind.RelativeOrAbsolute);
                            hyperlink.RequestNavigate += (s, e) =>
                            {
                                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                            };
                        }
                        RenderInlines(link, hyperlink.Inlines);
                        inlines.Add(hyperlink);
                    }
                    break;
                case LineBreakInline lineBreak:
                    inlines.Add(new LineBreak());
                    break;
            }
        }
    }

    private bool IsSystemDarkTheme()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key == null) return false;
            var value = key.GetValue("AppsUseLightTheme");
            if (value is int intValue)
            {
                return intValue == 0;
            }
        }
        catch
        {
        }
        return false;
    }

    private Brush GetCodeBackground()
    {
        return _isDarkTheme 
            ? new SolidColorBrush(Color.FromRgb(50, 50, 50)) 
            : Brushes.LightGray;
    }

    private Brush GetQuoteBorder()
    {
        return _isDarkTheme 
            ? new SolidColorBrush(Color.FromRgb(100, 100, 100)) 
            : Brushes.Gray;
    }

    private Brush GetThematicBreakBrush()
    {
        return _isDarkTheme 
            ? new SolidColorBrush(Color.FromRgb(80, 80, 80)) 
            : Brushes.Gray;
    }
}
