﻿namespace Client.Model
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
	using Microsoft.Xna.Framework;

    [Serializable] //XML
    [DataContract] //Json
    public class PlanetarySystem
    {
        [XmlAttribute]
        [DataMember]
        public int Id { get; set; }

        [XmlAttribute]
        [DataMember]
        public int FleetBonusPerTurn { get; set; }
        
        [XmlAttribute]
        [DataMember]
        public string Name { get; set; }

        [XmlArray("Planets")]
        [XmlArrayItem("PlanetId")]
        [DataMember]
        public int[] Planets { get; set; }

		//[DataMember]
		public Color Color { get; set; }

		[XmlArray("Bounds")]
		[DataMember]
		public Point3[] Bounds { get; set; }
    }
}
