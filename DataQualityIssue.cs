using System;
using System.Collections.Generic;
using System.Text;

namespace ChaosSeparatedValues
{
    public record DataQualityIssue(
        string Id,
        string Field,
        string Message);
}
