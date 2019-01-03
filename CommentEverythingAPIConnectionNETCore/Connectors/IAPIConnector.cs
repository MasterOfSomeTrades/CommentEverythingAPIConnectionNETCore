using CommentEverythingAPIConnectionNETCore.DataObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommentEverythingAPIConnectionNETCore.Connectors
{
    public interface IAPIConnector {
        List<string> _urlFormat { get; }
        List<string[]> _headers { get; }
        string Name { get; }
        int WaitTime { get; }

        Task<IData> DoUpdate(string requestData = "", string[] requestDataArray = null);

        Task<Dictionary<string, IData>> DoUpdate(IList<string> requestData);

        /// <summary>
        /// Implemented method should deserialize the JSON to its respective object (e.g. Stock, Option, etc.).
        /// The JSON object should then be converted to an InvestmentData object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns>InvestmentData object</returns>
        IData ConvertJSONToDataObject(List<string> json);

        /// <summary>
        /// Get a JSON response from a REST data service for a given symbol (uses WebClient).
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        Task<List<string>> GetJSONResponse(string requestData);
    }
}
