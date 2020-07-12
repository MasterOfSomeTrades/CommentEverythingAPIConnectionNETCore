using System;
using System.Collections.Generic;
using System.Text;

namespace CommentEverythingAPIConnectionNETCore.DataObjects {
    public class EmptyData : IDataDescription {
        public string Topic { get; set; }

        public string DescriptiveContent { get; set; }

        public string DataType { get; set; }

        public string CorrelationID { get; set; }

        public string ToDisplayString1() {
            return $"No data found for {Topic}";
        }

        public string ToDisplayString2() {
            return "No data found";
        }

        public string ToDisplayString3() {
            return "No data found";
        }
    }
}
