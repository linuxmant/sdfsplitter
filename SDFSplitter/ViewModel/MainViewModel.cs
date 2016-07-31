using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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
    public class MainViewModel : ViewModelBase
    {
        private Splitter splitter;

        private string inFile = "";
        private string outDir = "";
        private int suffix = 0;
        private string results = "";

        public string InFile
        {
            get { return inFile; }
            set { if (value == inFile) return;
                inFile = value;
                RaisePropertyChanged();
            }
        }
        public string OutDir
        {
            get { return outDir; }
            set { if (value == outDir) return;
                outDir = value;
                RaisePropertyChanged();
            }
        }
        public int Suffix
        {
            get { return suffix; }
            set { if (value == suffix) return;
                suffix = value;
                RaisePropertyChanged();
            }
        }
        public string Results
        {
            get { return results; }
            set { if (value == results) return;
                results = value;
                RaisePropertyChanged();
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
            Results += e.Message;
        }
        private void showError(object sender, string e) {
            Results += e;
        }

        private RelayCommand exitCommand;
        public RelayCommand ExitCommand
        {
            get { return exitCommand ?? (exitCommand = new RelayCommand(() => App.Current.Shutdown(), () => true)); }
        }

        private RelayCommand<object> splitCommand;
        public RelayCommand<object> SplitCommand
        {
            get { return splitCommand ?? (splitCommand = new RelayCommand<object>(Split, CanSplit)); }
        }
        private void Split(object obj) {
            Debug.WriteLine(InFile + "\n" + OutDir + "\n" + Suffix);

            splitter.process(InFile, OutDir, Suffix);
        }
        private bool CanSplit(object obj) {
            return !string.IsNullOrWhiteSpace(InFile) && !string.IsNullOrWhiteSpace(outDir) && !string.IsNullOrWhiteSpace(suffix.ToString());
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
    }
}