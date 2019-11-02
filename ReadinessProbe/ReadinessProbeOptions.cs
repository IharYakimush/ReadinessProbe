using System;
using System.Net;

namespace ReadinessProbe
{
    public class ReadinessProbeOptions 
    {
        public HttpStatusCode ReadyCode { get; set; } = HttpStatusCode.OK;
        public HttpStatusCode NotReadyCode { get; set; } = HttpStatusCode.ServiceUnavailable;        
        public HttpStatusCode ErrorCode { get; set; } = HttpStatusCode.InternalServerError;

        public bool LogNotReady { get; set; } = false;
        public bool LogError { get; set; } = true;
        public bool LogRequestAborted { get; set; } = true;

        public string LoggerCategory { get; set; } = "ReadinessProbe";
    }
}