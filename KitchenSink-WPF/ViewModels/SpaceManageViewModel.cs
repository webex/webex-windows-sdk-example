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
    public class SpaceManageViewModel : ViewModelBase
    {
        private Webex webex;
        public RelayCommand CreateSpaceCMD { get; set; }
        public RelayCommand DeleteSpaceCMD { get; set; }
        public RelayCommand CreateMembershipCMD { get; set; }
        public RelayCommand DeleteMembershipCMD { get; set; }

        public SpaceManageViewModel()
        {
            webex = ApplicationController.Instance.CurWebexManager.CurWebex;
            CreateSpaceCMD = new RelayCommand(CreateSpace);
            DeleteSpaceCMD = new RelayCommand(DeleteSpace);
            CreateMembershipCMD = new RelayCommand(CreateMembership);
            DeleteMembershipCMD = new RelayCommand(DeleteMembership);
            FetchSpaces();
        }

        private List<Space> spaceList;
        public List<Space> SpaceList
        {
            get
            {
                return this.spaceList;
            }
            set
            {
                this.spaceList = value;
                OnPropertyChanged("SpaceList");
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

        private Space selectedSpace;
        public Space SelectedSpace
        {
            get
            {
                return this.selectedSpace;
            }
            set
            {
                this.selectedSpace = value;
                if (this.selectedSpace != null)
                {

                    FetchMemberships(SelectedSpace.Id);
                }
            }
        }

        public string SpaceTitle { get; set; }

        public Membership SelectedMembership { get; set; }



        public string AddedPersonEmail { get; set; }



        private void FetchSpaces()
        {
            webex?.Spaces?.List(null, null, SpaceType.Group, SpaceSortType.ByLastActivity, r =>
            {
                if (r.IsSuccess)
                {
                    SpaceList = new List<Space>((IList<Space>)r.Data);
                }
            });
        }

        private void CreateSpace(object o)
        {
            webex?.Spaces.Create(SpaceTitle, null, r =>
            {
                if(r.IsSuccess)
                {
                    FetchSpaces();
                }

            });
        }

        private void DeleteSpace(object o)
        {
            webex?.Spaces.Delete(SelectedSpace.Id, r =>
            {
                if (r.IsSuccess)
                {
                    FetchSpaces();
                }

            });
        }

        private void FetchMemberships(string spaceId)
        {
            webex?.Memberships.List(spaceId, null, r =>
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
                    webex?.Memberships.CreateByPersonId(SelectedSpace.Id, personId, null, rr =>
                    {
                        if (rr.IsSuccess)
                        {
                            FetchMemberships(SelectedSpace.Id);
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
                    FetchMemberships(SelectedSpace.Id);
                }
            });
        }
    }
}
