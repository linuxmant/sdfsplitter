using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace SDFSplitter {
    class Splitter : INotifyPropertyChanged {
		private string sdfFilePath;
		private string molPath;
		private TextBlock results;
        private int suffix;
        
        public int Suffix { get { return suffix; }
            set { suffix = value; }
        }

		public event PropertyChangedEventHandler PropertyChanged;

		public TextBlock Results {
			get { return results; }
			set {
				results = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("results"));
            }
		}

		public Splitter(string inFile, string outPath, int suffix, TextBlock monitor) {
			this.sdfFilePath = inFile;
			this.molPath = outPath;
			this.Results = monitor;
            Suffix = suffix;
		}

		public bool process() {
			bool res = false;
			Debug.WriteLine(" ...file " + sdfFilePath);
			Results.Text += "\n ...file " + sdfFilePath;
			try {
				if (!Directory.Exists(molPath)) {
					Directory.CreateDirectory(molPath);
					Debug.WriteLine("Created directory " + molPath);
				}

				StringBuilder strBuild = new StringBuilder();

                var lineNo = 1;
				foreach (string line in File.ReadAllLines(sdfFilePath)) {
                    if (!line.Equals("$$$$")) {
						strBuild.AppendLine(line);
                        if (strBuild.ToString().Contains("  0  0  0  0  0  0  0  0  0  0  1 V2000\r\nM  END")) {
                            Results.Text += "\nInvalid molecule [Block #" + Suffix++ + " -- Line #" + lineNo + "]";
                            strBuild.Clear();
                            lineNo++;
                            continue;
                        }

                    } else {
                        strBuild.AppendLine(line);

                        this.saveMol(("file_" + string.Format("{0:d7}", Suffix++)), strBuild.ToString());
						strBuild.Clear();
					}
                    lineNo++;
				}
			} catch (Exception e) {
				Debug.WriteLine("The file could not be read:");
                Debug.WriteLine(e.Message);
				Results.Text = "The file could not be read:\n";
				Results.Text += e.Message;
			}

			return res;
		}

		private void saveMol(string name, string data) {
			name += ".mol";
			try {
				using(StreamWriter sw = new StreamWriter(molPath + "\\" + name)) {
					sw.Write(data);
					sw.Flush();
					Results.Text += "\nSaved file " + name;
				}
			} catch (Exception e) {
                Debug.WriteLine("The file " + name + " could not be saved:");
                Debug.WriteLine(e.Message);
				Results.Text = "The file " + name + " could not be saved:\n";
				Results.Text += e.Message;
			}
		}
	}
}
