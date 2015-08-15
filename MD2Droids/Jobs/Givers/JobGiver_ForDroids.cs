using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MD2
{
    public class JobGiver_ForDroids : JobGiver_Work
    {
        //public bool emergency;

        protected override Job TryGiveTerminalJob(Pawn pawn)
        {
            
            if (!(pawn is Droid))
                return null;
            Droid droid = pawn as Droid;

            if (!droid.Active)
                return null;

            List<WorkGiver> list = this.emergency ? droid.work.WorkGiversInOrderEmergency : droid.work.WorkGiversInOrder;

            int num = -999;
            TargetInfo targetInfo = TargetInfo.Invalid;
            WorkGiver_Scanner workGiver_Scanner = null;
            for (int i = 0; i < list.Count; i++)
            {
                WorkGiver workGiver = list[i];
				if (workGiver.def.priorityInType != num && targetInfo.IsValid)
                {
                    break;
                }
				if (workGiver.MissingRequiredCapacity(pawn) == null)
                {
					if (!workGiver.ShouldSkip(pawn))
                    {
                        try
                        {
							Job job = workGiver.NonScanJob(pawn);
							if (job != null)
							{
								Job result = job;
								return result;
							}
							WorkGiver_Scanner scanner = workGiver as WorkGiver_Scanner;
							if (scanner != null)
							{
								if (workGiver.def.scanThings)
								{
									Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t);
									IEnumerable<Thing> enumerable = scanner.PotentialWorkThingsGlobal(pawn);
									Predicate<Thing> validator = predicate;
									Thing thing = GenClosest.ClosestThingReachable(pawn.Position, scanner.PotentialWorkThingRequest, scanner.PathEndMode, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, enumerable, scanner.LocalRegionsToScanFirst, enumerable != null);
									if (thing != null)
									{
										targetInfo = thing;
										workGiver_Scanner = scanner;
									}
								}
								if (workGiver.def.scanCells)
								{
									IntVec3 position = pawn.Position;
									float num2 = 99999f;
									foreach (IntVec3 current in scanner.PotentialWorkCellsGlobal(pawn))
									{
										float lengthHorizontalSquared = (current - position).LengthHorizontalSquared;
										if (lengthHorizontalSquared < num2 && !current.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, current))
										{
											targetInfo = current;
											workGiver_Scanner = scanner;
											num2 = lengthHorizontalSquared;
										}
									}
								}
								num = workGiver.def.priorityInType;
							}
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Concat(new object[]
							{
								pawn,
								" threw exception in WorkGiver ",
								workGiver.def.defName,
								": ",
								ex.ToString()
							}));
                        }
                        finally
                        {
                        }
                        if (targetInfo.IsValid)
                        {
							pawn.mindState.lastGivenWorkType = workGiver.def.workType;
                            if (targetInfo.HasThing)
                            {
                                return workGiver_Scanner.JobOnThing(pawn, targetInfo.Thing);
                            }
                            return workGiver_Scanner.JobOnCell(pawn, targetInfo.Cell);
                        }
                    }
                }
            }
            return null;
        }
    }
}
