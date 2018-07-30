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
    public class RoomManageViewModel : ViewModelBase
    {
        private Webex webex;
        public RelayCommand CreateRoomCMD { get; set; }
        public RelayCommand DeleteRoomCMD { get; set; }
        public RelayCommand CreateMembershipCMD { get; set; }
        public RelayCommand DeleteMembershipCMD { get; set; }

        public RoomManageViewModel()
        {
            webex = ApplicationController.Instance.CurWebexManager.CurWebex;
            CreateRoomCMD = new RelayCommand(CreateRoom);
            DeleteRoomCMD = new RelayCommand(DeleteRoom);
            CreateMembershipCMD = new RelayCommand(CreateMembership);
            DeleteMembershipCMD = new RelayCommand(DeleteMembership);
            FetchRooms();
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

        private List<Membership> membershipList;
        public List<Membership> MembersipList
        {
            get
            {
                return this.membershipList;
            }
            set
            {
                this.membershipList = value;
                OnPropertyChanged("MembersipList");
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

                    FetchMemberships(SelectedRoom.Id);
                }
            }
        }

        public string RoomTitle { get; set; }

        public Membership SelectedMembership { get; set; }



        public string AddedPersonEmail { get; set; }



        private void FetchRooms()
        {
            webex?.Rooms?.List(null, null, RoomType.Group, RoomSortType.ByLastActivity, r =>
            {
                if (r.IsSuccess)
                {
                    RoomList = new List<Room>((IList<Room>)r.Data);
                }
            });
        }

        private void CreateRoom(object o)
        {
            webex?.Rooms.Create(RoomTitle, null, r =>
            {
                if(r.IsSuccess)
                {
                    FetchRooms();
                }

            });
        }

        private void DeleteRoom(object o)
        {
            webex?.Rooms.Delete(SelectedRoom.Id, r =>
            {
                if (r.IsSuccess)
                {
                    FetchRooms();
                }

            });
        }

        private void FetchMemberships(string roomId)
        {
            webex?.Memberships.List(roomId, null, r =>
            {
                if (r.IsSuccess)
                {
                    MembersipList = new List<Membership>((IList<Membership>)r.Data);
                }
            });
        }

        private void CreateMembership(object o)
        {
            webex?.People.List(AddedPersonEmail, null, null, r =>
            {
                if (r.IsSuccess)
                {
                    var personId = r.Data[0].Id;
                    webex?.Memberships.CreateByPersonId(SelectedRoom.Id, personId, null, rr =>
                    {
                        if (rr.IsSuccess)
                        {
                            FetchMemberships(SelectedRoom.Id);
                        }
                    });
                }
            });
        }

        private void DeleteMembership(object o)
        {
            webex?.Memberships.Delete(SelectedMembership.Id, r =>
            {
                if (r.IsSuccess)
                {
                    FetchMemberships(SelectedRoom.Id);
                }
            });
        }
    }
}
