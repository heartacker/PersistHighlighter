using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using StickyHighlighterClassifier;

namespace Heartacker.StickyHighlighter
{
    // Token: 0x02000002 RID: 2
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid("23323416-516f-4683-9365-ab02892c8a90")]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    public sealed class StickyHighlighterPackage : Package
    {
        // Token: 0x06000001 RID: 1 RVA: 0x000020D0 File Offset: 0x000002D0
        public StickyHighlighterPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", new object[]
            {
                this.ToString()
            }));
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002108 File Offset: 0x00000308
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", new object[]
            {
                this.ToString()
            }));
            base.Initialize();
            OleMenuCommandService oleMenuCommandService = base.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (oleMenuCommandService != null)
            {
                CommandID command = new CommandID(GuidList.guidStickyHighlightCmdSet, 256);
                MenuCommand command2 = new MenuCommand(new EventHandler(this.MenuItemCallback), command);
                oleMenuCommandService.AddCommand(command2);
                command = new CommandID(GuidList.guidStickyHighlightCmdSet, 257);
                command2 = new MenuCommand(new EventHandler(this.MenuItemCallback), command);
                oleMenuCommandService.AddCommand(command2);
            }
        }

        // Token: 0x06000003 RID: 3 RVA: 0x000021B8 File Offset: 0x000003B8
        private static bool WordExtentIsValid(SnapshotPoint currentRequest, TextExtent word)
        {
            if (word.IsSignificant)
            {
                return currentRequest.Snapshot.GetText(word.Span).Any((char c) => char.IsLetter(c));
            }
            return false;
        }

        // Token: 0x06000004 RID: 4 RVA: 0x0000220C File Offset: 0x0000040C
        public string GetCurrentlyHighlightedWord()
        {
            IVsTextManager vsTextManager = (IVsTextManager)base.GetService(typeof(SVsTextManager));
            IVsTextView vsTextView = null;
            int num = 1;
            vsTextManager.GetActiveView(num, null, ref vsTextView);
            IComponentModel componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            IVsEditorAdaptersFactoryService vsEditorAdaptersFactoryService = (IVsEditorAdaptersFactoryService)base.GetService(typeof(IVsEditorAdaptersFactoryService));
            IVsEditorAdaptersFactoryService service = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            ITextView wpfTextView = service.GetWpfTextView(vsTextView);
            ITextBuffer textBuffer = wpfTextView.TextBuffer;
            SnapshotPoint? point = wpfTextView.Caret.Position.Point.GetPoint(textBuffer, wpfTextView.Caret.Position.Affinity);
            if (point == null)
            {
                return null;
            }
            ITextStructureNavigatorSelectorService service2 = componentModel.GetService<ITextStructureNavigatorSelectorService>();
            ITextStructureNavigator textStructureNavigator = service2.GetTextStructureNavigator(textBuffer);
            TextExtent extentOfWord = textStructureNavigator.GetExtentOfWord(point.Value);
            bool flag = true;
            if (!StickyHighlighterPackage.WordExtentIsValid(point.Value, extentOfWord))
            {
                if (extentOfWord.Span.Start != point.Value || point.Value == point.Value.GetContainingLine().Start || char.IsWhiteSpace((point.Value - 1).GetChar()))
                {
                    flag = false;
                }
                else
                {
                    extentOfWord = textStructureNavigator.GetExtentOfWord(point.Value - 1);
                    if (!StickyHighlighterPackage.WordExtentIsValid(point.Value, extentOfWord))
                    {
                        flag = false;
                    }
                }
            }
            if (!flag)
            {
                return null;
            }
            return extentOfWord.Span.GetText();
        }

        // Token: 0x06000005 RID: 5 RVA: 0x000023A0 File Offset: 0x000005A0
        private void MenuItemCallback(object sender, EventArgs e)
        {
            MenuCommand menuCommand = (MenuCommand)sender;
            switch (menuCommand.CommandID.ID)
            {
                case 256:
                    {
                        string currentlyHighlightedWord = this.GetCurrentlyHighlightedWord();
                        if (currentlyHighlightedWord != null)
                        {
                            HighlightedWordCollection.Instance.ToggleHighlightWord(currentlyHighlightedWord);
                            return;
                        }
                        break;
                    }
                case 257:
                    HighlightedWordCollection.Instance.ClearHighlightedWords();
                    break;
                default:
                    return;
            }
        }
    }
}
