using System;
using System.Collections.Generic;
using System.Text;

namespace CommentEverythingAPIConnectionNETCore.DataObjects
{
    public interface IDataDescription : IData {
        string Topic { get; set; }
        string DescriptiveContent { get; }
        string DataType { get; }
        string ToDisplayString1();
        string ToDisplayString2();
        string ToDisplayString3();
        string ToString();
    }
}
