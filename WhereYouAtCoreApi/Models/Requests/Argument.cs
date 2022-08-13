namespace WhereYouAtCoreApi.Models.Requests
{
    public class Argument
    {
        public string name { get; set; }
        public object value { get; set; }

        public Argument(string name, object value)
        {
            this.name = name;
            this.value = value;
        }

    }
}
