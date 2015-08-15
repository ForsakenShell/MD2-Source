using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;

namespace MD2
{
    public class ModController : MonoBehaviour
    {
        public static readonly string GameObjectName = "MD2BaseModController";

        public virtual void Start()
        {
            this.enabled = true;
            AddDesignators();
            this.enabled = false;
        }

        private void AddDesignators()
        {
            foreach(var designator in DefDatabase<DesignatorDef>.AllDefs)
            {
                Designator des = (Designator)Activator.CreateInstance(designator.designatorClass);
                if(des!=null)
                {
                    var def = DefDatabase<DesignationCategoryDef>.GetNamed(designator.designationCategory);
                    if(def!=null && def.resolvedDesignators!=null)
                    {
                        def.resolvedDesignators.Add(des);
                    }
                }
            }
        }
    }
}
