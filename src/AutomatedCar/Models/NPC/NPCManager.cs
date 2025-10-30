namespace AutomatedCar.Models.NPC
{
    using System.Collections.Generic;
    using System.Diagnostics;

    public class NPCManager : GameBase
    {
        private readonly List<INPC> npcs = new();
        private readonly Stopwatch sw = Stopwatch.StartNew();
        private long lastTicks;

        public NPCManager() { lastTicks = sw.ElapsedTicks; }

        public void Add(INPC npc) => npcs.Add(npc);

        protected override void Tick()
        {
            long now = sw.ElapsedTicks;
            double dt = (now - lastTicks) / (double)Stopwatch.Frequency;
            lastTicks = now;

            // extrém frame ugrások ellen
            if (dt <= 0 || dt > 0.25) dt = 1.0 / 60.0;

            foreach (var n in npcs) n.Move(dt);
        }
    }
}
