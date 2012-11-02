﻿namespace Client.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Serialization;
    using Client.Renderer;

    [DataContract]
    public class Map
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<Planet> Planets { get; set; }

        [DataMember]
        public List<PlanetLink> Links { get; set; }

        [DataMember]
        public List<PlanetarySystem> PlanetarySystems { get; set; }

        [DataMember]
        public List<StartingData> PlayerStartingData { get; set; }

        public MapVisual Visual { get; set; }

        public List<Planet> StartingPositions { get { return PlayerStartingData.Select(data => GetPlanetById(data.PlanetId)).ToList(); } }
        public int MaxPlayersCount { get { return StartingPositions.Count; } }

        private const string MapsPath = "Content/Maps/{0}.xml";

        // Attributes names
        private const string NameAttribute = "Name";
        private const string PlanetIdAttribute = "PlanetId";
        private const string ColorAttribute = "Color";

        // Element names
        private const string MapElement = "Map";
        private const string PlanetsElement = "Planets";
        private const string LinksElement = "Links";
        private const string SystemsElement = "Systems";
        private const string PlayerStartingDataElement = "PlayerStartingData";
        private const string StartingDataElement = "StartingData";

        /// <summary>
        /// Reads localWorld map from XML file (no extension required). 
        /// </summary>
        public Map(string mapFileName) : this()
        {
            string path = string.Format(Map.MapsPath, mapFileName);
            using (FileStream fileStream = File.OpenRead(path))
            {
                var reader = XmlReader.Create(fileStream);
                
                // Read map name
                reader.ReadToFollowing(MapElement);
                this.Name = reader.GetAttribute(NameAttribute);

                // Read planets
                reader.ReadToDescendant(PlanetsElement);
                reader.ReadToDescendant(typeof(Planet).Name);
                var planetSerializer = new XmlSerializer(typeof(Planet));                
                do
                {
                    var planet = (Planet) planetSerializer.Deserialize(reader);

                    if (planet != null)
                    {
                        planet.NumFleetsPresent = 1;
                        Planets.Add(planet);
                    }
                } while (reader.ReadToNextSibling(typeof(Planet).Name));

                // Read links
                reader.ReadToFollowing(LinksElement);
                reader.ReadToDescendant(typeof(PlanetLink).Name);
                var linkSerializer = new XmlSerializer(typeof(PlanetLink));
                do
                {
                    var planetLink = (PlanetLink) linkSerializer.Deserialize(reader);

                    if (planetLink != null)
                    {
                        Links.Add(planetLink);
                    }
                } while (reader.ReadToNextSibling(typeof(PlanetLink).Name));

                // Read systems
                reader.ReadToFollowing(SystemsElement);
                reader.ReadToDescendant(typeof(PlanetarySystem).Name);
                var systemSerializer = new XmlSerializer(typeof(PlanetarySystem));                
                do
                {
                    var planetarySystem = (PlanetarySystem) systemSerializer.Deserialize(reader);

                    if (planetarySystem != null)
                    {
                        PlanetarySystems.Add(planetarySystem);
                    }
                } while (reader.ReadToNextSibling(typeof(PlanetarySystem).Name));

                // Read starting positions
                reader.ReadToFollowing(PlayerStartingDataElement);
                reader.ReadToDescendant(StartingDataElement);
                do
                {
                    if (reader.HasAttributes)
                    {
                        int planetId = Convert.ToInt32(reader.GetAttribute(PlanetIdAttribute));
                        string colorHex = reader.GetAttribute(ColorAttribute);
                        PlayerStartingData.Add(new StartingData(planetId, Convert.ToInt32(colorHex, 16)));
                    }
                } while (reader.ReadToNextSibling(StartingDataElement));
            }
        }

        [OnDeserialized]
        public void OnJsonDeserialized(StreamingContext context)
        {
            // TODO Make Player (& Planet) references to be the same objects?
        }

        public Map()
        {            
            Planets = new List<Planet>();
            Links = new List<PlanetLink>();
            PlanetarySystems = new List<PlanetarySystem>();
            PlayerStartingData = new List<StartingData>();
        }

        public Planet GetPlanetById(int planetId)
        {
            return Planets.Find(planet => planet.Id == planetId);
        }

        /// <summary>
        /// The function determines which planets should have details visible, based on their owner
        /// and proximity to the client players planets (that is only neighbouring ones).
        /// </summary>        
        public void UpdatePlanetShowDetails(Player clientPlayer)
        {
            var bidirectionalLinks = Links.SelectMany(linkElement =>
            {
                var swappedLink = new PlanetLink
                {
                    SourcePlanet = linkElement.TargetPlanet,
                    TargetPlanet = linkElement.SourcePlanet
                };
                return new List<PlanetLink> { linkElement, swappedLink };
            });

            foreach (var planet in Planets)
            {
                planet.ShowDetails = false;

                // Check if the client player is the owner
                if (planet.Owner == clientPlayer)
                {
                    planet.ShowDetails = true;
                }
                // Or...
                // Find any neighbouring planets also owned by the client player
                else
                {
                    // Select the links and duplicate them with swapped target and source
                    // because the graph edges are bidirectional
                    var neighbourIds = bidirectionalLinks.Where(link => link.SourcePlanet == planet.Id);                    
                    var neighbours = neighbourIds.Select(link => GetPlanetById(link.TargetPlanet));
                    foreach (var neighbour in neighbours)
                    {
                        if (neighbour.Owner == clientPlayer)
                        {
                            planet.ShowDetails = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}
