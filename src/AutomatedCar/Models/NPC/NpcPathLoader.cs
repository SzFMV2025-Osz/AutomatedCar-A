namespace AutomatedCar.Models.NPC
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;

    public static class NpcPathLoader
    {
        public static (List<NPCPath> Points, bool Repeating) LoadFromJson(string filePath)
        {
            var json = File.ReadAllText(filePath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            bool repeating = true;
            if (root.TryGetProperty("repeating", out var rep) && (rep.ValueKind == JsonValueKind.True || rep.ValueKind == JsonValueKind.False))
                repeating = rep.GetBoolean();

            JsonElement pointsArray = default;
            bool found = false;

            if (root.TryGetProperty("points", out var p) && p.ValueKind == JsonValueKind.Array)
            {
                pointsArray = p; found = true;
            }
            else
            {
                foreach (var prop in root.EnumerateObject())
                {
                    if (prop.Value.ValueKind == JsonValueKind.Array) { pointsArray = prop.Value; found = true; break; }
                }
            }

            var list = new List<NPCPath>();
            if (found)
            {
                foreach (var el in pointsArray.EnumerateArray())
                {
                    int x = el.GetProperty("x").GetInt32();
                    int y = el.GetProperty("y").GetInt32();
                    int rotation = el.GetProperty("rotation").GetInt32();
                    int speed = el.GetProperty("speed").GetInt32();
                    int waitMs = el.TryGetProperty("waitMs", out var waitProp) && waitProp.ValueKind == JsonValueKind.Number ? waitProp.GetInt32() : 0;

                    list.Add(new NPCPath(x, y, rotation, speed, waitMs));
                }
            }

            return (list, repeating);
        }
    }
}
