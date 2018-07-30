#region License
// Copyright (c) 2016-2018 Cisco Systems, Inc.

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
namespace KitchenSink
{
    class FeedBackViewModel : ViewModelBase
    {
        private List<string> lstFiles;
        public RelayCommand SendCMD { get; set; }
        public RelayCommand AttachFileCMD { get; set; }

        private string selectedSubject = "UI";
        public string SelectedSubject
        {
            get { return selectedSubject; }
            set
            {
                if (value != this.selectedSubject)
                {
                    this.selectedSubject = value;
                    OnPropertyChanged("SelectedSubject");
                }

            }
        }

        private string strFilesAttached = "0 file(s) attached";
        public string FilesAttached
        {
            get
            {
                return this.strFilesAttached;
            }
            set
            {
                if (value != strFilesAttached)
                {
                    this.strFilesAttached = value;
                    OnPropertyChanged("FilesAttached");
                }
            }
        }

        public string Recipient
        {
            get
            {
                return "devsupport@ciscospark.com";
            }
        }

        private string comment = string.Empty;

        public string Comment
        {
            get
            {
                return this.comment;
            }
            set
            {
                if (value != this.comment)
                {
                    this.comment = value;
                    OnPropertyChanged("Comment");
                }
            }
        }

        public FeedBackViewModel()
        {
            lstFiles = new List<string>();
            SendCMD = new RelayCommand(SendMail, CanSendMail);
            AttachFileCMD = new RelayCommand(AttachFile);
        }


        private void AttachFile(object o)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == true)
            {
                lstFiles.AddRange(ofd.FileNames);
                this.FilesAttached = string.Format("{0} file(s) attached", lstFiles.Count);
            }
        }

        private void SendMail(object o)
        { 
            MAPI mapi = new MAPI();
            mapi.AddRecipientTo(this.Recipient);
            foreach(string path in this.lstFiles)
            {
                mapi.AddAttachment(path);
            }
          
            int errorCount=mapi.SendMailPopup(this.SelectedSubject, this.Comment);
            //sometimes mapi could not work, we have to fallback to mailto way
            if (errorCount > 0)
            {
                Process.Start(string.Format("mailto:{0}?subject={1}&body={2}",this.Recipient,this.SelectedSubject,this.Comment));
            }
        }

        private bool CanSendMail(object o)
        {
            return this.lstFiles.Count > 0 || this.comment.Length > 0;
        }
    }
}
