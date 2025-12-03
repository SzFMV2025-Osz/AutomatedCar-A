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
            using var doc = JsonDocument.Parse(
                             json,
                            new JsonDocumentOptions
                 {
                    CommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                 });
            var root = doc.RootElement;

            bool repeating = true;
            if (root.TryGetProperty("repeating", out var rep) && (rep.ValueKind == JsonValueKind.True || rep.ValueKind == JsonValueKind.False))
                repeating = rep.GetBoolean();

            JsonElement pointsArray = default;
            bool found = false;

            // prefer a standard name if present
            if (root.TryGetProperty("points", out var p) && p.ValueKind == JsonValueKind.Array)
            {
                pointsArray = p; found = true;
            }
            else
            {
                // prefer explicit pedestrian/car arrays when present,
                // otherwise fall back to the first array property
                JsonElement firstArray = default;
                foreach (var prop in root.EnumerateObject())
                {
                    if (prop.Value.ValueKind != JsonValueKind.Array) continue;

                    if (prop.NameEquals("pedestrian") || prop.NameEquals("car") || prop.NameEquals("points"))
                    {
                        pointsArray = prop.Value;
                        found = true;
                        break;
                    }

                    if (firstArray.ValueKind == JsonValueKind.Undefined) firstArray = prop.Value;
                }

                if (!found && firstArray.ValueKind == JsonValueKind.Array)
                {
                    pointsArray = firstArray;
                    found = true;
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
