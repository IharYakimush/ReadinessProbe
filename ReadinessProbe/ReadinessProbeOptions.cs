using System;
using System.Net;
using Microsoft.Extensions.Options;

namespace ReadinessProbe
{
    public class ReadinessProbeOptions : IOptions<ReadinessProbeOptions>
    {
        public HttpStatusCode ReadyCode { get; set; } = HttpStatusCode.OK;
        public HttpStatusCode NotReadyCode { get; set; } = HttpStatusCode.ServiceUnavailable;        
        public HttpStatusCode ErrorCode { get; set; } = HttpStatusCode.InternalServerError;

        public bool LogNotReady { get; set; } = false;
        public bool LogError { get; set; } = true;
        public bool LogRequestAborted { get; set; } = true;

        public string LoggerCategory { get; set; } = "ReadinessProbe";

        ReadinessProbeOptions IOptions<ReadinessProbeOptions>.Value => this;
    }
}