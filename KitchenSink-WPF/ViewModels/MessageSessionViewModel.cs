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

using WebexSDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Drawing.Imaging;
using System.Reflection;
using System.Collections;

namespace KitchenSink
{
    class MessageSessionViewModel : ViewModelBase
    {
        public RelayCommand BackCommand { get; set; }
        public RelayCommand SndMsgCommand { get; set; }
        public RelayCommand AttachFileCMD { get; set; }
        public RelayCommand DeleteMsgCommand { get; set; }
        public RelayCommand DownloadFileCMD { get; set; }
        public RelayCommand DownloadThumbnailCMD { get; set; }
        public RelayCommand ListMessageCMD { get; set; }
        public RelayCommand GetMessageCMD { get; set; }



        readonly WebexSDK.Webex webex;
        readonly WebexSDK.MessageClient messageClient;

        private readonly bool isDirectMessage = false;
        private readonly string toPersonEmail=null;
        private readonly string toSpaceId = null;

        private string messageText=null;
        public string MessageText
        {
            get
            {
                return this.messageText;
            }
            set
            {
                this.messageText = value;
                OnPropertyChanged("MessageText");
            }
        }

        public ObservableCollection<string> AttachedFiles { get; set; }
        public TimelineMessage SelectedMessage { get; set; }
        
        private string selectedFileIndex;
        public string SelectedFileIndex
        {
            get
            {
                return this.selectedFileIndex;
            }
            set
            {
                this.selectedFileIndex = value;
            }
        }

        private ObservableCollection<Membership> membershipList = new ObservableCollection<Membership>();
        public ObservableCollection<Membership> MembershipList
        {
            get
            {
                return this.membershipList;
            }
            set
            {
                this.membershipList = value;
            }
        }

        private Membership selectedMembership;
        public Membership SelectedMembership
        {
            get
            {
                return this.selectedMembership;
            }
            set
            {
                this.selectedMembership = value;
                if (this.selectedMembership != null)
                {
                    if (this.SelectedMembership.PersonDisplayName == "ALL")
                    {
                        Mentions.Add(new MentionAll());
                    }
                    else
                    {
                        Mentions.Add(new MentionPerson(SelectedMembership.PersonId));
                    }
                    
                    MessageText += "<spark-mention>" + SelectedMembership.PersonDisplayName + "</spark-mention>";
                }
            }
        }

        private List<Mention> Mentions = new List<Mention>();

