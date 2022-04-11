using System.ComponentModel;
public class GetSourceConsumersRequestBody
{
    public int sourceid { get; set; }
    public long client { get; set; }

    [DefaultValue(null)]
    public string jsonData { get; set; } 

}
