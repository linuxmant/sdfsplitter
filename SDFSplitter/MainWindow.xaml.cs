using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SDFSplitter {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow :Window {

		private Splitter splitter;

		public MainWindow() {
			InitializeComponent();
			this.Loaded += new RoutedEventHandler(MainWindow_Loaded);

			this.DataContext = splitter;
		}

		void MainWindow_Loaded(object sender, RoutedEventArgs e) {
			sdfFilePath.Text = "Click to select sdf file...";
			sdfFilePath.Foreground = Brushes.Silver;
		}

		private void exitBtn_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void TextBox_MouseUp(object sender, MouseButtonEventArgs e) {
			resultsTbx.Text = "";
			
			Microsoft.Win32.OpenFileDialog openDlg = new Microsoft.Win32.OpenFileDialog();

			openDlg.DefaultExt = ".sdf";
			openDlg.Filter = "SD File (.sdf)|*.sdf";

			Nullable<bool> result = openDlg.ShowDialog();

			if (result == true) {
				TextBox path = (TextBox) (((FrameworkElement) sender).Parent as FrameworkElement).FindName("sdfFilePath");
				path.Text = openDlg.FileName;
				path.Foreground = Brushes.Black;
				path.Focus();

				molPath.Text = path.Text.Substring(0, path.Text.LastIndexOf("\\")) + "\\molFiles";

				splitBtn.IsEnabled = true;
				splitBtn.Focus();
			}
		}

		private void splitBtn_Click(object sender, RoutedEventArgs e) {
			splitter = new Splitter(sdfFilePath.Text, molPath.Text, resultsTbx);
			resultsTbx.Text = "Processing...";
			resultsTbx.BringIntoView();
			splitter.process();
		}

		private void sdfFilePath_MouseEnter(object sender, MouseEventArgs e) {
			if (sdfFilePath.Text.Equals("Click to select sdf file...")) {
				sdfFilePath.Text = "";
			}
		}

		private void sdfFilePath_MouseLeave(object sender, MouseEventArgs e) {
			if (sdfFilePath.Text.Equals("")) {
				sdfFilePath.Foreground = Brushes.Silver;
				sdfFilePath.Text = "Click to select sdf file...";
			} else {
				sdfFilePath.Foreground = Brushes.Black;
			}
		}
	}
}
