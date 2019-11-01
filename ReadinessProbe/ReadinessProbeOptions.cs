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
        public HttpStatusCode TimeoutCode { get; set; } = HttpStatusCode.GatewayTimeout;

        public bool LogNotReady { get; set; } = false;
        public bool LogError { get; set; } = true;
        public bool LogTimeout { get; set; } = true;
        public bool LogRequestAborted { get; set; } = true;
        public string LoggerCategory { get; set; } = "ReadinessProbe";

        public TimeSpan CheckAllTimeout { get; set; } = TimeSpan.FromSeconds(50);

        ReadinessProbeOptions IOptions<ReadinessProbeOptions>.Value => this;
    }
}