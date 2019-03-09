using System;

[Serializable()]
public class UnimplementedCaseException : System.Exception
{
    public UnimplementedCaseException() : base() { }
    public UnimplementedCaseException(string message) : base(message) { }
    public UnimplementedCaseException(string message, System.Exception inner) : base(message, inner) { }

    // A constructor is needed for serialization when an
    // exception propagates from a remoting server to the client. 
    protected UnimplementedCaseException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}