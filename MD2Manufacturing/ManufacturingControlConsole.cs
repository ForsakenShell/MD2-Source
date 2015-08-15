using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class ManufacturingControlConsole : Building
    {
        public static readonly Texture2D Manufacturing_Icon = ContentFinder<Texture2D>.Get("UI/Commands/Manufacturing_Icon");
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            if(!Find.Map.components.Any((MapComponent c)=>c is MPmanager))
                Find.Map.components.Add(new MPmanager());
        }

        public override void Tick()
        {
            if(Power!=null)
            {
                Power.powerOutputInt = MPmanager.manager.PowerUsage;
            }
            base.Tick();
        }

        public void OpenManagerUI()
        {
            if(Power!=null&&Power.PowerOn)
            {
                Find.LayerStack.Add(new Page_ManufacturingPlantMainUI());
            }
            else
            {
                Messages.Message("Building must be powered!", MessageSound.RejectInput);
            }
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            foreach(var m in base.GetFloatMenuOptions(myPawn))
            {
                yield return m;
            }
            FloatMenuOption option = new FloatMenuOption();
            option.action = () => OpenManagerUI();
            option.label = "Open manager";
            option.priority = MenuOptionPriority.High;
            option.autoTakeable = true;
            yield return option;
        }
        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (base.GetGizmos() != null)
            {
                foreach (Gizmo c in base.GetGizmos())
                {
                    yield return c;
                }
            }
            Command_Action com = new Command_Action();
            com.action = () => OpenManagerUI();
            com.defaultDesc = "Opens the manufacturing manager user interface";
            com.defaultLabel = "Open manufacturing manager";
            com.icon = Manufacturing_Icon;
            com.disabled = false;
            com.groupKey = 123456789;
            yield return com;
        }

        public CompPowerTrader Power
        {
            get
            {
                return base.GetComp<CompPowerTrader>();
            }
        }
    }
}
