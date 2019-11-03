using CommentEverythingAPIConnectionNETCore.DataObjects;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CommentEverythingAPIConnectionNETCore.Connectors
{
    public abstract class POSTConnector : IAPIConnector {
        public abstract List<string> _urlFormat { get; }
        public abstract List<string[]> _headers { get; }
        protected abstract string _name { get; } // --- Set to name of Connector in concrete implementation
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
        protected abstract int _concurrentCalls { get; }
        public int ConcurrentCalls {
            get {
                return _concurrentCalls;
            }
        }


        public virtual async Task<IData> DoUpdate(string requestData = "", string[] requestDataArray = null) {
            IData theData = ConvertJSONToDataObject(await GetJSONResponse(FormatRequest(requestData, requestDataArray)));
            ((IDataDescription) theData).Topic = requestData;
            return theData;
        }

        protected virtual async Task<IData> DoUpdateNoWait(string requestData, string[] requestDataArray = null) {
            IData theData = null;
            int retry = 0;
            while (retry < 10) {
                try {
                    theData = ConvertJSONToDataObject(await GetJSONResponse(requestData));
                    retry = 100;
                } catch (Exception ex) {
                    retry++;
                    if (retry == 10) {
                        throw new ApplicationException("ERROR - Maximum retries of " + retry.ToString() + " exceeded - " + ex.Message + ex.StackTrace);
                    }
                    await Task.Delay(_waitTime);
                }
            }
            ((IDataDescription) theData).Topic = requestData;
            return theData;
        }

        public virtual async Task<Dictionary<string, IData>> DoUpdate(IList<string> requestData) {
            Dictionary<string, IData> result = new Dictionary<string, IData>();
            List<Task<IData>> tasks = new List<Task<IData>>();

            for (int i = 0; i < requestData.Count; i++) {
                tasks.Add(DoUpdateNoWait(requestData[i]));
                if (((i + 1) % _concurrentCalls) == 0 || i == requestData.Count - 1) {
                    IData[] tempData = await Task.WhenAll(tasks.ToArray());
                    int index = -1;
                    foreach (IData dataResult in tempData) {
                        index++;
                        if (((IDataDescription) dataResult).Topic is null) {
                            ((IDataDescription) dataResult).Topic = requestData[index];
                        }
                        result.TryAdd(((IDataDescription) dataResult).Topic, dataResult);
                    }
                    tasks = new List<Task<IData>>();
                    await Task.Delay(_waitTime);
                }
            }

            return result;
        }

        /// <summary>
        /// Implemented method should deserialize the JSON to its respective object (e.g. Stock, Option, etc.).
        /// The JSON object should then be converted to an InvestmentData object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns>InvestmentData object</returns>
        public abstract IData ConvertJSONToDataObject(List<string> json);

        /// <summary>
        /// Request data to be POSTed may need to be formatted into JSON.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        protected abstract string FormatRequest(string requestData = "", string[] requestDataArray = null);

        /// <summary>
        /// Get a JSON response from a REST data service for a given symbol (uses WebClient).
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public virtual async Task<List<string>> GetJSONResponse(string requestData) {
            List<string> jsonResults = new List<string>();
            string json = "";

            try {
                foreach (string urlStr in _urlFormat) {
                    using (WebClient web = new WebClient()) {
                        foreach (string[] sArray in _headers) { // TODO: same headers apply to all connections - should have separate headers for each connection
                            web.Headers.Add(sArray[0], sArray[1]);
                        }

                        json = await web.UploadStringTaskAsync(urlStr, "POST", requestData);
                        jsonResults.Add(json);
                    }
                }
            } catch (WebException wex) {
                throw new ApplicationException("ERROR updating data in " + Name + " - check URL or service availability - caused by: " + wex.Message);
            } catch (Exception ex) {
                throw new ApplicationException("ERROR updating data in " + Name + " - caused by: " + ex.Message);
            }

            return jsonResults;
        }
    }
}
