using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using PersistHighlighter.Classifier;
using PersistHighlighter.Cmd;
using Task = System.Threading.Tasks.Task;

namespace PersistHighlighter
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PersistHighlighterPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class PersistHighlighterPackage : AsyncPackage
    {
        /// <summary>
        /// PersistHighlighterPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "8a87e89b-1bf0-449f-a2cf-777ba3bdf4f2";

        #region ctor
        public PersistHighlighterPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", new object[]
            {
                this.ToString()
            }));
            //todo Provider;
            hlwPrvder = new HighlightWordTaggerProvider();
        }


        #endregion

        #region field
        HighlightWordTaggerProvider hlwPrvder;
        #endregion

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {

            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", new object[]
            {
                this.ToString()
            }));

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);


#if false
            using (OleMenuCommandService oleMenuCommandService =
        await base.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService)
            {
                if (oleMenuCommandService != null)
                {
                    CommandID cmdhl = new CommandID(GuidList.guidStickyHighlightCmdSet, (int)PkgCmdIDList.cmdidHighlightWord);
                    MenuCommand menucmdhl = new MenuCommand(new EventHandler(this.MenuItemCallback), cmdhl);




                    CommandID cmdclr = new CommandID(GuidList.guidStickyHighlightCmdSet, (int)PkgCmdIDList.cmdidClearHighlights);
                    MenuCommand menucmdclr = new MenuCommand(new EventHandler(this.MenuItemCallback), cmdclr);


                    oleMenuCommandService.AddCommand(menucmdhl);
                    oleMenuCommandService.AddCommand(menucmdclr);
                }
            } 
#endif
            await PersistHighlighter.Cmd.ToggleHighlight.InitializeAsync(this, this.MenuItemCallback);
            await PersistHighlighter.Cmd.ClearHighlight.InitializeAsync(this, this.MenuItemCallback);
        }

        #endregion

        #region business

        private static bool WordExtentIsValid(SnapshotPoint currentRequest, TextExtent word)
        {
            if (word.IsSignificant)
            {
                return currentRequest.Snapshot.GetText(word.Span).Any((char c) => char.IsLetter(c));
            }
            return false;
        }

        public string GetCurrentlyHighlightedWord()
        {
            IVsTextManager vsTextManager = (IVsTextManager)base.GetService(typeof(SVsTextManager));
            IVsTextView vsTextView = null;
            int num = 1;
            vsTextManager.GetActiveView(num, null, out vsTextView);
            IComponentModel componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            IVsEditorAdaptersFactoryService vsEditorAdaptersFactoryService =
                (IVsEditorAdaptersFactoryService)base.GetService(typeof(IVsEditorAdaptersFactoryService));
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
            if (!PersistHighlighterPackage.WordExtentIsValid(point.Value, extentOfWord))
            {
                if (extentOfWord.Span.Start != point.Value || point.Value == point.Value.GetContainingLine().Start || char.IsWhiteSpace((point.Value - 1).GetChar()))
                {
                    flag = false;
                }
                else
                {
                    extentOfWord = textStructureNavigator.GetExtentOfWord(point.Value - 1);
                    if (!PersistHighlighterPackage.WordExtentIsValid(point.Value, extentOfWord))
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


        private void MenuItemCallback(object sender, EventArgs e)
        {
            MenuCommand menuCommand = (MenuCommand)sender;
            switch (menuCommand.CommandID.ID)
            {
                case ToggleHighlight.CommandId:
                    {
                        string currentlyHighlightedWord = this.GetCurrentlyHighlightedWord();
                        if (currentlyHighlightedWord != null)
                        {
                            HighlightedWordCollection.Instance.ToggleHighlightWord(currentlyHighlightedWord);
                            return;
                        }
                        break;
                    }
                case ClearHighlight.CommandId:
                    HighlightedWordCollection.Instance.ClearHighlightedWords();
                    break;
                default:
                    return;
            }
        }
        #endregion
    }
}
