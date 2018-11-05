using CommentEverythingAPIConnectionNETCore.DataObjects;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CommentEverythingAPIConnectionNETCore.Connectors
{
    public abstract class GETConnector : IAPIConnector {
        public abstract List<string> _urlFormat { get; }
        public abstract List<string[]> _headers { get; }
        public abstract string _name { get; } // --- Set to name of Connector in concrete implementation
        public string Name {
            get {
                return _name;
            }
        }
        protected abstract int _waitTime { get; }
        public int WaitTime {
            get {
                return _waitTime;
            }
        }

        public virtual IData DoUpdate(string requestData, string[] requestDataArray = null) {
            Task.Delay(WaitTime).GetAwaiter().GetResult();

            IData theData = ConvertJSONToDataObject(GetJSONResponse(requestData));
            ((IDataDescription) theData).Topic = requestData;
            return theData;
        }

        /// <summary>
        /// Implemented method should deserialize the JSON to its respective object (e.g. Stock, Option, etc.).
        /// The JSON object should then be converted to an InvestmentData object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns>InvestmentData object</returns>
        public abstract IData ConvertJSONToDataObject(List<string> json);

        /// <summary>
        /// Get a JSON response from a REST data service for a given symbol (uses WebClient).
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public virtual List<string> GetJSONResponse(string requestData) {
            List<string> jsonResults = new List<string>();
            string json = "";
            StringBuilder sb = new StringBuilder();

            try {
                foreach (string urlStr in _urlFormat) {
                    //requestData = WebUtility.UrlEncode(requestData);
                    sb = new StringBuilder();
                    sb.AppendFormat(urlStr, requestData);
                    using (WebClient web = new WebClient()) {
                        foreach (string[] sArray in _headers) { // TODO: same headers apply to all connections - should have separate headers for each connection
                            web.Headers.Add(sArray[0], sArray[1]);
                        }

                        json = web.DownloadString(sb.ToString());
                        jsonResults.Add(json);
                    }
                }
            } catch (WebException wex) {
                throw new ApplicationException("ERROR updating data (URL: " + sb.ToString() + ") in " + Name + " - check URL or service availability - caused by: " + wex.Message);
            } catch (Exception ex) {
                throw new ApplicationException("ERROR updating data in " + Name + " - caused by: " + ex.Message);
            }

            return jsonResults;
        }
    }
}
