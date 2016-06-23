// -----------------------------------------------------------------------
// <copyright file="RaisedEvent.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// An instance of an MAEvent that was raised by an object
    /// </summary>
    public class RaisedEvent
    {
        /// <summary>
        /// Initializes a new instance of the RaisedEvent class
        /// </summary>
        private RaisedEvent()
        {
            this.RaisedEventProperties = new Dictionary<string, object>();
        }

        public RaisedEvent(AcmaEvent eventObject)
            : this()
        {
            this.Event = eventObject;
            this.Event.OnEventRaised(this, null);
        }

        /// <summary>
        /// Initializes a new instance of the RaisedEvent class
        /// </summary>
        /// <param name="eventObject">The event that is being raised</param>
        /// <param name="eventSource">The object that raised the event</param>
        public RaisedEvent(AcmaEvent eventObject, MAObjectHologram eventSource)
            : this()
        {
            this.Event = eventObject;
            this.Source = eventSource;
            this.Event.OnEventRaised(this, eventSource);
        }

        /// <summary>
        /// Gets the MAEvent that was raised
        /// </summary>
        public AcmaEvent Event { get; private set; }

        public string EventID
        {
            get
            {
                return this.Event.ID;
            }
        }

        public Dictionary<string, object> RaisedEventProperties { get; private set; }

        /// <summary>
        /// Gets the object that raised the event
        /// </summary>
        public MAObjectHologram Source { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            if (this.Source != null)
            {
                return this.Event.ID + " raised by " + this.Source.DisplayText;
            }
            else
            {
                return this.Event.ID;
            }
        }
    }
}
