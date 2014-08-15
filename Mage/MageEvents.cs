using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Mage {

    /// <summary>
    /// Definition for a single column for standard tabular output
    /// </summary>
    public class MageColumnDef {
        /// <summary>
        /// name of the column
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// data type of the column
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// display/data size of the column (in characters)
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// is column hidden?
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// construct an empty Mage standard tabular input/outpu column definition object
        /// </summary>
        public MageColumnDef() {
            Name = string.Empty;
            DataType = string.Empty;
            Size = string.Empty;
            Hidden = false;
        }

        /// <summary>
        /// construct a Mage standard tabular input/outpu column definition object
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="type">Column data type</param>
        /// <param name="size">Column size (in characters)</param>
        public MageColumnDef(string name, string type, string size) {
            Name = name;
            DataType = type;
            Size = size;
        }

		/// <summary>
		/// Overrides the default ToString to return the column def name and data type
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Name + ": " + DataType;
		}
    }

    /// <summary>
    /// Argument for a column definition event for standard tabular output
    /// </summary>
    public class MageColumnEventArgs : EventArgs {
        private readonly List<MageColumnDef> columDefs;

        /// <summary>
        /// list of column definitions
        /// </summary>
        public Collection<MageColumnDef> ColumnDefs {
            get { return new Collection<MageColumnDef>(columDefs); }
        }

	    /// <summary>
        /// construct new MageColumnEventArgs object
        /// with given column definitions list
        /// </summary>
        /// <param name="colDefs">Column defintion list</param>
        public MageColumnEventArgs(IEnumerable<MageColumnDef> colDefs) {
            columDefs = new List<MageColumnDef>(colDefs);
        }
    }

    /// <summary>
    /// Argument for data row event for standard tabular output
    /// </summary>
    public class MageDataEventArgs : EventArgs {
		private readonly string[] fields;

        /// <summary>
        /// the event contains a data row to process
        /// (false signals end of input data stream)
        /// </summary>
        public bool DataAvailable {
            get { return fields != null; }
        }

        /// <summary>
        /// data row
        /// </summary>
		public string[] Fields
		{
            get { return fields; }
        }

        /// <summary>
        /// construct new MageDataEventArgs object with given data row
        /// </summary>
        /// <param name="data"></param>
		public MageDataEventArgs(string[] data)
		{
            fields = data;
        }
    }

    /// <summary>
    /// Argument for a status message event
    /// </summary>
    public class MageStatusEventArgs : EventArgs {

        /// <summary>
        /// message text
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// error code
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// construct a new MageStatusEventArgs object
        /// with given message text
        /// </summary>
        /// <param name="msg"></param>
        public MageStatusEventArgs(string msg) {
            Message = msg;
            ErrorCode = 0;
        }

        /// <summary>
        /// construct a new MageStatusEventArgs object
        /// with given message text and error code
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="errorCode"></param>
        public MageStatusEventArgs(string msg, int errorCode) {
            Message = msg;
            ErrorCode = errorCode;
        }
    }

    /// <summary>
    /// An exception type to call our own
    /// </summary>
    [Serializable]
    public class MageException : Exception {

	    /// <summary>
        /// construct a new MageException object
        /// with the given exception message
        /// </summary>
        public MageException(string message)
            : base(message) {
        }

        /// <summary>
        /// construct a new MageException object
        /// from contents of .Net Exception object
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public MageException(string message, Exception innerException)
            : base(message, innerException) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected MageException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

    }

}
