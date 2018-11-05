using System;
using System.Collections.Generic;
using System.Text;

namespace CommentEverythingAPIConnectionNETCore.DataObjects
{
    public interface IDataMeasurement : IData {
        decimal CurrentValue { get; }
        decimal Change { get; }
        decimal PercentChange { get; }
        string UpdateTime { get; }
        long DataVolume { get; }
        decimal StartValue { get; }

        decimal ExtendedPeriodValue { get; }
        decimal ExtendedPeriodChange { get; }
        decimal ExtendedPeriodPercentChange { get; }
        string ExtendedPeriodUpdateTime { get; }
        long ExtendedPeriodDataVolume { get; }

        decimal PreviousPeriodValue { get; }
        decimal PreviousPeriodChange { get; }
        decimal PreviousPeriodPercentChange { get; }
        long PreviousPeriodDataVolume { get; }
    }
}
