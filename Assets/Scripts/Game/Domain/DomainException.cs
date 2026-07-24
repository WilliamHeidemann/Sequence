using System;

namespace Game.Domain
{
    public class DomainException : Exception
    {
        public DomainException() 
            : base("A game rule violation occurred.") { }

        public DomainException(string message) 
            : base(message) { }
    }
}