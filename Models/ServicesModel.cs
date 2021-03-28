using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace Office365HealthPage.Models
{
    public class MSService
    {
        public string ServiceName { get; set; }
        public string Status { get; set; }
    }

    public class FeatureStatus
    {

        [JsonProperty("FeatureDisplayName")]
        public string FeatureDisplayName { get; set; }

        [JsonProperty("FeatureName")]
        public string FeatureName { get; set; }

        [JsonProperty("FeatureServiceStatus")]
        public string FeatureServiceStatus { get; set; }

        [JsonProperty("FeatureServiceStatusDisplayName")]
        public string FeatureServiceStatusDisplayName { get; set; }
    }

    public class Value
    {

        [JsonProperty("FeatureStatus")]
        public IList<FeatureStatus> FeatureStatus { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("IncidentIds")]
        public IList<string> IncidentIds { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("StatusDisplayName")]
        public string StatusDisplayName { get; set; }

        [JsonProperty("StatusTime")]
        public DateTime StatusTime { get; set; }

        [JsonProperty("Workload")]
        public string Workload { get; set; }

        [JsonProperty("WorkloadDisplayName")]
        public string WorkloadDisplayName { get; set; }
    }

    public class Example
    {

        [JsonProperty("value")]
        public IList<Value> Value { get; set; }
    }
}
