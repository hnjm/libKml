﻿// 
//  KmlDocument.cs
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
	public class KmlDocument : KmlContainer, ISearchable {

		private List<KmlSchema> _schemas = new List<KmlSchema>();
			
		public KmlDocument() : base() {}
		public KmlDocument(XmlNode parent, Logger log) : this() {
			Log += log;
			fromXml(parent, log);
		}
		
		#region properties
		public List<KmlSchema> Schemas {
			get { return _schemas; }
			set { _schemas = value; }
		}
		#endregion properties

		#region helpers

		private void fromXml(XmlNode parent, Logger log) {
			foreach (XmlNode childNode in parent.ChildNodes) {
				handleNode(childNode, log);
			}
		}
		public new void handleNode (XmlNode node, Logger log) {
			string nodeKey = node.Name.ToLower();
			debug("handling :: " + nodeKey);
			switch (nodeKey) {
				// handle the feature nodes
				case "placemark":
					_features.Add(new KmlPlacemark(node, log));
					break;
				case "document":
					_features.Add(new KmlDocument(node, log));
					break;
				case "folder":
					_features.Add(new KmlFolder(node, log));
					break;
				case "networklink":
					_features.Add(new KmlNetworkLink(node, log));
					break;
				case "groundoverlay":
					_features.Add(new KmlGroundOverlay(node, log));
					break;
				case "photooverlay":
					_features.Add(new KmlPhotoOverlay(node, log));
					break;
				case "screenoverlay":
					_features.Add(new KmlScreenOverlay(node, log));
					break;
				// or a schema node
				case "schema":
					_schemas.Add(new KmlSchema(node, log));
					break;
				default:
					base.handleNode(node, log);
					break;
			};
		}

		public override XmlNode ToXml(XmlNode parent) {

			XmlNode result = parent.OwnerDocument.CreateNode(XmlNodeType.Element, "Document", string.Empty);

			
			if (null != _schemas && _schemas.Count > 0) {
				foreach (KmlSchema schema in _schemas) {
					result.AppendChild(schema.ToXml(result));
				}
			}

			base.ToXml(result);

			if (null != _features && _features.Count > 0) {
				foreach (KmlFeature feature in _features) {
					result.AppendChild(feature.ToXml(result));
				}
			}
			return result;
		}

		public new void findElementsOfType<T> (List<object> elements) {
			
			base.findElementsOfType<T>(elements);
			if (this is T) elements.Add(this);
			else base.findElementsOfType<T>(elements);
			if (null != _schemas) {
				foreach(KmlSchema schema in _schemas) {
					schema.findElementsOfType<T>(elements);
				}
			}
		}
		#endregion helpers
	}//	class
}//	namespace
