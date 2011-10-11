﻿// 
//  KmlColourStyle.cs
//  
//  Author:
//       mat rowlands <code-account@podulator.com>
//  
//  Copyright (c) 2011 mat rowlands
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Pod.Kml {

	public enum ColourModes : int {
		Normal = 0,
		Random = 1
	};

	public abstract class KmlColourStyle : ISearchable {

		private string _id = string.Empty;
		private KmlColour _colour = null;
		private int _colourMode = (int)ColourModes.Normal;
		
		public KmlColourStyle() {}
		public KmlColourStyle(XmlNode parent, Logger log) : this() {
			Log += log;
			if (null != parent.Attributes["id"]) 
				_id = parent.Attributes["id"].Value;

			foreach (XmlNode node in parent.ChildNodes) {
				string key = node.Name.ToLower();
				switch (key) {
					case "color":
						_colour = new KmlColour(node.InnerText, log);
						break;
					case "colormode":
						_colourMode = colourModeFromString(node.InnerText);
						break;
				};
			}
		}
		#region properties
		public string Id {
			get { return _id; }
			set { _id = value; }
		}
		/// <summary>
		/// Colour is hex 0 - 256 in form aa bb gg rr
		/// </summary>
		public KmlColour Colour {
			get { return _colour; }
			set { _colour = value; }
		}

		public string ColourMode {
			get { return colourModeToString(_colourMode); }
		}
		
		public void setColourMode(ColourModes value) {
			_colourMode = (int)value;
		}

		#endregion properties

		#region helpers
		private int colourModeFromString(string mode) {
			switch (mode.ToLower()) {
				case "random":
					return (int)ColourModes.Random;
				default:
					return (int)ColourModes.Normal;
			};
		}
		private string colourModeToString(int mode) {
			switch ((ColourModes)mode) {
				case ColourModes.Random:
					return "random";

				default:
					return "normal";
			};
		}
		public virtual XmlNode ToXml(XmlNode parent) {
			if (_id.Length > 0) {
				XmlAttribute attId = parent.OwnerDocument.CreateAttribute("id");
				attId.Value = _id;
				parent.Attributes.Append(attId);
			}
			if (null != Colour) {
				XmlNode nodColour = Colour.ToXml(parent);
				parent.AppendChild(nodColour);
			}

			XmlNode nodColourMode = parent.OwnerDocument.CreateNode(XmlNodeType.Element, "colorMode", string.Empty);
			nodColourMode.InnerText = ColourMode;
			parent.AppendChild(nodColourMode);
			return null;
		}
		public virtual void findElementsOfType<T> (List<object> elements) {
			if (this is T) elements.Add(this);
		}
		protected void debug (string message) {
			if (Log != null) Log(message);
		}
		public event Logger Log;
		#endregion helpers

	}//	class
}//	namespace
