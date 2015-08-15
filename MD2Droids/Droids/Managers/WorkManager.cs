using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class WorkManager : IExposable
    {
        private Droid parent;
        public SpecialistManager specialist;
        private List<WorkPackageDef> workPackages = new List<WorkPackageDef>();
        private List<WorkTypeDef> workTypesCache = null;
        private float? powerNeedsCache = null;
        public List<WorkGiver> workGiversInOrderCache = null;
        public List<WorkGiver> workGiversInOrderEmergencyCache = null;

        public WorkManager(Droid droid)
        {
            parent = droid;
        }

        public WorkManager(List<WorkPackageDef> workPackages, Droid droid)
            : this(droid)
        {
            specialist = new SpecialistManager(droid);
            foreach(var package in workPackages)
            {
                AddWorkPackage(package);
            }
        }

        public int Count
        {
            get
            {
                return AllWorkTypes.Count;
            }
        }

        public float PowerNeeds
        {
            get
            {
                if (powerNeedsCache == null)
                {
                    float num = 0f;
                    foreach (var package in AllWorkPackages)
                    {
                        num += package.PowerRequirement;
                    }
                    powerNeedsCache = num;
                }
                return (float)powerNeedsCache;
            }
        }

        public List<WorkTypeDef> AllWorkTypes
        {
            get
            {
                if (workTypesCache == null)
                {
                    workTypesCache = new List<WorkTypeDef>();
                    foreach (var package in AllWorkPackages)
                    {
                        foreach (var workType in package.workTypes)
                        {
                            if (!workTypesCache.Contains(workType))
                            {
                                workTypesCache.Add(workType);
                            }
                        }
                    }
                }
                return workTypesCache;
            }
        }

        public List<WorkPackageDef> AllWorkPackages
        {
            get
            {
                return workPackages;
            }
        }

        public List<WorkGiver> WorkGiversInOrder
        {
            get
            {
                if (workGiversInOrderCache == null)
                {
                    List<WorkGiver> list = new List<WorkGiver>();

                    foreach (var wg in parent.workSettings.WorkGiversInOrderNormal)
                    {
                        if (this.Contains(wg.def.workType))
                        {
                            list.Add(wg);
                        }
                    }
                    workGiversInOrderCache = list;
                }
                return workGiversInOrderCache;
            }
        }

        public List<WorkGiver> WorkGiversInOrderEmergency
        {
            get
            {
                if (workGiversInOrderEmergencyCache == null)
                {
                    List<WorkGiver> list = new List<WorkGiver>();

                    foreach (var wg in parent.workSettings.WorkGiversInOrderEmergency)
                    {
                        if (this.Contains(wg.def.workType))
                        {
                            list.Add(wg);
                        }
                    }

                    workGiversInOrderEmergencyCache = list;
                }
                return workGiversInOrderEmergencyCache;
            }
        }

        public IEnumerable<WorkTags> AllRequiredWorkTags
        {
            get
            {
                List<WorkTags> list = new List<WorkTags>();
                foreach (var def in this)
                {
                    foreach (WorkTags v in Enum.GetValues(typeof(WorkTags)))
                    {
                        if ((def.workTags & v) == v)
                        {
                            if (!list.Contains(v))
                                list.Add(v);
                        }
                    }
                }
                return list.AsEnumerable();
            }
        }

        public void AddWorkPackage(WorkPackageDef package)
        {
            if (!workPackages.Contains(package))
            {
                workTypesCache = null;
                powerNeedsCache = null;
                RefreshWorkGiverCaches();
                workPackages.Add(package);
                if (package.specialistWorker != null)
                {
                    SpecialistWorker worker = (SpecialistWorker)Activator.CreateInstance(package.specialistWorker, new object[] { parent });
                    specialist.AddWorker(worker);
                }
            }
        }

        public IEnumerator<WorkTypeDef> GetEnumerator()
        {
            return AllWorkTypes.GetEnumerator();
        }

        public bool Contains(WorkTypeDef def)
        {
            return AllWorkTypes.Contains(def);
        }

        public bool Contains(WorkPackageDef def)
        {
            return AllWorkPackages.Contains(def);
        }

        public void RefreshWorkGiverCaches()
        {
            workGiversInOrderEmergencyCache = null;
            workGiversInOrderCache = null;
            //workTypesCache = null;
            //powerNeedsCache = null;
        }

        public void SpawnSetup()
        {
            specialist.SpawnSetup();
        }

        public void ExposeData()
        {
            Scribe_Collections.LookList(ref this.workPackages, "workPackages", LookMode.DefReference);
            Scribe_Deep.LookDeep(ref this.specialist, "specialist", new object[] { parent });
        }
    }
}
