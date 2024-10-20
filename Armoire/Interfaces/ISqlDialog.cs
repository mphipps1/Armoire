namespace Armoire.Interfaces;

public interface ISqlDialog
{
    public string Heading0 { get; set; }
    public string Heading1 { get; set; }
    public string Heading2 { get; set; }
    public string Body1 { get; set; }
    public string Body2 { get; set; }
    public long LastRowId { get; set; }
    public string? ValueToAdd1 { get; set; }
    public string? ValueToAdd2 { get; set; }

    public void AppendToBody1(string str);
    public void AppendToBody2(string str);
    public void AddToDb();
    public void ReadFromDb();
}
