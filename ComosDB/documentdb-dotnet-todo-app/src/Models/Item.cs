namespace todo.Models
{
    using Microsoft.Azure.Documents;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "workingDetail")]
        public List<WorkingDetail> WorkingDetail { set; get; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "isComplete")]
        public bool Completed { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }
    }

    public class WorkingDetail
    {
        [JsonProperty(PropertyName = "workingDetailId")]
        public string WorkingDetailId { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }

        [JsonProperty(PropertyName = "workingDate")]
        public DateTime WorkingDate { get; set; }
    }
}