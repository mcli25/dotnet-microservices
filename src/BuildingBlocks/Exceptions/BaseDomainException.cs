using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingBlocks.Exceptions
{
    public abstract class BaseDomainException : Exception
    {
        public abstract int StatusCode { get; }

        protected BaseDomainException(string message) : base(message) { }
        
    }
}