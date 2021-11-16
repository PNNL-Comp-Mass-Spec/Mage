using System;

namespace Mage
{
    /// <summary>
    /// Definition of custom attribute than can be used to decorate
    /// Mage modules and associated GUI parameter panels to allow discovery
    /// at runtime
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class MageAttribute : Attribute
    {
        // Ignore Spelling: Mage

        /// <summary>
        /// Type of module
        /// </summary>
        public string ModType { get; set; }

        /// <summary>
        /// Unique identifier (used to associate module with its parameter panel)
        /// </summary>
        public string ModID { get; set; }

        /// <summary>
        /// Short public identifier to appear in selection lists
        /// </summary>
        public string ModLabel { get; set; }

        /// <summary>
        /// Longer description to appear in selection lists (optional)
        /// </summary>
        public string ModDescription { get; set; }

        /// <summary>
        /// Name of class (used for dynamic loading)
        /// </summary>
        public string ModClassName { get; set; }

        /// <summary>
        /// Get current module type
        /// </summary>
        public string type => ModType;

        /// <summary>
        /// Get current module ID
        /// </summary>
        public string ID => ModID;

        /// <summary>
        /// Get current short label
        /// </summary>
        public string label => ModLabel;

        /// <summary>
        /// Get current descriptions
        /// </summary>
        public string description => ModDescription;

        /// <summary>
        /// Construct new MageAttribute object
        /// </summary>
        /// <param name="type">attribute type</param>
        /// <param name="id">attribute ID</param>
        /// <param name="label">attribute label</param>
        /// <param name="description">attribute description</param>
        public MageAttribute(string type, string id, string label, string description)
        {
            ModType = type;
            ModID = id;
            ModLabel = label;
            ModDescription = description;
        }
    }
}
