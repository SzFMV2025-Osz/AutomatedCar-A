namespace AutomatedCar.Models.NPC
{
    public class NPCPath
    {
        public NPCPath(int x, int y, int rotation, int speed, int waitMs = 0)
        {
            X = x; Y = y; Rotation = rotation; Speed = speed; WaitMs = waitMs;
        }

        public int X { get; }
        public int Y { get; }
        public int Rotation { get; }  
        public int Speed { get; }      
        public int WaitMs { get; }    
    }
}
