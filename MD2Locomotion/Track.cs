using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Track : Building
    {
        public static readonly SettingsDef Config = DefDatabase<SettingsDef>.GetNamed("MD2CartSettings");

        public override void Draw()
        {
            base.Draw();

            if (Config.drawTrackDebug)
            {
                foreach (var track in AllConnectedTracks)
                {
                    GenDraw.DrawTargetHighlight(track);
                }
            }
        }

        public int ConnectedTracksCount
        {
            get
            {
                return AllConnectedTracks.Count;
            }
        }

        public List<Track> AllConnectedTracks
        {
            get
            {
                List<Track> list = new List<Track>();
                foreach(var c in GenAdj.CellsAdjacentCardinal(this))
                {
                    Track t = Find.ThingGrid.ThingAt<Track>(c);
                    if(t!=null)
                    {
                        list.Add(t);
                    }
                }
                return list;
            }
        }

        public virtual List<Track> AvailableConnectedTracks
        {
            get
            {
                return AllConnectedTracks.Where(t => t.CanBeTravelledTo).ToList();
            }
        }

        public virtual Track NextTrackInDirection(Direction dir)
        {
            IntVec3 pos = Position.AdjacentInDirection(dir);
            if (pos.InBounds() && !pos.InNoBuildEdgeArea())
            {
                foreach (var t in AvailableConnectedTracks)
                {
                    if (t.Position == pos)
                    {
                        return t;
                    }
                }
            }
            return null;
        }

        public virtual bool Occupied
        {
            get
            {
                return Find.ThingGrid.ThingAt<Cart>(Position) != null;
            }
        }

        public virtual bool Accessible
        {
            get
            {
                return Position.Walkable();
            }
        }

        public virtual bool Obstructed
        {
            get
            {
                //TODO: Calculate obstructions
                return false;
            }
        }

        public virtual bool CanBeTravelledTo
        {
            get
            {
                return Accessible && !Occupied;
            }
        }

        public Direction DirectionTo(Track track)
        {
            if (track.Position.AdjacentToCardinal(this.Position))
            {
                if ((Position + IntVec3.North) == track.Position)
                {
                    return Direction.Up;
                }
                else if ((Position + IntVec3.South) == track.Position)
                {
                    return Direction.Down;
                }
                else if ((Position + IntVec3.East) == track.Position)
                {
                    return Direction.Right;
                }
                else if ((Position + IntVec3.West) == track.Position)
                {
                    return Direction.Left;
                }
                return Direction.Invalid;
            }
            return Direction.Invalid;
        }

        public virtual Track NextTrack(Direction preferredDir, Track previousTrack = null)
        {
            if (preferredDir != Direction.Any && preferredDir!=Direction.Invalid)
            {
                Track t = NextTrackInDirection(preferredDir);
                if (t == null)
                    t = NextTrackRandom(previousTrack);
                return t;
            }
            else
            {
                return NextTrackRandom(previousTrack);
            }
        }

        public virtual Track NextTrack(Track previousTrack = null)
        {
            return NextTrack(Direction.Any, previousTrack);
        }

        public virtual Track NextTrackRandom(Track previousTrack = null)
        {
            if (HasValidPath)
            {
                List<Track> list;
                if (previousTrack != null && AvailableConnectedTracks.Count > 1)
                {
                    list = AvailableConnectedTracks.Where(t => t != previousTrack).ToList();
                }
                else
                {
                    list = AvailableConnectedTracks;
                }
                if (list.Count > 0)
                    return list.RandomElement();
                else
                    return null;
            }
            return null;
        }

        public virtual bool ShouldStopCart
        {
            get
            {
                return false;
            }
        }

        public virtual bool HasTrackLeft
        {
            get
            {
                return NextTrackInDirection(Direction.Left)!=null;
            }
        }

        public virtual bool HasTrackRight
        {
            get
            {
                return NextTrackInDirection(Direction.Right)!=null;
            }
        }

        public virtual bool HasTrackUp
        {
            get
            {
                return NextTrackInDirection(Direction.Up) != null;
            }
        }

        public virtual bool HasTrackDown
        {
            get
            {
                return NextTrackInDirection(Direction.Down)!=null;
            }
        }

        public virtual bool HasValidPath
        {
            get
            {
                return this.AvailableConnectedTracks.Count >= 0;
            }
        }

        public bool IsAdjacentTo(Track track)
        {
            return this.AllConnectedTracks.Contains(track);
        }
    }
}
