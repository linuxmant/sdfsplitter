using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SDFSplitter {
	class Splitter:INotifyPropertyChanged {
		private String sdfFilePath;
		private String molPath;
		private TextBlock _results;

		public event PropertyChangedEventHandler PropertyChanged;

		public TextBlock results {
			get { return _results; }
			set {
				_results = value;

				if (PropertyChanged != null) {
					PropertyChanged(this, new PropertyChangedEventArgs("results"));
				}
			}
		}

		public Splitter(String inFile, String outPath, TextBlock monitor) {
			this.sdfFilePath = inFile;
			this.molPath = outPath;
			this.results = monitor;
		}

		public bool process() {
			bool res = false;
			Console.WriteLine(" ...file " + sdfFilePath);
			results.Text += "\n ...file " + sdfFilePath;
			try {
				if (!Directory.Exists(molPath)) {
					Directory.CreateDirectory(molPath);
					Console.WriteLine("Created directory " + molPath);
				}

				StringBuilder strBuild = new StringBuilder();
				int fileCtr = 0;
				foreach (String line in File.ReadAllLines(sdfFilePath)) {
					if (!line.Equals("$$$$")) {
						strBuild.AppendLine(line);
					} else {
						strBuild.AppendLine(line);
						this.saveMol(("file" + fileCtr++), strBuild.ToString());
						strBuild.Clear();
					}
				}
			} catch (Exception e) {
				Console.WriteLine("The file could not be read:");
				Console.WriteLine(e.Message);
				results.Text = "The file could not be read:\n";
				results.Text += e.Message;
			}

			return res;
		}

		private void saveMol(String name, String data) {
			name += ".mol";
			try {
				using(StreamWriter sw = new StreamWriter(molPath + "\\" + name)) {
					sw.Write(data);
					sw.Flush();
					results.Text += "\nSaved file " + name;
				}
			} catch (Exception e) {
				Console.WriteLine("The file " + name + " could not be saved:");
				Console.WriteLine(e.Message);
				results.Text = "The file " + name + " could not be saved:\n";
				results.Text += e.Message;
			}
		}
	}
}
