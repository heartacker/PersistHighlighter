using System;
using Microsoft.VisualStudio.Text.Classification;

namespace StickyHighlighterClassifier
{
    // Token: 0x02000005 RID: 5
    internal class BaseStickyHighlightWordFormatDefinition : MarkerFormatDefinition
    {
        // Token: 0x06000004 RID: 4 RVA: 0x000020F7 File Offset: 0x000002F7
        public BaseStickyHighlightWordFormatDefinition()
        {
            base.BackgroundCustomizable = new bool?(true);
            base.ForegroundCustomizable = new bool?(true);
            base.ZOrder = 5;
        }
    }
}
