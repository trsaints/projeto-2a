using System;

namespace Agendai.Services.Recurrence
{
    public interface ICloneableWithDate
    {
        DateTime Due { get; set; }
        object Clone();
    }
}