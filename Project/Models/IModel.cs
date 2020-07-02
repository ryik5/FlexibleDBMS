namespace FlexibleDBMS
{
    public interface IModel
    {
        int ID { get; set; }
        string Name { get; set; }
        string Alias { get; set; }
    }
}