        public void FetchMemberships()
        {
            if (toSpaceId == null)
            {
                return;
            }

            webex?.Memberships.List(toSpaceId, null, r =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MembershipList.Clear();

                    var all = new Membership()
                    {
                        PersonDisplayName = "ALL"
                    };
                    MembershipList.Add(all);

                    if (r.IsSuccess)
                    {
                        foreach (var i in r.Data)
                            MembershipList.Add(i);
                    }
                });
            });
        }

        private string payLoad;
        public string PayLoad
        {
            get
            {
                return this.payLoad;
            }
            set
            {
                this.payLoad = value;
                OnPropertyChanged("PayLoad");
            }
        }

        private string spaceTitle;
        public string SpaceTitle
        {
            get
            {
                return this.spaceTitle;
            }
            set
            {
                this.spaceTitle = value;
                OnPropertyChanged("SpaceTitle");
            }
        }

        public MessageSessionViewModel()
        {
            BackCommand = new RelayCommand(BackToMain);
            SndMsgCommand = new RelayCommand(SendMessage);
            DownloadFileCMD = new RelayCommand(DownLoadFile);
            DownloadThumbnailCMD = new RelayCommand(DownloadThumbnail);
            AttachFileCMD = new RelayCommand(AttachFile);
            DeleteMsgCommand = new RelayCommand(DeleteMsg);
            ListMessageCMD = new RelayCommand(ListMessage);
            GetMessageCMD = new RelayCommand(GetMessage);

            webex = ApplicationController.Instance.CurWebexManager.CurWebex;
            messageClient = webex?.Messages;
            messageClient.OnEvent += OnMessageEvent;
            MessageList = new ObservableCollection<TimelineMessage>();
            AttachedFiles = new ObservableCollection<string>();

            FilesList = new ObservableCollection<RemoteFile>();

            var address = ApplicationController.Instance.CurWebexManager.CurCalleeAddress;
            if (address.Contains('@') || StringExtention.Base64UrlDecode(address).Contains("PEOPLE"))
            {
                SpaceTitle = address;
                isDirectMessage = true;
                toPersonEmail = address;
            }
            else
            {
                webex?.Spaces.Get(address, r =>
                {
                    if (r.IsSuccess)
                    {
                        SpaceTitle = r.Data.Title;
                    }
                });
                toSpaceId = address;
            }
        }

        private ObservableCollection<TimelineMessage> messageList;
        public ObservableCollection<TimelineMessage> MessageList
        {
            get
            {
                return this.messageList;
            }
            set
            {
                this.messageList = value;
                OnPropertyChanged("MessageList");
            }
        }

        public ObservableCollection<RemoteFile> FilesList;

        private void DownloadThumbnail(object o)
        {
            if (SelectedMessage == null || SelectedMessage.MessageInfo == null 
                ||SelectedMessage.MessageInfo.Files == null || SelectedMessage.MessageInfo.Files.Count == 0)
            {
                return;
            }
            foreach (var item in SelectedMessage.MessageInfo.Files)
            {
                if (item.RemoteThumbnail != null)
                {
                    DownloadThumbnail(SelectedMessage.MessageInfo, item, SelectedMessage.MessageInfo.Files.IndexOf(item));
                }
            }
        }
        private void DownloadThumbnail(Message message, RemoteFile file, int index)
        {
            int contentIndex = index;
            webex?.Messages.DownloadThumbnail(file, null, r =>
            {
                if (r.IsSuccess)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var m = MessageList.Where(x => x.MessageInfo.Id == message.Id).First();
                        int i = MessageList.IndexOf(m);
                        m.Files[contentIndex].ThumbnailPath = r.Data;
                        MessageList.Remove(m);
                        MessageList.Insert(i, m);
                    });
                }
            });
        }

        private void OnMessageEvent(MessageEvent e)
        {
            if (e is MessageArrived)
            {
                Output("received a message.");
                var messageArrived = e as MessageArrived;
                var msgInfo = messageArrived?.Message;
                if (msgInfo != null)
                {
                    // self is mentioned
                    if (msgInfo.IsSelfMentioned)
                    {
                        Output($"{msgInfo.PersonEmail} mentioned you.");
                    }

                    var timelineMessage = BuildTimelineMessage(msgInfo);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageList.Add(timelineMessage);
                    });

                    if (msgInfo.Files != null && msgInfo.Files.Count > 0)
                    {
                        int index = 0;
                        foreach (var file in msgInfo.Files)
                        {
                            if (file.RemoteThumbnail != null)
                            {
                                DownloadThumbnail(msgInfo, file, index);
                            }
                            index++;
                        }
                    }

                    PrintPayload(msgInfo);
                    if (msgInfo.SpaceType == SpaceType.Direct)
                    {
                        UpdateRecentContactsStore(msgInfo.PersonId);
                    }
                }
            }
            else if (e is MessageDeleted)
            {
                var messageDeleted = e as MessageDeleted;
                Output("a message is deleted");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var a = MessageList.Where(x => x.MessageInfo.Id == messageDeleted.MessageId)?.First();
                        if (a != null)
                        {
                            MessageList.Remove(a);
                        }
                    }
                    catch
                    {

                    }
                    

                });
            }
        }        

        private void BackToMain(object o)
        {
            ApplicationController.Instance.CurCallView = null;
            ApplicationController.Instance.ChangeState(State.Main);
        }

        private void Output(String format, params object[] args)
        {
            ApplicationController.Instance.AppLogOutput(format, args);
        }

        private bool IsImage(string fileName)
        {
            System.IO.FileStream fs = null;
            System.IO.BinaryReader r = null;
            try
            {
                fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                r = new System.IO.BinaryReader(fs);

            }
            catch(Exception e)
            {
                Output($"{fileName}: {e.Message}");
                return false;
            }
            string fileclass = "";
            byte buffer;
            try
            {
                buffer = r.ReadByte();
                fileclass = buffer.ToString();
                buffer = r.ReadByte();
                fileclass += buffer.ToString();

            }
            catch
            {
            }
            r.Close();
            fs.Close();
            if (fileclass == "255216" || fileclass == "7173" || fileclass == "6677" || fileclass == "13780")//255216[jpg];7173[gif];6677[BMP],13780[PNG];
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //private string GetMimeTypeOfImage(System.Drawing.Image image)
        //{
        //    string mimeType = @"image/unknown";
        //    ImageFormat format = image.RawFormat;
        //    foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
        //    {
        //        if (codec.FormatID == image.RawFormat.Guid)
        //        {
        //            mimeType = codec.MimeType;
        //            break;
        //        }

        //    }
        //    return mimeType;
        //}

        public void SendMessage(object o)
        {
            var localFiles = BuildLocalFiles();
            var sendMessage = ConstructSendMessage(MessageText, localFiles);
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageList.Add(sendMessage);
            });

            if (isDirectMessage)
            {
                webex?.Messages.PostToPerson(toPersonEmail, MessageText, localFiles, r =>
                {
                    if (r.IsSuccess)
                    {
                        MessageSet(sendMessage, r.Data);
                        PrintPayload(r.Data);
                        Output($"send a message successfully.");
                        UpdateRecentContactsStore(r.Data.ToPersonId);
                    }
                    else
                    {
                        Output($"send the message failed. {r.Error.ErrorCode} {r.Error.Reason}");
                    }
                });
            }
            else
            {
                webex?.Messages.PostToSpace(toSpaceId, MessageText, Mentions, localFiles, r =>
                {
                    if (r.IsSuccess)
                    {
                        MessageSet(sendMessage, r.Data);
                        PrintPayload(r.Data);
                        Output($"send a message successfullly.");
                    }
                    else
                    {
                        Output($"send the message failed. {r.Error.ErrorCode} {r.Error.Reason}");
                    }
                });
            }

            MessageText = null;
            SelectedMembership = null;
            Mentions = new List<Mention>();
        }
        List<LocalFile>  BuildLocalFiles()
        {
            Image image = null;
            List<LocalFile> files = new List<LocalFile>();

            if (AttachedFiles != null && AttachedFiles.Count > 0)
            {
                foreach (var item in AttachedFiles)
                {
                    var file = new LocalFile
                    {
                        Path = item,
                        Name = Path.GetFileName(item),
                        Size = (ulong)new System.IO.FileInfo(item).Length,
                        Mime = Mime.GetMimeType(Path.GetExtension(item))
                    };
                    file.UploadProgressHandler = (r) =>
                    {
                        if (r.IsSuccess)
                        {
                            Output($"{file.Name} is uploading {r.Data}%");
                        }
                    };

                    if (IsImage(item))
                    {
                        // Get thumbnail
                        image = Image.FromFile(item);
                        var thumbnail = image?.GetThumbnailImage(image.Width, image.Height, null, System.IntPtr.Zero);
                        if(thumbnail != null)
                        {
                            // Save to file
                            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + System.Diagnostics.Process.GetCurrentProcess().ProcessName + "\\thumbnails\\";
                            try
                            {
                                System.IO.Directory.CreateDirectory(path);
                            }
                            catch (Exception e)
                            {
                                Output($"{e.Message}");
                            }

                            string name = "thumb-" + Guid.NewGuid().ToString() + "-" + Path.GetFileName(file.Path);
                            var fullPath = path + name;
                            thumbnail.Save(fullPath);

                            var mimeType = Mime.GetMimeType(Path.GetExtension(fullPath));

                            // Build Thumbnail
                            file.LocalThumbnail = new LocalFile.Thumbnail()
                            {
                                Width = thumbnail.Width,
                                Height = thumbnail.Height,
                                Mime = mimeType,
                                Path = fullPath,
                            };
                        }

                    }

                    files.Add(file);
                }
                AttachedFiles.Clear();
            }
            return files;
        }
        TimelineMessage ConstructSendMessage(string text, List<LocalFile> files = null)
        {
            var message = new Message();
            var timelineMessage = new TimelineMessage
            {
                MessageInfo = message
            };

            message.PersonEmail = "You";
            message.Created = DateTime.UtcNow;
            message.Text = text;
            
            if (files != null && files.Count > 0)
            {
                message.Files = new List<RemoteFile>();
                timelineMessage.Files = new List<TimelineMessage.File>();
                foreach (var i in files)
                {
                    RemoteFile.Thumbnail thumbnail = null;
                    if (i.LocalThumbnail != null)
                    {
                        thumbnail = new RemoteFile.Thumbnail()
                        {
                            Height = i.LocalThumbnail.Height,
                            Width = i.LocalThumbnail.Width,
                            Mime = i.LocalThumbnail.Mime,
                        };
                    }
                    message.Files.Add(new RemoteFile()
                    {
                        Name = i.Name,
                        Size = i.Size,
                        RemoteThumbnail = thumbnail,
                    });
                    timelineMessage.Files.Add(new TimelineMessage.File()
                    {
                        Name = i.Name,
                        Size = i.Size,
                        ThumbnailPath = i.LocalThumbnail?.Path,
                    });

                }
            }
            
            return timelineMessage;
        }

        TimelineMessage BuildTimelineMessage(Message message)
        {
            var timelineMessage = new TimelineMessage
            {
                MessageInfo = message,
                Files = new List<TimelineMessage.File>()
            };

            if (message.Files != null && message.Files.Count > 0)
            {
                foreach(var item in message.Files)
                {
                    timelineMessage.Files.Add(new TimelineMessage.File()
                    {
                        Name = item.Name,
                        Size = item.Size,
                        ThumbnailPath = null,
                    });
                }
            }

            return timelineMessage;

        }
        void MessageSet(TimelineMessage to, Message from)
        {
            if (to.MessageInfo == null)
            {
                return;
            }
            to.MessageInfo.Id = from.Id;
            to.MessageInfo.SpaceId = from.SpaceId;
            to.MessageInfo.SpaceType = from.SpaceType;
            to.MessageInfo.PersonId = from.PersonId;
            to.MessageInfo.PersonEmail = from.PersonEmail;
            to.MessageInfo.ToPersonId = from.ToPersonId;
            to.MessageInfo.ToPersonEmail = from.ToPersonEmail;
            to.MessageInfo.Created = from.Created;
            to.MessageInfo.IsSelfMentioned = from.IsSelfMentioned;
            to.MessageInfo.Text = from.Text;
            to.MessageInfo.Files = from.Files;
        }

        private void DownLoadFile(object o)
        {
            if (SelectedMessage == null || SelectedMessage.MessageInfo == null 
                || SelectedMessage.MessageInfo.Files == null || SelectedMessage.MessageInfo.Files.Count == 0)
            {
                return;
            }

            string downloadPath = null;
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    downloadPath = dialog.SelectedPath + "\\";
                }
                else
                {
                    return;
                }
            }

            foreach (var item in SelectedMessage.MessageInfo.Files)
            {
                webex?.Messages.DownloadFile(item, downloadPath, r =>
                {
                    if (r.IsSuccess)
                    {
                        Output($"downloading {r.Data}%");
                    }
                    else
                    {
                        Output($"download failed {r.Data}");
                    }
                });
            }

  
            
        }

        private void AttachFile(object o)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = true
            };
            if (ofd.ShowDialog() == true)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var i in ofd.FileNames)
                    {
                        AttachedFiles.Add(i);
                    }
                });
            }
        }
        private void DeleteMsg(object o)
        {
            if (SelectedMessage != null)
            {
                webex?.Messages.Delete(SelectedMessage.MessageInfo.Id, r=>
                {
                    if (!r.IsSuccess)
                    {
                        Output($"delete message failed {r.Error.ErrorCode}:{r.Error.Reason}");
                    }
                    else
                    {
                        Output("delete message success.");
                    }
                });
            }
        }

        private void ListMessage(object o)
        {
            int max = 10;
            Message beforeMessage = null;
            if (SelectedMessage != null)
            {
                beforeMessage = SelectedMessage.MessageInfo;
            }
            else if (MessageList.Count > 0)
            {
                beforeMessage = MessageList[0].MessageInfo;
            }
            else
            {
                return;
            }

            if (beforeMessage != null)
            {
                webex?.Messages.List(beforeMessage.SpaceId, null, beforeMessage.Id, max, r =>
                {
                    if (r.IsSuccess)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            foreach (var item in r.Data)
                            {
                                var timelineMessage = BuildTimelineMessage(item);
                                MessageList.Insert(0, timelineMessage);
                            }
                        });
                        Output($"List {r.Data.Count} messages.");
                    }
                });
            }
          
        }
        void GetMessage(object o)
        {
            if (SelectedMessage == null)
            {
                return;
            }

            webex?.Messages.Get(SelectedMessage.MessageInfo.Id, r =>
            {
                if (r.IsSuccess)
                {
                    PrintPayload(r.Data);
                    Output($"Get the detail of the message.");
                }
            });

        }

        void PrintPayload(Message message)
        {
            string outStr = "";       
            if (message == null)
            {
                return;
            }
            PrintProperties(message, 0, ref outStr);

            PayLoad = outStr;
        }

        private void PrintProperties(object obj, int indent, ref string outStr)
        {
            if (obj == null) return;

            string indentString = new string(' ', indent);
            Type objType = obj.GetType();
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object propValue = property.GetValue(obj, null);
                var elems = propValue as IList;
                if (elems != null)
                {  
                    foreach (var item in elems)
                    {
                        outStr += String.Format($"{indentString}[{property.Name}]:\n\n");
                        PrintProperties(item, indent + 3, ref outStr);
                    }
                }
                else
                {
                    // This will not cut-off System.Collections because of the first check
                    if (property.PropertyType.Assembly == objType.Assembly)
                    {
                        outStr += String.Format($"{indentString}[{property.Name}]: {propValue}\n\n");

                        PrintProperties(propValue, indent + 2, ref outStr);
                    }
                    else
                    {
                        outStr += String.Format($"{indentString}[{property.Name}]: {propValue}\n\n");
                    }
                }
            }
        }
        private void UpdateRecentContactsStore(string personId)
        {
            ApplicationController.Instance.CurWebexManager.RecentContacts.AddRecentContactsStore(personId);
        }

    }
}
