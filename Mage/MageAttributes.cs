using System;

namespace Mage {

    /// <summary>
    /// Defintion of custom attribute than can be used to decorate
    /// Mage modules and associated GUI parameter panels to allow discovery
    /// at runtime
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class MageAttribute : Attribute {

        /// <summary>
        /// type of module
        /// </summary>
        public string ModType { get; set; }

        /// <summary>
        /// unique identifier (used to associate module with its parameter panel)
        /// </summary>
        public string ModID { get; set; }

        /// <summary>
        /// short public identifier to appear in selection lists
        /// </summary>
        public string ModLabel { get; set; }

        /// <summary>
        /// longer description to appear in selection lists (optional)
        /// </summary>
        public string ModDescription { get; set; }

        /// <summary>
        /// name of class (used for dynamic loading)
        /// </summary>
        public string ModClassName { get; set; }

        /// <summary>
        /// get current module type
        /// </summary>
        public string type { get { return ModType; } }

        /// <summary>
        /// get current module ID
        /// </summary>
        public string ID { get { return ModID; } }

        /// <summary>
        /// get current short label
        /// </summary>
        public string label { get { return ModLabel; } }

        /// <summary>
        /// get current decriptions
        /// </summary>
        public string description { get { return ModDescription; } }

        /// <summary>
        /// construct new MageAttribute object
        /// </summary>
        /// <param name="type">attribute type</param>
        /// <param name="ID">attribute ID</param>
        /// <param name="label">attribute label</param>
        /// <param name="description">attribute description</param>
        public MageAttribute(string type, string ID, string label, string description) {
            ModID = ID;
            ModLabel = label;
            ModType = type;
            ModDescription = description;
        }
    }
}
