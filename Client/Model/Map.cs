﻿namespace Client.Model
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Xml;
	using System.Xml.Serialization;
	using Client.Common;
	using Client.Common.AnimationSystem;
	using Client.Renderer;

	[DataContract]
    public class Map
    {
		[DataMember]
		public int Points { get; set; }

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

		[DataMember]
		public List<BackgroundLayer> Background { get; set; }

        [DataMember]
        public List<PlayerColor> Colors { get; set; }

		[DataMember]
		public SimpleCamera Camera { get; set; }

        public MapVisual Visual { get; set; }

        public List<Planet> StartingPositions { get { return PlayerStartingData.Select(data => GetPlanetById(data.PlanetId)).ToList(); } }
        public int MaxPlayersCount { get { return StartingPositions.Count; } }

		// Map path template
        private const string MapsPath = "Content/Maps/{0}.xml";

        // Attributes names
        private const string NameAttribute = "Name";
		private const string PointsAttribute = "Points";
        private const string PlanetIdAttribute = "PlanetId";
        private const string ColorIdAttribute = "ColorId";
        private const string ValueAttribute = "Value";
        private const string IdAttribute = "Id";
		private const string MinAttribute = "Min";
		private const string MaxAttribute = "Max";
		private const string TextureAttribute = "Texture";
		private const string OriginAttribute = "Origin";
		private const string SizeAttribute = "Size";
		private const string SpeedAttribute = "Speed";

        // Element names
        private const string MapElement = "Map";
        private const string PlanetsElement = "Planets";
        private const string LinksElement = "Links";
        private const string SystemsElement = "Systems";
        private const string PlayerStartingDataElement = "PlayerStartingData";
        private const string StartingDataElement = "StartingData";
		private const string BackgroundElement = "Background";
		private const string BackgroundLayerElement = "Layer";
        private const string ColorsElement = "Colors";
        private const string ColorElement = "Color";
		private const string CameraElement = "Camera";

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
				this.Points = Convert.ToInt32(reader.GetAttribute(PointsAttribute));

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
				
                reader.ReadToFollowing(LinksElement);
                reader.ReadToDescendant(typeof(PlanetLink).Name);
                var linkSerializer = new XmlSerializer(typeof(PlanetLink));
                do
                {
                    var planetLink = (PlanetLink) linkSerializer.Deserialize(reader);

                    if (planetLink != null)
                    {
						planetLink.OnXmlDeserialized();
                        Links.Add(planetLink);
					}
                } while (reader.ReadToNextSibling(typeof(PlanetLink).Name));

                // Read links and find neighbouring planets
				LoadNeighbours();

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
                        int colorId = Convert.ToInt32(reader.GetAttribute(ColorIdAttribute));
                        PlayerStartingData.Add(new StartingData(planetId, colorId));
                    }
                } while (reader.ReadToNextSibling(StartingDataElement));

				// read background layers
				reader.ReadToFollowing(BackgroundElement);
				reader.ReadToDescendant(BackgroundLayerElement);
				do
				{
					if (reader.HasAttributes)
					{
						var layer = new BackgroundLayer();
						layer.Texture = reader.GetAttribute(TextureAttribute);
						layer.Origin = XnaExtensions.ParseVector2(reader.GetAttribute(OriginAttribute));
						layer.Size = XnaExtensions.ParseVector2(reader.GetAttribute(SizeAttribute));
						layer.Speed = float.Parse(reader.GetAttribute(SpeedAttribute));
						Background.Add(layer);
					}
				} while (reader.ReadToNextSibling(BackgroundLayerElement));

                // Read available colors
                reader.ReadToFollowing(ColorsElement);
                reader.ReadToDescendant(ColorElement);
                do
                {
                    if (reader.HasAttributes)
                    {
                        int colorId = Convert.ToInt32(reader.GetAttribute(IdAttribute));
                        string colorHex = reader.GetAttribute(ValueAttribute);
                        uint value = Convert.ToUInt32(colorHex, 16);
                        Colors.Add(new PlayerColor(colorId, value));
                    }
                } while (reader.ReadToNextSibling(ColorElement));

				// Camera
				reader.ReadToFollowing(CameraElement);
				Camera = new SimpleCamera();
				Camera.Min = XnaExtensions.ParseVector3(reader.GetAttribute(MinAttribute));
				Camera.Max = XnaExtensions.ParseVector3(reader.GetAttribute(MaxAttribute));
				Camera.SetPosition((Camera.Min + Camera.Max) / 2.0f);
            }
        }

		private void LoadNeighbours()
		{
			var neighbours = new Dictionary<int, List<Planet>>();

			foreach (var planetLink in Links)
			{
				#region Configure neighbour planets based on links
				var sourcePlanet = Planets.First(p => p.Id == planetLink.SourcePlanet);
				var targetPlanet = Planets.First(p => p.Id == planetLink.TargetPlanet);

				List<Planet> sourceNeighbours = null;
				List<Planet> targetNeighbours = null;

				if (neighbours.Keys.Contains(sourcePlanet.Id))
				{
					sourceNeighbours = neighbours[sourcePlanet.Id];
				}
				else
				{
					sourceNeighbours = new List<Planet>();
					neighbours[sourcePlanet.Id] = sourceNeighbours;
				}

				if (!sourceNeighbours.Contains(targetPlanet))
				{
					sourceNeighbours.Add(targetPlanet);
				}


				if (neighbours.Keys.Contains(targetPlanet.Id))
				{
					targetNeighbours = neighbours[targetPlanet.Id];
				}
				else
				{
					targetNeighbours = new List<Planet>();
					neighbours[targetPlanet.Id] = targetNeighbours;
				}

				if (!targetNeighbours.Contains(sourcePlanet))
				{
					targetNeighbours.Add(sourcePlanet);
				}
				#endregion
			}

			foreach (var pair in neighbours)
			{
				int planetId = pair.Key;
				var neighbourPlanets = pair.Value;

				var planet = Planets.First(p => p.Id == planetId);
				planet.SetNeighbours(neighbourPlanets);
			}
		}

        [OnDeserialized]
        public void OnJsonDeserialized(StreamingContext context)
        {
			Camera.SetPosition((Camera.Min + Camera.Max) / 2.0f);
			LoadNeighbours();
        }

        public Map()
        {            
            Planets = new List<Planet>();
            Links = new List<PlanetLink>();
            PlanetarySystems = new List<PlanetarySystem>();
            PlayerStartingData = new List<StartingData>();
			Background = new List<BackgroundLayer>();
            Colors = new List<PlayerColor>();
        }
		public void Update(double delta, double time)
		{
			Camera.Update(delta, time);
		}
        public Planet GetPlanetById(int planetId)
        {
            return Planets.Find(planet => planet.Id == planetId);
        }
		public PlanetarySystem GetSystemByPlanetid(int planetId)
		{
			return PlanetarySystems.FirstOrDefault(x => x.Planets.Contains(planetId));
		}        
        public PlayerColor GetColorById(int colorId)
        {
            return Colors.First(c => c.ColorId == colorId);
        }
    }
}
