using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using QuestomAssets.AssetOps;
using QuestomAssets.AssetsChanger;
using QuestomAssets.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.Mods
{
    public class ModDefinition
    {
        /// <summary>
        /// The list of individual components of this mod
        /// </summary>
        [JsonProperty("components")]
        public List<ModComponent> Components { get; set; } = new List<ModComponent>();

        public virtual bool ShouldSerializeComponents()
        {
            return false;
        }

        public List<AssetOp> GetInstallOps(ModContext context)
        {
            List<AssetOp> ops = new List<AssetOp>();
            using (new LogTiming($"Installing mod ID {ID}"))
            {
                if (Components == null || Components.Count < 1)
                {
                    Log.LogErr($"The mod with ID {ID} has no components to install.");
                    throw new Exception($"The mod with ID {ID} has no components to install.");
                }
                try
                {
                    foreach (var comp in Components)
                    {
                        ops.AddRange(comp.GetInstallOps(context));
                    }
                }
                catch (Exception ex)
                {
                    Log.LogErr($"Mod ID {ID} threw an exception while installing.", ex);
                    throw new Exception($"Mod ID {ID} failed to install.", ex);
                }
            }
            ops.Add(new ModStatusOp(this, ModStatusType.Installed));
            return ops;
        }

        public List<AssetOp> GetUninstallOps(ModContext context)
        {
            List<AssetOp> ops = new List<AssetOp>();
            using (new LogTiming($"Uninstalling mod ID {ID}"))
            {
                if (Components == null || Components.Count < 1)
                {
                    Log.LogErr($"The mod with ID {ID} has no components to uninstall.");
                    throw new Exception($"The mod with ID {ID} has no components to uninstall.");
                }
                try
                {
                    foreach (var comp in Components)
                    {
                        ops.AddRange(comp.GetUninstallOps(context));
                    }
                }
                catch (Exception ex)
                {
                    Log.LogErr($"Mod ID {ID} threw an exception while uninstalling.", ex);
                    throw new Exception($"Mod ID {ID} failed to uninstall.", ex);
                }
            }
            ops.Add(new ModStatusOp(this, ModStatusType.NotInstalled));
            return ops;
        }

        public ModDefinition ToBase()
        {
            return JsonConvert.DeserializeObject<ModDefinition>(JsonConvert.SerializeObject(this));
        }

        private ModStatusType _status;

        /// <summary>
        /// Gets or sets the installation status of the mod
        /// </summary>
        public ModStatusType Status
        {
            get
            {
                return _status;
            }
            set
            {
                bool change = (_status == value);
                _status = value;
                if (change)
                    PropChanged(nameof(Status));
            }
        }

        public string Platform { get; set; }

        /// <summary>
        /// Unique identifier of this mod
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The (display) name of the mod
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The filename of the cover image of the mod.
        /// </summary>
        public string CoverImageFilename { get; set; }

        /// <summary>
        /// The author of the mod
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The porter of the mod
        /// </summary>
        public string Porter { get; set; }

        /// <summary>
        /// The mod version
        /// </summary>
        public string Version { get; set; }

        #region json properties for supporting beatmods2 format

        public string version
        {
            get
            {
                return Version;
            }
            set
            {
                Version = value;
            }
        }

        public Dictionary<string, string> links { get; set; } = new Dictionary<string, string>();

        public List<string> description{ get; set; } = new List<string>();

        public string gameVersion
        {
            get
            {
                return TargetBeatSaberVersion;
            }
            set
            {
                TargetBeatSaberVersion = value;
            }
        }
        
        public string platform
        {
            get
            {
                return Platform;
            }
            set
            {
                Platform = value;
            }
        }

        public string id
        {
            get
            {
                return ID;
            }
            set
            {
                ID = value;
            }
        }
        
        public string name
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
            }
        }
        public string coverImageFilename
        {
            get
            {
                return CoverImageFilename;
            }
            set
            {
                CoverImageFilename = value;
            }
        }

        public string icon
        {
            get
            {
                return CoverImageFilename;
            }
            set
            {
                CoverImageFilename = value;
            }
        }

        public string porter
        {
            get
            {
                return Porter;
            }
            set
            {
                Porter = value;
            }
        }

        public string author
        {
            get
            {
                return Author;
            }
            set
            {
                Author = value;
            }
        }

        public ModCategory category
        {
            get
            {
                return Category;
            }
            set
            {
                Category = value;
            }
        }

        #endregion

        /// <summary>
        /// A link to more information about the mod
        /// </summary>
        public string InfoUrl {
            get
            {
                if (links == null || links.Count < 1)
                    return null;

                if (links.ContainsKey("project-home"))
                    return links["project-home"];

                return links.First().Value;                
            }
        }

        /// <summary>
        /// The description of the mod
        /// </summary>
        public string Description {
            get
            {
                if (description == null || description.Count < 1)
                    return null;
                return String.Join("\n", description);
            }
            set
            {
                if (description == null)
                    description = new List<string>();

                description.Clear();

                if (value == null)
                    return;
                description.AddRange(value.Split('\n'));
            }
        }

        /// <summary>
        /// The category this mod falls into for display and organizational purposes
        /// </summary>
        public ModCategory Category { get; set; }

        /// <summary>
        /// The version of Beat Saber that this mod was designed for
        /// </summary>
        public string TargetBeatSaberVersion { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void PropChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public virtual bool ShouldSerializePlatform() => true;
        public virtual bool ShouldSerializeStatus() => true;
        public virtual bool ShouldSerializeID() => true;
        public virtual bool ShouldSerializeName() => true;
        public virtual bool ShouldSerializeCoverImageFilename() => true;
        public virtual bool ShouldSerializeAuthor() => true;
        public virtual bool ShouldSerializePorter() => true;
        public virtual bool ShouldSerializeVersion() => true;
        public virtual bool ShouldSerializeTargetBeatSaberVersion() => true;
        public virtual bool ShouldSerializeDescription() => true;
        public virtual bool ShouldSerializeInfoUrl() => true;
        public virtual bool ShouldSerializeCategory() => true;
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ModStatusType
    {
        NotInstalled,
        Installed
    }
}
