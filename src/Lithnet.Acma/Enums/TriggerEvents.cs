// -----------------------------------------------------------------------
// <copyright file="TriggerEvents.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;

    /// <summary>
    /// Types of events that can trigger actions on a constructor
    /// </summary>
    [Flags]
    [DataContract(Name = "triggers", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public enum TriggerEvents
    {
        /// <summary>
        /// The event is triggered on an add operation
        /// </summary>
        [EnumMember(Value = "unconfigured")]
        Unconfigured = 0,

        /// <summary>
        /// The event is triggered on an add operation
        /// </summary>
        [EnumMember(Value = "add")]
        Add = 1,

        /// <summary>
        /// The event is triggered on an update operation
        /// </summary>
        [EnumMember(Value = "update")]
        Update = 2,

        /// <summary>
        /// The event is triggered on a delete operation
        /// </summary>
        [EnumMember(Value = "delete")]
        Delete = 4,

        /// <summary>
        /// The event is triggered on an undelete operation
        /// </summary>
        [EnumMember(Value = "undelete")]
        Undelete = 8,

        /// <summary>
        /// The event is triggered on the absence of an event
        /// </summary>
        [EnumMember(Value = "none")]
        None = 16
    }
}
