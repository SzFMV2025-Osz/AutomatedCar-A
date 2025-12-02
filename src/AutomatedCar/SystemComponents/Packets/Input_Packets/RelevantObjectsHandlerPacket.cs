namespace AutomatedCar.SystemComponents.Packets.Input_Packets
{
    using AutomatedCar.Models;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RelevantObjectsHandlerPacket : ReactiveObject, IReadOnlyRelevantObjects
    {
        List<RelevantObject> relevantObjects;
        int limitSpeed;
        public int LimitSpeed
        {
            get
            {
                return this.LimitSpeed;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref this.limitSpeed, value);
            }
        }
        public List<RelevantObject> RelevantObjects
        {
            get 
            {
                return this.relevantObjects;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref this.relevantObjects, value);
            }
        }
    }
}
