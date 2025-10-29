namespace AutomatedCar.Models.NPC
{
    using System.Collections.Generic;

    public interface INPC
    {
        IReadOnlyList<NPCPath> Points { get; }
        bool Repeating { get; }
        int CurrentPoint { get; }

        /// <summary>Aktuális szakasz sebessége px/s.</summary>
        int Speed { get; }

        /// <summary>Visszafelé kompatibilis tick (ha nem akarsz dt-t számolni).</summary>
        void Move();

        /// <summary>Időalapú mozgás dt másodperccel.</summary>
        void Move(double dt);
    }
}
