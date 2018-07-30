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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebexSDK;

namespace KitchenSink
{
    public class MessageViewModel : ViewModelBase
    {
        public RelayCommand SendDirectMessageCMD { get; set; }
        public RelayCommand SendRoomMessageCMD { get; set; }

        public string Callee
        {
            get { return ApplicationController.Instance.CurWebexManager.CurCalleeAddress; }
            set
            {
                if (value != ApplicationController.Instance.CurWebexManager.CurCalleeAddress)
                {
                    ApplicationController.Instance.CurWebexManager.CurCalleeAddress = value;
                    OnPropertyChanged("Callee");
                }
            }
        }

        private string searchString;
        public string SearchString
        {
            get { return this.searchString; }
            set
            {
                this.searchString = value;
                OnPropertyChanged("SearchString");

                if (searchString.Length > 2)
                {
                    FetchPersons(searchString);
                }
            }
        }

        private List<Room> roomList;
        public List<Room> RoomList
        {
            get
            {
                return this.roomList;
            }
            set
            {
                this.roomList = value;
                OnPropertyChanged("RoomList");
            }
        }

        private Room selectedRoom;
        public Room SelectedRoom
        {
            get
            {
                return this.selectedRoom;
            }
            set
            {
                this.selectedRoom = value;
                if (this.selectedRoom != null)
                {
                    ApplicationController.Instance.CurWebexManager.CurCalleeAddress = this.selectedRoom.Id;
                    ApplicationController.Instance.ChangeViewCmd = ChangeViewCmd.MessageSessionView;
                    ApplicationController.Instance.ChangeState(State.MessageSession);

                }
            }
        }

        private List<Person> personList;
        public List<Person> PersonList
        {
            get
            {
                return this.personList;
            }
            set
            {
                this.personList = value;
                OnPropertyChanged("PersonList");
            }
        }

        private Person selectedPerson;
        public Person SelectedPerson
        {
            get
            {
                return this.selectedPerson;
            }
            set
            {
                this.selectedPerson = value;
                if (this.selectedPerson != null)
                {
                    ApplicationController.Instance.CurWebexManager.CurCalleeAddress = this.selectedPerson.Emails[0];
                    ApplicationController.Instance.ChangeViewCmd = ChangeViewCmd.MessageSessionView;
                    ApplicationController.Instance.ChangeState(State.MessageSession);

                }
            }
        }

        private List<Person> recentContacts;
        public List<Person> RecentContacts
        {
            get
            {
                return this.recentContacts;
            }
            set
            {
                this.recentContacts = value;
                OnPropertyChanged("RecentContacts");
            }
        }

        private void FetchRooms()
        {
            var webex = ApplicationController.Instance.CurWebexManager.CurWebex;

            webex?.Rooms?.List(null, null, RoomType.Group, RoomSortType.ByLastActivity, r =>
            {
                if (r.IsSuccess)
                {
                    RoomList = new List<Room>((IList<Room>)r.Data);
                }
            });
        }

        private void FetchPersons(string searchStr)
        {
            var webex = ApplicationController.Instance.CurWebexManager.CurWebex;

            // Lists people with display name in the authenticated user's organization.
            webex?.People?.List(null, searchStr, 10, r =>
            {
                if (r.IsSuccess)
                {
                    PersonList = new List<Person>((IList<Person>)r.Data);
                }
            });
        }

        private void FetchRecentContacts()
        {
            var webex = ApplicationController.Instance.CurWebexManager.CurWebex;
            List<string> recentContacts = ApplicationController.Instance.CurWebexManager?.RecentContacts?.RecentContactsStore;
            List<Person> personList = new List<Person>();

            if (recentContacts == null || personList == null)
            {
                return;
            }

            foreach (var item in recentContacts)
            {
                webex?.People?.Get(item, r =>
                {
                    if (r.IsSuccess)
                    {
                        personList.Add((Person)(r.Data));
                        RecentContacts = new List<Person>(personList);
                    }
                });
            }
        }
        public MessageViewModel()
        {
            SendDirectMessageCMD = new RelayCommand(SendDirectMessage, CanMessage);
            SendRoomMessageCMD = new RelayCommand(SendRoomMessage);
            FetchRooms();
            FetchRecentContacts();
        }

        private void SendDirectMessage(object o)
        {
            ApplicationController.Instance.ChangeViewCmd = ChangeViewCmd.MessageSessionView;
            ApplicationController.Instance.ChangeState(State.MessageSession);
        }

        private bool CanMessage(object o)
        {
            return !string.IsNullOrEmpty(this.Callee);
        }


        private void SendRoomMessage(object o)
        {
            if (SelectedRoom != null)
            {
                ApplicationController.Instance.CurWebexManager.CurCalleeAddress = this.selectedRoom.Id;
                ApplicationController.Instance.ChangeViewCmd = ChangeViewCmd.MessageSessionView;
                ApplicationController.Instance.ChangeState(State.MessageSession);

            }
        }


    }
}
