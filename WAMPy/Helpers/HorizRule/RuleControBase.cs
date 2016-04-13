//
// RuleControlBase.cs
//
// Implementation of the RuleControlBase type.
//
// Copyright © 2003 Stephen Quattlebaum
//
// You may use this code as you see fit.  No warranty of any kind, express or
// implied, is included in this software.
//
// The only restriction is that you must credit the original author of the
// code, either by leaving this copyright notice intact or by noting in
// derived works that portions are copyrighted by Stephen Quattlebaum (and
// noting which portions those are), in your derived source or documentation.
//

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Covidimus.Forms
{
	/// <summary>
	/// Implements a standard Windows horizontal rule.
	/// </summary>
	[ Description("Base class for rule controls (like HorizRule and VertRule).") ]
	public abstract class RuleControlBase : Control
	{
		/// <summary>
		/// Hides the <see cref="Control.Text"/> property in the property
		/// browser b/c it doesn't make sense for a rule.
		/// </summary>
		[ Browsable(false) ]
		public new string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		/// <summary>
		/// Hides the <see cref="Control.TabStop"/> property in the property
		/// browser b/c it doesn't make sense for a rule.
		/// </summary>
		[ Browsable(false) ]
		public new bool TabStop
		{
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}

		/// <summary>
		/// Hides the <see cref="Control.BackColor"/> property in the property
		/// browser b/c it doesn't make sense for a rule.
		/// </summary>
		[ Browsable(false) ]
		public new System.Drawing.Color BackColor
		{
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}

		/// <summary>
		/// Hides the <see cref="Control.BackgroundImage"/> property in the
		/// property browser b/c it doesn't make sense for a rule.
		/// </summary>
		[ Browsable(false) ]
		public new System.Drawing.Image BackgroundImage
		{
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}

		/// <summary>
		/// Hides the <see cref="Control.RightToLeft"/> property in the property
		/// browser b/c it doesn't make sense for a rule.
		/// </summary>
		[ Browsable(false) ]
		public new RightToLeft RightToLeft
		{
			get { return base.RightToLeft; }
			set { base.RightToLeft = value; }
		}

		/// <summary>
		/// Hides the <see cref="Control.Enabled"/> property in the property
		/// browser b/c it doesn't make sense for a rule.
		/// </summary>
		[ Browsable(false) ]
		public new bool Enabled
		{
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}

		/// <summary>
		/// Hides the <see cref="Control.ImeMode"/> property in the property
		/// browser b/c it doesn't make sense for a rule.
		/// </summary>
		[ Browsable(false) ]
		public new ImeMode ImeMode
		{
			get { return base.ImeMode; }
			set { base.ImeMode = value; }
		}
	}
}