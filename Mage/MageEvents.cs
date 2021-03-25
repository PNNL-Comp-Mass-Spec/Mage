using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Mage
{
    /// <summary>
    /// Definition for a single column for standard tabular output
    /// </summary>
    public class MageColumnDef
    {
        /// <summary>
        /// Name of the column
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Data type of the column
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Display/data size of the column (in characters)
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Is column hidden?
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Construct an empty Mage standard tabular input/output column definition object
        /// </summary>
        public MageColumnDef()
        {
            Name = string.Empty;
            DataType = string.Empty;
            Size = string.Empty;
            Hidden = false;
        }

        /// <summary>
        /// Construct a Mage standard tabular input/output column definition object
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="type">Column data type</param>
        /// <param name="size">Column size (in characters)</param>
        public MageColumnDef(string name, string type, string size)
        {
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
    public class MageColumnEventArgs : EventArgs
    {
        private readonly List<MageColumnDef> columnDefs;

        /// <summary>
        /// List of column definitions
        /// </summary>
        public Collection<MageColumnDef> ColumnDefs => new(columnDefs);

        /// <summary>
        /// Construct new MageColumnEventArgs object
        /// with given column definitions list
        /// </summary>
        /// <param name="colDefs">Column definition list</param>
        public MageColumnEventArgs(IEnumerable<MageColumnDef> colDefs)
        {
            columnDefs = new List<MageColumnDef>(colDefs);
        }
    }

    /// <summary>
    /// Argument for data row event for standard tabular output
    /// </summary>
    public class MageDataEventArgs : EventArgs
    {
        private readonly string[] fields;

        /// <summary>
        /// The event contains a data row to process
        /// (false signals end of input data stream)
        /// </summary>
        public bool DataAvailable => fields != null;

        /// <summary>
        /// Data row
        /// </summary>
        public string[] Fields => fields;

        /// <summary>
        /// Construct new MageDataEventArgs object with given data row
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
    public class MageStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Message text
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Error code
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// Construct a new MageStatusEventArgs object
        /// with given message text
        /// </summary>
        /// <param name="msg">Message</param>
        public MageStatusEventArgs(string msg)
        {
            Message = msg;
            ErrorCode = 0;
        }

        /// <summary>
        /// Construct a new MageStatusEventArgs object
        /// with given message text and error code
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="errorCode">Error code</param>
        public MageStatusEventArgs(string msg, int errorCode)
        {
            Message = msg;
            ErrorCode = errorCode;
        }
    }

    /// <summary>
    /// Argument for an Exception
    /// </summary>
    public class MageExceptionEventArgs : MageStatusEventArgs
    {
        /// <summary>
        /// Message text
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Construct a new MageStatusEventArgs object
        /// with given message text
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="ex">Exception</param>
        public MageExceptionEventArgs(string msg, Exception ex) : base(msg)
        {
            Exception = ex;
        }

        /// <summary>
        /// Construct a new MageStatusEventArgs object
        /// with given message text and error code
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="ex">Exception</param>
        /// <param name="errorCode">Error code</param>
        public MageExceptionEventArgs(string msg, int errorCode, Exception ex) : base(msg, errorCode)
        {
            Exception = ex;
        }
    }

    /// <summary>
    /// An exception type to call our own
    /// </summary>
    [Serializable]
    public class MageException : Exception
    {
        /// <summary>
        /// Construct a new MageException object
        /// with the given exception message
        /// </summary>
        public MageException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Construct a new MageException object
        /// from contents of .Net Exception object
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public MageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected MageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
