using System;
using System.Runtime.Serialization;

namespace AutoFactory
{
    /// <summary>
    /// AutoFactory Exception class.
    /// </summary>
    public class AutoFactoryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoFactoryException"/> class.
        /// </summary>
        public AutoFactoryException()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AutoFactoryException(string message) : base(message)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoFactoryException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public AutoFactoryException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
