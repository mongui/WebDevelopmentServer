//
// HorizRule.cs
//
// Implementation of the HorizRule type.
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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Win32;

namespace Covidimus.Forms
{
	/// <summary>
	/// Implements a standard Windows horizontal rule.
	/// </summary>
	[ Description("A horizontal rule.") ]
	public sealed class HorizRule : RuleControlBase
	{
		private const int DefaultFixedHeight = 2;
		private const string FixedHeightDesc =
			"Set to a fixed height >= 2, or -1 to allow rule to expand.";
		private int m_fixedHeight = DefaultFixedHeight;

		/// <summary>
		/// Sets a fixed height for the control.  Set to a value >= 0 to fix
		/// the height of the rule at that value, or -1 to allow the rule to
		/// expand.
		/// </summary>
		[
			Description(FixedHeightDesc),
			Category("Layout"),
			DefaultValue(DefaultFixedHeight)
		]
		public int FixedHeight
		{
			get { return m_fixedHeight; }

			set
			{
				if(value < DefaultFixedHeight && value != -1)
				{
					throw new InvalidOperationException(
						"Value for FixedHeight must be >= "
						+ DefaultFixedHeight + "."
					);
				}

				m_fixedHeight = value;

				if(m_fixedHeight != -1)
					Height = m_fixedHeight;	// Update height.
			}
		}

		/// <summary>
		/// Override of <see cref="Control.CreateParams"/>.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams p = base.CreateParams;
				p.ClassName = "STATIC";
				p.Style = User.WS_CHILD | User.WS_VISIBLE | User.SS_ETCHEDHORZ;
				return p;
			}
		}

		/// <summary>
		/// Override of <see cref="Control.SetBoundsCore"/>.
		/// </summary>
		protected override void SetBoundsCore(
			int x, int y, int width, int height, BoundsSpecified specified)
		{
			if(m_fixedHeight != -1)
			{
				// Restrict the height to the defined fixed height.
				height = m_fixedHeight;
			}
			else if(height < DefaultFixedHeight)
			{
				// Never allow the height to get smaller than the default fixed
				// height (to prevent it from disappearing), but otherwise place
				// no restriction on the height.
				if(height < DefaultFixedHeight)
					height = DefaultFixedHeight;
			}

			base.SetBoundsCore(x, y, width, height, specified);
		}
	}
}
