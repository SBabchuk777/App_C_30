using System.ComponentModel;

public enum ReqType
{
    [Description("Init Request")]
    init,
    [Description("Start Request")]
    start,
    [Description("Update Request")]
    update,
    [Description("Track Request")]
    track
}