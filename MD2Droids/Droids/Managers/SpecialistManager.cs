using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class SpecialistManager : IExposable
    {
        private Droid parent;
        private List<SpecialistWorker> specialistWorkers = new List<SpecialistWorker>();

        public SpecialistManager(Droid droid)
        {
            parent = droid;
        }

        public List<SpecialistWorker> SpecialistWorkers
        {
            get
            {
                return specialistWorkers;
            }
        }

        public T GetWorker<T>() where T : SpecialistWorker
        {
            foreach (var worker in SpecialistWorkers)
            {
                T t = worker as T;
                if (t != null) return t;
            }
            return (T)((object)null);
        }

        public void AddWorker(SpecialistWorker worker)
        {
            bool flag = false;
            foreach (var w in specialistWorkers)
            {
                if (worker.GetType() == w.GetType())
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                specialistWorkers.Add(worker);
            }
        }

        public IEnumerable<Gizmo> GetSpecialistGizmos()
        {
            foreach (var worker in SpecialistWorkers)
            {
                foreach (var g in worker.GetGizmos())
                    yield return g;
            }
        }

        public void SpawnSetup()
        {
            foreach (var worker in SpecialistWorkers)
            {
                worker.SpawnSetup();
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.LookList(ref this.specialistWorkers, "specialistWorkers", LookMode.Deep, new object[] { parent });
        }
    }
}
