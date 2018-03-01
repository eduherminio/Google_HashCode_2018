using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Model
{
    public class Relationship
    {
        public long VehicleId { get; set; }
        public long StepWhereVehicleWillBeFree { get; set; }

        public Relationship(long id, long stepdef = -1)
        {
            VehicleId = id;
            StepWhereVehicleWillBeFree = stepdef; // free
        }
    }
}
