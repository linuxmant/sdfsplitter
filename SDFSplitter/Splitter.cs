using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SDFSplitter {
    public class Splitter : ViewModelBase {
        private BackgroundWorker bgWorker;

        public Splitter(BackgroundWorker bgWorker) {
            this.bgWorker = bgWorker;
        }

        public Splitter() { }

        public string process(string sdfFilePath, string molPath, int suffix) {
            var result = "";

            if (!Directory.Exists(molPath)) {
                Directory.CreateDirectory(molPath);
                Debug.WriteLine("Created directory " + molPath);
            } else {
                Directory.Delete(molPath, true);
                Directory.CreateDirectory(molPath);
            }

            StringBuilder strBuild = new StringBuilder();

            var data = File.ReadAllLines(sdfFilePath);
            var mols = new List<string>();

            try {
                foreach (string line in data) {
                    if (!line.Equals("$$$$")) {
                        strBuild.AppendLine(line);
                    } else {
                        strBuild.AppendLine(line);
                        mols.Add(strBuild.ToString());
                        strBuild.Clear();
                    }
                }

                int idx = 0;
                mols.ForEach(mol => {
                    if (mol.Contains("  0  0  0  0  0  0  0  0  0  0  1 V2000\r\nM  END")) {
                        OnInvalidMolecule("\nInvalid molecule [Block #" + idx + "]");
                    } else {
                        saveMol(("file_" + string.Format("{0:d7}", suffix++)), mol, molPath);
                    }
                    idx++;
                });
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                result += ex.Message;
            }

            return result;
        }

        private void saveMol(string name, string data, string molPath) {
            name += ".mol";

            var fileProcessArgs = new FileProcessingEventArgs();

            try {
                using (StreamWriter sw = new StreamWriter(molPath + "\\" + name)) {
                    sw.Write(data);
                    sw.Flush();
                    fileProcessArgs.Status = FileSaveStatus.Success;
                    fileProcessArgs.Message = "\nSaved file " + name;
                    //OnFileSaved(fileProcessArgs);
                }
            } catch (Exception e) {
                Debug.WriteLine("The file " + name + " could not be saved:");
                Debug.WriteLine(e.Message);
                fileProcessArgs.Status = FileSaveStatus.Failure;
                fileProcessArgs.Message = "The file " + name + " could not be saved:\n" + e.Message;
                //OnFileSaved(fileProcessArgs);
            }

            OnFileSaved(fileProcessArgs);
        }

        public EventHandler<FileProcessingEventArgs> FileSaved;
        public EventHandler<string> InvalidMolecule;
        protected virtual void OnFileSaved(FileProcessingEventArgs e) {
            FileSaved?.Invoke(this, e);
        }
        protected virtual void OnInvalidMolecule(string e) {
            InvalidMolecule?.Invoke(this, e);
        }
    }

    public enum FileSaveStatus : byte { Failure, Success }

    public class FileProcessingEventArgs {
        public FileSaveStatus Status { get; set; }
        public string Message { get; set; }
    }
}
