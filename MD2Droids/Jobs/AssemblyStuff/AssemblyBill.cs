using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class AssemblyBill : IExposable, IThingContainerGiver
    {
        private Blueprint _bp;
        private readonly AssemblyBillStack _stack;
        private ThingContainer _container;
        private int _workAmount;

        public AssemblyBill(AssemblyBillStack stack, Blueprint bp)
        {
            _bp = bp;
            _stack = stack;
            _container = new ThingContainer(this);
            WorkAmount = _bp.WorkAmount;
        }

        public AssemblyBill(AssemblyBillStack stack)
        {
            _stack = stack;
        }

        public Blueprint Blueprint
        {
            get { return _bp; }
        }

        public bool DesiresMaterials
        {
            get
            {
                foreach (var count in _bp.CostList)
                {
                    if (_container.NumContained(count.thingDef) < count.count)
                        return true;
                }
                return false;
            }
        }

        public ThingCount GetNextMaterial
        {
            get
            {
                foreach (var count in _bp.CostList)
                {
                    int num = _container.NumContained(count.thingDef);
                    if ( num < count.count)
                    {
                        return new ThingCount(count.thingDef, count.count - num);
                    }
                }

                return null;
            }
        }

        public int WorkAmount
        {
            get { return _workAmount; }
            set { _workAmount = value; }
        }

        public void ExposeData()
        {
            Scribe_Deep.LookDeep(ref _container, "gatheredMaterials", this);
            Scribe_Values.LookValue(ref _workAmount, "workAmount", _bp.WorkAmount);
        }

        public ThingContainer GetContainer()
        {
            return _container;
        }

        public IntVec3 GetPosition()
        {
            return _stack.Assembly.InteractionCell;
        }
    }
}
