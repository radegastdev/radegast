// 
// Radegast Metaverse Client Speech Interface
// Copyright (c) 2009-2014, Radegast Development Team
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of the application "Radegast", nor the names of its
//       contributors may be used to endorse or promote products derived from
//       this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// $Id: Surroundings.cs 203 2009-10-06 17:26:02Z mojitotech@gmail.com $
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Radegast;
using OpenMetaverse;

namespace RadegastSpeech.Conversation
{
    class Surroundings : Mode
    {
        private Radegast.ObjectsConsole obTab;
        public bool Announce { get; set; }
        private Primitive currentPrim = new Primitive();
        
        #region statechange
        internal Surroundings(PluginControl pc)
            : base(pc)
        {
            Title = "surroundings";
            Announce = false;
            obTab = (ObjectsConsole)control.instance.TabConsole.Tabs["objects"].Control;
        }

        /// <summary>
        /// conversation becomes active
        /// </summary>
        internal override void Start()
        {
            base.Start();
            obTab.lstPrims.SelectedIndexChanged += Objects_SelectedIndexChanged;
            Talker.SayMore("Surroundings");
            Objects_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// Friends conversation becomes inactive
        /// </summary>
        internal override void Stop()
        {
            obTab.lstPrims.SelectedIndexChanged -= Objects_SelectedIndexChanged;
            base.Stop();
        }
        #endregion

        #region generalevents
        #endregion

        #region focusevents
 
        /// <summary>
        /// Speech input commands for Friends conversation
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        internal override bool Hear(string message)
        {
            switch (message)
            {
                case "what can I sit on":
                    return true;
            }

            return false;
        }
        #endregion

        /// <summary>
        /// Announce which object has been selected in the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Objects_SelectedIndexChanged(object sender, EventArgs e)
        {
            string description;

            if (obTab.lstPrims.SelectedIndices.Count != 1)
                return;

            currentPrim = obTab.Prims[obTab.lstPrims.SelectedIndices[0]];

            Vector3 pos = Vector3.Zero;
            if (currentPrim.ParentID == 0)
            {
                pos = currentPrim.Position;
            }

            if (currentPrim.Properties == null)
            {
                Talker.Say( "Object data still loading.  Please wait." );
                return;
            }
            else
            {
                description = control.env.people.Location(pos);

                if ((currentPrim.Flags & PrimFlags.Scripted) != 0)
                    description += " scripted,";
                if (currentPrim.Properties.SitName != "")
                    description += " is sittable,";
                if (currentPrim.Properties.TouchName != "")
                    description += " is touchable";
            }

            Talker.SayObject(
                currentPrim.Properties.Name,
                description,
                control.env.people.SameDirection( pos ) );
      }

        /// <summary>
        /// 
        /// </summary>
       private void TellMeMore()
        {
            string description;

            description = currentPrim.Properties.Description;

            string ownerName = "Loading...";
#if NOT
            if (currentPrim.Properties == null)
            {
                if (prim.ParentID != 0) throw new Exception("Requested properties for non root prim");
                propRequester.RequestProps(prim.LocalID);
            }
            else
            {
                name = prim.Properties.Name;
                ownerName = instance.getAvatarName(prim.Properties.OwnerID);
            }

            txtHover.Text = currentPrim.Text;
            txtOwner.AgentID = currentPrim.Properties.OwnerID;
            txtCreator.AgentID = currentPrim.Properties.CreatorID;

            Permissions p = currentPrim.Properties.Permissions;
            cbOwnerModify.Checked = (p.OwnerMask & PermissionMask.Modify) != 0;
            cbOwnerCopy.Checked = (p.OwnerMask & PermissionMask.Copy) != 0;
            cbOwnerTransfer.Checked = (p.OwnerMask & PermissionMask.Transfer) != 0;
            cbNextOwnModify.Checked = (p.NextOwnerMask & PermissionMask.Modify) != 0;
            cbNextOwnCopy.Checked = (p.NextOwnerMask & PermissionMask.Copy) != 0;
            cbNextOwnTransfer.Checked = (p.NextOwnerMask & PermissionMask.Transfer) != 0;

            if ((currentPrim.Flags & PrimFlags.Money) != 0)
            {
                btnPay.Enabled = true;
            }
            else
            {
                btnPay.Enabled = false;
            }
#endif
            description += " owned by " + ownerName;
            if (currentPrim.Light != null)
            {
                description += " is a light,";
            }
            if (currentPrim.Properties.SaleType != SaleType.Not)
            {
                description += string.Format(", for sale at $L{0} Lindens.", currentPrim.Properties.SalePrice);
            }
            Talker.SayMore(description);
        }
    }
}
