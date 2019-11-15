using System;

namespace K1.CommonInterfaces
{
    public interface ILogger
    {
        void Trace(string message);
        void Trace(string message,Guid traceLogID);
        void Trace(string message, Guid traceLogID,object data);

        void Info(string message);
        void Info(string message, Guid traceLogID);
        void Info(string message, Guid traceLogID, object data);

        void Error(string message);
        void Error(string message, Exception exception);
        void Error(string message, Guid traceLogID);
        void Error(string message, Guid traceLogID, Exception exception);
        void Error(string message, Guid traceLogID, Exception exception, object data);
    }
}
