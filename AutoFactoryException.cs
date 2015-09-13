using System;
using System.Runtime.Serialization;

namespace AutoFactory
{
    /// <summary>
    /// AutoFactory Exception class.
    /// </summary>
    [Serializable]
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
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected AutoFactoryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
