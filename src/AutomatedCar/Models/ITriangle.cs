namespace AutomatedCar.Models

{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ITriangle
    {

        double Angle { get; }
        double Distance { get; }

        List<WorldObject> GetIntersections();

        void UpdateRelativePosition();

        void Tick();
    }
}
