using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MD2
{
    public struct Blueprint : IExposable
    {
        public string BpName;

        public PawnKindDef KindDef;
        public GraphicDef BodyGraphicDef;
        public GraphicDef HeadGraphicDef;
        public List<WorkPackageDef> WorkPackages;
        public int StartingSkillLevel;
        public Passion SkillPassion;
        public bool ExplodeOnDeath;
        public float ExplosionRadius;
        public ThingDef BatteryDef;

        public string Name;

        public Droid GenerateDroid(Faction faction)
        {
            return GenerateDroid(this, faction);
        }

        public Droid GenerateDroid()
        {
            return GenerateDroid(Faction.OfColony);
        }

        public static Blueprint Default
        {
            get
            {
                Blueprint c = new Blueprint
                {
                    BpName=BlueprintFiles.UnusedDefaultName(),
                    KindDef = PawnKindDef.Named("MD2Droid"),
                    BodyGraphicDef = GraphicDef.Named("MD2BodyOne"),
                    HeadGraphicDef = GraphicDef.Named("MD2HeadOne"),
                    WorkPackages = new List<WorkPackageDef>() { WorkPackageDef.Named("MD2MaintenancePackage"), WorkPackageDef.Named("MD2FirefighterPackage") },
                    Name = ListerDroids.GetNumberedName(),
                    StartingSkillLevel = 10,
                    SkillPassion = Passion.None,
                    ExplodeOnDeath = false,
                    ExplosionRadius = 0.9f,
                    BatteryDef = DefDatabase<ThingDef>.GetNamed("MD2BasicPowerCell")
                };
                return c;
            }

        }

        //public static bool operator !=(Blueprint b1, Blueprint b2)
        //{
        //    return b1.KindDef != b2.KindDef ||
        //           b1.BodyGraphicDef != b2.BodyGraphicDef ||
        //           b1.HeadGraphicDef != b2.HeadGraphicDef ||
        //           b1.WorkPackages != b2.WorkPackages ||
        //           b1.StartingSkillLevel != b2.StartingSkillLevel ||
        //           b1.SkillPassion != b2.SkillPassion ||
        //           b1.ExplodeOnDeath != b2.ExplodeOnDeath ||
        //           b1.ExplosionRadius != b2.ExplosionRadius ||
        //           b1.BatteryDef != b2.BatteryDef;
        //}

        //public static bool operator ==(Blueprint b1, Blueprint b2)
        //{
        //    return b1.KindDef == b2.KindDef &&
        //           b1.BodyGraphicDef == b2.BodyGraphicDef &&
        //           b1.HeadGraphicDef == b2.HeadGraphicDef &&
        //           b1.WorkPackages == b2.WorkPackages &&
        //           b1.StartingSkillLevel == b2.StartingSkillLevel &&
        //           b1.SkillPassion == b2.SkillPassion &&
        //           b1.ExplodeOnDeath == b2.ExplodeOnDeath &&
        //           b1.ExplosionRadius == b2.ExplosionRadius &&
        //           b1.BatteryDef == b2.BatteryDef;
        //}

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("BpName" + BpName);
            if (KindDef != null)
                b.AppendLine("KindDef: " + KindDef.defName);
            if (BodyGraphicDef != null)
                b.AppendLine("BodyGraphicDef: " + BodyGraphicDef.defName);
            if (HeadGraphicDef != null)
                b.AppendLine("HeadGraphicDef: " + HeadGraphicDef.defName);
            if (WorkPackages != null)
            {
                b.AppendLine("Work packages: ");
                foreach (var p in WorkPackages)
                    b.AppendLine(p.label);
                b.AppendLine();
            }
            b.AppendLine("Starting skill: " + StartingSkillLevel.ToString());
            b.AppendLine("Passion: " + SkillPassion.ToString());
            b.AppendLine("Explode on death: " + ExplodeOnDeath.ToString());
            b.AppendLine("Explosion radius: " + ExplosionRadius.ToString());
            if (BatteryDef != null)
                b.AppendLine("Battery: " + BatteryDef.defName);
            b.AppendLine("Name: " + Name);
            return b.ToString();
        }

        public float PowerUsage
        {
            get
            {
                float num = 100f;
                num += (from p in WorkPackages
                        select p.PowerRequirement).Sum();
                if (ExplodeOnDeath) num += 20f;
                return num;

            }
        }

        public int WorkAmount
        {
            get
            {
                int num = 800;
                foreach (var p in WorkPackages)
                {
                    num += p.workAmount;
                }
                if (ExplodeOnDeath)
                    num += 100;
                num += (10 * StartingSkillLevel);
                num += SkillPassion == Passion.None ? 0 : SkillPassion == Passion.Minor ? 100 : 200;
                return num;

            }
        }

        public List<ThingCount> CostList
        {
            get
            {
                List<ThingCount> list = new List<ThingCount>();
                BaseCostDef baseCosts = (BaseCostDef)DefDatabase<ThingDef>.GetNamed("MD2DroidCost");
                if (baseCosts == null)
                {
                    Log.Error("Unable to find droid base costs def MD2DroidCost");
                    return null;
                }
                //Base costs
                foreach (var count in baseCosts.baseCosts)
                {
                    list.Add(count);
                }
                //Battery
                list.Add(new ThingCount(BatteryDef, 1));

                //Cost for work packages
                foreach (var package in WorkPackages)
                {
                    foreach (var count in package.packageCost)
                        list.Add(count);
                }

                //Cost for explode
                float modifier = 0f;
                if (ExplodeOnDeath)
                {
                    if (ExplosionRadius > 0.9f)
                        modifier = ExplosionRadius - 0.9f;
                    foreach (var count in baseCosts.explosionCost)
                    {
                        int num = count.count;
                        if (modifier > 0f)
                            num += Mathf.RoundToInt(modifier * num);
                        list.Add(new ThingCount(count.thingDef, num));
                    }
                }

                //Cost for skills
                modifier = StartingSkillLevel / 10f;
                foreach (var count in baseCosts.skillsCost)
                {
                    list.Add(new ThingCount(count.thingDef, Mathf.RoundToInt(count.count * modifier)));
                }

                //Cost for passion
                modifier = SkillPassion == Passion.None ? 0f : SkillPassion == Passion.Minor ? 2f : 3f;
                foreach (var count in baseCosts.passionCost)
                {
                    int num = Mathf.RoundToInt(count.count * modifier);
                    if (num > 0)
                        list.Add(new ThingCount(count.thingDef, num));
                }

                //Combine all items
                List<ThingCount> list2 = new List<ThingCount>();
                foreach (var count in list)
                {
                    if (!list2.Any((c) => c.thingDef == count.thingDef))
                        list2.Add(new ThingCount(count.thingDef, count.count));
                    else
                    {
                        list2.First((c) => c.thingDef == count.thingDef).count += count.count;
                    }
                }

                return list2;
            }
        }

        public static Droid GenerateDroid(Blueprint bp, Faction faction)
        {
            Droid droid = (Droid)ThingMaker.MakeThing(bp.KindDef.race);

            droid.SetFactionDirect(faction);
            droid.kindDef = bp.KindDef;
            droid.RaceProps.corpseDef = ThingDef.Named("MD2DroidCorpse");

            #region Trackers
            droid.thinker = new Pawn_Thinker(droid);
            droid.playerController = new Pawn_PlayerController(droid);
            droid.inventory = new Pawn_InventoryTracker(droid);
            droid.pather = new Pawn_PathFollower(droid);
            droid.jobs = new Pawn_JobTracker(droid);
            droid.health = new Pawn_HealthTracker(droid);
            droid.ageTracker = new Pawn_AgeTracker(droid);
            droid.filth = new Pawn_FilthTracker(droid);
            droid.mindState = new Pawn_MindState(droid);
            droid.equipment = new Pawn_EquipmentTracker(droid);
            droid.apparel = new Pawn_ApparelTracker(droid);
            droid.natives = new Pawn_NativeVerbs(droid);
            droid.meleeVerbs = new Pawn_MeleeVerbs(droid);
            droid.carryHands = new Pawn_CarryHands(droid);
            droid.ownership = new Pawn_Ownership(droid);
            droid.skills = new Pawn_SkillTracker(droid);
            droid.story = new Pawn_StoryTracker(droid);
            droid.workSettings = new Pawn_WorkSettings(droid);
            droid.guest = new Pawn_GuestTracker(droid);
            droid.needs = new Pawn_NeedsTracker(droid);
            droid.stances = new Pawn_StanceTracker(droid);

            typeof(Pawn_NeedsTracker).GetMethod("AddNeed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(droid.needs, new object[] { DefDatabase<NeedDef>.GetNamed("Mood") });
            #endregion

            droid.gender = droid.RaceProps.hasGenders ? Gender.Male : Gender.None;

            droid.ageTracker.SetChronologicalBirthDate(GenDate.CurrentYear, GenDate.DayOfMonth);
            droid.story.skinColor = PawnSkinColors.PaleWhiteSkin;
            droid.story.crownType = CrownType.Narrow;
            droid.story.headGraphicPath = GraphicDatabaseHeadRecords.GetHeadRandom(Gender.Male, droid.story.skinColor, droid.story.crownType).GraphicPath;
            droid.story.hairColor = PawnHairColors.RandomHairColor(droid.story.skinColor, droid.ageTracker.AgeBiologicalYears);
            droid.story.name = new PawnName()
            {
                first = "Droid",
                last = "Droid",
                nick = bp.Name
            };

            #region Managers

            droid.meta = new MetaDataManager(droid, bp);
            droid.utilities = new UtilityManager(droid);

            WorkPackageDef maintenance = WorkPackageDef.Named("MD2MaintenancePackage");
            WorkPackageDef firefighting = WorkPackageDef.Named("MD2FirefighterPackage");
            List<WorkPackageDef> list = bp.WorkPackages;
            if (!list.Contains(maintenance)) list.Add(maintenance);
            if (!list.Contains(firefighting)) list.Add(firefighting);
            droid.work = new WorkManager(list, droid);

            droid.backstory = new BackstoryManager(droid);
            droid.backstory.SpawnSetup();

            droid.drawManager = new DrawManager(bp.BodyGraphicDef, bp.HeadGraphicDef, droid);
            droid.drawManager.SpawnSetup();

            droid.parts = new PartsManager(droid);
            #endregion

            foreach (SkillRecord sk in droid.skills.skills)
            {
                sk.level = (bp.StartingSkillLevel > 20) ? 20 : (bp.StartingSkillLevel <= 0) ? 1 : bp.StartingSkillLevel;
                sk.passion = bp.SkillPassion;
            }
            droid.workSettings.EnableAndInitialize();

            foreach (var def in DefDatabase<WorkTypeDef>.AllDefs)
            {
                if (droid.work.Contains(def))
                {
                    droid.workSettings.SetPriority(def, 4);
                }
                else
                {
                    droid.workSettings.SetPriority(def, 0);
                    droid.workSettings.Disable(def);
                }
            }

            return droid;
        }

        public void ExposeData()
        {
            Scribe_Values.LookValue(ref BpName, "BpName");
            Scribe_Defs.LookDef(ref KindDef, "kindDef");
            Scribe_Defs.LookDef(ref BodyGraphicDef, "bodygraphicdef");
            Scribe_Defs.LookDef(ref HeadGraphicDef, "headGraphicDef");
            Scribe_Collections.LookList(ref WorkPackages, "workPackages", LookMode.DefReference);
            Scribe_Values.LookValue(ref Name, "name");
            Scribe_Values.LookValue(ref StartingSkillLevel, "startingSkillLevel");
            Scribe_Values.LookValue(ref SkillPassion, "skillPassion");
            Scribe_Values.LookValue(ref ExplodeOnDeath, "ExplodeOnDeath");
            Scribe_Values.LookValue(ref ExplosionRadius, "ExplosionRadius");
            Scribe_Defs.LookDef(ref BatteryDef, "BatteryDef");
        }
    }
}
