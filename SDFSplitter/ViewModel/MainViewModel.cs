using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace SDFSplitter.ViewModel {
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase {
        private Splitter splitter;
        private bool isSplitting = false;

        private string inFile = "";
        private string outDir = "";
        private int suffix = 0;
        private string results = "";

        public string InFile
        {
            get { return inFile; }
            set
            {
                if (value == inFile) return;
                inFile = value;
                RaisePropertyChanged();
            }
        }
        public string OutDir
        {
            get { return outDir; }
            set
            {
                if (value == outDir) return;
                outDir = value;
                RaisePropertyChanged();
            }
        }
        public int Suffix
        {
            get { return suffix; }
            set
            {
                if (value == suffix) return;
                suffix = value;
                RaisePropertyChanged();
            }
        }
        public string Results
        {
            get { return results; }
            set
            {
                if (value == results) return;
                results = value;
                RaisePropertyChanged();
            }
        }
        private bool IsSplitting {
            get { return isSplitting; }
            set { if (value == isSplitting) return;
                isSplitting = value;
                ExitCommand.RaiseCanExecuteChanged();
                SplitCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel() {
            splitter = new Splitter();
            splitter.FileSaved += showProgress;
            splitter.InvalidMolecule += showError;
        }

        private void showProgress(object sender, FileProcessingEventArgs e) {
            Results += e.Message + "\n";
        }
        private void showError(object sender, string e) {
            Results += e + "\n";
        }

        private RelayCommand exitCommand;
        public RelayCommand ExitCommand
        {
            get { return exitCommand ?? (exitCommand = new RelayCommand(() => App.Current.Shutdown(), () => !this.isSplitting)); }
        }

        private RelayCommand<object> splitCommand;
        public RelayCommand<object> SplitCommand
        {
            get { return splitCommand ?? (splitCommand = new RelayCommand<object>(Split, CanSplit)); }
        }
        private void Split(object obj) {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += splitter_DoWork;
            bg.RunWorkerCompleted += splitter_Complete;
            splitter.bgWorker = bg;

            IsSplitting = true;
            bg.RunWorkerAsync(new SplitterArgs() { infile = InFile, outdir = OutDir, suff = Suffix });
        }

        public void splitter_Complete(object sender, RunWorkerCompletedEventArgs e) {
            IsSplitting = false;
        }

        private void splitter_DoWork(object sender, DoWorkEventArgs e) {
            var args = (SplitterArgs)e.Argument;
            splitter.process(args.infile, args.outdir, args.suff);
        }
        private bool CanSplit(object obj) {
            return !IsSplitting && 
                !string.IsNullOrWhiteSpace(InFile) && 
                !string.IsNullOrWhiteSpace(outDir) && 
                !string.IsNullOrWhiteSpace(suffix.ToString());
        }

        private RelayCommand<Window> windowLoaded;
        public RelayCommand<Window> WindowLoaded
        {
            get { return windowLoaded ?? (windowLoaded = new RelayCommand<Window>(InitWindow, wnd => true)); }
        }
        private void InitWindow(Window wnd) {
            Suffix = 0;
            SplitCommand.RaiseCanExecuteChanged();
        }

        private RelayCommand<object> selectInFile;
        public RelayCommand<object> SelectInFile
        {
            get { return selectInFile ?? (selectInFile = new RelayCommand<object>(ShowFileDialog, obj => true)); }
        }
        private void ShowFileDialog(object sender) {
            Microsoft.Win32.OpenFileDialog openDlg = new Microsoft.Win32.OpenFileDialog();

            openDlg.DefaultExt = ".sdf";
            openDlg.Filter = "SD File (.sdf)|*.sdf";

            if (openDlg.ShowDialog() == true) {
                InFile = openDlg.FileName;

                OutDir = InFile.Substring(0, InFile.LastIndexOf("\\")) + "\\molFiles";
            }
        }

        private class SplitterArgs {
            public string infile { get; set; }
            public string outdir { get; set; }
            public int suff { get; set; }
        }
    }
}