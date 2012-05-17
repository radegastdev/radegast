﻿// 
// Radegast Metaverse Client
// Copyright (c) 2009-2012, Radegast Development Team
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
// $Id: 
//
using System;
using System.Windows.Forms;
using OpenMetaverse;
using Radegast;

namespace RadegastSpeech.GUI
{
    public class NotecardReadAction : ContextAction
    {
        private PluginControl control;
        public NotecardReadAction(RadegastInstance inst, PluginControl pc)
            : base(inst)
        {
            control = pc;
            Label = "Read";
            ContextType = typeof(InventoryNotecard);
        }

        public override bool IsEnabled(object target)
        {
            return true;
        }

        /// <summary>
        /// Respond to a "Read Notecard" context menu action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="target"></param>
        public override void OnInvoke(object sender, EventArgs e, object target)
        {
            InventoryNotecard nc;

            // This action applies to notecards, which can come in
            // various forms.
            if (target is InventoryNotecard)
            {
                nc = target as InventoryNotecard;
            }
            else
            {
                control.talker.SayMore("Can not read this notecard.", RadegastSpeech.Talk.BeepType.Bad);
                return;
            }

            // Create a conversation for reading notecards.
            control.converse.AddInterruption(new Conversation.InvNotecard(control, nc));
        }
    }
}
