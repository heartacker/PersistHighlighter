using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;

namespace StickyHighlighterClassifier
{
	// Token: 0x02000009 RID: 9
	internal class StickyHighlightWordTagger : ITagger<TextMarkerTag>
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002220 File Offset: 0x00000420
		// (set) Token: 0x06000009 RID: 9 RVA: 0x00002228 File Offset: 0x00000428
		private ITextView View { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002231 File Offset: 0x00000431
		// (set) Token: 0x0600000B RID: 11 RVA: 0x00002239 File Offset: 0x00000439
		private ITextBuffer SourceBuffer { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002242 File Offset: 0x00000442
		// (set) Token: 0x0600000D RID: 13 RVA: 0x0000224A File Offset: 0x0000044A
		private ITextSearchService TextSearchService { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002253 File Offset: 0x00000453
		// (set) Token: 0x0600000F RID: 15 RVA: 0x0000225B File Offset: 0x0000045B
		private ITextStructureNavigator TextStructureNavigator { get; set; }

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000010 RID: 16 RVA: 0x00002264 File Offset: 0x00000464
		// (remove) Token: 0x06000011 RID: 17 RVA: 0x0000229C File Offset: 0x0000049C
		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

		// Token: 0x06000012 RID: 18 RVA: 0x000022D4 File Offset: 0x000004D4
		public StickyHighlightWordTagger(ITextView view, ITextBuffer sourceBuffer, ITextSearchService textSearchService, ITextStructureNavigator textStructureNavigator)
		{
			this.View = view;
			this.SourceBuffer = sourceBuffer;
			this.TextSearchService = textSearchService;
			this.TextStructureNavigator = textStructureNavigator;
			this.View.LayoutChanged += this.ViewLayoutChanged;
			this.WordSpanMap = new Dictionary<string, NormalizedSnapshotSpanCollection>();
			HighlightedWordCollection.Instance.Changed += this.HighlightWordAdded;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002347 File Offset: 0x00000547
		private void HighlightWordAdded(object sender, EventArgs e)
		{
			this.UpdateWordAdornments();
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002350 File Offset: 0x00000550
		private TextMarkerTag GetHighlightDefinition(int nIndex)
		{
			switch (nIndex % 3)
			{
			case 0:
				return new StickyHighlightWordTag1();
			case 1:
				return new StickyHighlightWordTag2();
			case 2:
				return new StickyHighlightWordTag3();
			default:
				return new StickyHighlightWordTag1();
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000238C File Offset: 0x0000058C
		private void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
		{
			if (e.NewSnapshot != e.OldSnapshot)
			{
				this.UpdateWordAdornments();
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000023A4 File Offset: 0x000005A4
		private void UpdateWordAdornments()
		{
			HighlightedWordCollection instance = HighlightedWordCollection.Instance;
			foreach (string word in HighlightedWordCollection.Instance.HighlightedWords)
			{
				this.UpdateSpansForWord(word);
			}
			this.TriggerTagChange();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002408 File Offset: 0x00000608
		public void UpdateSpansForWord(string Word)
		{
			List<SnapshotSpan> list = new List<SnapshotSpan>();
			FindData findData;
			findData..ctor(Word, this.View.TextSnapshot);
			findData.FindOptions = 5;
			list.AddRange(this.TextSearchService.FindAll(findData));
			NormalizedSnapshotSpanCollection value = new NormalizedSnapshotSpanCollection(list);
			this.WordSpanMap[Word] = value;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000245C File Offset: 0x0000065C
		private void TriggerTagChange()
		{
			lock (this.updateLock)
			{
				EventHandler<SnapshotSpanEventArgs> tagsChanged = this.TagsChanged;
				if (tagsChanged != null)
				{
					tagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(this.SourceBuffer.CurrentSnapshot, 0, this.SourceBuffer.CurrentSnapshot.Length)));
				}
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002808 File Offset: 0x00000A08
		public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			for (int i = 0; i < HighlightedWordCollection.Instance.HighlightedWords.Count; i++)
			{
				if (HighlightedWordCollection.Instance.HighlightedWords[i] != null)
				{
					string HighlightWord = HighlightedWordCollection.Instance.HighlightedWords[i];
					NormalizedSnapshotSpanCollection wordSpans = this.WordSpanMap[HighlightWord];
					if (spans.Count != 0 && wordSpans.Count != 0)
					{
						if (spans[0].Snapshot != wordSpans[0].Snapshot)
						{
							wordSpans = new NormalizedSnapshotSpanCollection(from span in wordSpans
							select span.TranslateTo(spans[0].Snapshot, 0));
						}
						foreach (SnapshotSpan span2 in NormalizedSnapshotSpanCollection.Overlap(spans, wordSpans))
						{
							yield return new TagSpan<TextMarkerTag>(span2, this.GetHighlightDefinition(i));
						}
					}
				}
			}
			yield break;
		}

		// Token: 0x04000001 RID: 1
		private Dictionary<string, NormalizedSnapshotSpanCollection> WordSpanMap;

		// Token: 0x04000002 RID: 2
		private object updateLock = new object();
	}
}
