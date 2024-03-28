namespace LightPoint.IdentityServer.Blazor.Components.Tables
{
    public class TableColumnInfo
    {
        public bool Sortable { get; set; }

        public string? Width { get; set; }

        public string? Header { get; set; }

        public string? Prop { get; set; }

        public bool HasSlot { get; set; }

        public string? Fixed { get; set; }
    }
}
