using Eto.Drawing;
using Eto.Forms;

namespace NAPS2.EtoForms.Layout;

public abstract class LayoutElement
{
    internal const bool DEBUG_LAYOUT = false;

    protected static LayoutElement[] ExpandChildren(LayoutElement[] children)
    {
        return children.SelectMany(x => x is ExpandLayoutElement expand ? expand.Children : new[] { x })
            .Where(c => c is not SkipLayoutElement).ToArray();
    }

    protected internal bool Scale { get; set; }
    protected internal LayoutAlignment Alignment { get; set; }
    protected internal LayoutVisibility? Visibility { get; set; }
    protected internal bool IsVisible => Visibility?.IsVisible ?? true;
    protected internal int? SpacingAfter { get; set; }

    public static implicit operator LayoutElement(Control control) =>
        new LayoutControl(control);

    public abstract void DoLayout(LayoutContext context, RectangleF bounds);

    public abstract SizeF GetPreferredSize(LayoutContext context, RectangleF parentBounds);
}