using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public static class BodyPartRecordUtil
    {
        public static bool HasParentMissing(this BodyPartRecord part, Pawn pawn)
        {
            for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
            {
                Hediff_MissingPart hediff_MissingPart = pawn.health.hediffSet.hediffs[i] as Hediff_MissingPart;
                if (hediff_MissingPart != null && hediff_MissingPart.Part == part.parent)
                {
                    return true;
                }
            }
            return false;
        }

        public static BodyPartRecord HighestMissingPart(this BodyPartRecord part, Pawn pawn)
        {
            if (part.HasParentMissing(pawn))
            {
                for(int i=0;i<pawn.health.hediffSet.hediffs.Count; i++)
                {
                    Hediff_MissingPart missingPart = pawn.health.hediffSet.hediffs[i] as Hediff_MissingPart;
                    if(missingPart!=null && missingPart.Part == part.parent)
                    {
                        return missingPart.Part.HighestMissingPart(pawn);
                    }
                }
            }
            return part;
        }
    }
}
