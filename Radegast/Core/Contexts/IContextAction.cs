// 
// Radegast Metaverse Client
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
// $Id: 
//
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Radegast
{
    public interface IContextAction : IDisposable
    {
        void IContextAction(RadegastInstance instance);
        /// <summary>
        /// Generate a list of ToolStripMenuItems that might be be embeded into a ContextMenuStrip host
        /// </summary>
        /// <param name="target">the context sensitive item</param>
        /// <param name="type">the dereferenced type</param>
        /// <returns>List of ToolStripMenuItem that will be appeneded to the current menu</returns>
        IEnumerable<ToolStripMenuItem> GetToolItems(object target, Type type);
        /// <summary>
        /// Get GUI items that one might include on the form to operate this action
        /// </summary>
        /// <param name="target">the context sensitive item</param>
        /// <param name="type">the dereferenced type</param>
        /// <returns>List of Buttons and Other hostable Controls that make sense to appear on the form when this item is selected</returns>
        IEnumerable<Control> GetControls(object target, Type type);
        /// <summary>
        /// If the menu item is Enabled
        /// </summary>
        /// <param name="target">the context sensiive item</param>
        /// <returns>false when the menu Item should be greyed</returns>
        bool IsEnabled(object target);
        /// <summary>
        /// The menu Item's text based on target 
        /// </summary>
        /// <param name="target">the context sensiive item</param>
        /// <returns>true if the Action is availble - for instance some Avatar might not even be logged in so "follow" would not even show up</returns>
        string LabelFor(object target);
        /// <summary>
        /// If the context menu is usable to the target
        /// </summary>
        /// <param name="target">the context sensitive item</param>
        /// <param name="type">the dereferenced type</param>
        /// <returns>The name that is displayed in a menu of options</returns>
        bool Contributes(Object target, Type type);
        /// <summary>
        /// The Action code goes here
        /// </summary>
        /// <param name="sender">the Control that originates the event</param>
        /// <param name="e">The EventArgs proprietary to the Controls event.. like MouseEventArgs or KeyEventArgs etc</param>
        /// <param name="target">The Context Item that is realy targeted</param>
        void OnInvoke(object sender, EventArgs e, object target);
    }
}